import ctypes
import struct
import time

kernel32 = ctypes.windll.kernel32
kernel32.MapViewOfFile.restype = ctypes.c_void_p

FILE_MAP_ALL_ACCESS = 0xF001F

# Open trainerShrSuperObj
hMap = kernel32.OpenFileMappingW(FILE_MAP_ALL_ACCESS, False, 'Local\\trainerShrSuperObj')
if not hMap:
    print("Failed to open Local\\trainerShrSuperObj")
    exit(1)

pMap = kernel32.MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, 148)

# Open GDU process
hGDU = kernel32.OpenProcess(0x1F0FFF, False, 6452)

addr_hdg = 0x312BB9F0
addr_pitch = 0x312BB9EC
addr_ias = 0x2D3BFE1C

def read_gdu_val(addr):
    buf = ctypes.create_string_buffer(4)
    br = ctypes.c_size_t()
    if kernel32.ReadProcessMemory(hGDU, ctypes.c_void_p(addr), buf, 4, ctypes.byref(br)):
        return struct.unpack('<f', buf.raw)[0]
    return None

print("BEFORE INJECTION:")
print(f"  GDU Heading: {read_gdu_val(addr_hdg):.2f}°")
print(f"  GDU Pitch:   {read_gdu_val(addr_pitch):.2f}°")
print(f"  GDU IAS:     {read_gdu_val(addr_ias):.2f} kts")

# Read current shared memory payload
orig_flag = ctypes.c_uint8.from_address(pMap + 0xc).value
orig_payload = ctypes.string_at(pMap + 0x3c, 88)
floats = list(struct.unpack('<22f', orig_payload))

print("\nSetting flag 0xc = 0 and writing new Heading=090.0°, Pitch=5.0°, Spd=220.0...")
ctypes.c_uint8.from_address(pMap + 0xc).value = 0

# Modify fields:
# float[5]: Alt
# float[7]: Speed
# float[12]: Heading (radians)
floats[5] = 1524.0   # 5000 ft in meters
floats[7] = 220.0    # 220 kts
floats[12] = 1.570796 # 90 degrees in radians

new_payload = struct.pack('<22f', *floats)
ctypes.memmove(pMap + 0x3c, new_payload, 88)

print("Monitoring GDU instruments for 4 seconds...")
for i in range(20):
    time.sleep(0.2)
    hdg = read_gdu_val(addr_hdg)
    pitch = read_gdu_val(addr_pitch)
    ias = read_gdu_val(addr_ias)
    print(f"  [{i*0.2:.1f}s] GDU Hdg={hdg:.2f}°, Pitch={pitch:.2f}°, IAS={ias:.2f} kts")

# Restore original
print("\nRestoring original payload and flag...")
ctypes.memmove(pMap + 0x3c, orig_payload, 88)
ctypes.c_uint8.from_address(pMap + 0xc).value = orig_flag

kernel32.UnmapViewOfFile(ctypes.c_void_p(pMap))
kernel32.CloseHandle(hMap)
kernel32.CloseHandle(hGDU)
