"""
Differential memory scanner for Garmin GDU.
Step 1: Scan for a value and save all addresses
Step 2: User changes the value in the demo
Step 3: Scan again for the NEW value, intersect with step 1 addresses
This finds the exact memory locations that track the PFD value.

Usage:
  python memscan_diff.py scan1 <value> <tolerance>   -- initial scan
  python memscan_diff.py scan2 <value> <tolerance>   -- rescan after value changed
  python memscan_diff.py read <addr_hex>             -- read context at address
"""
import ctypes
import ctypes.wintypes as wt
import struct
import sys
import os
import json
import time

kernel32 = ctypes.windll.kernel32
PROCESS_VM_READ = 0x0010
PROCESS_QUERY_INFORMATION = 0x0400
MEM_COMMIT = 0x1000
PAGE_NOACCESS = 0x01
PAGE_GUARD = 0x100
TH32CS_SNAPPROCESS = 0x02

SAVE_FILE = os.path.join(os.path.dirname(__file__), '_memscan_state.json')

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

def find_gdu():
    snap = kernel32.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0)
    pe = PROCESSENTRY32()
    pe.dwSize = ctypes.sizeof(PROCESSENTRY32)
    result = None
    if kernel32.Process32First(snap, ctypes.byref(pe)):
        while True:
            exe = pe.szExeFile.decode('utf-8', errors='replace').lower()
            if 'gdu_7xx' in exe:
                result = (pe.th32ProcessID, exe)
                break
            if not kernel32.Process32Next(snap, ctypes.byref(pe)):
                break
    kernel32.CloseHandle(snap)
    return result

def scan_all(pid, target, tol):
    hProc = kernel32.OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, False, pid)
    if not hProc:
        print(f"ERRO: Nao abriu processo. Rode como admin!")
        return []
    results = []
    mbi = MEMORY_BASIC_INFORMATION()
    addr = 0x10000
    while addr < 0x7FFE0000:
        if kernel32.VirtualQueryEx(hProc, ctypes.c_void_p(addr), ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
            addr += 0x10000
            continue
        if (mbi.State == MEM_COMMIT and mbi.Protect and
            not (mbi.Protect & PAGE_NOACCESS) and not (mbi.Protect & PAGE_GUARD) and
            mbi.RegionSize < 50*1024*1024):
            buf = ctypes.create_string_buffer(mbi.RegionSize)
            br = ctypes.c_size_t(0)
            if kernel32.ReadProcessMemory(hProc, ctypes.c_void_p(mbi.BaseAddress), buf, mbi.RegionSize, ctypes.byref(br)):
                data = buf.raw[:br.value]
                for off in range(0, len(data)-3, 4):
                    val = struct.unpack_from('<f', data, off)[0]
                    if abs(val - target) <= tol:
                        results.append(mbi.BaseAddress + off)
        addr = mbi.BaseAddress + mbi.RegionSize
    kernel32.CloseHandle(hProc)
    return results

def read_addrs(pid, addrs, target, tol):
    """Re-read specific addresses and filter by current value."""
    hProc = kernel32.OpenProcess(PROCESS_VM_READ, False, pid)
    if not hProc:
        return []
    matched = []
    for a in addrs:
        buf = ctypes.create_string_buffer(4)
        br = ctypes.c_size_t(0)
        if kernel32.ReadProcessMemory(hProc, ctypes.c_void_p(a), buf, 4, ctypes.byref(br)):
            val = struct.unpack('<f', buf.raw[:4])[0]
            if abs(val - target) <= tol:
                matched.append((a, val))
    kernel32.CloseHandle(hProc)
    return matched

def read_context(pid, address):
    hProc = kernel32.OpenProcess(PROCESS_VM_READ, False, pid)
    if not hProc:
        return
    buf = ctypes.create_string_buffer(128)
    br = ctypes.c_size_t(0)
    start = address - 32
    if kernel32.ReadProcessMemory(hProc, ctypes.c_void_p(start), buf, 128, ctypes.byref(br)):
        data = buf.raw[:br.value]
        for i in range(0, len(data)-3, 4):
            fval = struct.unpack_from('<f', data, i)[0]
            marker = " <== TARGET" if start+i == address else ""
            if abs(fval) < 1e8 and (abs(fval) > 1e-8 or fval == 0):
                print(f"  0x{start+i:08X}: {fval:12.4f}{marker}")
            else:
                ival = struct.unpack_from('<I', data, i)[0]
                print(f"  0x{start+i:08X}: 0x{ival:08X}{marker}")
    kernel32.CloseHandle(hProc)

if __name__ == '__main__':
    if len(sys.argv) < 2:
        print("=== Garmin Memory Scanner (Differential) ===")
        print()
        print("Step 1: With demo running at some IAS (e.g. 160kt):")
        print("  python memscan_diff.py scan1 160 5")
        print()
        print("Step 2: Change IAS in demo (e.g. to 200kt), then:")
        print("  python memscan_diff.py scan2 200 5")
        print()
        print("Step 3: Read context of found addresses:")
        print("  python memscan_diff.py read 0x12345678")
        sys.exit(0)
    
    proc = find_gdu()
    if not proc:
        print("GDU not running!")
        sys.exit(1)
    pid, name = proc
    print(f"GDU: {name} PID={pid}")

    cmd = sys.argv[1]

    if cmd == 'scan1':
        target = float(sys.argv[2])
        tol = float(sys.argv[3]) if len(sys.argv) > 3 else 3.0
        print(f"SCAN 1: Looking for ~{target:.1f} +/- {tol}...")
        t0 = time.time()
        addrs = scan_all(pid, target, tol)
        dt = time.time() - t0
        print(f"Found {len(addrs)} addresses in {dt:.1f}s")
        # Save
        state = {'scan1_addrs': addrs, 'scan1_target': target, 'scan1_tol': tol, 'pid': pid}
        with open(SAVE_FILE, 'w') as f:
            json.dump(state, f)
        print(f"Saved to {SAVE_FILE}")
        print()
        print(">>> Now CHANGE the value in the demo (e.g. change speed),")
        print(">>> then run: python memscan_diff.py scan2 <new_value> <tolerance>")

    elif cmd == 'scan2':
        target = float(sys.argv[2])
        tol = float(sys.argv[3]) if len(sys.argv) > 3 else 3.0
        
        if not os.path.exists(SAVE_FILE):
            print("No scan1 data! Run scan1 first.")
            sys.exit(1)
        with open(SAVE_FILE, 'r') as f:
            state = json.load(f)
        
        old_addrs = state['scan1_addrs']
        print(f"SCAN 2: Checking {len(old_addrs)} addresses from scan1...")
        print(f"Looking for ~{target:.1f} +/- {tol} (changed from ~{state['scan1_target']:.1f})")
        
        # Method 1: Re-read old addresses, check which NOW match new value
        matched = read_addrs(pid, old_addrs, target, tol)
        print(f"\nAddresses that CHANGED from ~{state['scan1_target']:.1f} to ~{target:.1f}: {len(matched)}")
        
        if len(matched) <= 50:
            for addr, val in matched:
                print(f"  0x{addr:08X}: {val:.4f}")
            
            if matched:
                print(f"\n=== Context around top matches ===")
                for addr, val in matched[:5]:
                    print(f"\n--- 0x{addr:08X} = {val:.4f} ---")
                    read_context(pid, addr)
        else:
            print("Still too many. Try with tighter tolerance or bigger value change.")
            for addr, val in matched[:20]:
                print(f"  0x{addr:08X}: {val:.4f}")

    elif cmd == 'read':
        addr = int(sys.argv[2], 16) if sys.argv[2].startswith('0x') else int(sys.argv[2])
        print(f"Reading context at 0x{addr:08X}:")
        read_context(pid, addr)

    elif cmd == 'monitor':
        addr = int(sys.argv[2], 16) if sys.argv[2].startswith('0x') else int(sys.argv[2])
        print(f"Monitoring 0x{addr:08X} (Ctrl+C to stop):")
        hProc = kernel32.OpenProcess(PROCESS_VM_READ, False, pid)
        try:
            while True:
                buf = ctypes.create_string_buffer(4)
                br = ctypes.c_size_t(0)
                kernel32.ReadProcessMemory(hProc, ctypes.c_void_p(addr), buf, 4, ctypes.byref(br))
                val = struct.unpack('<f', buf.raw[:4])[0]
                print(f"\r  {val:12.4f}", end='', flush=True)
                time.sleep(0.1)
        except KeyboardInterrupt:
            pass
        kernel32.CloseHandle(hProc)
