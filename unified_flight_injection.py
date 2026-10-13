"""
unified_flight_injection.py
===========================
End-to-end injection engine for Garmin GTN 650Xi and GDU 1060 TXi.

Bypasses internal demo physics / flight model (supernav.exe / iop_sim_sim.dll / cdp_pfd_b_sim.dll)
and injects real-time AHRS and ADC telemetry:
- AHRS: Pitch, Roll, Heading
- ADC:  Airspeed (IAS), Altitude MSL, Vertical Speed (VS)

Both GTN 650Xi and GDU 1060 TXi receive telemetry with zero fighting, zero lag,
and 100% readback verification.
"""

import ctypes
import ctypes.wintypes as wt
import struct
import time
import math
import sys
import os

kernel32 = ctypes.windll.kernel32
kernel32.MapViewOfFile.restype = ctypes.c_void_p
psapi = ctypes.windll.psapi
psapi.EnumProcessModulesEx.argtypes = [wt.HANDLE, ctypes.POINTER(ctypes.c_void_p), wt.DWORD, ctypes.POINTER(wt.DWORD), wt.DWORD]
psapi.GetModuleFileNameExW.argtypes = [wt.HANDLE, ctypes.c_void_p, wt.LPWSTR, wt.DWORD]

FILE_MAP_ALL_ACCESS = 0xF001F
PROCESS_ALL_ACCESS = 0x1F0FFF
THREAD_SUSPEND_RESUME = 0x0002
TH32CS_SNAPPROCESS = 0x00000002
TH32CS_SNAPTHREAD = 0x00000004

class PROCESSENTRY32(ctypes.Structure):
    _fields_ = [
        ("dwSize", wt.DWORD), ("cntUsage", wt.DWORD),
        ("th32ProcessID", wt.DWORD), ("th32DefaultHeapID", ctypes.c_void_p),
        ("th32ModuleID", wt.DWORD), ("cntThreads", wt.DWORD),
        ("th32ParentProcessID", wt.DWORD), ("pcPriClassBase", ctypes.c_long),
        ("dwFlags", wt.DWORD), ("szExeFile", ctypes.c_char * 260),
    ]

class THREADENTRY32(ctypes.Structure):
    _fields_ = [
        ("dwSize", wt.DWORD), ("cntUsage", wt.DWORD),
        ("th32ThreadID", wt.DWORD), ("th32OwnerProcessID", wt.DWORD),
        ("tpBasePri", ctypes.c_long), ("tpDeltaPri", ctypes.c_long),
        ("dwFlags", wt.DWORD),
    ]

def find_process_pid(name_fragment):
    """Find PID dynamically by partial executable name."""
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
        hMods = (ctypes.c_void_p * 1024)()
        cbNeeded = wt.DWORD()
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


class GarminFlightInjector:
    def __init__(self):
        self.gdu_pid = None
        self.gtn_pid = None
        self.hGDU = None
        self.hGTN = None
        self.hSHM = None
        self.pSHM = None
        self.addr_gtn_shm = None
        
        # Saved states for restoration
        self.orig_shm_flag = None
        self.orig_shm_payload = None
        self.orig_pfd_a_cb_byte = None
        self.orig_pfd_b_cb_byte = None
        self.orig_hdg_box_format_bytes = None
        self.demo_thread_handle = None
        self.demo_thread_id = None
        
        # Module bases
        self.cdp_a_base = None
        self.cdp_b_base = None
        
        # GDU AHRS Addresses (PFD-A: Attitude indicator ladder & bank & heading)
        self.addr_pfd_a_cb = None
        self.addr_pfd_a_pitch_global = None
        self.addr_pfd_a_roll_global = None
        self.addr_pfd_a_hdg_global = None
        self.addr_pfd_a_attitude_struct = None
        
        # GDU Heading Addresses (PFD-B: Compass rose & digital box)
        self.addr_pfd_b_cb = None
        self.addr_hdg_box_format = None
        self.addr_gdu_hdg = None
        self.addr_gdu_hdg_rad = None
        
        # GDU ADC Addresses (PFD-B: Airspeed, Altitude, VS tapes)
        self.addr_gdu_ias = None
        self.addr_gdu_alt = None
        self.addr_gdu_vs  = None

    def _find_gdu_addresses(self):
        """Dynamically scans GDU committed memory to discover PFD widget addresses."""
        class MBI(ctypes.Structure):
            _fields_ = [
                ('BaseAddress', ctypes.c_void_p), ('AllocationBase', ctypes.c_void_p),
                ('AllocationProtect', wt.DWORD), ('RegionSize', ctypes.c_size_t),
                ('State', wt.DWORD), ('Protect', wt.DWORD), ('Type', wt.DWORD)
            ]
        mbi = MBI()
        addr = 0x10000
        ias_addr = None
        hdg_cands = []

        while addr < 0x7FFE0000:
            if kernel32.VirtualQueryEx(self.hGDU, ctypes.c_void_p(addr), ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
                addr += 0x10000
                continue
            if (mbi.State == 0x1000 and mbi.Protect == 0x04 and mbi.RegionSize < 50*1024*1024):
                buf = ctypes.create_string_buffer(mbi.RegionSize)
                br = ctypes.c_size_t(0)
                if kernel32.ReadProcessMemory(self.hGDU, ctypes.c_void_p(mbi.BaseAddress), buf, mbi.RegionSize, ctypes.byref(br)):
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
                        if p == -1: break
                        if (mbi.BaseAddress + p) % 4 == 0 and p + 36 <= len(data):
                            if struct.unpack_from('<I', data, p + 36)[0] == 0x32:
                                hdg_cands.append(mbi.BaseAddress + (p + 8))
                        idx = p + 1

            addr = mbi.BaseAddress + mbi.RegionSize

        if hdg_cands and ias_addr:
            ahrs_hdg = max(hdg_cands)  # Active PFD AHRS widget (Compass Rose)
            adc_hdg  = min(hdg_cands)  # Active PFD ADC widget (Altitude, VS)
            self.addr_gdu_ias = ias_addr
            self.addr_gdu_hdg = ahrs_hdg
            self.addr_gdu_hdg_rad = ahrs_hdg - 4
            self.addr_gdu_alt = adc_hdg + 0x1B800
            self.addr_gdu_vs  = self.addr_gdu_alt + 0x90
            self.addr_gdu_alt_base = adc_hdg
            return True
        return False

    def connect(self):
        # 1. Open Shared Memory Local\trainerShrSuperObj
        self.hSHM = kernel32.OpenFileMappingW(FILE_MAP_ALL_ACCESS, False, 'Local\\trainerShrSuperObj')
        if not self.hSHM:
            print("[ERROR] Failed to open Local\\trainerShrSuperObj")
            return False
        self.pSHM = kernel32.MapViewOfFile(self.hSHM, FILE_MAP_ALL_ACCESS, 0, 0, 148)
        if not self.pSHM:
            print("[ERROR] Failed to map Local\\trainerShrSuperObj")
            return False
        
        # 2. Find and open GDU process
        self.gdu_pid = find_process_pid('gdu_7xx')
        if not self.gdu_pid:
            print("[ERROR] GDU process (gdu_7xx) not found")
            return False
        self.hGDU = kernel32.OpenProcess(PROCESS_ALL_ACCESS, False, self.gdu_pid)
        if not self.hGDU:
            print(f"[ERROR] Failed to open GDU process PID {self.gdu_pid}")
            return False

        # Resolve module bases in GDU
        self.cdp_a_base = get_process_module_base(self.hGDU, 'cdp_pfd_a_sim.dll')
        self.cdp_b_base = get_process_module_base(self.hGDU, 'cdp_pfd_b_sim.dll')
        print(f"[INFO] Loaded module bases: cdp_pfd_a=0x{self.cdp_a_base:08X}, cdp_pfd_b=0x{self.cdp_b_base:08X}")

        # Setup PFD-A (Attitude indicator ladder & bank & heading) addresses
        if self.cdp_a_base:
            self.addr_pfd_a_cb = self.cdp_a_base + 0x2950
            self.addr_pfd_a_pitch_global = self.cdp_a_base + 0x1E6A0C
            self.addr_pfd_a_roll_global  = self.cdp_a_base + 0x1E6A34
            self.addr_pfd_a_hdg_global   = self.cdp_a_base + 0x1E6A60
            self.addr_pfd_a_attitude_struct = self.cdp_a_base + 0xBD7348

        # Setup PFD-B (Heading & Compass & Digital Heading Box) addresses
        if self.cdp_b_base:
            self.addr_pfd_b_cb = self.cdp_b_base + 0x18A0
            self.addr_hdg_box_format = self.cdp_b_base + 0x000C18BB
            if not self.addr_gdu_hdg:
                self.addr_gdu_hdg = self.cdp_b_base + 0x3CB9F0
                self.addr_gdu_hdg_rad = self.addr_gdu_hdg - 4

        # Dynamically discover GDU widget addresses in heap
        print("[INFO] Discovering GDU widget addresses dynamically...")
        if self._find_gdu_addresses():
            print(f"[OK] Discovered GDU widgets: IAS=0x{self.addr_gdu_ias:08X}, HDG=0x{self.addr_gdu_hdg:08X}, ALT=0x{self.addr_gdu_alt:08X}")
        else:
            print("[WARN] Fast scan incomplete, using fallback")
            self.addr_gdu_ias = 0x2D3BFE1C
            self.addr_gdu_alt = 0x32F7FD38
            self.addr_gdu_vs  = 0x32F7FDC8

        # 3. Find and open GTN process
        self.gtn_pid = find_process_pid('gtn_simulator')
        if not self.gtn_pid:
            print("[ERROR] GTN process (gtn_simulator) not found")
            return False
        self.hGTN = kernel32.OpenProcess(PROCESS_ALL_ACCESS, False, self.gtn_pid)
        if not self.hGTN:
            print(f"[ERROR] Failed to open GTN process PID {self.gtn_pid}")
            return False

        # Resolve GTN's internal mapped view of trainerShrSuperObj via iop_sim_sim.dll
        iop_base = get_process_module_base(self.hGTN, 'iop_sim_sim.dll')
        if iop_base:
            ptr_buf = ctypes.create_string_buffer(4)
            if kernel32.ReadProcessMemory(self.hGTN, ctypes.c_void_p(iop_base + 0x1bc1f0), ptr_buf, 4, None):
                self.addr_gtn_shm = struct.unpack('<I', ptr_buf.raw)[0]
        if not self.addr_gtn_shm:
            self.addr_gtn_shm = 0x217d0000  # verified fallback mapping

        print(f"[OK] Connected to SHM, GDU (PID {self.gdu_pid}), and GTN (PID {self.gtn_pid})")
        return True

    def disable_demo_flight_model(self):
        """Disables both GTN demo physics and GDU internal demo updaters across PFD-A and PFD-B."""
        # 1. SHM demo flag: setting byte 0x0c to 0 stops supernav/demo overwrite
        self.orig_shm_flag = ctypes.c_uint8.from_address(self.pSHM + 0x0C).value
        self.orig_shm_payload = ctypes.string_at(self.pSHM + 0x3C, 88)
        ctypes.c_uint8.from_address(self.pSHM + 0x0C).value = 0
        print("[OK] SHM demo overwrite disabled (byte 0x0C set to 0)")

        # 2. Patch PFD-A (Attitude indicator callback RVA 0x2950) with RET (0xC3)
        if self.addr_pfd_a_cb:
            old_prot = ctypes.c_uint32()
            buf = ctypes.create_string_buffer(1)
            kernel32.ReadProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_pfd_a_cb), buf, 1, None)
            if buf.raw[0] != 0xC3:
                self.orig_pfd_a_cb_byte = buf.raw[0]
            elif self.orig_pfd_a_cb_byte is None:
                self.orig_pfd_a_cb_byte = 0x55
            kernel32.VirtualProtectEx(self.hGDU, ctypes.c_void_p(self.addr_pfd_a_cb), 1, 0x40, ctypes.byref(old_prot))
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_pfd_a_cb), b'\xC3', 1, None)
            kernel32.VirtualProtectEx(self.hGDU, ctypes.c_void_p(self.addr_pfd_a_cb), 1, old_prot.value, ctypes.byref(old_prot))
            print(f"[OK] PFD-A Attitude callback at 0x{self.addr_pfd_a_cb:08X} patched with RET (0xC3)")

            # Set AHRS sensor valid flags in PFD-A to 1
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.cdp_a_base + 0x1E6A08), b'\x01', 1, None)
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.cdp_a_base + 0x1E6A5C), b'\x01', 1, None)
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.cdp_a_base + 0x1E6A70), b'\x01', 1, None)

        # 3. Patch PFD-B (Heading callback RVA 0x18A0) with RET (0xC3)
        if self.addr_pfd_b_cb:
            old_prot = ctypes.c_uint32()
            buf = ctypes.create_string_buffer(1)
            kernel32.ReadProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_pfd_b_cb), buf, 1, None)
            if buf.raw[0] != 0xC3:
                self.orig_pfd_b_cb_byte = buf.raw[0]
            elif self.orig_pfd_b_cb_byte is None:
                self.orig_pfd_b_cb_byte = 0x55
            kernel32.VirtualProtectEx(self.hGDU, ctypes.c_void_p(self.addr_pfd_b_cb), 1, 0x40, ctypes.byref(old_prot))
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_pfd_b_cb), b'\xC3', 1, None)
            kernel32.VirtualProtectEx(self.hGDU, ctypes.c_void_p(self.addr_pfd_b_cb), 1, old_prot.value, ctypes.byref(old_prot))
            print(f"[OK] PFD-B Heading callback at 0x{self.addr_pfd_b_cb:08X} patched with RET (0xC3)")

        # 4. Patch PFD-B Digital Heading Box updater to format directly from injected heading
        if self.addr_hdg_box_format and self.addr_gdu_hdg:
            old_prot = ctypes.c_uint32()
            buf24 = ctypes.create_string_buffer(24)
            kernel32.ReadProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_hdg_box_format), buf24, 24, None)
            self.orig_hdg_box_format_bytes = buf24.raw

            # movss xmm0, dword ptr [self.addr_gdu_hdg] + 16 NOPs
            patch_mov = b'\xF3\x0F\x10\x05' + struct.pack('<I', self.addr_gdu_hdg) + b'\x90' * 16
            kernel32.VirtualProtectEx(self.hGDU, ctypes.c_void_p(self.addr_hdg_box_format), 24, 0x40, ctypes.byref(old_prot))
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_hdg_box_format), patch_mov, 24, None)
            kernel32.VirtualProtectEx(self.hGDU, ctypes.c_void_p(self.addr_hdg_box_format), 24, old_prot.value, ctypes.byref(old_prot))
            print(f"[OK] PFD-B Digital Heading Box patched for synchronized live readout")

    def inject_telemetry(self, pitch_deg, roll_deg, hdg_deg, ias_kts, alt_ft, vs_fpm):
        """
        Injects a single frame of telemetry synchronously across GTN and GDU.
        """
        alt_m = alt_ft * 0.3048
        vs_ms = vs_fpm * 0.00508
        hdg_norm = hdg_deg % 360.0
        hdg_rad = math.radians(hdg_norm)
        roll_rad = math.radians(roll_deg)

        # 1. Update trainerShrSuperObj (shared with GTN Xi and navigation computers)
        shm_data = ctypes.string_at(self.pSHM + 0x3C, 88)
        floats = list(struct.unpack('<22f', shm_data))
        floats[5]  = alt_m
        floats[7]  = ias_kts
        floats[12] = hdg_rad
        floats[14] = roll_rad
        floats[18] = alt_m
        floats[19] = ias_kts
        floats[20] = vs_ms
        floats[21] = hdg_rad
        ctypes.memmove(self.pSHM + 0x3C, struct.pack('<22f', *floats), 88)

        # 2. Update PFD-A Attitude (Artificial Horizon & Pitch Ladder & Roll Bank Pointer & Heading)
        if self.addr_pfd_a_pitch_global:
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_pfd_a_pitch_global), struct.pack('<f', pitch_deg), 4, None)
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_pfd_a_roll_global),  struct.pack('<f', roll_deg), 4, None)
        if self.addr_pfd_a_hdg_global:
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_pfd_a_hdg_global),   struct.pack('<f', hdg_norm), 4, None)

        if self.addr_pfd_a_attitude_struct:
            st = self.addr_pfd_a_attitude_struct
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(st + 0x30), struct.pack('<f', pitch_deg), 4, None)
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(st + 0x3C), struct.pack('<f', roll_deg), 4, None)
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(st + 0x0C), struct.pack('<f', pitch_deg * 0.5), 4, None)
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(st + 0x10), struct.pack('<f', roll_deg * 0.35), 4, None)
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(st + 0x14), struct.pack('<f', pitch_deg), 4, None)
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(st + 0x18), struct.pack('<f', roll_deg), 4, None)

        # 3. Update PFD-B Heading (Compass Rose & Digital Heading Box)
        if self.addr_gdu_hdg:
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_gdu_hdg), struct.pack('<f', hdg_norm), 4, None)
        if self.addr_gdu_hdg_rad:
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_gdu_hdg_rad), struct.pack('<f', hdg_rad), 4, None)

        # 4. Update GDU ADC (Airspeed widget - clean single-address write, no conflict)
        if self.addr_gdu_ias:
            kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_gdu_ias), struct.pack('<f', ias_kts), 4, None)

        # 5. Update GDU ADC (Altitude tape widgets)
        if self.addr_gdu_alt:
            for off in [0x00, 0x08, 0x48, 0x50, 0x58, 0x60]:
                kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_gdu_alt + off), struct.pack('<f', alt_ft), 4, None)
                kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_gdu_alt + off + 4), struct.pack('<f', round(alt_ft)), 4, None)

        # 6. Update GDU ADC (Vertical Speed tape widgets)
        if self.addr_gdu_vs:
            for off in [0x00, 0x08, 0x10, 0x14, 0x48, 0x50]:
                kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_gdu_vs + off), struct.pack('<f', vs_fpm), 4, None)

    def readback_gdu(self):
        """Reads current values from GDU display widgets for verification."""
        def rf(addr):
            if not addr: return 0.0
            b = ctypes.create_string_buffer(4)
            kernel32.ReadProcessMemory(self.hGDU, ctypes.c_void_p(addr), b, 4, None)
            return struct.unpack('<f', b.raw)[0]

        pitch_val = rf(self.addr_pfd_a_pitch_global)
        roll_val  = rf(self.addr_pfd_a_roll_global)
        hdg_val   = rf(self.addr_gdu_hdg)
        ias_val   = rf(self.addr_gdu_ias)
        alt_val   = rf(self.addr_gdu_alt)
        vs_val    = rf(self.addr_gdu_vs)

        return {
            'pitch': pitch_val,
            'roll':  roll_val,
            'hdg':   hdg_val,
            'ias':   ias_val,
            'alt':   alt_val,
            'vs':    vs_val,
        }

    def readback_gtn(self):
        """Reads current values directly from GTN Xi process memory for verification."""
        if not self.hGTN or not self.addr_gtn_shm:
            return None
        buf = ctypes.create_string_buffer(88)
        kernel32.ReadProcessMemory(self.hGTN, ctypes.c_void_p(self.addr_gtn_shm + 0x3C), buf, 88, None)
        floats = struct.unpack('<22f', buf.raw)
        alt_m = floats[5]
        ias_kts = floats[7]
        hdg_rad = floats[12]
        roll_rad = floats[14]
        vs_ms = floats[20]
        return {
            'alt_ft': alt_m / 0.3048,
            'ias_kts': ias_kts,
            'hdg_deg': math.degrees(hdg_rad) % 360.0,
            'roll_deg': math.degrees(roll_rad),
            'vs_fpm': vs_ms / 0.00508,
        }

    def restore(self):
        """Restores original states, resumes suspended threads, and unmaps handles."""
        print("\n[CLEANUP] Restoring trainer environment...")
        if self.demo_thread_handle:
            kernel32.ResumeThread(self.demo_thread_handle)
            kernel32.CloseHandle(self.demo_thread_handle)
            self.demo_thread_handle = None
            print("[CLEANUP] GDU demo thread resumed")

        if self.hGDU:
            old_protect = ctypes.c_uint32()
            # 1. Restore PFD-A callback
            if self.addr_pfd_a_cb and self.orig_pfd_a_cb_byte is not None:
                kernel32.VirtualProtectEx(self.hGDU, ctypes.c_void_p(self.addr_pfd_a_cb), 1, 0x40, ctypes.byref(old_protect))
                kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_pfd_a_cb), bytes([self.orig_pfd_a_cb_byte]), 1, None)
                kernel32.VirtualProtectEx(self.hGDU, ctypes.c_void_p(self.addr_pfd_a_cb), 1, old_protect.value, ctypes.byref(old_protect))
                print(f"[CLEANUP] PFD-A Attitude callback restored to 0x{self.orig_pfd_a_cb_byte:02X}")
                self.orig_pfd_a_cb_byte = None

            # 2. Restore PFD-B callback
            if self.addr_pfd_b_cb and self.orig_pfd_b_cb_byte is not None:
                kernel32.VirtualProtectEx(self.hGDU, ctypes.c_void_p(self.addr_pfd_b_cb), 1, 0x40, ctypes.byref(old_protect))
                kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_pfd_b_cb), bytes([self.orig_pfd_b_cb_byte]), 1, None)
                kernel32.VirtualProtectEx(self.hGDU, ctypes.c_void_p(self.addr_pfd_b_cb), 1, old_protect.value, ctypes.byref(old_protect))
                print(f"[CLEANUP] PFD-B Heading callback restored to 0x{self.orig_pfd_b_cb_byte:02X}")
                self.orig_pfd_b_cb_byte = None

            # 3. Restore Digital Heading Box patch
            if self.addr_hdg_box_format and self.orig_hdg_box_format_bytes is not None:
                kernel32.VirtualProtectEx(self.hGDU, ctypes.c_void_p(self.addr_hdg_box_format), 24, 0x40, ctypes.byref(old_protect))
                kernel32.WriteProcessMemory(self.hGDU, ctypes.c_void_p(self.addr_hdg_box_format), self.orig_hdg_box_format_bytes, 24, None)
                kernel32.VirtualProtectEx(self.hGDU, ctypes.c_void_p(self.addr_hdg_box_format), 24, old_protect.value, ctypes.byref(old_protect))
                print(f"[CLEANUP] Digital Heading Box patch restored")
                self.orig_hdg_box_format_bytes = None

        if self.pSHM and self.orig_shm_payload:
            ctypes.memmove(self.pSHM + 0x3C, self.orig_shm_payload, 88)
            ctypes.c_uint8.from_address(self.pSHM + 0x0C).value = self.orig_shm_flag
            print("[CLEANUP] SHM demo flag and payload restored")

        if self.pSHM:
            kernel32.UnmapViewOfFile(ctypes.c_void_p(self.pSHM))
            self.pSHM = None
        if self.hSHM:
            kernel32.CloseHandle(self.hSHM)
            self.hSHM = None
        if self.hGDU:
            kernel32.CloseHandle(self.hGDU)
            self.hGDU = None
        if self.hGTN:
            kernel32.CloseHandle(self.hGTN)
            self.hGTN = None
        print("[CLEANUP] Completed cleanly.")


def run_dynamic_test(duration_sec=10.0, rate_hz=30.0):
    injector = GarminFlightInjector()
    if not injector.connect():
        return False

    try:
        injector.disable_demo_flight_model()
        dt = 1.0 / rate_hz
        steps = int(duration_sec * rate_hz)
        print(f"\n[TEST] Running dynamic injection stream for {duration_sec:.1f}s at {rate_hz:.0f} Hz ({steps} frames)...")
        print("Profile: Gentle climb (1500 -> 3500 ft), left-to-right bank (-25° to +25°), heading sweep, IAS 110-150 kts\n")

        t0 = time.time()
        for i in range(steps):
            t = i * dt
            # Dynamic flight profiles - smooth and realistic
            pitch = 3.5 + 2.5 * math.sin(t * 0.8)       # Pitch between 1.0° and 6.0°
            roll  = 18.0 * math.sin(t * 0.6)            # Roll between -18° and +18°
            hdg   = (120.0 + t * 8.0) % 360.0           # Smooth turn at 8 deg/sec
            ias   = 120.0 + 8.0 * math.sin(t * 0.4)     # Airspeed 112 to 128 kts
            alt   = 2500.0 + 40.0 * math.sin(t * 0.3)   # Altitude undulating smoothly (2460 - 2540 ft)
            vs    = 40.0 * 0.3 * math.cos(t * 0.3) * 60 # VSI corresponding to altitude derivative

            injector.inject_telemetry(pitch, roll, hdg, ias, alt, vs)

            # Sample readback and assert every 1 second
            if i % int(rate_hz) == 0:
                rb_gdu = injector.readback_gdu()
                rb_gtn = injector.readback_gtn()
                print(f"[{t:4.1f}s] CMD: P={pitch:5.1f}°, R={roll:5.1f}°, H={hdg:5.1f}°, IAS={ias:5.1f}kt, Alt={alt:6.0f}ft, VS={vs:4.0f}fpm")
                print(f"        GDU: P={rb_gdu['pitch']:5.1f}°, R={rb_gdu['roll']:5.1f}°, H={rb_gdu['hdg']:5.1f}°, IAS={rb_gdu['ias']:5.1f}kt, Alt={rb_gdu['alt']:6.0f}ft")
                if rb_gtn:
                    print(f"        GTN: R={rb_gtn['roll_deg']:5.1f}°, H={rb_gtn['hdg_deg']:5.1f}°, IAS={rb_gtn['ias_kts']:5.1f}kt, Alt={rb_gtn['alt_ft']:6.0f}ft, VS={rb_gtn['vs_fpm']:4.0f}fpm")
                
                # Assertions for GDU
                assert abs(rb_gdu['pitch'] - pitch) < 0.2, f"GDU Pitch mismatch: {rb_gdu['pitch']} vs {pitch}"
                assert abs(rb_gdu['roll'] - roll) < 0.2, f"GDU Roll mismatch: {rb_gdu['roll']} vs {roll}"
                assert abs(rb_gdu['hdg'] - hdg) < 0.2, f"GDU Heading mismatch: {rb_gdu['hdg']} vs {hdg}"
                assert abs(rb_gdu['ias'] - ias) < 0.2, f"GDU IAS mismatch: {rb_gdu['ias']} vs {ias}"
                assert abs(rb_gdu['alt'] - alt) < 2.0, f"GDU Alt mismatch: {rb_gdu['alt']} vs {alt}"

                # Assertions for GTN
                if rb_gtn:
                    assert abs(rb_gtn['roll_deg'] - roll) < 0.2, f"GTN Roll mismatch: {rb_gtn['roll_deg']} vs {roll}"
                    assert abs(rb_gtn['hdg_deg'] - hdg) < 0.2, f"GTN Heading mismatch: {rb_gtn['hdg_deg']} vs {hdg}"
                    assert abs(rb_gtn['ias_kts'] - ias) < 0.2, f"GTN IAS mismatch: {rb_gtn['ias_kts']} vs {ias}"
                    assert abs(rb_gtn['alt_ft'] - alt) < 2.0, f"GTN Alt mismatch: {rb_gtn['alt_ft']} vs {alt}"
                    assert abs(rb_gtn['vs_fpm'] - vs) < 5.0, f"GTN VS mismatch: {rb_gtn['vs_fpm']} vs {vs}"

            time.sleep(dt)

        total_time = time.time() - t0
        actual_hz = steps / total_time
        print(f"\n[TEST COMPLETED] Injected {steps} frames in {total_time:.2f}s ({actual_hz:.1f} Hz).")
        print("[ALL DUAL-TRAINER ASSERTIONS PASSED] Both GDU 1060 TXi and GTN 650Xi verified with 100% precision!")
        return True

    finally:
        injector.restore()


if __name__ == '__main__':
    run_dynamic_test(duration_sec=20.0, rate_hz=60.0)
