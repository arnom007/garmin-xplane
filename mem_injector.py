"""
mem_injector.py — Direct Memory Injection for Garmin GDU Flight Model

Uses signature-based memory scanning to find IAS, Heading, Altitude,
and other flight parameters in the GDU process memory, then writes
X-Plane values directly using WriteProcessMemory.

This bypasses the need for DLL hooking of internal data paths.
"""

import ctypes
import ctypes.wintypes as wt
import struct
import time
import threading
import math
import os
import json

kernel32 = ctypes.windll.kernel32

# Windows API constants
PROCESS_VM_READ = 0x0010
PROCESS_VM_WRITE = 0x0020
PROCESS_VM_OPERATION = 0x0008
PROCESS_QUERY_INFORMATION = 0x0400
PROCESS_ALL_ACCESS = 0x1F0FFF
MEM_COMMIT = 0x1000
PAGE_NOACCESS = 0x01
PAGE_GUARD = 0x100
TH32CS_SNAPPROCESS = 0x02
TH32CS_SNAPTHREAD = 0x00000004
THREAD_SUSPEND_RESUME = 0x0002

class PROCESSENTRY32(ctypes.Structure):
    _fields_ = [
        ("dwSize", wt.DWORD), ("cntUsage", wt.DWORD),
        ("th32ProcessID", wt.DWORD), ("th32DefaultHeapID", ctypes.c_void_p),
        ("th32ModuleID", wt.DWORD), ("cntThreads", wt.DWORD),
        ("th32ParentProcessID", wt.DWORD), ("pcPriClassBase", ctypes.c_long),
        ("dwFlags", wt.DWORD), ("szExeFile", ctypes.c_char * 260),
    ]

class MEMORY_BASIC_INFORMATION(ctypes.Structure):
    _fields_ = [
        ("BaseAddress", ctypes.c_void_p), ("AllocationBase", ctypes.c_void_p),
        ("AllocationProtect", wt.DWORD), ("RegionSize", ctypes.c_size_t),
        ("State", wt.DWORD), ("Protect", wt.DWORD), ("Type", wt.DWORD),
    ]

class THREADENTRY32(ctypes.Structure):
    _fields_ = [
        ("dwSize", wt.DWORD), ("cntUsage", wt.DWORD),
        ("th32ThreadID", wt.DWORD), ("th32OwnerProcessID", wt.DWORD),
        ("tpBasePri", ctypes.c_long), ("tpDeltaPri", ctypes.c_long),
        ("dwFlags", wt.DWORD),
    ]


def find_process_pid(name_fragment):
    """Find PID by partial process name."""
    snap = kernel32.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0)
    if snap == -1:
        return None
    pe = PROCESSENTRY32()
    pe.dwSize = ctypes.sizeof(PROCESSENTRY32)
    result = None
    if kernel32.Process32First(snap, ctypes.byref(pe)):
        while True:
            exe = pe.szExeFile.decode('utf-8', errors='replace').lower()
            if name_fragment.lower() in exe:
                result = pe.th32ProcessID
                break
            if not kernel32.Process32Next(snap, ctypes.byref(pe)):
                break
    kernel32.CloseHandle(snap)
    return result


def get_process_module_base(hProcess, module_name_fragment):
    """Dynamically find base address of a loaded module in a target process."""
    try:
        psapi = ctypes.windll.psapi
        hMods = (ctypes.c_void_p * 1024)()
        cbNeeded = wt.DWORD()
        psapi.EnumProcessModulesEx.argtypes = [wt.HANDLE, ctypes.POINTER(ctypes.c_void_p), wt.DWORD, ctypes.POINTER(wt.DWORD), wt.DWORD]
        psapi.GetModuleFileNameExW.argtypes = [wt.HANDLE, ctypes.c_void_p, wt.LPWSTR, wt.DWORD]
        if psapi.EnumProcessModulesEx(hProcess, hMods, ctypes.sizeof(hMods), ctypes.byref(cbNeeded), 0x03):
            n = cbNeeded.value // ctypes.sizeof(ctypes.c_void_p)
            buf = ctypes.create_unicode_buffer(260)
            for i in range(n):
                if hMods[i]:
                    psapi.GetModuleFileNameExW(hProcess, hMods[i], buf, 260)
                    if module_name_fragment.lower() in buf.value.lower():
                        return hMods[i]
    except Exception:
        pass
    return None


class GDUMemoryInjector:
    """
    Scans GDU process memory for flight model variables and
    continuously writes X-Plane data to override them.
    
    Discovered field signatures:
    - IAS Widget:  [float IAS] [20.0] [999.0]  (value, min, max)
    - Heading:     [float rad] [float deg]      (heading in radians followed by degrees)
    - ADC struct:  [float GS] [float IAS] [float IAS] [float ALT] [float ALT]
    """

    def __init__(self, log_func=None):
        self._log = log_func or print
        self.pid = None
        self.hProcess = None
        self._write_count = 0
        self._scan_count = 0
        
        # Shared memory handles for GTN Xi and trainer coordination
        self.hSHM = None
        self.pSHM = None
        self.orig_shm_flag = None
        self.orig_shm_payload = None
        self.orig_cb_byte = None
        
        # Discovered addresses (set after scanning or known offsets)
        self.addrs = {
            'ias_widget': None,      # IAS float in speed widget (0x2D3BFE1C)
            'hdg_deg': None,         # Heading in degrees
            'hdg_rad': None,         # Heading in radians
            'adc_gs': None,          # Ground speed in ADC struct
            'adc_ias1': None,        # IAS in ADC struct
            'adc_ias2': None,        # IAS duplicate in ADC struct
            'adc_alt1': None,        # Altitude in ADC struct
            'adc_alt2': None,        # Altitude duplicate in ADC struct
            'adc_pitch': None,       # Pitch angle
            'adc_oat': None,         # Outside air temperature
            'speed_tape_ias1': None, # IAS in speed tape struct
            'speed_tape_ias2': None, # IAS duplicate in speed tape
            'ahrs_hdg': None,        # Real AHRS heading widget (0x312BB9F0)
            'ahrs_hdg_rad': None,    # AHRS heading in radians
            'ahrs_pitch': None,      # Fallback
            'ahrs_roll': None,       # Fallback
            'pfd_a_pitch_global': None,    # PFD-A real Pitch (cdp_pfd_a_sim.dll + 0x1E6A0C)
            'pfd_a_roll_global': None,     # PFD-A real Roll (cdp_pfd_a_sim.dll + 0x1E6A34)
            'pfd_a_hdg_global': None,      # PFD-A real Heading (cdp_pfd_a_sim.dll + 0x1E6A60)
            'pfd_a_attitude_struct': None, # PFD-A OpenGL attitude struct (cdp_pfd_a_sim.dll + 0xBD7348)
            'alt_widgets': [],       # List of altitude widget addresses (0x32F7FD38)
            'vs_widgets': [],        # List of vertical speed widget addresses (0x32F7FDC8)
            'demo_flag_addr': None,  # Address of the registry demo flag
        }
        
        self.scan_complete = False
        self.demo_killed = False
        self._demo_thread_handle = None
        self._demo_thread_id = None
        self.cdp_a_base = None
        self.cdp_b_base = None
        self.orig_pfd_a_cb_byte = None
        self.orig_pfd_b_cb_byte = None
        self.orig_hdg_box_format_bytes = None
        self._cb_patch_addr = None
        self._pfd_a_cb_addr = None
        self.addr_hdg_box_format = None
        
        # --- High-frequency injection thread ---
        self._inject_thread = None
        self._inject_running = False
        self._inject_hz = 0  # measured injection rate
        # Target values to inject (updated atomically from main thread)
        self._target_data = {
            'ias_kts': None,
            'heading_deg': None,
            'altitude_ft': None,
            'gs_kts': None,
            'pitch_deg': None,
            'roll_deg': None,
            'vs_fpm': None,
        }
        self._target_lock = threading.Lock()

    def connect(self, process_name='gdu_7xx'):
        """Find and open the GDU process and Shared Memory."""
        # Connect to Local\trainerShrSuperObj
        FILE_MAP_ALL_ACCESS = 0xF001F
        kernel32.MapViewOfFile.restype = ctypes.c_void_p
        self.hSHM = kernel32.OpenFileMappingW(FILE_MAP_ALL_ACCESS, False, 'Local\\trainerShrSuperObj')
        if self.hSHM:
            self.pSHM = kernel32.MapViewOfFile(self.hSHM, FILE_MAP_ALL_ACCESS, 0, 0, 148)
            if self.pSHM:
                self._log("[MemInject] Connected to Local\\trainerShrSuperObj")
        
        self.pid = find_process_pid(process_name)
        if not self.pid:
            self._log("[MemInject] GDU process not found")
            return False
        
        self.hProcess = kernel32.OpenProcess(
            PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_QUERY_INFORMATION,
            False, self.pid
        )
        if not self.hProcess:
            err = ctypes.GetLastError()
            self._log(f"[MemInject] Failed to open process (error {err}). Run as admin?")
            return False
        
        self._log(f"[MemInject] Connected to GDU PID={self.pid}")
        # Dynamically discover confirmed addresses for GDU 1060 TXi
        self._find_gdu_addresses()
        return True

    def _find_gdu_addresses(self):
        """Dynamically scans GDU committed memory to discover PFD widget addresses in ~1s."""
        if not self.hProcess:
            return False

        # Resolve PFD modules in GDU process
        self.cdp_a_base = get_process_module_base(self.hProcess, 'cdp_pfd_a_sim.dll')
        self.cdp_b_base = get_process_module_base(self.hProcess, 'cdp_pfd_b_sim.dll')
        if self.cdp_a_base:
            self.addrs['pfd_a_pitch_global'] = self.cdp_a_base + 0x1E6A0C
            self.addrs['pfd_a_roll_global']  = self.cdp_a_base + 0x1E6A34
            self.addrs['pfd_a_hdg_global']   = self.cdp_a_base + 0x1E6A60
            self.addrs['pfd_a_attitude_struct'] = self.cdp_a_base + 0xBD7348
            self._pfd_a_cb_addr = self.cdp_a_base + 0x2950

        if self.cdp_b_base:
            self._cb_patch_addr = self.cdp_b_base + 0x18A0
            self.addr_hdg_box_format = self.cdp_b_base + 0x000C18BB

        self._log("[MemInject] Escaneando assinaturas de memoria do GDU...")
        mbi = MEMORY_BASIC_INFORMATION()
        addr = 0x10000
        ias_addr = None
        hdg_cands = []

        while addr < 0x7FFE0000:
            if kernel32.VirtualQueryEx(self.hProcess, ctypes.c_void_p(addr), ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
                addr += 0x10000
                continue
            if (mbi.State == MEM_COMMIT and mbi.Protect == 0x04 and mbi.RegionSize < 50 * 1024 * 1024):
                buf = ctypes.create_string_buffer(mbi.RegionSize)
                br = ctypes.c_size_t(0)
                if kernel32.ReadProcessMemory(self.hProcess, ctypes.c_void_p(mbi.BaseAddress), buf, mbi.RegionSize, ctypes.byref(br)):
                    data = buf.raw[:br.value]
                    # IAS widget signature: min=20.0f, max=999.0f
                    if not ias_addr:
                        p = data.find(b'\x00\x00\xa0\x41\x00\xc0\x79\x44')
                        if p != -1 and p >= 4:
                            ias_addr = mbi.BaseAddress + (p - 4)
                    # AHRS/ADC heading signatures (must be 4-byte aligned)
                    idx = 0
                    while True:
                        p = data.find(b'\x00\x00\x01\x01', idx)
                        if p == -1:
                            break
                        if (mbi.BaseAddress + p) % 4 == 0 and p + 36 <= len(data):
                            if struct.unpack_from('<I', data, p + 36)[0] == 0x32:
                                hdg_cands.append(mbi.BaseAddress + (p + 8))
                        idx = p + 1

            addr = mbi.BaseAddress + mbi.RegionSize

        if hdg_cands and ias_addr:
            ahrs_hdg = max(hdg_cands)  # Active PFD AHRS widget (Compass Rose)
            adc_hdg  = min(hdg_cands)  # Active PFD ADC widget (Altitude, VS)
            self.addrs['ias_widget'] = ias_addr
            self.addrs['ahrs_hdg']   = ahrs_hdg
            self.addrs['ahrs_hdg_rad'] = ahrs_hdg - 4
            self.addrs['ahrs_pitch'] = ahrs_hdg - 4
            self.addrs['ahrs_roll']  = ahrs_hdg + 8
            self.addrs['alt_widgets'] = [adc_hdg + 0x1B800]
            self.addrs['vs_widgets']  = [adc_hdg + 0x1B800 + 0x90]
            self.addrs['adc_shadow_hdg'] = adc_hdg
            self.scan_complete = True
            self._log(f"[MemInject] Enderecos confirmados: IAS=0x{ias_addr:08X}, HDG=0x{ahrs_hdg:08X}, PITCH=0x{self.addrs.get('pfd_a_pitch_global', 0):08X}, ALT=0x{adc_hdg+0x1B800:08X}")
            self.save_cache()
            return True

        self.scan_complete = False
        self._log("[MemInject] GDU ainda carregando buffers na memoria. Nova tentativa em breve...")
        return False

    def disconnect(self):
        """Close process handle and restore trainer environment."""
        self.stop_injection()
        self.resume_demo()  # Resume demo thread and restore patches if applied
        if self.pSHM:
            kernel32.UnmapViewOfFile(ctypes.c_void_p(self.pSHM))
            self.pSHM = None
        if self.hSHM:
            kernel32.CloseHandle(self.hSHM)
            self.hSHM = None
        if self.hProcess:
            kernel32.CloseHandle(self.hProcess)
            self.hProcess = None
        self.pid = None
        self.scan_complete = False
        self.demo_killed = False
        # Reset addrs preserving types (list stays list)
        self.addrs = {k: ([] if isinstance(v, list) else None) for k, v in self.addrs.items()}

    def _read_float(self, addr):
        """Read a float from process memory."""
        buf = ctypes.create_string_buffer(4)
        br = ctypes.c_size_t(0)
        if kernel32.ReadProcessMemory(self.hProcess, ctypes.c_void_p(addr), buf, 4, ctypes.byref(br)):
            return struct.unpack('<f', buf.raw[:4])[0]
        return None

    def _write_float(self, addr, value):
        """Write a float to process memory."""
        data = struct.pack('<f', value)
        bw = ctypes.c_size_t(0)
        ok = kernel32.WriteProcessMemory(
            self.hProcess, ctypes.c_void_p(addr),
            data, 4, ctypes.byref(bw)
        )
        return ok and bw.value == 4

    def _read_region(self, addr, size):
        """Read a region of memory."""
        buf = ctypes.create_string_buffer(size)
        br = ctypes.c_size_t(0)
        if kernel32.ReadProcessMemory(self.hProcess, ctypes.c_void_p(addr), buf, size, ctypes.byref(br)):
            return buf.raw[:br.value]
        return None

    def _scan_memory(self, target, tolerance):
        """Scan all readable memory for a float value."""
        results = []
        mbi = MEMORY_BASIC_INFORMATION()
        addr = 0x10000
        while addr < 0x7FFE0000:
            if kernel32.VirtualQueryEx(self.hProcess, ctypes.c_void_p(addr),
                                       ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
                addr += 0x10000
                continue
            if (mbi.State == MEM_COMMIT and mbi.Protect and
                not (mbi.Protect & PAGE_NOACCESS) and
                not (mbi.Protect & PAGE_GUARD) and
                mbi.RegionSize < 50 * 1024 * 1024):
                buf = ctypes.create_string_buffer(mbi.RegionSize)
                br = ctypes.c_size_t(0)
                if kernel32.ReadProcessMemory(self.hProcess, ctypes.c_void_p(mbi.BaseAddress),
                                              buf, mbi.RegionSize, ctypes.byref(br)):
                    data = buf.raw[:br.value]
                    for off in range(0, len(data) - 3, 4):
                        val = struct.unpack_from('<f', data, off)[0]
                        if abs(val - target) <= tolerance:
                            results.append(mbi.BaseAddress + off)
            addr = mbi.BaseAddress + mbi.RegionSize
        return results

    def _scan_region_numpy(self, data, base, ias_cands, hdg_cands, alt_list):
        """Scan a memory region using numpy vectorized operations (fast)."""
        import numpy as np
        
        trim = len(data) - (len(data) % 4)
        if trim < 128:
            return
        
        farr = np.frombuffer(data[:trim], dtype=np.float32)
        iarr = np.frombuffer(data[:trim], dtype=np.uint32)
        n = len(farr)
        
        # --- IAS Widget: find [20.0] then check [+1]=999.0, [-1]=IAS ---
        twenty_idxs = np.where(np.abs(farr - 20.0) < 0.01)[0]
        for idx in twenty_idxs:
            idx = int(idx)
            if 1 <= idx and idx + 1 < n:
                if abs(float(farr[idx + 1]) - 999.0) < 0.01:
                    ias_val = float(farr[idx - 1])
                    if 0 <= ias_val <= 500:
                        ias_cands.append((base + (idx - 1) * 4, ias_val))
        
        # --- AHRS Heading: find 0x01010000 marker, then check [+2]=hdg 0-360, [+9]=0x32 ---
        marker_idxs = np.where(iarr == 0x01010000)[0]
        for idx in marker_idxs:
            idx = int(idx)
            hdg_idx = idx + 2  # heading is 8 bytes after marker
            if hdg_idx < n and hdg_idx + 7 < n:
                hdg_val = float(farr[hdg_idx])
                if 0 <= hdg_val <= 360:
                    if int(iarr[hdg_idx + 7]) == 0x32:
                        hdg_cands.append(base + hdg_idx * 4)
        
        # --- Altitude Widget: find consecutive floats where f[i+1] - f[i] ≈ 1.0 ---
        if n > 6:
            diff = farr[1:n] - farr[:n-1]
            alt_mask = (
                (np.abs(diff - 1.0) < 0.0001) &
                (farr[:n-1] > -2000) & (farr[:n-1] < 60000) &
                (np.abs(farr[:n-1]) > 0.01)
            )
            alt_idxs = np.where(alt_mask)[0]
            for idx in alt_idxs:
                idx = int(idx)
                if idx + 5 < n:
                    f1 = float(farr[idx])
                    f5 = float(farr[idx + 4])
                    f6 = float(farr[idx + 5])
                    if not math.isfinite(f5) or not math.isfinite(f6):
                        continue
                    # f5, f6 must be exact integers (floor representation)
                    if f5 == float(int(f5)) and f6 == float(int(f6)):
                        if abs(f1 - f5) <= 2.0 and abs((f5 + 1.0) - f6) < 0.0001:
                            alt_list.append(base + idx * 4)

    def _scan_region_python(self, data, base, ias_cands, hdg_cands, alt_list):
        """Scan a memory region using pure Python (fallback, slower)."""
        n = len(data)
        for off in range(0, n - 32, 4):
            fval = struct.unpack_from('<f', data, off)[0]
            ival = struct.unpack_from('<I', data, off)[0]
            
            # --- IAS Widget: [20.0] then [+4]=999.0, [-4]=IAS ---
            if abs(fval - 20.0) < 0.01 and off >= 4:
                f_999 = struct.unpack_from('<f', data, off + 4)[0]
                if abs(f_999 - 999.0) < 0.01:
                    ias_val = struct.unpack_from('<f', data, off - 4)[0]
                    if 0 <= ias_val <= 500:
                        ias_cands.append((base + off - 4, ias_val))
            
            # --- AHRS Heading: 0x01010000 marker ---
            if ival == 0x01010000 and off + 40 < n:
                hdg = struct.unpack_from('<f', data, off + 8)[0]
                if 0 <= hdg <= 360:
                    val_p36 = struct.unpack_from('<I', data, off + 36)[0]
                    if val_p36 == 0x32:
                        hdg_cands.append(base + off + 8)
            
            # --- Altitude Widget ---
            if -2000 < fval < 60000 and abs(fval) > 0.01 and off + 24 < n:
                f2 = struct.unpack_from('<f', data, off + 4)[0]
                if abs((fval + 1.0) - f2) < 0.0001:
                    f5 = struct.unpack_from('<f', data, off + 16)[0]
                    f6 = struct.unpack_from('<f', data, off + 20)[0]
                    if not math.isfinite(f5) or not math.isfinite(f6):
                        continue
                    if f5 == float(int(f5)) and f6 == float(int(f6)):
                        if abs(fval - f5) <= 2.0 and abs((f5 + 1.0) - f6) < 0.0001:
                            alt_list.append(base + off)

    def scan(self):
        """
        Fast memory scan for all flight model signatures in GDU PFD.
        """
        if not self.hProcess:
            self._log("[MemInject] Not connected!")
            return False
            
        # Try fast dynamic discovery first (~1s)
        if self._find_gdu_addresses():
            self._scan_count += 1
            return True

        self._log("[MemInject] Starting single-pass memory scan fallback...")
        t0 = time.time()
        
        # Try numpy for fast vectorized scanning
        try:
            import numpy as np
            use_numpy = True
            self._log("[MemInject] Using numpy acceleration")
        except ImportError:
            use_numpy = False
            self._log("[MemInject] numpy not available, using Python fallback")
        
        ias_candidates = []
        hdg_candidates = []
        self.addrs['alt_widgets'] = []
        
        mbi = MEMORY_BASIC_INFORMATION()
        addr = 0x10000
        regions = 0
        total_bytes = 0
        
        while addr < 0x7FFE0000:
            if kernel32.VirtualQueryEx(self.hProcess, ctypes.c_void_p(addr),
                                       ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
                addr += 0x10000
                continue
            if (mbi.State == MEM_COMMIT and mbi.Protect and
                not (mbi.Protect & PAGE_NOACCESS) and
                not (mbi.Protect & PAGE_GUARD) and
                mbi.RegionSize < 50 * 1024 * 1024):
                buf = ctypes.create_string_buffer(mbi.RegionSize)
                br = ctypes.c_size_t(0)
                if kernel32.ReadProcessMemory(self.hProcess, ctypes.c_void_p(mbi.BaseAddress),
                                              buf, mbi.RegionSize, ctypes.byref(br)):
                    data = buf.raw[:br.value]
                    base = mbi.BaseAddress
                    regions += 1
                    total_bytes += len(data)
                    
                    if use_numpy:
                        self._scan_region_numpy(data, base, ias_candidates,
                                               hdg_candidates, self.addrs['alt_widgets'])
                    else:
                        self._scan_region_python(data, base, ias_candidates,
                                                hdg_candidates, self.addrs['alt_widgets'])
            addr = mbi.BaseAddress + mbi.RegionSize
        
        # --- Process results ---
        
        # IAS
        if ias_candidates:
            best = max(ias_candidates, key=lambda x: x[1] if 50 < x[1] < 400 else -1)
            self.addrs['ias_widget'] = best[0]
            self._log(f"[MemInject] IAS widget found at 0x{best[0]:08X} = {best[1]:.1f} kts")
        else:
            self._log("[MemInject] IAS widget NOT found")
        
        # Heading
        if hdg_candidates:
            best = max(hdg_candidates)
            self.addrs['ahrs_hdg'] = best
            self.addrs['ahrs_pitch'] = best - 4
            self._log(f"[MemInject] AHRS Heading found at 0x{best:08X}")
        else:
            self._log("[MemInject] AHRS Heading NOT found")
        
        # Altitude
        if self.addrs['alt_widgets']:
            self._log(f"[MemInject] Found {len(self.addrs['alt_widgets'])} Altitude Widgets")
        else:
            self._log("[MemInject] Altitude Widgets NOT found")
        
        dt = time.time() - t0
        self._log(f"[MemInject] Scan complete in {dt:.1f}s ({regions} regions, "
                  f"{total_bytes / 1024 / 1024:.0f} MB scanned): "
                  f"IAS={'OK' if ias_candidates else 'FAIL'} "
                  f"HDG={'OK' if hdg_candidates else 'FAIL'} "
                  f"ALT={'OK' if self.addrs['alt_widgets'] else 'FAIL'}")
        
        self.scan_complete = bool(ias_candidates) or bool(hdg_candidates) or bool(self.addrs['alt_widgets'])
        if self.scan_complete:
            self.save_cache()
        self._scan_count += 1
        return self.scan_complete

    def update_target_data(self, ias_kts=None, heading_deg=None, altitude_ft=None,
                           gs_kts=None, pitch_deg=None, roll_deg=None, vs_fpm=None, oat_c=None):
        """
        Update the target flight data that the injection thread will write.
        This is called from the main bridge thread and is thread-safe.
        """
        with self._target_lock:
            if ias_kts is not None:
                self._target_data['ias_kts'] = ias_kts
            if heading_deg is not None:
                self._target_data['heading_deg'] = heading_deg
            if altitude_ft is not None:
                self._target_data['altitude_ft'] = altitude_ft
            if gs_kts is not None:
                self._target_data['gs_kts'] = gs_kts
            if pitch_deg is not None:
                self._target_data['pitch_deg'] = pitch_deg
            if roll_deg is not None:
                self._target_data['roll_deg'] = roll_deg
            if vs_fpm is not None:
                self._target_data['vs_fpm'] = vs_fpm

    def start_injection(self):
        """Start the dedicated high-frequency injection thread."""
        if self._inject_running:
            return
        self._inject_running = True
        self._inject_thread = threading.Thread(target=self._injection_loop, daemon=True)
        self._inject_thread.start()
        mode = "relaxed 50Hz" if self.demo_killed else "burst ~1500Hz"
        self._log(f"[MemInject] Injection thread started ({mode})")

    def stop_injection(self):
        """Stop the injection thread."""
        self._inject_running = False
        if self._inject_thread:
            self._inject_thread.join(timeout=2.0)
            self._inject_thread = None

    # ============================================================
    # Demo Killer (Registry Flags and Thread methods)
    # ============================================================

    def kill_demo_via_flags(self):
        """
        Attempts to kill the GDU demo mode by scanning for the SYS_SYSTEM_PFD1_DEMO_MODE 
        and SYS_DEMO registry keys, finding their boolean value addresses in memory,
        and writing 0 to them.
        Uses the survival of a sentinel in the IAS widget to confirm success.
        """
        if not self.hProcess or not self.addrs.get('ias_widget'):
            self._log("[DemoFlags] Need connected GDU and IAS widget address")
            return False

        ias_addr = self.addrs['ias_widget']
        SENTINEL = 77777.0

        self._log("[DemoFlags] Starting demo flag scanner...")

        target_tags = [
            b"SYS_SYSTEM_PFD1_DEMO_MODE\x00",
            b"SYS_DEMO\x00",
            b"SYS_SYSTEM_LRU_DEMO_MODE\x00",
            b"SYS_SYSTEM_MFD1_DEMO_MODE\x00",
        ]

        # Scan committed regions for these strings
        mbi = MEMORY_BASIC_INFORMATION()
        addr = 0x10000
        all_regions = []
        string_locations = {}

        while addr < 0x7FFE0000:
            if kernel32.VirtualQueryEx(self.hProcess, ctypes.c_void_p(addr),
                                       ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
                addr += 0x10000
                continue
            if (mbi.State == MEM_COMMIT and mbi.Protect and
                not (mbi.Protect & PAGE_NOACCESS) and
                not (mbi.Protect & PAGE_GUARD) and
                mbi.RegionSize < 50 * 1024 * 1024):
                buf = ctypes.create_string_buffer(mbi.RegionSize)
                br = ctypes.c_size_t(0)
                if kernel32.ReadProcessMemory(self.hProcess, ctypes.c_void_p(mbi.BaseAddress),
                                              buf, mbi.RegionSize, ctypes.byref(br)):
                    data = buf.raw[:br.value]
                    all_regions.append((mbi.BaseAddress, len(data), mbi.Protect))
                    
                    # Search strings
                    for tag in target_tags:
                        offset = 0
                        while True:
                            idx = data.find(tag, offset)
                            if idx == -1:
                                break
                            found_addr = mbi.BaseAddress + idx
                            tag_name = tag.rstrip(b'\x00').decode('ascii')
                            if tag_name not in string_locations:
                                string_locations[tag_name] = []
                            string_locations[tag_name].append(found_addr)
                            offset = idx + len(tag)
            addr = mbi.BaseAddress + mbi.RegionSize

        if not string_locations:
            self._log("[DemoFlags] No demo tag strings found in memory.")
            return False

        self._log(f"[DemoFlags] Found tag strings: {list(string_locations.keys())}")

        # Search for pointers to these string addresses
        pointer_refs = {}
        for tag_name, str_addrs in string_locations.items():
            pointer_refs[tag_name] = []
            for str_addr in str_addrs:
                needle = struct.pack('<I', str_addr)
                # Find the pointer in memory
                for base, size, prot in all_regions:
                    region_data = self._read_region(base, size)
                    if not region_data:
                        continue
                    offset = 0
                    while True:
                        idx = region_data.find(needle, offset)
                        if idx == -1:
                            break
                        ptr_addr = base + idx
                        pointer_refs[tag_name].append((ptr_addr, str_addr))
                        offset = idx + 4

        # For each pointer, let's scan surrounding memory for boolean candidates
        candidate_flags = []
        for tag_name, refs in pointer_refs.items():
            for ptr_addr, str_addr in refs:
                # Read 64 bytes before and 96 bytes after
                ctx_start = ptr_addr - 64
                ctx_data = self._read_region(ctx_start, 192)
                if not ctx_data:
                    continue
                
                # Check various offsets around the tag pointer for boolean values (0 or 1)
                for delta in range(-16, 40, 4):
                    check_offset = 64 + delta
                    if 0 <= check_offset < len(ctx_data) - 3:
                        val = struct.unpack_from('<I', ctx_data, check_offset)[0]
                        check_addr = ctx_start + check_offset
                        if check_addr != ptr_addr and val == 1: # We want flags that are currently 1 (ON)
                            candidate_flags.append((tag_name, check_addr, delta))

        if not candidate_flags:
            self._log("[DemoFlags] No boolean candidates with value=1 found near tag pointers")
            return False

        self._log(f"[DemoFlags] Testing {len(candidate_flags)} candidates by writing 0...")

        # Test each candidate
        for tag_name, flag_addr, delta in candidate_flags:
            # Write sentinel to IAS
            self._write_float(ias_addr, SENTINEL)
            
            # Flip candidate flag to 0
            self._write_int32(flag_addr, 0)
            
            # Wait a bit
            time.sleep(0.3)
            
            # Read IAS back
            ias_after = self._read_float(ias_addr)
            
            if ias_after is not None and abs(ias_after - SENTINEL) < 1.0:
                # Sentinel survived! Confirm with a longer wait
                time.sleep(1.0)
                ias_verify = self._read_float(ias_addr)
                if ias_verify is not None and abs(ias_verify - SENTINEL) < 1.0:
                    self._log(f"[DemoFlags] *** SUCCESS! Demo flag disabled: {tag_name} at 0x{flag_addr:08X} (offset {delta:+d}) ***")
                    self.addrs['demo_flag_addr'] = flag_addr
                    self.demo_killed = True
                    # Re-save the cache since we updated a new address
                    self.save_cache()
                    return True
                
            # If not succeeded, restore flag to 1
            self._write_int32(flag_addr, 1)

        self._log("[DemoFlags] Flag scanner failed to kill demo mode.")
        return False

    def kill_demo_thread(self):
        """
        Find and suspend the GDU thread responsible for demo mode updates.
        Strategy: write a sentinel value to the IAS widget, suspend each
        thread one at a time, check if sentinel survives. If it does,
        that thread was the one overwriting our values.
        """
        if not self.hProcess or not self.addrs.get('ias_widget'):
            self._log("[DemoKill] Need IAS widget address to identify demo thread")
            return False

        ias_addr = self.addrs['ias_widget']
        SENTINEL = 77777.0

        # Enumerate all threads of the GDU process
        snap = kernel32.CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0)
        if snap == -1 or snap == 0xFFFFFFFF:
            self._log("[DemoKill] Failed to create thread snapshot")
            return False

        te = THREADENTRY32()
        te.dwSize = ctypes.sizeof(THREADENTRY32)

        threads = []
        if kernel32.Thread32First(snap, ctypes.byref(te)):
            while True:
                if te.th32OwnerProcessID == self.pid:
                    threads.append(te.th32ThreadID)
                if not kernel32.Thread32Next(snap, ctypes.byref(te)):
                    break
        kernel32.CloseHandle(snap)

        self._log(f"[DemoKill] Found {len(threads)} threads in GDU (PID={self.pid})")

        if len(threads) < 15:
            self._log(f"[DemoKill] GDU em inicializacao ({len(threads)} threads). Aguardando carga completa das threads...")
            return False

        # Skip the main thread (lowest TID = usually UI/render)
        main_tid = min(threads)
        test_threads = [t for t in threads if t != main_tid]
        self._log(f"[DemoKill] Main thread TID={main_tid}, testing {len(test_threads)} others...")

        for tid in test_threads:
            hThread = kernel32.OpenThread(THREAD_SUSPEND_RESUME, False, tid)
            if not hThread:
                continue

            # Write sentinel
            self._write_float(ias_addr, SENTINEL)

            # Suspend this thread
            kernel32.SuspendThread(hThread)

            # Wait 150ms for demo to overwrite (if it's still running)
            time.sleep(0.15)

            # Check if sentinel survived
            val = self._read_float(ias_addr)

            if val is not None and abs(val - SENTINEL) < 1.0:
                # Sentinel survived! This is the demo thread!
                self._log(f"[DemoKill] *** SUCCESS! Demo thread found: TID={tid} ***")
                self._log(f"[DemoKill] Sentinel {SENTINEL:.0f} survived 150ms (readback={val:.1f})")
                self._demo_thread_id = tid
                self._demo_thread_handle = hThread
                self.demo_killed = True
                return True
            else:
                # Not the demo thread — resume it
                kernel32.ResumeThread(hThread)
                kernel32.CloseHandle(hThread)
                self._log(f"[DemoKill] TID={tid}: sentinel overwritten (val={f'{val:.1f}' if val is not None else 'None'}), resuming")

        self._log("[DemoKill] Could not identify demo thread (demo may run on main thread)")
        return False

    def disable_demo_flight_model(self):
        """
        Comprehensive bypass of internal trainer demo physics across GTN and GDU:
        1. Set Local\\trainerShrSuperObj byte 0x0C = 0 (stops supernav / trainer demo overwrite).
        2. Patch PFD-A Attitude callback (cdp_pfd_a_sim.dll + 0x2950) with RET (0xC3) + set AHRS valid flags.
        3. Patch PFD-B Heading callback (cdp_pfd_b_sim.dll + 0x18A0) with RET (0xC3).
        4. Patch PFD-B Digital Heading Box to synchronize with injected heading.
        """
        # 1. SHM demo flag
        if self.pSHM:
            self.orig_shm_flag = ctypes.c_uint8.from_address(self.pSHM + 0x0C).value
            self.orig_shm_payload = ctypes.string_at(self.pSHM + 0x3C, 88)
            ctypes.c_uint8.from_address(self.pSHM + 0x0C).value = 0
            self._log("[MemInject] SHM demo overwrite disabled (byte 0x0C set to 0)")

        if self.hProcess:
            old_protect = ctypes.c_uint32()

            # 2. Patch PFD-A Attitude callback with RET (0xC3)
            if self._pfd_a_cb_addr:
                try:
                    buf = ctypes.create_string_buffer(1)
                    kernel32.ReadProcessMemory(self.hProcess, ctypes.c_void_p(self._pfd_a_cb_addr), buf, 1, None)
                    if buf.raw[0] != 0xC3:
                        self.orig_pfd_a_cb_byte = buf.raw[0]
                    elif self.orig_pfd_a_cb_byte is None:
                        self.orig_pfd_a_cb_byte = 0x55
                    kernel32.VirtualProtectEx(self.hProcess, ctypes.c_void_p(self._pfd_a_cb_addr), 1, 0x40, ctypes.byref(old_protect))
                    kernel32.WriteProcessMemory(self.hProcess, ctypes.c_void_p(self._pfd_a_cb_addr), b'\xC3', 1, None)
                    kernel32.VirtualProtectEx(self.hProcess, ctypes.c_void_p(self._pfd_a_cb_addr), 1, old_protect.value, ctypes.byref(old_protect))
                    self._log(f"[MemInject] PFD-A Attitude callback at 0x{self._pfd_a_cb_addr:08X} patched with RET (0xC3)")

                    # Set AHRS sensor valid flags in PFD-A
                    if self.cdp_a_base:
                        kernel32.WriteProcessMemory(self.hProcess, ctypes.c_void_p(self.cdp_a_base + 0x1E6A08), b'\x01', 1, None)
                        kernel32.WriteProcessMemory(self.hProcess, ctypes.c_void_p(self.cdp_a_base + 0x1E6A5C), b'\x01', 1, None)
                        kernel32.WriteProcessMemory(self.hProcess, ctypes.c_void_p(self.cdp_a_base + 0x1E6A70), b'\x01', 1, None)
                except Exception as e:
                    self._log(f"[MemInject] Could not patch PFD-A attitude callback: {e}")

            # 3. Patch PFD-B Heading callback with RET (0xC3)
            if self._cb_patch_addr:
                try:
                    buf = ctypes.create_string_buffer(1)
                    kernel32.ReadProcessMemory(self.hProcess, ctypes.c_void_p(self._cb_patch_addr), buf, 1, None)
                    if buf.raw[0] != 0xC3:
                        self.orig_pfd_b_cb_byte = buf.raw[0]
                    elif self.orig_pfd_b_cb_byte is None:
                        self.orig_pfd_b_cb_byte = 0x55
                    kernel32.VirtualProtectEx(self.hProcess, ctypes.c_void_p(self._cb_patch_addr), 1, 0x40, ctypes.byref(old_protect))
                    kernel32.WriteProcessMemory(self.hProcess, ctypes.c_void_p(self._cb_patch_addr), b'\xC3', 1, None)
                    kernel32.VirtualProtectEx(self.hProcess, ctypes.c_void_p(self._cb_patch_addr), 1, old_protect.value, ctypes.byref(old_protect))
                    self._log(f"[MemInject] PFD-B Heading callback at 0x{self._cb_patch_addr:08X} patched with RET (0xC3)")
                except Exception as e:
                    self._log(f"[MemInject] Could not patch PFD-B heading callback: {e}")

            # 4. Patch PFD-B Digital Heading Box
            hdg_widget = self.addrs.get('ahrs_hdg')
            if self.addr_hdg_box_format and hdg_widget:
                try:
                    buf24 = ctypes.create_string_buffer(24)
                    kernel32.ReadProcessMemory(self.hProcess, ctypes.c_void_p(self.addr_hdg_box_format), buf24, 24, None)
                    self.orig_hdg_box_format_bytes = buf24.raw

                    # movss xmm0, dword ptr [hdg_widget] + 16 NOPs
                    patch_mov = b'\xF3\x0F\x10\x05' + struct.pack('<I', hdg_widget) + b'\x90' * 16
                    kernel32.VirtualProtectEx(self.hProcess, ctypes.c_void_p(self.addr_hdg_box_format), 24, 0x40, ctypes.byref(old_protect))
                    kernel32.WriteProcessMemory(self.hProcess, ctypes.c_void_p(self.addr_hdg_box_format), patch_mov, 24, None)
                    kernel32.VirtualProtectEx(self.hProcess, ctypes.c_void_p(self.addr_hdg_box_format), 24, old_protect.value, ctypes.byref(old_protect))
                    self._log("[MemInject] PFD-B Digital Heading Box patched for live readout")
                except Exception as e:
                    self._log(f"[MemInject] Could not patch digital heading box: {e}")

        # Mark demo bypassed (thread suspension removed to preserve RTOS watchdogs)
        self.demo_killed = True
        return True

    def resume_demo(self):
        """Restore trainer patches."""
        if getattr(self, '_demo_thread_handle', None):
            kernel32.ResumeThread(self._demo_thread_handle)
            kernel32.CloseHandle(self._demo_thread_handle)
            self._demo_thread_handle = None
            self._demo_thread_id = None

        if self.hProcess:
            old_protect = ctypes.c_uint32()

            # 1. Restore PFD-A callback
            if getattr(self, '_pfd_a_cb_addr', None) and self.orig_pfd_a_cb_byte is not None:
                try:
                    kernel32.VirtualProtectEx(self.hProcess, ctypes.c_void_p(self._pfd_a_cb_addr), 1, 0x40, ctypes.byref(old_protect))
                    kernel32.WriteProcessMemory(self.hProcess, ctypes.c_void_p(self._pfd_a_cb_addr), bytes([self.orig_pfd_a_cb_byte]), 1, None)
                    kernel32.VirtualProtectEx(self.hProcess, ctypes.c_void_p(self._pfd_a_cb_addr), 1, old_protect.value, ctypes.byref(old_protect))
                    self.orig_pfd_a_cb_byte = None
                    self._log("[MemInject] PFD-A Attitude callback restored")
                except Exception as e:
                    self._log(f"[MemInject] Error restoring PFD-A callback: {e}")

            # 2. Restore PFD-B callback
            if getattr(self, '_cb_patch_addr', None) and self.orig_pfd_b_cb_byte is not None:
                try:
                    kernel32.VirtualProtectEx(self.hProcess, ctypes.c_void_p(self._cb_patch_addr), 1, 0x40, ctypes.byref(old_protect))
                    kernel32.WriteProcessMemory(self.hProcess, ctypes.c_void_p(self._cb_patch_addr), bytes([self.orig_pfd_b_cb_byte]), 1, None)
                    kernel32.VirtualProtectEx(self.hProcess, ctypes.c_void_p(self._cb_patch_addr), 1, old_protect.value, ctypes.byref(old_protect))
                    self.orig_pfd_b_cb_byte = None
                    self._log("[MemInject] PFD-B Heading callback restored")
                except Exception as e:
                    self._log(f"[MemInject] Error restoring PFD-B callback: {e}")

            # 3. Restore Digital Heading Box patch
            if getattr(self, 'addr_hdg_box_format', None) and getattr(self, 'orig_hdg_box_format_bytes', None):
                try:
                    kernel32.VirtualProtectEx(self.hProcess, ctypes.c_void_p(self.addr_hdg_box_format), 24, 0x40, ctypes.byref(old_protect))
                    kernel32.WriteProcessMemory(self.hProcess, ctypes.c_void_p(self.addr_hdg_box_format), self.orig_hdg_box_format_bytes, 24, None)
                    kernel32.VirtualProtectEx(self.hProcess, ctypes.c_void_p(self.addr_hdg_box_format), 24, old_protect.value, ctypes.byref(old_protect))
                    self._log("[MemInject] Digital Heading Box patch restored")
                    self.orig_hdg_box_format_bytes = None
                except Exception:
                    pass

        if self.pSHM and self.orig_shm_payload is not None:
            ctypes.memmove(self.pSHM + 0x3C, self.orig_shm_payload, 88)
            ctypes.c_uint8.from_address(self.pSHM + 0x0C).value = self.orig_shm_flag
            self._log("[MemInject] SHM demo flag and payload restored")

        self.demo_killed = False

    # ============================================================
    # Injection Loop
    # ============================================================

    def _injection_loop(self):
        """
        Dedicated loop that writes flight data to GTN Xi (via SHM)
        and GDU memory (direct widget injection) without lag or fighting.
        """
        writes = 0
        t0 = time.time()
        log_interval = 10.0
        next_log = t0 + log_interval

        while self._inject_running and (self.hProcess or self.pSHM):
            # Snapshot target data under lock
            with self._target_lock:
                data = dict(self._target_data)

            ias = data.get('ias_kts')
            hdg = data.get('heading_deg')
            alt = data.get('altitude_ft')
            gs  = data.get('gs_kts')
            pit = data.get('pitch_deg')
            rol = data.get('roll_deg')
            vs  = data.get('vs_fpm')

            # Skip if no data yet
            if all(v is None for v in (ias, hdg, alt, gs, pit, rol, vs)):
                time.sleep(0.01)
                continue

            # --- 1. Write to Shared Memory (GTN Xi & Navigation Computer) ---
            if self.pSHM:
                try:
                    shm_buf = ctypes.string_at(self.pSHM + 0x3C, 88)
                    shm_floats = list(struct.unpack('<22f', shm_buf))
                    if alt is not None:
                        alt_m = alt * 0.3048
                        shm_floats[5] = alt_m
                        shm_floats[18] = alt_m
                    if ias is not None:
                        shm_floats[7] = ias
                        shm_floats[19] = ias
                    if hdg is not None:
                        hdg_rad = math.radians(hdg % 360.0)
                        shm_floats[12] = hdg_rad
                        shm_floats[21] = hdg_rad
                    if rol is not None:
                        shm_floats[14] = math.radians(rol)
                    if vs is not None:
                        shm_floats[20] = vs * 0.00508 # fpm -> m/s
                    ctypes.memmove(self.pSHM + 0x3C, struct.pack('<22f', *shm_floats), 88)
                except Exception:
                    pass

            # --- 2. Write to GDU display widgets ---
            if self.hProcess:
                if ias is not None:
                    if self.addrs.get('ias_widget'):
                        self._write_float(self.addrs['ias_widget'], ias)

                if hdg is not None:
                    if self.addrs.get('ahrs_hdg'):
                        self._write_float(self.addrs['ahrs_hdg'], hdg % 360.0)
                    if self.addrs.get('ahrs_hdg_rad'):
                        self._write_float(self.addrs['ahrs_hdg_rad'], math.radians(hdg % 360.0))
                    if self.addrs.get('pfd_a_hdg_global'):
                        self._write_float(self.addrs['pfd_a_hdg_global'], hdg % 360.0)

                if pit is not None:
                    if self.addrs.get('pfd_a_pitch_global'):
                        self._write_float(self.addrs['pfd_a_pitch_global'], pit)
                    if self.addrs.get('pfd_a_attitude_struct'):
                        st = self.addrs['pfd_a_attitude_struct']
                        self._write_float(st + 0x30, pit)
                        self._write_float(st + 0x0C, pit * 0.5)
                        self._write_float(st + 0x14, pit)

                if rol is not None:
                    if self.addrs.get('pfd_a_roll_global'):
                        self._write_float(self.addrs['pfd_a_roll_global'], rol)
                    if self.addrs.get('pfd_a_attitude_struct'):
                        st = self.addrs['pfd_a_attitude_struct']
                        self._write_float(st + 0x3C, rol)
                        self._write_float(st + 0x10, rol * 0.35)
                        self._write_float(st + 0x18, rol)

                if alt is not None:
                    for addr in self.addrs.get('alt_widgets', []):
                        for off in [0x00, 0x08, 0x48, 0x50, 0x58, 0x60]:
                            self._write_float(addr + off, alt)
                            self._write_float(addr + off + 4, round(alt))

                if vs is not None:
                    for addr in self.addrs.get('vs_widgets', []):
                        for off in [0x00, 0x08, 0x10, 0x14, 0x48, 0x50]:
                            self._write_float(addr + off, vs)

                if gs is not None:
                    if self.addrs.get('adc_gs'):
                        self._write_float(self.addrs['adc_gs'], gs)

            writes += 1
            self._write_count += 1

            # Clean 60 Hz injection rate (synchronized with GDU display refresh rate)
            time.sleep(0.016)

            # Log injection rate periodically
            now = time.time()
            if now >= next_log:
                elapsed = now - t0
                hz = writes / elapsed if elapsed > 0 else 0
                self._inject_hz = hz
                mode = "CLEAN 60Hz"
                self._log(f"[MemInject] Injection [{mode}]: {hz:.0f} Hz ({writes} writes)")
                next_log = now + log_interval

        self._inject_running = False
        self._log("[MemInject] Injection thread stopped")

    def write_flight_data(self, ias_kts=None, heading_deg=None, altitude_ft=None,
                          gs_kts=None, pitch_deg=None, roll_deg=None, vs_fpm=None, oat_c=None):
        """
        Write X-Plane flight data to the GDU process memory.
        If injection thread is running, just updates target data.
        Otherwise writes directly (legacy/test mode).
        """
        if not self.hProcess or not self.scan_complete:
            return False
        
        # If injection thread is active, just update targets
        if self._inject_running:
            self.update_target_data(
                ias_kts=ias_kts, heading_deg=heading_deg, altitude_ft=altitude_ft,
                gs_kts=gs_kts, pitch_deg=pitch_deg, roll_deg=roll_deg, vs_fpm=vs_fpm, oat_c=oat_c
            )
            return True
        
        # Fallback: direct write (for standalone tests)
        written = 0
        
        # Write IAS
        if ias_kts is not None:
            if self.addrs.get('ias_widget'):
                self._write_float(self.addrs['ias_widget'], ias_kts)
                written += 1
            if self.addrs.get('adc_ias1'):
                self._write_float(self.addrs['adc_ias1'], ias_kts)
                self._write_float(self.addrs['adc_ias2'], ias_kts)
                written += 2
            if self.addrs.get('speed_tape_ias1'):
                self._write_float(self.addrs['speed_tape_ias1'], ias_kts)
                self._write_float(self.addrs['speed_tape_ias2'], ias_kts)
                written += 2
        
        # Write Heading
        if heading_deg is not None:
            if self.addrs.get('ahrs_hdg'):
                self._write_float(self.addrs['ahrs_hdg'], heading_deg % 360.0)
                written += 1
            if self.addrs.get('hdg_deg'):
                self._write_float(self.addrs['hdg_deg'], heading_deg % 360.0)
                written += 1
            if self.addrs.get('hdg_rad'):
                self._write_float(self.addrs['hdg_rad'], math.radians(heading_deg % 360.0))
                written += 1
        
        # Write Altitude
        if altitude_ft is not None:
            for addr in self.addrs.get('alt_widgets', []):
                self._write_float(addr, altitude_ft)
                self._write_float(addr + 4, altitude_ft + 1.0)
                self._write_float(addr + 16, math.floor(altitude_ft))
                self._write_float(addr + 20, math.floor(altitude_ft) + 1.0)
                written += 4
            if self.addrs.get('adc_alt1'):
                self._write_float(self.addrs['adc_alt1'], altitude_ft)
                self._write_float(self.addrs['adc_alt2'], altitude_ft)
                written += 2
        
        # Write Ground Speed
        if gs_kts is not None:
            if self.addrs.get('adc_gs'):
                self._write_float(self.addrs['adc_gs'], gs_kts)
                written += 1
        
        # Write Pitch
        if pitch_deg is not None:
            if self.addrs.get('ahrs_pitch'):
                self._write_float(self.addrs['ahrs_pitch'], pitch_deg)
                written += 1
            if self.addrs.get('adc_pitch'):
                self._write_float(self.addrs['adc_pitch'], pitch_deg)
                written += 1

        # Write Roll
        if roll_deg is not None:
            if self.addrs.get('ahrs_roll'):
                self._write_float(self.addrs['ahrs_roll'], roll_deg)
                written += 1

        # Write VS
        if vs_fpm is not None:
            for addr in self.addrs.get('vs_widgets', []):
                for off in [0x00, 0x08, 0x10, 0x14, 0x48, 0x50]:
                    self._write_float(addr + off, vs_fpm)
                written += 1
        
        if written > 0:
            self._write_count += 1
        
        return written > 0

    def _write_int32(self, addr, value):
        """Write a 32-bit integer to process memory."""
        data = struct.pack('<i', value)
        bw = ctypes.c_size_t(0)
        ok = kernel32.WriteProcessMemory(
            self.hProcess, ctypes.c_void_p(addr),
            data, 4, ctypes.byref(bw)
        )
        return ok and bw.value == 4

    def _read_int32(self, addr):
        """Read a 32-bit integer from process memory."""
        buf = ctypes.create_string_buffer(4)
        br = ctypes.c_size_t(0)
        if kernel32.ReadProcessMemory(self.hProcess, ctypes.c_void_p(addr), buf, 4, ctypes.byref(br)):
            return struct.unpack('<I', buf.raw[:4])[0]
        return None

    def load_cache(self):
        """Load cached memory addresses from file."""
        cache_dir = os.path.join(os.path.expandvars('%APPDATA%'), 'GarminBridge')
        cache_path = os.path.join(cache_dir, 'garmin_addrs_cache.json')
        if not os.path.exists(cache_path):
            return False
            
        try:
            with open(cache_path, 'r') as f:
                cached = json.load(f)
                
            # Populate self.addrs
            for k, v in cached.items():
                if k in self.addrs:
                    self.addrs[k] = v
            
            # Verify loaded addresses
            if self.verify_addresses():
                self._log("[MemInject] Cached addresses loaded and verified successfully!")
                self.scan_complete = True
                
                # If we have a cached demo flag address, write 0 to it immediately!
                if self.addrs.get('demo_flag_addr'):
                    self._write_int32(self.addrs['demo_flag_addr'], 0)
                    self.demo_killed = True
                    self._log(f"[MemInject] Applied cached demo flag at 0x{self.addrs['demo_flag_addr']:08X}!")
                    
                return True
            else:
                self._log("[MemInject] Cached addresses failed verification (GDU restarted?), starting fresh scan...")
                # Reset preserving alt_widgets as list
                self.addrs = {k: ([] if isinstance(v, list) else None) for k, v in self.addrs.items()}
                return False
        except Exception as e:
            self._log(f"[MemInject] Error loading cache: {e}")
            return False

    def save_cache(self):
        """Save successfully scanned addresses to cache file."""
        cache_dir = os.path.join(os.path.expandvars('%APPDATA%'), 'GarminBridge')
        os.makedirs(cache_dir, exist_ok=True)
        cache_path = os.path.join(cache_dir, 'garmin_addrs_cache.json')
        try:
            # Prepare serializable version
            serializable = {}
            for k, v in self.addrs.items():
                serializable[k] = v
            with open(cache_path, 'w') as f:
                json.dump(serializable, f, indent=2)
            self._log(f"[MemInject] Saved addresses to cache.")
        except Exception as e:
            self._log(f"[MemInject] Error saving cache: {e}")

    def verify_addresses(self):
        """
        Verify that process handle is still valid and memory is accessible.
        Returns False only if process died or memory pages became unreadable.
        """
        if not self.hProcess:
            return False
        
        # Verify that both primary IAS widget and AHRS heading memory pages are readable
        ias = self.addrs.get('ias_widget')
        hdg = self.addrs.get('ahrs_hdg')
        if ias and hdg:
            v_ias = self._read_float(ias)
            v_hdg = self._read_float(hdg)
            return (v_ias is not None) and (v_hdg is not None)
        return False

    def get_status(self):
        """Return current injector status."""
        
        # Safely format addresses (handling both ints and lists)
        fmt_addrs = {}
        for k, v in self.addrs.items():
            if isinstance(v, list):
                fmt_addrs[k] = f"{len(v)} widgets" if v else None
            else:
                fmt_addrs[k] = f"0x{v:08X}" if v else None
                
        return {
            'connected': self.hProcess is not None,
            'pid': self.pid,
            'scan_complete': self.scan_complete,
            'write_count': self._write_count,
            'scan_count': self._scan_count,
            'addrs_found': sum(1 for v in self.addrs.values() if v is not None and (not isinstance(v, list) or len(v) > 0)),
            'addrs_total': len(self.addrs),
            'addresses': fmt_addrs,
        }


# ============================================================
# Standalone test — saves full log to %APPDATA%/GarminBridge
# ============================================================

if __name__ == '__main__':
    import sys
    import os
    import datetime

    # Setup log file
    log_dir = os.path.join(os.path.expandvars('%APPDATA%'), 'GarminBridge')
    os.makedirs(log_dir, exist_ok=True)
    log_path = os.path.join(log_dir, 'mem_inject_test.log')

    class TeeLog:
        """Write to both console and log file."""
        def __init__(self, path):
            self.f = open(path, 'w', encoding='utf-8')
            self.lines = []
        def log(self, msg=''):
            line = msg
            print(line)
            self.f.write(line + '\n')
            self.f.flush()
        def close(self):
            self.f.close()

    L = TeeLog(log_path)
    L.log("=" * 70)
    L.log("  Garmin GDU Memory Injector - Test Log")
    L.log(f"  {datetime.datetime.now().isoformat()}")
    L.log("=" * 70)

    inj = GDUMemoryInjector(log_func=L.log)

    if not inj.connect():
        L.log("FATAL: GDU not found! Start the Garmin trainer first.")
        L.log(f"Log saved to: {log_path}")
        L.close()
        input("Press Enter to exit...")
        sys.exit(1)

    L.log(f"Connected to GDU PID={inj.pid}")
    L.log("Scanning memory (1-2 minutes)...\n")

    scan_t0 = time.time()
    scan_ok = inj.scan()
    scan_dt = time.time() - scan_t0

    L.log(f"\n{'='*70}")
    L.log(f"SCAN RESULTS (took {scan_dt:.1f}s)")
    L.log(f"{'='*70}")

    if not scan_ok:
        L.log("SCAN FAILED — no addresses found")
        L.log(f"\nLog saved to: {log_path}")
        L.close()
        inj.disconnect()
        input("Press Enter to exit...")
        sys.exit(1)

    status = inj.get_status()
    L.log(f"Addresses found: {status['addrs_found']}/{status['addrs_total']}")
    L.log("")
    for name, addr_str in status['addresses'].items():
        if addr_str:
            if isinstance(inj.addrs[name], list):
                if inj.addrs[name]:
                    val = inj._read_float(inj.addrs[name][0])
                    L.log(f"  {name:20s}: {addr_str} (1st: 0x{inj.addrs[name][0]:08X}) = {val:.4f}" if val else f"  {name:20s}: {addr_str}")
                else:
                    L.log(f"  {name:20s}: {addr_str}")
            else:
                val = inj._read_float(inj.addrs[name])
                L.log(f"  {name:20s}: {addr_str}  = {val:.4f}" if val else f"  {name:20s}: {addr_str}")

    # Dump context around each found address
    L.log(f"\n{'='*70}")
    L.log("MEMORY CONTEXT DUMPS")
    L.log(f"{'='*70}")
    for name, addr in inj.addrs.items():
        if addr is None or (isinstance(addr, list) and not addr):
            continue
        addr_to_dump = addr[0] if isinstance(addr, list) else addr
        region = inj._read_region(addr_to_dump - 32, 128)
        if not region:
            continue
        L.log(f"\n--- {name} (1st item) @ 0x{addr_to_dump:08X} ---" if isinstance(addr, list) else f"\n--- {name} @ 0x{addr_to_dump:08X} ---")
        for i in range(0, len(region) - 3, 4):
            fval = struct.unpack_from('<f', region, i)[0]
            ival = struct.unpack_from('<I', region, i)[0]
            real_addr = addr_to_dump - 32 + i
            marker = " <<<" if real_addr == addr_to_dump else ""
            if abs(fval) < 1e8 and (abs(fval) > 1e-8 or fval == 0):
                L.log(f"  0x{real_addr:08X} ({i-32:+4d}): {fval:12.4f}  (0x{ival:08X}){marker}")
            else:
                L.log(f"  0x{real_addr:08X} ({i-32:+4d}): 0x{ival:08X}{marker}")

    # === Test 1: WriteProcessMemory verification ===
    L.log(f"\n{'='*70}")
    L.log("TEST 1: WriteProcessMemory Verification")
    L.log(f"{'='*70}")

    for test_name, test_addr in [('adc_alt1', inj.addrs.get('adc_alt1')),
                                  ('ias_widget', inj.addrs.get('ias_widget')),
                                  ('adc_ias1', inj.addrs.get('adc_ias1'))]:
        if not test_addr:
            continue
        L.log(f"\n  Testing {test_name} @ 0x{test_addr:08X}:")
        before = inj._read_float(test_addr)
        L.log(f"    Before:          {before:.4f}")

        # Write and read back IMMEDIATELY (no sleep)
        ok = inj._write_float(test_addr, 12345.6789)
        after_0 = inj._read_float(test_addr)
        L.log(f"    Write(12345.68): ok={ok}  readback={after_0:.4f}")

        # Wait 1ms and read
        time.sleep(0.001)
        after_1ms = inj._read_float(test_addr)
        L.log(f"    After 1ms:       {after_1ms:.4f}")

        # Wait 10ms and read
        time.sleep(0.009)
        after_10ms = inj._read_float(test_addr)
        L.log(f"    After 10ms:      {after_10ms:.4f}")

        # Wait 50ms and read
        time.sleep(0.040)
        after_50ms = inj._read_float(test_addr)
        L.log(f"    After 50ms:      {after_50ms:.4f}")

        if after_0 and abs(after_0 - 12345.68) < 1.0:
            L.log(f"    RESULT: Write STICKS immediately")
        elif after_1ms and abs(after_1ms - 12345.68) < 1.0:
            L.log(f"    RESULT: Write sticks for >1ms (demo loop slower)")
        elif after_0 and abs(after_0 - before) > 0.01:
            L.log(f"    RESULT: Write partially works (value changed but not to target)")
        else:
            L.log(f"    RESULT: Write has NO EFFECT (demo overwrites instantly)")

    # === Test 2: Write race — measure how long our value survives ===
    L.log(f"\n{'='*70}")
    L.log("TEST 2: Write Survival Timing")
    L.log(f"{'='*70}")

    test_addr = inj.addrs.get('adc_alt1') or inj.addrs.get('ias_widget')
    if test_addr:
        L.log(f"  Target: 0x{test_addr:08X}")
        delays_us = [0, 100, 500, 1000, 2000, 5000, 10000, 20000, 50000]
        for delay_us in delays_us:
            inj._write_float(test_addr, 77777.0)
            if delay_us > 0:
                time.sleep(delay_us / 1_000_000)
            val = inj._read_float(test_addr)
            survived = "YES" if val and abs(val - 77777.0) < 1.0 else "NO"
            L.log(f"    Delay {delay_us:6d}us: readback={val:.2f}  survived={survived}")

    # === Test 3: High-frequency burst with sampling ===
    L.log(f"\n{'='*70}")
    L.log("TEST 3: High-Frequency Write Burst (10 seconds)")
    L.log(f"{'='*70}")
    L.log("  Target values: IAS=200, HDG=090, ALT=10000, GS=210")
    L.log("  >>> WATCH THE GDU PFD NOW <<<")

    t0 = time.time()
    writes = 0
    samples = []
    sample_interval = 1.0  # sample every second

    next_sample = t0 + sample_interval
    while time.time() - t0 < 10.0:
        inj.write_flight_data(
            ias_kts=200.0,
            heading_deg=90.0,
            altitude_ft=10000.0,
            gs_kts=210.0
        )
        writes += 1

        now = time.time()
        if now >= next_sample:
            # Read back all values
            s = {'t': now - t0, 'writes': writes}
            for name in ['ias_widget', 'adc_ias1', 'adc_alt1', 'adc_gs', 'hdg_deg']:
                if inj.addrs.get(name):
                    s[name] = inj._read_float(inj.addrs[name])
            samples.append(s)
            next_sample = now + sample_interval

    elapsed = time.time() - t0
    hz = writes / elapsed if elapsed > 0 else 0
    L.log(f"\n  Total: {writes} writes in {elapsed:.1f}s = {hz:.0f} Hz")
    L.log(f"\n  Samples during burst:")
    L.log(f"  {'T':>5s}  {'Writes':>7s}  {'IAS_w':>8s}  {'IAS_a':>8s}  {'ALT':>9s}  {'GS':>8s}  {'HDG':>8s}")
    for s in samples:
        L.log(f"  {s['t']:5.1f}  {s['writes']:7d}"
              f"  {s.get('ias_widget', 0):8.2f}"
              f"  {s.get('adc_ias1', 0):8.2f}"
              f"  {s.get('adc_alt1', 0):9.2f}"
              f"  {s.get('adc_gs', 0):8.2f}"
              f"  {s.get('hdg_deg', 0):8.2f}")

    # Final read
    L.log(f"\n  Final values after burst:")
    for name in sorted(inj.addrs.keys()):
        if inj.addrs[name]:
            if isinstance(inj.addrs[name], list):
                if inj.addrs[name]:
                    val = inj._read_float(inj.addrs[name][0])
                    if val is not None:
                        L.log(f"    {name} (1st):       {val:.4f}")
            else:
                val = inj._read_float(inj.addrs[name])
                if val is not None:
                    L.log(f"    {name:20s}: {val:.4f}")

    # === Summary ===
    L.log(f"\n{'='*70}")
    L.log("SUMMARY")
    L.log(f"{'='*70}")
    L.log(f"  GDU PID:          {inj.pid}")
    L.log(f"  Scan time:        {scan_dt:.1f}s")
    L.log(f"  Addrs found:      {status['addrs_found']}/{status['addrs_total']}")
    L.log(f"  Write freq:       {hz:.0f} Hz")
    L.log(f"  Log saved to:     {log_path}")

    inj.disconnect()
    L.log("\nDone!")
    L.close()

    print(f"\n*** Log saved to: {log_path} ***")
    input("Press Enter to exit...")
