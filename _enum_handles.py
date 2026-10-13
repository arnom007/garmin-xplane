import ctypes
from ctypes import wintypes
import struct
import sys

ntdll = ctypes.windll.ntdll
kernel32 = ctypes.windll.kernel32

SystemExtendedHandleInformation = 64
STATUS_INFO_LENGTH_MISMATCH = 0xC0000004
ObjectNameInformation = 1

class SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX(ctypes.Structure):
    _fields_ = [
        ("Object", ctypes.c_void_p),
        ("UniqueProcessId", ctypes.c_size_t),
        ("HandleValue", ctypes.c_size_t),
        ("GrantedAccess", wintypes.ULONG),
        ("CreatorBackTraceIndex", wintypes.USHORT),
        ("ObjectTypeIndex", wintypes.USHORT),
        ("HandleAttributes", wintypes.ULONG),
        ("Reserved", wintypes.ULONG),
    ]

class UNICODE_STRING(ctypes.Structure):
    _fields_ = [
        ("Length", wintypes.USHORT),
        ("MaximumLength", wintypes.USHORT),
        ("Buffer", ctypes.c_wchar_p),
    ]

# Find handles for PIDs
pids = {6452, 20368}

buf_size = 1024 * 1024 * 16
buf = ctypes.create_string_buffer(buf_size)
needed = wintypes.ULONG()

status = ntdll.NtQuerySystemInformation(
    SystemExtendedHandleInformation, buf, buf_size, ctypes.byref(needed)
)
if (status & 0xFFFFFFFF) == STATUS_INFO_LENGTH_MISMATCH:
    buf_size = needed.value + 1024 * 1024 * 4
    buf = ctypes.create_string_buffer(buf_size)
    status = ntdll.NtQuerySystemInformation(
        SystemExtendedHandleInformation, buf, buf_size, ctypes.byref(needed)
    )

if status != 0:
    print(f"NtQuerySystemInformation failed with {hex(status & 0xFFFFFFFF)}")
    sys.exit(1)

count = ctypes.c_size_t.from_buffer_copy(buf[:8]).value
print(f"Total handles in system: {count}")

entry_size = ctypes.sizeof(SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX)
offset = 16  # skip NumberOfHandles and Reserved on x64

found = []
for i in range(count):
    if offset + entry_size > buf_size:
        break
    entry = SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX.from_buffer_copy(buf[offset:offset+entry_size])
    offset += entry_size
    
    if entry.UniqueProcessId in pids:
        # duplicate handle to inspect object name
        hProc = kernel32.OpenProcess(0x0040, False, entry.UniqueProcessId) # PROCESS_DUP_HANDLE
        if not hProc:
            continue
        hDup = wintypes.HANDLE()
        if kernel32.DuplicateHandle(
            hProc, entry.HandleValue,
            kernel32.GetCurrentProcess(), ctypes.byref(hDup),
            0, False, 2 # DUPLICATE_SAME_ACCESS
        ):
            name_buf = ctypes.create_string_buffer(2048)
            ret_len = wintypes.ULONG()
            s = ntdll.NtQueryObject(hDup, ObjectNameInformation, name_buf, 2048, ctypes.byref(ret_len))
            if s == 0:
                us = UNICODE_STRING.from_buffer_copy(name_buf[:ctypes.sizeof(UNICODE_STRING)])
                if us.Length > 0 and us.Buffer:
                    try:
                        name_str = ctypes.wstring_at(us.Buffer, us.Length // 2)
                        print(f"PID {entry.UniqueProcessId} Handle {hex(entry.HandleValue)}: {name_str}")
                    except Exception:
                        pass
            kernel32.CloseHandle(hDup)
        kernel32.CloseHandle(hProc)
