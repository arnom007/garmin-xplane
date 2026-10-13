import ctypes
import struct

kernel32 = ctypes.windll.kernel32
hProc = kernel32.OpenProcess(0x1F0FFF, False, 6452)

addr = 0x312BB9EC
buf = ctypes.create_string_buffer(64)
br = ctypes.c_size_t()
kernel32.ReadProcessMemory(hProc, ctypes.c_void_p(addr - 16), buf, 64, ctypes.byref(br))

raw = buf.raw
print("Memory around AHRS block in GDU:")
for i in range(0, 64, 4):
    cur_addr = addr - 16 + i
    f = struct.unpack('<f', raw[i:i+4])[0]
    u = struct.unpack('<I', raw[i:i+4])[0]
    print(f"  0x{cur_addr:08x}: hex=0x{u:08x} float={f:.4f}")

kernel32.CloseHandle(hProc)
