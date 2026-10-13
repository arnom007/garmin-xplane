"""Find heading 048 in GDU memory."""
import ctypes, ctypes.wintypes as wt, struct

kernel32 = ctypes.windll.kernel32
TH32CS_SNAPPROCESS = 0x02
PROCESS_VM_READ = 0x0010

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

# Search in 0x5530-0x5580 region (where IAS was found)
for region_start in [0x55300000, 0x55340000, 0x55380000, 0x553C0000]:
    SIZE = 0x40000
    buf = ctypes.create_string_buffer(SIZE)
    br = ctypes.c_size_t(0)
    kernel32.ReadProcessMemory(h, ctypes.c_void_p(region_start), buf, SIZE, ctypes.byref(br))
    data = buf.raw[:br.value]
    
    # Heading 48 degrees
    for i in range(0, len(data)-3, 4):
        f = struct.unpack_from('<f', data, i)[0]
        if abs(f - 48.0) < 1.0:
            addr = region_start + i
            # Check context for avionics-like neighbors
            nearby_interesting = 0
            for j in [-8, -4, 4, 8, 12, 16]:
                if 0 <= i+j < len(data)-3:
                    nf = struct.unpack_from('<f', data, i+j)[0]
                    if (0 < nf < 360) or (abs(nf) < 90) or (1000 < nf < 50000):
                        nearby_interesting += 1
            if nearby_interesting >= 1:
                print("0x{:08X}: {:.4f}  (nearby_avionics={})".format(addr, f, nearby_interesting))

    # Also heading in radians: 48 deg = 0.8378 rad
    hdg_rad = 48.0 * 3.14159265 / 180.0
    for i in range(0, len(data)-3, 4):
        f = struct.unpack_from('<f', data, i)[0]
        if abs(f - hdg_rad) < 0.02:
            addr = region_start + i
            print("0x{:08X}: {:.4f} rad ({:.1f} deg)".format(addr, f, f*180/3.14159265))

kernel32.CloseHandle(h)

# Also check altitude region
print("\n=== Checking altitude at 0x553073B8 ===")
h = kernel32.OpenProcess(PROCESS_VM_READ, False, pid)
buf = ctypes.create_string_buffer(256)
br = ctypes.c_size_t(0)
start = 0x553073B8 - 64
kernel32.ReadProcessMemory(h, ctypes.c_void_p(start), buf, 256, ctypes.byref(br))
data = buf.raw[:br.value]
for i in range(0, len(data)-3, 4):
    f = struct.unpack_from('<f', data, i)[0]
    addr = start + i
    rel = addr - 0x553073B8
    if abs(f) < 1e8 and (abs(f) > 1e-8 or f == 0):
        print("  0x{:08X} ({:+4d}): {:12.4f}".format(addr, rel, f))
    else:
        iv = struct.unpack_from('<I', data, i)[0]
        print("  0x{:08X} ({:+4d}): 0x{:08X}".format(addr, rel, iv))
kernel32.CloseHandle(h)
