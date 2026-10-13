import ctypes
import struct
import time

kernel32 = ctypes.windll.kernel32
kernel32.MapViewOfFile.restype = ctypes.c_void_p

FILE_MAP_ALL_ACCESS = 0xF001F

h = kernel32.OpenFileMappingW(FILE_MAP_ALL_ACCESS, False, 'Local\\trainerShrSuperObj')
if not h:
    print("Could not open Local\\trainerShrSuperObj")
    exit(1)

ptr = kernel32.MapViewOfFile(h, FILE_MAP_ALL_ACCESS, 0, 0, 148)

# Read flag at offset 0xc
flag = ctypes.c_uint8.from_address(ptr + 0xc).value
print(f"Current flag at 0xc: {flag}")

# Set flag at 0xc to 0
ctypes.c_uint8.from_address(ptr + 0xc).value = 0
print("Set flag at 0xc to 0")

# Write test values
payload = ctypes.string_at(ptr + 0x3c, 88)
floats = list(struct.unpack('<22f', payload))
floats[5] = 9999.0
floats[7] = 300.0
floats[12] = 1.0

ctypes.memmove(ptr + 0x3c, struct.pack('<22f', *floats), 88)
print("Wrote test values! Monitoring for 2 seconds...")

for s in range(10):
    time.sleep(0.2)
    cur_buf = ctypes.string_at(ptr, 148)
    cur_flag = cur_buf[0xc]
    cur_floats = struct.unpack('<22f', cur_buf[0x3c:0x3c+88])
    print(f"  Sample {s}: flag={cur_flag}, Alt={cur_floats[5]:.1f}, Spd={cur_floats[7]:.1f}, Hdg={cur_floats[12]:.4f}")

# Restore
ctypes.c_uint8.from_address(ptr + 0xc).value = flag
ctypes.memmove(ptr + 0x3c, payload, 88)
print("Restored original values.")

kernel32.UnmapViewOfFile(ctypes.c_void_p(ptr))
kernel32.CloseHandle(h)
