import ctypes
import struct
import time
import math

kernel32 = ctypes.windll.kernel32
kernel32.MapViewOfFile.restype = ctypes.c_void_p

FILE_MAP_ALL_ACCESS = 0xF001F

hMap = kernel32.OpenFileMappingW(FILE_MAP_ALL_ACCESS, False, 'Local\\trainerShrSuperObj')
if not hMap:
    print("Failed to open Local\\trainerShrSuperObj")
    exit(1)

pMap = kernel32.MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, 148)

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

# Read current shared memory
orig_flag = ctypes.c_uint8.from_address(pMap + 0xc).value
orig_payload = ctypes.string_at(pMap + 0x3c, 88)
floats = list(struct.unpack('<22f', orig_payload))

print("\nCurrent values at 0x84..0x94 (floats 18..21):")
print(f"  Alt   float[18]: {floats[18]:.2f}")
print(f"  IAS   float[19]: {floats[19]:.2f}")
print(f"  VS    float[20]: {floats[20]:.2f}")
print(f"  Hdg   float[21]: {floats[21]:.2f}")

# Keep flag 0xc = 0 to prevent internal simulator overwrite
ctypes.c_uint8.from_address(pMap + 0xc).value = 0

# Inject new values:
target_hdg_deg = 90.0
target_hdg_rad = math.radians(target_hdg_deg)
target_alt_m = 914.4 # 3000 ft
target_ias_kts = 150.0
target_vs_ms = 5.08  # 1000 fpm

floats[18] = target_alt_m
floats[19] = target_ias_kts
floats[20] = target_vs_ms
floats[21] = target_hdg_rad

new_payload = struct.pack('<22f', *floats)
ctypes.memmove(pMap + 0x3c, new_payload, 88)
print(f"\nInjected! Alt={target_alt_m}m, IAS={target_ias_kts}kt, VS={target_vs_ms}m/s, Hdg={target_hdg_deg}° ({target_hdg_rad:.4f} rad)")

print("Monitoring for 3 seconds...")
for i in range(15):
    time.sleep(0.2)
    hdg = read_gdu_val(addr_hdg)
    pitch = read_gdu_val(addr_pitch)
    ias = read_gdu_val(addr_ias)
    # also check if shared memory was modified
    cur_buf = ctypes.string_at(pMap + 0x3c, 88)
    cur_floats = struct.unpack('<22f', cur_buf)
    print(f"  [{i*0.2:.1f}s] GDU Hdg={hdg:.2f}°, Pitch={pitch:.2f}°, IAS={ias:.2f} kts | SHM Hdg={math.degrees(cur_floats[21]):.2f}°")

# Restore
ctypes.memmove(pMap + 0x3c, orig_payload, 88)
ctypes.c_uint8.from_address(pMap + 0xc).value = orig_flag

kernel32.UnmapViewOfFile(ctypes.c_void_p(pMap))
kernel32.CloseHandle(hMap)
kernel32.CloseHandle(hGDU)
