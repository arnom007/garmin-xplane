import ctypes, struct

hGDU = ctypes.windll.kernel32.OpenProcess(0x1F0FFF, False, 22540)
base_addr = 0x2FFBB9F0
start = base_addr - 128
buf = ctypes.create_string_buffer(512)
ctypes.windll.kernel32.ReadProcessMemory(hGDU, ctypes.c_void_p(start), buf, 512, None)
data = buf.raw

print(f"Memory dump around 0x{base_addr:08X} (from 0x{start:08X}):")
for i in range(0, len(data), 4):
    curr = start + i
    f = struct.unpack_from('<f', data, i)[0]
    u = struct.unpack_from('<I', data, i)[0]
    marker = " <--- 0x2FFBB9F0 (HDG)" if curr == base_addr else ""
    if curr == base_addr - 4: marker = " <--- PITCH candidate"
    if curr == base_addr + 8: marker = " <--- ROLL candidate"
    print(f"0x{curr:08X} (offset {curr - base_addr:+4d}): {f:14.4f} | 0x{u:08X} {marker}")
