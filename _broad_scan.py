"""Scan the 0x5530-0x5580 region for altitude/heading/attitude values."""
import ctypes, ctypes.wintypes as wt, struct

kernel32 = ctypes.windll.kernel32
PROCESS_VM_READ = 0x0010
TH32CS_SNAPPROCESS = 0x02

class PE32(ctypes.Structure):
    _fields_ = [("dwSize", wt.DWORD),("cntUsage", wt.DWORD),("th32ProcessID", wt.DWORD),
        ("th32DefaultHeapID", ctypes.c_void_p),("th32ModuleID", wt.DWORD),("cntThreads", wt.DWORD),
        ("th32ParentProcessID", wt.DWORD),("pcPriClassBase", ctypes.c_long),
        ("dwFlags", wt.DWORD),("szExeFile", ctypes.c_char * 260)]

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

pid = find_gdu()
h = kernel32.OpenProcess(PROCESS_VM_READ, False, pid)

# Scan a big region
START = 0x55300000
SIZE = 0x100000  # 1MB
buf = ctypes.create_string_buffer(SIZE)
br = ctypes.c_size_t(0)
kernel32.ReadProcessMemory(h, ctypes.c_void_p(START), buf, SIZE, ctypes.byref(br))
data = buf.raw[:br.value]
kernel32.CloseHandle(h)

print("Read {} bytes from 0x{:08X}".format(len(data), START))

# Search for altitude ~4000
print("\n=== Values near 4000 (altitude ft) ===")
count = 0
for i in range(0, len(data)-3, 4):
    f = struct.unpack_from('<f', data, i)[0]
    if abs(f - 4000) < 10:
        addr = START + i
        print("  0x{:08X}: {:.4f}".format(addr, f))
        count += 1
        if count > 30: break

# Search for altitude in meters ~1219 (4000ft = 1219m)
print("\n=== Values near 1219 (altitude meters) ===")
count = 0
for i in range(0, len(data)-3, 4):
    f = struct.unpack_from('<f', data, i)[0]
    if abs(f - 1219) < 10:
        addr = START + i
        print("  0x{:08X}: {:.4f}".format(addr, f))
        count += 1
        if count > 20: break

# Search for GS/TAS ~190
print("\n=== Values near 190 (GS/TAS kts) ===")
count = 0
for i in range(0, len(data)-3, 4):
    f = struct.unpack_from('<f', data, i)[0]
    if abs(f - 190) < 3:
        addr = START + i
        print("  0x{:08X}: {:.4f}".format(addr, f))
        count += 1
        if count > 20: break

# Search for heading - look for angles that could be degrees
# We don't know the heading, so look for patterns like value + nearby 0-360 values
print("\n=== Potential heading (float 0-360 range, common avionics values) ===")
# Let's look at the second IAS match context - it had 266 nearby which could be heading
for val in [266.0]:
    count = 0
    for i in range(0, len(data)-3, 4):
        f = struct.unpack_from('<f', data, i)[0]
        if abs(f - val) < 1:
            addr = START + i
            print("  0x{:08X}: {:.4f} (near {})".format(addr, f, val))
            count += 1
            if count > 15: break

# Search for pitch/roll in degrees (small angles, likely -30 to +30)
print("\n=== Values near 0x5535D654 region (had TAS 136.8, IAS, 266) ===")
base = 0x5535D600 - START
for i in range(base, min(base + 256, len(data)-3), 4):
    f = struct.unpack_from('<f', data, i)[0]
    addr = START + i
    rel = (START + i) - 0x5535D690  # relative to second IAS
    if abs(f) < 1e8 and (abs(f) > 1e-8 or f == 0):
        print("  0x{:08X} (IAS2{:+5d}): {:12.4f}".format(addr, rel, f))
    else:
        iv = struct.unpack_from('<I', data, i)[0]
        print("  0x{:08X} (IAS2{:+5d}): 0x{:08X}".format(addr, rel, iv))
