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

print("Mapped Local\\trainerShrSuperObj successfully!")
buf = ctypes.string_at(ptr, 148)
print("Header (offset 0..0x3c):")
# 0: format / size / etc
hdr = struct.unpack('<IIIIII', buf[:24])
print(f"  [0..24]: {hdr}")

print("\nPayload (offset 0x3c..0x94 = 88 bytes):")
payload = buf[0x3c:0x3c+88]
floats = struct.unpack('<22f', payload)
for i, f in enumerate(floats):
    print(f"  float[{i:02d}] (offset +0x{i*4:02x} = 0x{0x3c+i*4:02x}): {f}")

kernel32.UnmapViewOfFile(ctypes.c_void_p(ptr))
kernel32.CloseHandle(h)
