import ctypes
import ctypes.wintypes as wt

ntdll = ctypes.windll.ntdll
kernel32 = ctypes.windll.kernel32

class S(ctypes.Structure):
    _fields_ = [
        ('Object', ctypes.c_void_p),
        ('UniqueProcessId', ctypes.c_size_t),
        ('HandleValue', ctypes.c_size_t),
        ('GrantedAccess', wt.ULONG),
        ('CreatorBackTraceIndex', wt.USHORT),
        ('ObjectTypeIndex', wt.USHORT),
        ('HandleAttributes', wt.ULONG),
        ('Reserved', wt.ULONG),
    ]

class UNICODE_STRING64(ctypes.Structure):
    _fields_ = [
        ("Length", wt.USHORT),
        ("MaximumLength", wt.USHORT),
        ("_pad", wt.ULONG),
        ("Buffer", ctypes.c_void_p),
    ]

buf_size = 1024 * 1024 * 32
buf = ctypes.create_string_buffer(buf_size)
needed = wt.ULONG()
st = ntdll.NtQuerySystemInformation(64, buf, buf_size, ctypes.byref(needed))
count = ctypes.c_size_t.from_buffer_copy(buf[:8]).value

pids = {6452: "TXi (6452)", 20368: "GTN (20368)"}
hProcs = {pid: kernel32.OpenProcess(0x1F0FFF, False, pid) for pid in pids}

entry_size = 40
off = 16
for i in range(count):
    entry = S.from_buffer_copy(buf[off:off+entry_size])
    off += entry_size
    if entry.UniqueProcessId in pids:
        hProc = hProcs[entry.UniqueProcessId]
        hDup = wt.HANDLE()
        if kernel32.DuplicateHandle(
            hProc, entry.HandleValue,
            kernel32.GetCurrentProcess(), ctypes.byref(hDup),
            0, False, 2
        ):
            type_buf = ctypes.create_string_buffer(1024)
            ret_len = wt.ULONG()
            if ntdll.NtQueryObject(hDup, 2, type_buf, 1024, ctypes.byref(ret_len)) == 0:
                us_type = UNICODE_STRING64.from_buffer_copy(type_buf[:16])
                if us_type.Buffer and us_type.Length > 0:
                    type_str = ctypes.wstring_at(us_type.Buffer, us_type.Length // 2)
                    if type_str == "Section":
                        name_buf = ctypes.create_string_buffer(2048)
                        ntdll.NtQueryObject(hDup, 1, name_buf, 2048, ctypes.byref(ret_len))
                        us_name = UNICODE_STRING64.from_buffer_copy(name_buf[:16])
                        name_str = ctypes.wstring_at(us_name.Buffer, us_name.Length // 2) if us_name.Buffer else ""
                        print(f"[{pids[entry.UniqueProcessId]}] Handle {hex(entry.HandleValue)}: Section '{name_str}'")
            kernel32.CloseHandle(hDup)

for h in hProcs.values():
    if h: kernel32.CloseHandle(h)
