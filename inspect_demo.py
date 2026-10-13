import ctypes, struct

kernel32 = ctypes.windll.kernel32
hGDU = kernel32.OpenProcess(0x1F0FFF, False, 22540)

# 1. Read current IAS widget
buf = ctypes.create_string_buffer(4)
kernel32.ReadProcessMemory(hGDU, ctypes.c_void_p(0x2EE6FE1C), buf, 4, None)
cur_ias = struct.unpack('<f', buf.raw)[0]
print(f"Current IAS at 0x2EE6FE1C: {cur_ias:.2f}")

# 2. Read current Pitch & Heading at 0x2EDC69D4, 0x2EDC69D8
buf8 = ctypes.create_string_buffer(8)
kernel32.ReadProcessMemory(hGDU, ctypes.c_void_p(0x2EDC69D4), buf8, 8, None)
p, hdg = struct.unpack('<2f', buf8.raw)
print(f"Current Pitch: {p:.2f}, Heading: {hdg:.2f}")

# 3. Read Local\trainerShrSuperObj
hSHM = kernel32.OpenFileMappingW(0xF001F, False, 'Local\\trainerShrSuperObj')
pSHM = kernel32.MapViewOfFile(hSHM, 0xF001F, 0, 0, 148)
flag = ctypes.c_uint8.from_address(pSHM + 0x0C).value
shm_bytes = ctypes.string_at(pSHM + 0x3C, 88)
shm_floats = struct.unpack('<22f', shm_bytes)
print(f"SHM flag (0x0C): {flag}")
print(f"SHM floats[7] (IAS): {shm_floats[7]:.2f}, floats[19] (IAS): {shm_floats[19]:.2f}")
print(f"SHM floats[12] (Hdg rad): {shm_floats[12]:.4f}")
