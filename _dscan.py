"""Direct differential scan - run from Gemini terminal."""
import ctypes, ctypes.wintypes as wt, struct, json, os, time

kernel32 = ctypes.windll.kernel32
PROCESS_VM_READ = 0x0010
PROCESS_QUERY_INFORMATION = 0x0400
MEM_COMMIT = 0x1000
PAGE_NOACCESS = 0x01
PAGE_GUARD = 0x100
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
            if 'gdu_7xx' in exe: r = (pe.th32ProcessID, exe); break
            if not kernel32.Process32Next(snap, ctypes.byref(pe)): break
    kernel32.CloseHandle(snap)
    return r

def scan(pid, target, tol):
    h = kernel32.OpenProcess(PROCESS_VM_READ|PROCESS_QUERY_INFORMATION, False, pid)
    if not h: return []
    res = []; mbi = MBI(); a = 0x10000
    while a < 0x7FFE0000:
        if not kernel32.VirtualQueryEx(h, ctypes.c_void_p(a), ctypes.byref(mbi), ctypes.sizeof(mbi)):
            a += 0x10000; continue
        if mbi.State==MEM_COMMIT and mbi.Protect and not(mbi.Protect&PAGE_NOACCESS) and not(mbi.Protect&PAGE_GUARD) and mbi.RegionSize<50*1024*1024:
            buf = ctypes.create_string_buffer(mbi.RegionSize)
            br = ctypes.c_size_t(0)
            if kernel32.ReadProcessMemory(h, ctypes.c_void_p(mbi.BaseAddress), buf, mbi.RegionSize, ctypes.byref(br)):
                d = buf.raw[:br.value]
                for o in range(0,len(d)-3,4):
                    v = struct.unpack_from('<f',d,o)[0]
                    if abs(v-target)<=tol: res.append(mbi.BaseAddress+o)
        a = mbi.BaseAddress + mbi.RegionSize
    kernel32.CloseHandle(h)
    return res

def read_at(pid, addrs, target, tol):
    h = kernel32.OpenProcess(PROCESS_VM_READ, False, pid)
    if not h: return []
    m = []
    for a in addrs:
        buf = ctypes.create_string_buffer(4); br = ctypes.c_size_t(0)
        if kernel32.ReadProcessMemory(h,ctypes.c_void_p(a),buf,4,ctypes.byref(br)):
            v = struct.unpack('<f',buf.raw[:4])[0]
            if abs(v-target)<=tol: m.append((a,v))
    kernel32.CloseHandle(h)
    return m

def context(pid, addr):
    h = kernel32.OpenProcess(PROCESS_VM_READ, False, pid)
    if not h: return
    buf = ctypes.create_string_buffer(256); br = ctypes.c_size_t(0)
    s = addr - 64
    if kernel32.ReadProcessMemory(h, ctypes.c_void_p(s), buf, 256, ctypes.byref(br)):
        d = buf.raw[:br.value]
        for i in range(0,len(d)-3,4):
            f = struct.unpack_from('<f',d,i)[0]
            mk = " <<<" if s+i==addr else ""
            if abs(f)<1e8 and (abs(f)>1e-8 or f==0):
                print("  0x{:08X} ({:+4d}): {:12.4f}{}".format(s+i, i-64, f, mk))
            else:
                iv = struct.unpack_from('<I',d,i)[0]
                print("  0x{:08X} ({:+4d}): 0x{:08X}{}".format(s+i, i-64, iv, mk))
    kernel32.CloseHandle(h)

import sys
if __name__ == '__main__':
    mode = sys.argv[1]  # scan1, scan2, context
    p = find_gdu()
    if not p: print("GDU not found!"); sys.exit(1)
    pid = p[0]
    print("GDU PID={}".format(pid))
    SF = os.path.join(os.path.dirname(os.path.abspath(__file__)), '_scan_state.json')

    if mode == 'scan1':
        t = float(sys.argv[2]); tol = float(sys.argv[3])
        print("Scan1 for {} +/- {}...".format(t, tol))
        r = scan(pid, t, tol)
        print("Found {} addrs".format(len(r)))
        with open(SF,'w') as f: json.dump({'a':r,'t':t,'tol':tol},f)

    elif mode == 'scan2':
        t = float(sys.argv[2]); tol = float(sys.argv[3])
        with open(SF) as f: st = json.load(f)
        print("Checking {} addrs for {} +/- {}...".format(len(st['a']), t, tol))
        m = read_at(pid, st['a'], t, tol)
        print("MATCHES: {}".format(len(m)))
        for a,v in m[:30]:
            print("  0x{:08X}: {:.4f}".format(a,v))
        if m and len(m) <= 10:
            for a,v in m[:3]:
                print("\n--- Context 0x{:08X} ---".format(a))
                context(pid, a)

    elif mode == 'ctx':
        a = int(sys.argv[2], 16)
        context(pid, a)
