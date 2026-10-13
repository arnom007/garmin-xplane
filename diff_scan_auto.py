"""
Auto Differential Scanner v2 — Memory-efficient
Saves raw snapshots to disk, compares region-by-region.
Only stores addresses of interesting floats (altitude/heading/IAS range).
Saves log to %APPDATA%/GarminBridge/diff_scan.log
"""
import ctypes, ctypes.wintypes as wt, struct, os, sys, time, datetime, math, tempfile

kernel32 = ctypes.windll.kernel32
PROCESS_VM_READ = 0x0010
PROCESS_VM_WRITE = 0x0020
PROCESS_VM_OPERATION = 0x0008
PROCESS_QUERY_INFORMATION = 0x0400
TH32CS_SNAPPROCESS = 0x02

class PE32(ctypes.Structure):
    _fields_ = [("dwSize", wt.DWORD),("cntUsage", wt.DWORD),("th32ProcessID", wt.DWORD),
        ("th32DefaultHeapID", ctypes.c_void_p),("th32ModuleID", wt.DWORD),("cntThreads", wt.DWORD),
        ("th32ParentProcessID", wt.DWORD),("pcPriClassBase", ctypes.c_long),
        ("dwFlags", wt.DWORD),("szExeFile", ctypes.c_char * 260)]

class MBI(ctypes.Structure):
    _fields_ = [("BaseAddress", ctypes.c_void_p),("AllocationBase", ctypes.c_void_p),
        ("AllocationProtect", wt.DWORD),("RegionSize", ctypes.c_size_t),
        ("State", wt.DWORD),("Protect", wt.DWORD),("Type", wt.DWORD)]

def find_gdu():
    snap = kernel32.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0)
    pe = PE32(); pe.dwSize = ctypes.sizeof(PE32)
    r = None
    if kernel32.Process32First(snap, ctypes.byref(pe)):
        while True:
            exe = pe.szExeFile.decode('utf-8','replace').lower()
            if 'gdu_7xx' in exe: r = pe.th32ProcessID; break
            if not kernel32.Process32Next(snap, ctypes.byref(pe)): break
    kernel32.CloseHandle(snap)
    return r

# Log setup
log_dir = os.path.join(os.path.expandvars('%APPDATA%'), 'GarminBridge')
os.makedirs(log_dir, exist_ok=True)
log_path = os.path.join(log_dir, 'diff_scan.log')
log_f = open(log_path, 'w', encoding='utf-8')

def L(msg=''):
    print(msg)
    log_f.write(msg + '\n')
    log_f.flush()

def read_region(h, addr, size):
    buf = ctypes.create_string_buffer(size)
    br = ctypes.c_size_t(0)
    if kernel32.ReadProcessMemory(h, ctypes.c_void_p(addr), buf, size, ctypes.byref(br)):
        return buf.raw[:br.value]
    return None

def read_float(h, addr):
    d = read_region(h, addr, 4)
    if d and len(d) == 4:
        return struct.unpack('<f', d)[0]
    return None

def write_float(h, addr, val):
    data = struct.pack('<f', val)
    bw = ctypes.c_size_t(0)
    ok = kernel32.WriteProcessMemory(h, ctypes.c_void_p(addr), data, 4, ctypes.byref(bw))
    return ok and bw.value == 4

def get_regions(h):
    """Get list of readable memory regions."""
    regions = []
    mbi = MBI()
    addr = 0x10000
    while addr < 0x7FFE0000:
        if kernel32.VirtualQueryEx(h, ctypes.c_void_p(addr), ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
            addr += 0x10000
            continue
        if (mbi.State == 0x1000 and mbi.Protect and
            not (mbi.Protect & 0x01) and not (mbi.Protect & 0x100) and
            mbi.RegionSize < 50*1024*1024):
            regions.append((mbi.BaseAddress, mbi.RegionSize))
        addr = mbi.BaseAddress + mbi.RegionSize
    return regions

def snapshot_filtered(h, regions):
    """Only store floats in ranges we care about: 0-500 (IAS/HDG) and 100-60000 (ALT)."""
    result = {}
    for base, size in regions:
        buf = ctypes.create_string_buffer(size)
        br = ctypes.c_size_t(0)
        if kernel32.ReadProcessMemory(h, ctypes.c_void_p(base), buf, size, ctypes.byref(br)):
            data = buf.raw[:br.value]
            for off in range(0, len(data) - 3, 4):
                val = struct.unpack_from('<f', data, off)[0]
                # Only store if in a useful range
                if -1000 < val < 60000 and (abs(val) > 0.01 or val == 0):
                    # Skip very common values (0.0, 1.0) unless nearby has structure
                    if abs(val) < 0.1:
                        continue
                    result[base + off] = val
    return result

def find_ias_widget(h, regions):
    """Find IAS widget by [value, 20.0, 999.0] signature."""
    twenty_b = struct.pack('<f', 20.0)
    nine99_b = struct.pack('<f', 999.0)
    for base, size in regions:
        buf = ctypes.create_string_buffer(size)
        br = ctypes.c_size_t(0)
        if kernel32.ReadProcessMemory(h, ctypes.c_void_p(base), buf, size, ctypes.byref(br)):
            data = buf.raw[:br.value]
            p = 0
            while True:
                idx = data.find(twenty_b, p)
                if idx == -1: break
                if idx + 8 <= len(data) and idx >= 4:
                    if data[idx+4:idx+8] == nine99_b:
                        ias_val = struct.unpack_from('<f', data, idx-4)[0]
                        if 0 < ias_val < 500:
                            return base + idx - 4, ias_val
                p = idx + 4
    return None, None

# ================================================================
L("=" * 70)
L("  Differential Scanner v2 — ALT/HDG Address Finder")
L("  {}".format(datetime.datetime.now().isoformat()))
L("  Memory-efficient: filters floats before storing")
L("=" * 70)

pid = find_gdu()
if not pid:
    L("ERROR: GDU not running!")
    log_f.close()
    input("Press Enter...")
    sys.exit(1)

h = kernel32.OpenProcess(
    PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_QUERY_INFORMATION,
    False, pid)
L("GDU PID={}".format(pid))

# Map memory regions
L("\n--- Mapping memory regions ---")
regions = get_regions(h)
total_mb = sum(s for _, s in regions) / (1024*1024)
L("{} regions, {:.0f} MB total".format(len(regions), total_mb))

# Find IAS widget
L("\n--- Finding IAS widget ---")
ias_addr, ias_val = find_ias_widget(h, regions)
if ias_addr:
    L("IAS widget: 0x{:08X} = {:.1f} kts".format(ias_addr, ias_val))
else:
    L("WARNING: IAS widget not found!")

# ================================================================
# PHASE 1: Snapshot 1 (filtered)
# ================================================================
L("\n" + "=" * 70)
L("PHASE 1: Taking filtered snapshot #1...")
t0 = time.time()
snap1 = snapshot_filtered(h, regions)
dt1 = time.time() - t0
L("Snapshot 1: {} values in {:.1f}s (~{:.0f} MB RAM)".format(
    len(snap1), dt1, len(snap1) * 40 / (1024*1024)))

ref_ias = read_float(h, ias_addr) if ias_addr else None
L("Reference IAS: {:.1f}".format(ref_ias) if ref_ias else "IAS: unknown")

# ================================================================
# PHASE 2: Wait for demo to change
# ================================================================
WAIT_SECS = 90
L("\n" + "=" * 70)
L("PHASE 2: Waiting {}s for demo mode to change...".format(WAIT_SECS))

for i in range(WAIT_SECS):
    if i % 15 == 0:
        curr_ias = read_float(h, ias_addr) if ias_addr else 0
        L("  {:3d}s: IAS={:.1f}".format(i, curr_ias))
    time.sleep(1)

# ================================================================
# PHASE 3: Snapshot 2 (filtered)
# ================================================================
L("\n" + "=" * 70)
L("PHASE 3: Taking filtered snapshot #2...")
t0 = time.time()
snap2 = snapshot_filtered(h, regions)
dt2 = time.time() - t0
L("Snapshot 2: {} values in {:.1f}s".format(len(snap2), dt2))

new_ias = read_float(h, ias_addr) if ias_addr else None
L("IAS now: {:.1f}".format(new_ias) if new_ias else "IAS: unknown")

ias_delta = (new_ias - ref_ias) if (ref_ias and new_ias) else 0
L("IAS delta: {:.1f}".format(ias_delta))

# ================================================================
# PHASE 4: Find changed addresses
# ================================================================
L("\n" + "=" * 70)
L("PHASE 4: Analyzing changes...")

alt_candidates = []
hdg_candidates = []
ias_candidates = []

for addr in snap1:
    if addr not in snap2:
        continue
    v1 = snap1[addr]
    v2 = snap2[addr]
    delta = v2 - v1
    
    if abs(delta) < 0.01:
        continue
    if abs(v1) > 60000 or abs(v2) > 60000:
        continue
    
    info = {'addr': addr, 'v1': v1, 'v2': v2, 'delta': delta}
    
    # Altitude: 100-20000 range, changed > 50ft
    if 100 < v1 < 20000 and 100 < v2 < 20000 and abs(delta) > 50:
        alt_candidates.append(info)
    # Heading: 0-360 range, any change > 0.5 deg
    elif 0 < v1 < 360 and 0 < v2 < 360 and abs(delta) > 0.5:
        hdg_candidates.append(info)
    # IAS-like: tracks IAS change
    elif (abs(ias_delta) > 1 and 20 < v1 < 500 and 20 < v2 < 500 and
          abs(delta - ias_delta) < 5):
        ias_candidates.append(info)

# Free memory
del snap1, snap2

L("\nChanges found:")
L("  Altitude-like: {}".format(len(alt_candidates)))
L("  Heading-like:  {}".format(len(hdg_candidates)))
L("  IAS-like:      {}".format(len(ias_candidates)))

# ================================================================
# PHASE 5: Test altitude candidates
# ================================================================
L("\n" + "=" * 70)
L("PHASE 5: Testing altitude candidates")
L("=" * 70)

if alt_candidates:
    alt_candidates.sort(key=lambda x: x['v2'])
    
    # Group by value
    groups = []
    grp = [alt_candidates[0]]
    for c in alt_candidates[1:]:
        if abs(c['v2'] - grp[-1]['v2']) < 10:
            grp.append(c)
        else:
            groups.append(grp)
            grp = [c]
    groups.append(grp)
    
    L("\n{} altitude value groups:".format(len(groups)))
    for gi, group in enumerate(groups):
        L("\n  Group {}: ~{:.0f} -> ~{:.0f} ({} addrs)".format(
            gi, group[0]['v1'], group[0]['v2'], len(group)))
        for c in group[:8]:
            L("    0x{:08X}: {:.2f} -> {:.2f}".format(c['addr'], c['v1'], c['v2']))
    
    # Write test
    L("\n--- Write test per group ---")
    for gi, group in enumerate(groups[:30]):
        a = group[0]['addr']
        before = read_float(h, a)
        ok = write_float(h, a, 55555.0)
        after = read_float(h, a)
        st = "STICKS" if after and abs(after - 55555.0) < 1 else "OVERWRITTEN"
        L("  G{:2d} 0x{:08X}: {:.0f}->{:.0f}  write={}  [{}]".format(
            gi, a, before or 0, after or 0, ok, st))
    
    # Burst write to ALL for 15s
    L("\n--- Burst ALT=55555 to ALL {} addrs (15s) ---".format(len(alt_candidates)))
    L(">>> WATCH THE GDU ALTITUDE TAPE NOW <<<")
    
    t0 = time.time()
    passes = 0
    while time.time() - t0 < 15.0:
        for c in alt_candidates:
            write_float(h, c['addr'], 55555.0)
        passes += 1
    L("  {} passes in {:.1f}s".format(passes, time.time() - t0))

else:
    L("  No altitude candidates found.")
    L("  NOTE: Demo mode may not have changed altitude enough in 90s.")

# ================================================================
# PHASE 6: Test heading candidates
# ================================================================
L("\n" + "=" * 70)
L("PHASE 6: Testing heading candidates")
L("=" * 70)

if hdg_candidates:
    hdg_candidates.sort(key=lambda x: x['v2'])
    
    groups = []
    grp = [hdg_candidates[0]]
    for c in hdg_candidates[1:]:
        if abs(c['v2'] - grp[-1]['v2']) < 2:
            grp.append(c)
        else:
            groups.append(grp)
            grp = [c]
    groups.append(grp)
    
    L("\n{} heading groups:".format(len(groups)))
    for gi, group in enumerate(groups[:40]):
        L("\n  Group {}: ~{:.1f} -> ~{:.1f} ({} addrs)".format(
            gi, group[0]['v1'], group[0]['v2'], len(group)))
        for c in group[:5]:
            L("    0x{:08X}: {:.2f} -> {:.2f}".format(c['addr'], c['v1'], c['v2']))
    
    # Burst write HDG=270
    L("\n--- Burst HDG=270 to ALL {} addrs (15s) ---".format(len(hdg_candidates)))
    L(">>> WATCH THE GDU COMPASS NOW <<<")
    
    t0 = time.time()
    passes = 0
    while time.time() - t0 < 15.0:
        for c in hdg_candidates:
            write_float(h, c['addr'], 270.0)
        passes += 1
    L("  {} passes in {:.1f}s".format(passes, time.time() - t0))

else:
    L("  No heading candidates found.")

# ================================================================
# PHASE 7: IAS candidates
# ================================================================
L("\n" + "=" * 70)
L("PHASE 7: IAS-like candidates")
L("=" * 70)

if ias_candidates:
    ias_candidates.sort(key=lambda x: x['addr'])
    L("\n{} IAS-tracking addresses:".format(len(ias_candidates)))
    for c in ias_candidates[:50]:
        has_sig = ""
        v4 = read_float(h, c['addr'] + 4)
        v8 = read_float(h, c['addr'] + 8)
        if v4 and v8 and abs(v4 - 20.0) < 1 and abs(v8 - 999.0) < 1:
            has_sig = " [IAS WIDGET!]"
        L("  0x{:08X}: {:.2f} -> {:.2f} (d={:.2f}){}".format(
            c['addr'], c['v1'], c['v2'], c['delta'], has_sig))

# ================================================================
# SUMMARY
# ================================================================
L("\n" + "=" * 70)
L("SUMMARY")
L("=" * 70)
L("  GDU PID:       {}".format(pid))
L("  IAS widget:    {}".format("0x{:08X}".format(ias_addr) if ias_addr else "NOT FOUND"))
L("  IAS snap1/2:   {:.1f} / {:.1f} (delta {:.1f})".format(
    ref_ias or 0, new_ias or 0, ias_delta))
L("  ALT candidates: {}".format(len(alt_candidates)))
L("  HDG candidates: {}".format(len(hdg_candidates)))
L("  IAS candidates: {}".format(len(ias_candidates)))
L("  Log saved to:  {}".format(log_path))
L("\nDone! {}".format(datetime.datetime.now().isoformat()))

kernel32.CloseHandle(h)
log_f.close()
print("\n*** Log saved to: {} ***".format(log_path))
input("Press Enter to exit...")
