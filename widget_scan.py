"""
Find altitude and heading widgets by scanning near the known IAS widget.
Strategy: The IAS widget has pattern [value, 20.0, 999.0] and a marker 0x0001EA5F at offset -12.
Other gauge widgets (ALT, HDG, VS) should have the same marker and similar structure.
"""
import ctypes, ctypes.wintypes as wt, struct, os, time, datetime

kernel32 = ctypes.windll.kernel32
PROCESS_VM_READ = 0x0010
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

# Setup log
log_dir = os.path.join(os.path.expandvars('%APPDATA%'), 'GarminBridge')
os.makedirs(log_dir, exist_ok=True)
log_path = os.path.join(log_dir, 'widget_scan.log')
log_f = open(log_path, 'w', encoding='utf-8')

def L(msg=''):
    print(msg)
    log_f.write(msg + '\n')
    log_f.flush()

L("=" * 70)
L("  Widget Scanner - Finding ALT/HDG/VS widgets")
L("  {}".format(datetime.datetime.now().isoformat()))
L("=" * 70)

pid = find_gdu()
if not pid:
    L("GDU not running!")
    log_f.close()
    input("Press Enter...")
    exit(1)

h = kernel32.OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, False, pid)
L("GDU PID={}\n".format(pid))

def read_region(addr, size):
    buf = ctypes.create_string_buffer(size)
    br = ctypes.c_size_t(0)
    if kernel32.ReadProcessMemory(h, ctypes.c_void_p(addr), buf, size, ctypes.byref(br)):
        return buf.raw[:br.value]
    return None

def read_float(addr):
    d = read_region(addr, 4)
    if d and len(d) == 4:
        return struct.unpack('<f', d)[0]
    return None

# ========================================================
# Strategy 1: Search for 0x0001EA5F marker (seen at IAS_widget - 12)
# This marker appears to identify gauge/widget objects
# ========================================================
L("=== Strategy 1: Scanning for widget marker 0x0001EA5F ===")
L("(This marker was found at IAS_widget_addr - 12)\n")

MARKER = 0x0001EA5F
marker_bytes = struct.pack('<I', MARKER)

mbi = MBI()
addr = 0x10000
widgets_found = []

while addr < 0x7FFE0000:
    if kernel32.VirtualQueryEx(h, ctypes.c_void_p(addr), ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
        addr += 0x10000
        continue
    if (mbi.State == 0x1000 and mbi.Protect and
        not (mbi.Protect & 0x01) and not (mbi.Protect & 0x100) and
        mbi.RegionSize < 50*1024*1024):
        buf = ctypes.create_string_buffer(mbi.RegionSize)
        br = ctypes.c_size_t(0)
        if kernel32.ReadProcessMemory(h, ctypes.c_void_p(mbi.BaseAddress), buf, mbi.RegionSize, ctypes.byref(br)):
            data = buf.raw[:br.value]
            # Find all occurrences of the marker
            pos = 0
            while True:
                idx = data.find(marker_bytes, pos)
                if idx == -1:
                    break
                # The value should be at marker_addr + 12
                val_offset = idx + 12
                if val_offset + 12 <= len(data):
                    val = struct.unpack_from('<f', data, val_offset)[0]
                    val_min = struct.unpack_from('<f', data, val_offset + 4)[0]
                    val_max = struct.unpack_from('<f', data, val_offset + 8)[0]
                    marker_addr = mbi.BaseAddress + idx
                    val_addr = mbi.BaseAddress + val_offset
                    
                    # Filter: must have reasonable min/max
                    if (abs(val) < 100000 and abs(val_min) < 100000 and abs(val_max) < 100000 and
                        val_max > val_min and val_min != val_max):
                        widgets_found.append({
                            'marker_addr': marker_addr,
                            'val_addr': val_addr,
                            'value': val,
                            'min': val_min,
                            'max': val_max,
                        })
                pos = idx + 4
    addr = mbi.BaseAddress + mbi.RegionSize

L("Found {} potential widgets with marker 0x{:08X}:".format(len(widgets_found), MARKER))
L("")
L("{:>12s}  {:>12s}  {:>12s}  {:>10s}  {:>10s}  {:>10s}  {}".format(
    "MarkerAddr", "ValueAddr", "Value", "Min", "Max", "Range", "Likely"))

for w in widgets_found:
    val_range = w['max'] - w['min']
    # Guess what this widget is
    likely = ""
    if abs(w['min'] - 20.0) < 1 and abs(w['max'] - 999.0) < 1:
        likely = "IAS (speed tape)"
    elif w['max'] > 30000 and w['min'] < 0:
        likely = "ALTITUDE"
    elif abs(w['max'] - 360) < 1 or abs(w['max'] - 359) < 1:
        likely = "HEADING"
    elif abs(val_range - 360) < 10:
        likely = "heading?"
    elif 2000 < w['max'] < 10000 and w['min'] < 100:
        likely = "VS?"
    elif abs(w['min']) < 50 and abs(w['max']) < 200:
        likely = "small gauge"
    
    L("  0x{:08X}  0x{:08X}  {:12.2f}  {:10.1f}  {:10.1f}  {:10.1f}  {}".format(
        w['marker_addr'], w['val_addr'], w['value'], w['min'], w['max'], val_range, likely))

# ========================================================
# Strategy 2: Scan for [float, min, max] patterns where
# we know the altitude is ~2700 and heading is unknown
# ========================================================
L("\n\n=== Strategy 2: Search for altitude widget (value-independent) ===")
L("Looking for [ANY float 500-20000, negative min, max > 30000]...\n")

mbi = MBI()
addr = 0x10000
alt_candidates = []

while addr < 0x7FFE0000:
    if kernel32.VirtualQueryEx(h, ctypes.c_void_p(addr), ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
        addr += 0x10000
        continue
    if (mbi.State == 0x1000 and mbi.Protect and
        not (mbi.Protect & 0x01) and not (mbi.Protect & 0x100) and
        mbi.RegionSize < 50*1024*1024):
        buf = ctypes.create_string_buffer(mbi.RegionSize)
        br = ctypes.c_size_t(0)
        if kernel32.ReadProcessMemory(h, ctypes.c_void_p(mbi.BaseAddress), buf, mbi.RegionSize, ctypes.byref(br)):
            data = buf.raw[:br.value]
            for off in range(0, len(data) - 11, 4):
                f_val = struct.unpack_from('<f', data, off)[0]
                f_min = struct.unpack_from('<f', data, off + 4)[0]
                f_max = struct.unpack_from('<f', data, off + 8)[0]
                # Altitude: value 500-20000, min < 0, max > 30000
                if (500 < f_val < 20000 and -2000 < f_min < 100 and 
                    30000 < f_max < 100000 and f_max > f_min):
                    real_addr = mbi.BaseAddress + off
                    alt_candidates.append((real_addr, f_val, f_min, f_max))
    addr = mbi.BaseAddress + mbi.RegionSize

L("Found {} altitude candidates:".format(len(alt_candidates)))
for addr, val, vmin, vmax in alt_candidates[:30]:
    # Check if it has the 0x0001EA5F marker at -12
    marker_val = read_region(addr - 12, 4)
    has_marker = ""
    if marker_val:
        m = struct.unpack('<I', marker_val)[0]
        if m == MARKER:
            has_marker = " [HAS WIDGET MARKER!]"
    L("  0x{:08X}: val={:.1f} min={:.1f} max={:.1f}{}".format(addr, val, vmin, vmax, has_marker))

# ========================================================
# Strategy 3: Dump ALL widgets in the IAS widget's heap region
# ========================================================
L("\n\n=== Strategy 3: Scan IAS widget region for similar structures ===")
# IAS widget was at 0x55011678. Read a big block around it.
IAS_ADDR = None
# Find IAS widget first
twenty_addrs = []
mbi2 = MBI()
a2 = 0x10000
while a2 < 0x7FFE0000:
    if kernel32.VirtualQueryEx(h, ctypes.c_void_p(a2), ctypes.byref(mbi2), ctypes.sizeof(mbi2)) == 0:
        a2 += 0x10000; continue
    if (mbi2.State == 0x1000 and mbi2.Protect and not (mbi2.Protect & 0x01) and 
        not (mbi2.Protect & 0x100) and mbi2.RegionSize < 50*1024*1024):
        buf2 = ctypes.create_string_buffer(mbi2.RegionSize)
        br2 = ctypes.c_size_t(0)
        if kernel32.ReadProcessMemory(h, ctypes.c_void_p(mbi2.BaseAddress), buf2, mbi2.RegionSize, ctypes.byref(br2)):
            d2 = buf2.raw[:br2.value]
            twenty_b = struct.pack('<f', 20.0)
            nine99_b = struct.pack('<f', 999.0)
            p = 0
            while True:
                idx = d2.find(twenty_b, p)
                if idx == -1: break
                if idx + 8 <= len(d2):
                    if d2[idx+4:idx+8] == nine99_b:
                        ias_val = struct.unpack_from('<f', d2, idx-4)[0] if idx >= 4 else 0
                        if 0 < ias_val < 500:
                            IAS_ADDR = mbi2.BaseAddress + idx - 4
                            L("IAS widget confirmed at 0x{:08X} = {:.1f}".format(IAS_ADDR, ias_val))
                p = idx + 4
    a2 = mbi2.BaseAddress + mbi2.RegionSize

# Now scan a large region around the IAS widget looking for value,min,max triplets
if IAS_ADDR:
    # Read 256KB around IAS widget
    scan_start = IAS_ADDR - 0x20000
    scan_size = 0x40000
    region = read_region(scan_start, scan_size)
    if region:
        L("\nSearching 256KB around IAS widget for [value, min, max] patterns:")
        L("")
        for i in range(0, len(region) - 11, 4):
            f0 = struct.unpack_from('<f', region, i)[0]
            f1 = struct.unpack_from('<f', region, i+4)[0]
            f2 = struct.unpack_from('<f', region, i+8)[0]
            # Look for reasonable [value, min, max] where max > min
            if (f2 > f1 and f2 - f1 > 10 and
                abs(f0) < 100000 and abs(f1) < 100000 and abs(f2) < 100000 and
                f1 != 0 and f0 >= f1 and f0 <= f2):
                real_addr = scan_start + i
                rng = f2 - f1
                likely = ""
                if abs(f1 - 20) < 1 and abs(f2 - 999) < 1: likely = "IAS"
                elif f2 > 30000: likely = "ALTITUDE?"
                elif abs(f2 - 360) < 5: likely = "HEADING?"
                elif 1000 < rng < 10000 and f1 < 0: likely = "VS?"
                
                if likely:
                    L("  0x{:08X}: val={:10.2f}  min={:10.1f}  max={:10.1f}  range={:10.1f}  {}".format(
                        real_addr, f0, f1, f2, rng, likely))

L("\n\nLog saved to: {}".format(log_path))
kernel32.CloseHandle(h)
log_f.close()
input("\nPress Enter to exit...")
