"""
Memory scanner for Garmin GDU flight model variables.
No external dependencies - uses only ctypes + Win32 API.

Usage: python memscan_garmin.py <value> [tolerance]
Example: python memscan_garmin.py 160 5
"""
import ctypes
import ctypes.wintypes as wt
import struct
import sys
import os

# Windows API
kernel32 = ctypes.windll.kernel32
PROCESS_VM_READ = 0x0010
PROCESS_QUERY_INFORMATION = 0x0400
MEM_COMMIT = 0x1000
PAGE_NOACCESS = 0x01
PAGE_GUARD = 0x100
TH32CS_SNAPPROCESS = 0x00000002

class PROCESSENTRY32(ctypes.Structure):
    _fields_ = [
        ("dwSize", wt.DWORD),
        ("cntUsage", wt.DWORD),
        ("th32ProcessID", wt.DWORD),
        ("th32DefaultHeapID", ctypes.POINTER(ctypes.c_ulong)),
        ("th32ModuleID", wt.DWORD),
        ("cntThreads", wt.DWORD),
        ("th32ParentProcessID", wt.DWORD),
        ("pcPriClassBase", ctypes.c_long),
        ("dwFlags", wt.DWORD),
        ("szExeFile", ctypes.c_char * 260),
    ]

class MEMORY_BASIC_INFORMATION(ctypes.Structure):
    _fields_ = [
        ("BaseAddress", ctypes.c_void_p),
        ("AllocationBase", ctypes.c_void_p),
        ("AllocationProtect", wt.DWORD),
        ("RegionSize", ctypes.c_size_t),
        ("State", wt.DWORD),
        ("Protect", wt.DWORD),
        ("Type", wt.DWORD),
    ]

def find_process(name_fragment):
    """Find PID by process name (no psutil needed)."""
    snap = kernel32.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0)
    if snap == -1:
        return None
    pe = PROCESSENTRY32()
    pe.dwSize = ctypes.sizeof(PROCESSENTRY32)
    pids = []
    if kernel32.Process32First(snap, ctypes.byref(pe)):
        while True:
            exe = pe.szExeFile.decode('utf-8', errors='replace').lower()
            if name_fragment.lower() in exe:
                pids.append((pe.th32ProcessID, exe))
            if not kernel32.Process32Next(snap, ctypes.byref(pe)):
                break
    kernel32.CloseHandle(snap)
    return pids

def scan_for_float(pid, target, tolerance):
    """Scan all readable memory in a process for a float value."""
    hProcess = kernel32.OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, False, pid)
    if not hProcess:
        err = ctypes.GetLastError()
        print(f"  ERRO: Nao conseguiu abrir processo (erro {err}). Tente rodar como admin.")
        return []

    results = []
    mbi = MEMORY_BASIC_INFORMATION()
    address = 0x00010000  # Skip null page
    max_addr = 0x7FFE0000  # 32-bit limit
    bytes_total = 0
    regions = 0

    while address < max_addr:
        ret = kernel32.VirtualQueryEx(hProcess, ctypes.c_void_p(address), ctypes.byref(mbi), ctypes.sizeof(mbi))
        if ret == 0:
            address += 0x10000
            continue

        region_end = mbi.BaseAddress + mbi.RegionSize
        
        # Only scan committed, readable memory
        if (mbi.State == MEM_COMMIT and 
            mbi.Protect != 0 and
            not (mbi.Protect & PAGE_NOACCESS) and 
            not (mbi.Protect & PAGE_GUARD) and
            mbi.RegionSize < 50 * 1024 * 1024):
            
            buf = ctypes.create_string_buffer(mbi.RegionSize)
            bytes_read = ctypes.c_size_t(0)
            ok = kernel32.ReadProcessMemory(hProcess, ctypes.c_void_p(mbi.BaseAddress), 
                                           buf, mbi.RegionSize, ctypes.byref(bytes_read))
            if ok and bytes_read.value >= 4:
                data = buf.raw[:bytes_read.value]
                for offset in range(0, len(data) - 3, 4):
                    val = struct.unpack_from('<f', data, offset)[0]
                    if abs(val - target) <= tolerance:
                        addr = mbi.BaseAddress + offset
                        results.append((addr, val))
                bytes_total += bytes_read.value
                regions += 1

        address = region_end

    kernel32.CloseHandle(hProcess)
    print(f"  Scanned {regions} regions, {bytes_total / 1024 / 1024:.1f} MB")
    return results

def read_context(pid, address, before=16, after=80):
    """Read memory around an address and show as floats."""
    hProcess = kernel32.OpenProcess(PROCESS_VM_READ, False, pid)
    if not hProcess:
        return
    start = address - before
    size = before + after
    buf = ctypes.create_string_buffer(size)
    br = ctypes.c_size_t(0)
    if kernel32.ReadProcessMemory(hProcess, ctypes.c_void_p(start), buf, size, ctypes.byref(br)):
        data = buf.raw[:br.value]
        for i in range(0, len(data) - 3, 4):
            fval = struct.unpack_from('<f', data, i)[0]
            marker = " <<<" if start + i == address else ""
            # Only show reasonable float values
            if abs(fval) < 1e10 and abs(fval) > 1e-10 or fval == 0:
                print(f"    0x{start+i:08X} (+{i-before:+3d}): {fval:12.4f}{marker}")
            else:
                ival = struct.unpack_from('<I', data, i)[0]
                print(f"    0x{start+i:08X} (+{i-before:+3d}): (0x{ival:08X}){marker}")
    kernel32.CloseHandle(hProcess)

if __name__ == '__main__':
    if len(sys.argv) < 2:
        print("Usage: python memscan_garmin.py <value> [tolerance]")
        print("Example: python memscan_garmin.py 160 5")
        sys.exit(0)

    target = float(sys.argv[1])
    tolerance = float(sys.argv[2]) if len(sys.argv) > 2 else 1.0
    
    # Find GDU process
    procs = find_process('gdu_7xx')
    if not procs:
        procs = find_process('gtn_sim')
    if not procs:
        print("Garmin simulator not found! Start the trainer first.")
        sys.exit(1)
    
    pid, name = procs[0]
    print(f"Found: {name} (PID={pid})")
    print(f"Scanning for float {target:.1f} +/- {tolerance:.1f}...")
    
    results = scan_for_float(pid, target, tolerance)
    print(f"Found {len(results)} matches")
    
    if len(results) > 200:
        print("Too many matches! Use a tighter tolerance.")
        print("First 20:")
        for addr, val in results[:20]:
            print(f"  0x{addr:08X}: {val:.4f}")
    elif results:
        print(f"\nAll matches:")
        for addr, val in results:
            print(f"  0x{addr:08X}: {val:.4f}")
        
        print(f"\n=== Context around first 3 matches ===")
        for addr, val in results[:3]:
            print(f"\n  Match at 0x{addr:08X} = {val:.4f}:")
            read_context(pid, addr)
