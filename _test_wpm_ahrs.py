import ctypes
import struct
import time

kernel32 = ctypes.windll.kernel32
hGDU = kernel32.OpenProcess(0x1F0FFF, False, 6452)

addr_pitch = 0x312BB9EC
addr_hdg = 0x312BB9F0
addr_roll = 0x312BB9F8

def read_ahrs():
    buf = ctypes.create_string_buffer(16)
    br = ctypes.c_size_t()
    kernel32.ReadProcessMemory(hGDU, ctypes.c_void_p(addr_pitch), buf, 16, ctypes.byref(br))
    pitch, hdg, _, roll = struct.unpack('<ffff', buf.raw)
    return pitch, hdg, roll

p0, h0, r0 = read_ahrs()
print(f"Current GDU AHRS: Pitch={p0:.2f}°, Hdg={h0:.2f}°, Roll={r0:.2f}°")

# Let's write single value:
test_p = 15.0
test_h = 45.0
test_r = 30.0

print(f"\nWriting test: Pitch={test_p}°, Hdg={test_h}°, Roll={test_r}°...")
kernel32.WriteProcessMemory(hGDU, ctypes.c_void_p(addr_pitch), struct.pack('<f', test_p), 4, None)
kernel32.WriteProcessMemory(hGDU, ctypes.c_void_p(addr_hdg), struct.pack('<f', test_h), 4, None)
kernel32.WriteProcessMemory(hGDU, ctypes.c_void_p(addr_roll), struct.pack('<f', test_r), 4, None)

print("Monitoring for 1 second (10 samples)...")
for s in range(10):
    time.sleep(0.1)
    p, h, r = read_ahrs()
    print(f"  [{s*0.1:.1f}s] Pitch={p:.2f}°, Hdg={h:.2f}°, Roll={r:.2f}°")

# Restore
kernel32.WriteProcessMemory(hGDU, ctypes.c_void_p(addr_pitch), struct.pack('<f', p0), 4, None)
kernel32.WriteProcessMemory(hGDU, ctypes.c_void_p(addr_hdg), struct.pack('<f', h0), 4, None)
kernel32.WriteProcessMemory(hGDU, ctypes.c_void_p(addr_roll), struct.pack('<f', r0), 4, None)

kernel32.CloseHandle(hGDU)
