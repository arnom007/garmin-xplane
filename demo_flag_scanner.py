"""
demo_flag_scanner.py — Garmin Kernel Register Scanner

Finds the SYS_SYSTEM_PFD1_DEMO_MODE register in the GDU process memory
by locating tag name strings and following pointer references to find
the actual boolean value.

Saves a detailed log to %APPDATA%/GarminBridge/demo_flag_scan.log

Strategy:
  1. Scan GDU process memory for tag strings (e.g. "SYS_SYSTEM_PFD1_DEMO_MODE")
  2. For each string found, record its runtime address
  3. Scan all memory for 4-byte pointers TO that address (registry entries)
  4. Dump context around each pointer reference
  5. Try to identify and flip boolean values near the registry entries
"""

import ctypes
import ctypes.wintypes as wt
import struct
import time
import sys
import os
import datetime

kernel32 = ctypes.windll.kernel32

# Windows API constants
PROCESS_VM_READ = 0x0010
PROCESS_VM_WRITE = 0x0020
PROCESS_VM_OPERATION = 0x0008
PROCESS_QUERY_INFORMATION = 0x0400
MEM_COMMIT = 0x1000
PAGE_NOACCESS = 0x01
PAGE_GUARD = 0x100
PAGE_READONLY = 0x02
PAGE_READWRITE = 0x04
PAGE_EXECUTE_READ = 0x20
PAGE_EXECUTE_READWRITE = 0x40
PAGE_WRITECOPY = 0x08
TH32CS_SNAPPROCESS = 0x02

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
                result = pe.th32ProcessID
                break
            if not kernel32.Process32Next(snap, ctypes.byref(pe)):
                break
    kernel32.CloseHandle(snap)
    return result


def read_mem(hProcess, addr, size):
    buf = ctypes.create_string_buffer(size)
    br = ctypes.c_size_t(0)
    if kernel32.ReadProcessMemory(hProcess, ctypes.c_void_p(addr), buf, size, ctypes.byref(br)):
        return buf.raw[:br.value]
    return None


def write_mem(hProcess, addr, data):
    bw = ctypes.c_size_t(0)
    return kernel32.WriteProcessMemory(hProcess, ctypes.c_void_p(addr), data, len(data), ctypes.byref(bw))


def read_float(hProcess, addr):
    data = read_mem(hProcess, addr, 4)
    if data and len(data) == 4:
        return struct.unpack('<f', data)[0]
    return None


def write_float(hProcess, addr, val):
    return write_mem(hProcess, addr, struct.pack('<f', val))


def prot_str(p):
    """Human-readable protection flags."""
    flags = []
    if p & PAGE_READONLY: flags.append("R")
    elif p & PAGE_READWRITE: flags.append("RW")
    elif p & PAGE_EXECUTE_READ: flags.append("RX")
    elif p & PAGE_EXECUTE_READWRITE: flags.append("RWX")
    elif p & PAGE_WRITECOPY: flags.append("WC")
    elif p & PAGE_NOACCESS: flags.append("NA")
    else: flags.append(f"0x{p:X}")
    if p & PAGE_GUARD: flags.append("+G")
    return "".join(flags)


# ============================================================
# Main
# ============================================================

def main():
    # Setup logging
    log_dir = os.path.join(os.path.expandvars('%APPDATA%'), 'GarminBridge')
    os.makedirs(log_dir, exist_ok=True)
    log_path = os.path.join(log_dir, 'demo_flag_scan.log')
    log_f = open(log_path, 'w', encoding='utf-8')

    def L(msg=''):
        print(msg)
        log_f.write(msg + '\n')
        log_f.flush()

    L("=" * 70)
    L("  Garmin GDU — Demo Flag Scanner")
    L(f"  {datetime.datetime.now().isoformat()}")
    L("=" * 70)

    # Find GDU
    pid = find_gdu()
    if not pid:
        L("\nERRO: gdu_7xx_simulator.exe nao encontrado!")
        L("Abra o Garmin Trainer primeiro.")
        log_f.close()
        input("Press Enter...")
        return

    L(f"\n  GDU PID: {pid}")

    hProcess = kernel32.OpenProcess(
        PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_QUERY_INFORMATION,
        False, pid
    )
    if not hProcess:
        L(f"  ERRO: Nao foi possivel abrir processo (erro {ctypes.GetLastError()})")
        log_f.close()
        input("Press Enter...")
        return

    # ============================================================
    # PHASE 1: Find tag strings in process memory
    # ============================================================
    L(f"\n{'='*70}")
    L("PHASE 1: Searching for demo-related tag strings in memory")
    L(f"{'='*70}")

    target_tags = [
        b"SYS_DEMO\x00",
        b"SYS_DEMO_CONFIG\x00",
        b"SYS_SYSTEM_PFD1_DEMO_MODE\x00",
        b"SYS_SYSTEM_MFD1_DEMO_MODE\x00",
        b"SYS_SYSTEM_LRU_DEMO_MODE\x00",
        b"IOP_D_DEMO_RESET\x00",
        b"IOP_D_DBG_DEMO_ALT\x00",
        b"IOP_D_DBG_DEMO_HDG\x00",
        b"IOP_D_DBG_DEMO_VEL\x00",
        b"IOP_D_DBG_DEMO_VSPD\x00",
    ]

    # Scan all memory regions
    mbi = MEMORY_BASIC_INFORMATION()
    addr = 0x10000
    all_regions = []  # (base, size, protect, data)
    string_locations = {}  # tag_name -> [(address, region_protect)]

    L("\n  Reading all committed memory regions...")
    t0 = time.time()
    total_bytes = 0

    while addr < 0x7FFE0000:
        if kernel32.VirtualQueryEx(hProcess, ctypes.c_void_p(addr),
                                   ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
            addr += 0x10000
            continue
        if (mbi.State == MEM_COMMIT and mbi.Protect and
            not (mbi.Protect & PAGE_NOACCESS) and
            not (mbi.Protect & PAGE_GUARD) and
            mbi.RegionSize < 50 * 1024 * 1024):
            buf = ctypes.create_string_buffer(mbi.RegionSize)
            br = ctypes.c_size_t(0)
            if kernel32.ReadProcessMemory(hProcess, ctypes.c_void_p(mbi.BaseAddress),
                                          buf, mbi.RegionSize, ctypes.byref(br)):
                data = buf.raw[:br.value]
                all_regions.append((mbi.BaseAddress, len(data), mbi.Protect, data))
                total_bytes += len(data)

                # Search for tag strings in this region
                for tag in target_tags:
                    tag_name = tag.rstrip(b'\x00').decode('ascii')
                    offset = 0
                    while True:
                        idx = data.find(tag, offset)
                        if idx == -1:
                            break
                        found_addr = mbi.BaseAddress + idx
                        if tag_name not in string_locations:
                            string_locations[tag_name] = []
                        string_locations[tag_name].append((found_addr, mbi.Protect))
                        offset = idx + len(tag)

        addr = mbi.BaseAddress + mbi.RegionSize

    dt = time.time() - t0
    L(f"  Scanned {len(all_regions)} regions, {total_bytes/1024/1024:.0f} MB in {dt:.1f}s")

    L(f"\n  Tag strings found:")
    for tag_name, locs in sorted(string_locations.items()):
        for loc_addr, loc_prot in locs:
            L(f"    {tag_name:40s} @ 0x{loc_addr:08X}  [{prot_str(loc_prot)}]")

    if not string_locations:
        L("\n  ERRO: Nenhuma tag string encontrada na memoria!")
        L("  O krnl_sim.dll pode nao estar carregado ainda.")
        kernel32.CloseHandle(hProcess)
        log_f.close()
        input("Press Enter...")
        return

    # ============================================================
    # PHASE 2: Search for pointers TO each string address
    # ============================================================
    L(f"\n{'='*70}")
    L("PHASE 2: Searching for pointer references to tag strings")
    L(f"{'='*70}")
    L("  (Looking for 4-byte values matching each string address)")

    # Focus on the most important tags
    priority_tags = [
        "SYS_DEMO",
        "SYS_DEMO_CONFIG",
        "SYS_SYSTEM_PFD1_DEMO_MODE",
        "SYS_SYSTEM_LRU_DEMO_MODE",
        "IOP_D_DEMO_RESET",
        "IOP_D_DBG_DEMO_ALT",
        "IOP_D_DBG_DEMO_HDG",
        "IOP_D_DBG_DEMO_VEL",
    ]

    pointer_refs = {}  # tag_name -> [(pointer_addr, region_protect)]

    t0 = time.time()
    for tag_name in priority_tags:
        if tag_name not in string_locations:
            continue
        pointer_refs[tag_name] = []
        for str_addr, _ in string_locations[tag_name]:
            # Pack as 32-bit pointer (GDU is 32-bit process)
            needle = struct.pack('<I', str_addr)
            for base, size, prot, data in all_regions:
                offset = 0
                while True:
                    idx = data.find(needle, offset)
                    if idx == -1:
                        break
                    ptr_addr = base + idx
                    pointer_refs[tag_name].append((ptr_addr, prot, str_addr))
                    offset = idx + 4

    dt = time.time() - t0
    L(f"\n  Pointer search completed in {dt:.1f}s")

    L(f"\n  Pointer references found:")
    for tag_name in priority_tags:
        refs = pointer_refs.get(tag_name, [])
        L(f"\n  {tag_name}: {len(refs)} references")
        for ptr_addr, prot, str_addr in refs[:10]:
            L(f"    0x{ptr_addr:08X} [{prot_str(prot)}] -> string @ 0x{str_addr:08X}")

    # ============================================================
    # PHASE 3: Dump context around each pointer reference
    # ============================================================
    L(f"\n{'='*70}")
    L("PHASE 3: Memory context around pointer references")
    L(f"{'='*70}")
    L("  Looking for registry entry structures near each pointer")

    # For the most important tags, dump context
    important_tags = ["SYS_DEMO", "SYS_SYSTEM_PFD1_DEMO_MODE",
                      "SYS_SYSTEM_LRU_DEMO_MODE", "IOP_D_DEMO_RESET",
                      "IOP_D_DBG_DEMO_ALT", "IOP_D_DBG_DEMO_HDG"]

    interesting_entries = []  # (tag_name, ptr_addr, context_data, context_base)

    for tag_name in important_tags:
        refs = pointer_refs.get(tag_name, [])
        if not refs:
            continue
        L(f"\n  --- {tag_name} ---")
        for ptr_addr, prot, str_addr in refs[:5]:
            # Read 64 bytes before and 96 bytes after the pointer
            ctx_start = ptr_addr - 64
            ctx_size = 192
            ctx_data = read_mem(hProcess, ctx_start, ctx_size)
            if not ctx_data:
                continue

            interesting_entries.append((tag_name, ptr_addr, ctx_data, ctx_start))

            L(f"\n  Context around pointer @ 0x{ptr_addr:08X} [{prot_str(prot)}]:")
            L(f"  (pointer to string '{tag_name}' @ 0x{str_addr:08X})")

            for i in range(0, len(ctx_data) - 3, 4):
                real_addr = ctx_start + i
                ival = struct.unpack_from('<I', ctx_data, i)[0]
                fval = struct.unpack_from('<f', ctx_data, i)[0]

                marker = ""
                if real_addr == ptr_addr:
                    marker = " <<< TAG PTR"
                elif ival == 0 or ival == 1:
                    marker = f" <<< BOOL={ival}"
                elif 0 < ival < 0x10000 and ival != str_addr:
                    marker = f" (small int: {ival})"

                # Show as both int and float if float is reasonable
                if abs(fval) < 1e6 and abs(fval) > 1e-6:
                    L(f"    0x{real_addr:08X} ({i-64:+4d}): 0x{ival:08X}  int={ival:10d}  float={fval:12.4f}{marker}")
                else:
                    L(f"    0x{real_addr:08X} ({i-64:+4d}): 0x{ival:08X}  int={ival:10d}{marker}")

    # ============================================================
    # PHASE 4: Look for patterns in the registry structure
    # ============================================================
    L(f"\n{'='*70}")
    L("PHASE 4: Pattern analysis — finding bool values near tag pointers")
    L(f"{'='*70}")

    # For each pointer reference, check offsets -16 to +32 for 0/1 values
    candidate_flags = []  # (tag_name, bool_addr, current_value, offset_from_ptr)

    for tag_name, ptr_addr, ctx_data, ctx_base in interesting_entries:
        ptr_offset_in_ctx = ptr_addr - ctx_base

        # Check various offsets around the tag pointer for boolean values
        for delta in range(-16, 40, 4):
            check_offset = ptr_offset_in_ctx + delta
            if 0 <= check_offset < len(ctx_data) - 3:
                val = struct.unpack_from('<I', ctx_data, check_offset)[0]
                check_addr = ctx_base + check_offset
                if check_addr != ptr_addr and val in (0, 1):
                    candidate_flags.append((tag_name, check_addr, val, delta))

    L(f"\n  Found {len(candidate_flags)} boolean candidates near tag pointers:")
    for tag_name, bool_addr, val, delta in candidate_flags:
        L(f"    {tag_name:40s} @ 0x{bool_addr:08X} = {val} (offset {delta:+d} from tag ptr)")

    # ============================================================
    # PHASE 5: Search for IAS widget (for sentinel test)
    # ============================================================
    L(f"\n{'='*70}")
    L("PHASE 5: Quick IAS widget scan (for verification)")
    L(f"{'='*70}")

    ias_addr = None
    try:
        import numpy as np
        for base, size, prot, data in all_regions:
            if size < 12:
                continue
            trim = size - (size % 4)
            farr = np.frombuffer(data[:trim], dtype=np.float32)
            twenty_idxs = np.where(np.abs(farr - 20.0) < 0.01)[0]
            for idx in twenty_idxs:
                idx = int(idx)
                if 1 <= idx and idx + 1 < len(farr):
                    if abs(float(farr[idx + 1]) - 999.0) < 0.01:
                        ias_val = float(farr[idx - 1])
                        if 0 <= ias_val <= 500:
                            ias_addr = base + (idx - 1) * 4
                            L(f"  IAS widget found at 0x{ias_addr:08X} = {ias_val:.1f} kts")
                            break
            if ias_addr:
                break
    except ImportError:
        L("  numpy not available, skipping IAS scan")

    # ============================================================
    # PHASE 6: Attempt to flip demo flags
    # ============================================================
    L(f"\n{'='*70}")
    L("PHASE 6: Attempting to flip demo mode flags")
    L(f"{'='*70}")

    # Filter candidates: focus on flags that are currently 1 (demo ON)
    # near SYS_DEMO or SYS_SYSTEM_PFD1_DEMO_MODE
    demo_on_flags = [
        (tag, addr, val, delta)
        for tag, addr, val, delta in candidate_flags
        if val == 1 and ("SYS_DEMO" in tag or "DEMO_MODE" in tag)
    ]

    L(f"\n  Demo-related flags currently = 1 (ON):")
    for tag, addr, val, delta in demo_on_flags:
        L(f"    {tag:40s} @ 0x{addr:08X} = {val} (offset {delta:+d})")

    if demo_on_flags and ias_addr:
        L(f"\n  === ATTEMPTING FLAG FLIP ===")
        L(f"  Will flip each flag from 1->0 and check if IAS widget stops being overwritten")

        for tag, flag_addr, val, delta in demo_on_flags:
            L(f"\n  Testing flag: {tag} @ 0x{flag_addr:08X}")

            # Read current IAS
            ias_before = read_float(hProcess, ias_addr)
            L(f"    IAS before: {ias_before:.1f}" if ias_before else "    IAS before: unreadable")

            # Write sentinel to IAS
            write_float(hProcess, ias_addr, 77777.0)

            # Flip flag to 0
            write_mem(hProcess, flag_addr, struct.pack('<I', 0))
            L(f"    Flipped 0x{flag_addr:08X}: 1 -> 0")

            # Wait and check
            time.sleep(0.5)

            # Read IAS back
            ias_after = read_float(hProcess, ias_addr)
            flag_readback = struct.unpack('<I', read_mem(hProcess, flag_addr, 4))[0] if read_mem(hProcess, flag_addr, 4) else None

            L(f"    After 500ms:")
            L(f"      IAS readback:  {ias_after:.1f}" if ias_after else "      IAS readback:  unreadable")
            L(f"      Flag readback: {flag_readback}")

            if ias_after and abs(ias_after - 77777.0) < 1.0:
                L(f"    *** SENTINEL SURVIVED! This flag may control the demo! ***")
                L(f"    *** TAG: {tag} @ 0x{flag_addr:08X} ***")

                # Verify with a longer wait
                time.sleep(2.0)
                ias_verify = read_float(hProcess, ias_addr)
                L(f"    After 2.5s total: IAS = {ias_verify:.1f}" if ias_verify else "    After 2.5s: unreadable")
                if ias_verify and abs(ias_verify - 77777.0) < 1.0:
                    L(f"    *** CONFIRMED: DEMO IS DEAD! ***")
                else:
                    L(f"    Sentinel was eventually overwritten. Not the right flag.")
                    # Restore flag
                    write_mem(hProcess, flag_addr, struct.pack('<I', 1))
                    L(f"    Flag restored to 1")
            else:
                L(f"    Sentinel overwritten immediately. Not the right flag.")
                # Restore flag
                write_mem(hProcess, flag_addr, struct.pack('<I', 1))
                L(f"    Flag restored to 1")

    elif not demo_on_flags:
        L("\n  No flags with value=1 found near demo tags.")
        L("  The register system may use a different structure.")
        L("  Check the PHASE 3 dumps for patterns.")
    elif not ias_addr:
        L("\n  IAS widget not found, cannot verify flag flips.")

    # ============================================================
    # PHASE 7: Alternative — search for SYS_DEMO value directly
    # ============================================================
    L(f"\n{'='*70}")
    L("PHASE 7: Alternative search — SYS_DEMO as inline struct")
    L(f"{'='*70}")
    L("  Searching for 'SYS_DEMO' string in WRITABLE regions (heap)")

    heap_demo_strings = []
    for base, size, prot, data in all_regions:
        # Only writable regions (heap, stack, etc.)
        if not (prot & (PAGE_READWRITE | PAGE_EXECUTE_READWRITE | PAGE_WRITECOPY)):
            continue
        offset = 0
        while True:
            idx = data.find(b"SYS_DEMO", offset)
            if idx == -1:
                break
            found_addr = base + idx
            # Get some context
            ctx_start = max(0, idx - 16)
            ctx_end = min(len(data), idx + 64)
            ctx = data[ctx_start:ctx_end]
            ascii_ctx = ctx.replace(b'\x00', b'.').decode('ascii', errors='replace')
            heap_demo_strings.append((found_addr, prot, ascii_ctx))
            offset = idx + 8

    L(f"  Found {len(heap_demo_strings)} 'SYS_DEMO' strings in writable memory:")
    for addr, prot, ctx in heap_demo_strings[:20]:
        L(f"    0x{addr:08X} [{prot_str(prot)}]: {ctx}")

    # ============================================================
    # SUMMARY
    # ============================================================
    L(f"\n{'='*70}")
    L("SUMMARY")
    L(f"{'='*70}")
    L(f"  GDU PID:                {pid}")
    L(f"  Memory scanned:         {total_bytes/1024/1024:.0f} MB")
    L(f"  Tag strings found:      {sum(len(v) for v in string_locations.values())}")
    L(f"  Pointer references:     {sum(len(v) for v in pointer_refs.values())}")
    L(f"  Boolean candidates:     {len(candidate_flags)}")
    L(f"  Demo-ON flags:          {len(demo_on_flags)}")
    L(f"  IAS widget:             {'0x{:08X}'.format(ias_addr) if ias_addr else 'NOT FOUND'}")
    L(f"  Heap SYS_DEMO strings:  {len(heap_demo_strings)}")
    L(f"  Log saved to:           {log_path}")

    kernel32.CloseHandle(hProcess)
    L("\nDone!")
    log_f.close()
    print(f"\n*** Log saved to: {log_path} ***")
    input("Press Enter to exit...")


if __name__ == '__main__':
    main()
