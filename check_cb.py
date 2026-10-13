import ctypes
kernel32 = ctypes.windll.kernel32
hGDU = kernel32.OpenProcess(0x1F0FFF, False, 22540)
buf = ctypes.create_string_buffer(4)
kernel32.ReadProcessMemory(hGDU, ctypes.c_void_p(0x2FBF18A0), buf, 4, None)
print("Bytes at 0x2FBF18A0:", [hex(b) for b in buf.raw])
