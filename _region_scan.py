"""Read a large memory region around the IAS address to find altitude/heading/etc."""
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
print("GDU PID={}".format(pid))

# Read 4KB around the IAS address to find related flight model fields
IAS_ADDR = 0x55351678
START = IAS_ADDR - 512
SIZE = 4096

h = kernel32.OpenProcess(PROCESS_VM_READ, False, pid)
buf = ctypes.create_string_buffer(SIZE)
br = ctypes.c_size_t(0)
kernel32.ReadProcessMemory(h, ctypes.c_void_p(START), buf, SIZE, ctypes.byref(br))
data = buf.raw[:br.value]
kernel32.CloseHandle(h)

print("Read {} bytes from 0x{:08X}".format(len(data), START))
print()

# Show all floats that look like avionics values
print("=== Interesting float values (avionics-like) ===")
for i in range(0, len(data)-3, 4):
    f = struct.unpack_from('<f', data, i)[0]
    addr = START + i
    rel = i - 512  # relative to IAS
    
    # Filter for interesting avionics-like values
    interesting = False
    label = ""
    
    if abs(f - 179.7) < 2: label = "IAS?"; interesting = True
    elif abs(f - 190) < 5: label = "GS/TAS?"; interesting = True
    elif abs(f - 4000) < 100: label = "ALT?"; interesting = True
    elif 0 < f < 360 and abs(f - int(f)) < 0.01: label = "HDG?"; interesting = True
    elif abs(f - 29.92) < 0.5: label = "BARO_inHg?"; interesting = True
    elif abs(f - 1013.25) < 5: label = "BARO_hPa?"; interesting = True
    elif abs(f - 15) < 5 and f != 20 and f != 10: label = "OAT?"; interesting = True  
    elif 100 < f < 500 and abs(f - round(f)) < 0.01: label = "speed/alt?"; interesting = True
    elif 1000 < f < 50000 and abs(f - round(f)) < 1: label = "alt_ft?"; interesting = True
    elif 0.01 < abs(f) < 6.3 and rel > -200: label = "angle_rad?"; interesting = True
    
    if interesting:
        print("  0x{:08X} (IAS{:+5d}): {:12.4f}  {}".format(addr, rel, f, label))

print()
print("=== Raw dump near IAS (offsets -128 to +256) ===")
for i in range(512-128, min(512+256, len(data)-3), 4):
    f = struct.unpack_from('<f', data, i)[0]
    addr = START + i
    rel = i - 512
    if abs(f) < 1e8 and (abs(f) > 1e-8 or f == 0):
        print("  0x{:08X} ({:+5d}): {:12.4f}".format(addr, rel, f))
    else:
        iv = struct.unpack_from('<I', data, i)[0]
        print("  0x{:08X} ({:+5d}): 0x{:08X}".format(addr, rel, iv))
