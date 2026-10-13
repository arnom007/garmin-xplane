import ctypes
import struct

kernel32 = ctypes.windll.kernel32
hGDU = kernel32.OpenProcess(0x1F0FFF, False, 6452)

addr = 0x30ef18a0
buf = ctypes.create_string_buffer(16)
br = ctypes.c_size_t()
kernel32.ReadProcessMemory(hGDU, ctypes.c_void_p(addr), buf, 16, ctypes.byref(br))
print("Bytes at 0x30ef18a0 in GDU:", buf.raw[:16].hex())
kernel32.CloseHandle(hGDU)
