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
if not ptr:
    print("Could not map view of file")
    exit(1)

print("Reading before write:")
buf = ctypes.string_at(ptr, 148)
payload = buf[0x3c:0x3c+88]
floats = list(struct.unpack('<22f', payload))
print(f"  Alt float[5]: {floats[5]}")
print(f"  Spd float[7]: {floats[7]}")
print(f"  Hdg float[12]: {floats[12]}")

# Let's try modifying Heading to 090 degrees (1.570796 rad)
# and Altitude to 5000.0 meters
test_floats = list(floats)
test_floats[5] = 5000.0
test_floats[7] = 250.0
test_floats[12] = 1.570796

new_payload = struct.pack('<22f', *test_floats)
ctypes.memmove(ptr + 0x3c, new_payload, 88)

print("\nWrote test values! Monitoring for 2 seconds (10 samples)...")
for s in range(10):
    time.sleep(0.2)
    cur_buf = ctypes.string_at(ptr, 148)
    cur_floats = struct.unpack('<22f', cur_buf[0x3c:0x3c+88])
    print(f"  Sample {s}: Alt={cur_floats[5]:.1f}, Spd={cur_floats[7]:.1f}, Hdg={cur_floats[12]:.4f}")

# Restore original values
ctypes.memmove(ptr + 0x3c, payload, 88)
print("Restored original values.")

kernel32.UnmapViewOfFile(ctypes.c_void_p(ptr))
kernel32.CloseHandle(h)
