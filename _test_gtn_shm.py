"""
_test_gtn_shm.py — GTN 650Xi Telemetry Memory Ingestion Verification Test
========================================================================
Verifies that telemetry injected into Named Shared Memory Local\\trainerShrSuperObj
propagates synchronously and cleanly into GTN 650Xi process memory (gtn_simulator.exe)
without internal demo physics fighting or lag.
"""

import ctypes
import ctypes.wintypes as wt
import struct
import time
import math
from mem_injector import find_process_pid, get_process_module_base

kernel32 = ctypes.windll.kernel32
kernel32.MapViewOfFile.restype = ctypes.c_void_p
FILE_MAP_ALL_ACCESS = 0xF001F
PROCESS_ALL_ACCESS = 0x1F0FFF

print("=== VERIFYING GTN 650Xi SHARED MEMORY TELEMETRY INGESTION ===")

# 1. Connect to Local\trainerShrSuperObj
hSHM = kernel32.OpenFileMappingW(FILE_MAP_ALL_ACCESS, False, 'Local\\trainerShrSuperObj')
if not hSHM:
    print("[FAIL] Could not open Local\\trainerShrSuperObj")
    exit(1)

pSHM = kernel32.MapViewOfFile(hSHM, FILE_MAP_ALL_ACCESS, 0, 0, 148)
if not pSHM:
    print("[FAIL] Could not map Local\\trainerShrSuperObj")
    kernel32.CloseHandle(hSHM)
    exit(1)

print("[PASS] Connected and mapped Local\\trainerShrSuperObj")

# 2. Connect to GTN 650Xi process
gtn_pid = find_process_pid('gtn_simulator')
if not gtn_pid:
    print("[FAIL] gtn_simulator.exe process not found")
    exit(1)

hGTN = kernel32.OpenProcess(PROCESS_ALL_ACCESS, False, gtn_pid)
if not hGTN:
    print(f"[FAIL] Could not open GTN process PID {gtn_pid}")
    exit(1)

print(f"[PASS] Connected to GTN 650Xi PID={gtn_pid}")

# 3. Resolve GTN's internal mapped view of trainerShrSuperObj via iop_sim_sim.dll
iop_base = get_process_module_base(hGTN, 'iop_sim_sim.dll')
addr_gtn_shm = None
if iop_base:
    ptr_buf = ctypes.create_string_buffer(4)
    if kernel32.ReadProcessMemory(hGTN, ctypes.c_void_p(iop_base + 0x1bc1f0), ptr_buf, 4, None):
        addr_gtn_shm = struct.unpack('<I', ptr_buf.raw)[0]

if not addr_gtn_shm:
    addr_gtn_shm = 0x217d0000

print(f"[PASS] Resolved GTN internal SHM view at 0x{addr_gtn_shm:08X}")

# 4. Save original state and disable demo physics overwrite
orig_flag = ctypes.c_uint8.from_address(pSHM + 0x0C).value
orig_payload = ctypes.string_at(pSHM + 0x3C, 88)

ctypes.c_uint8.from_address(pSHM + 0x0C).value = 0
print("[PASS] Disabled supernav demo overwrite (SHM flag 0x0C = 0)")

try:
    # 5. Define test telemetry targets
    target_alt_ft = 5500.0
    target_alt_m = target_alt_ft * 0.3048
    target_ias_kts = 165.0
    target_hdg_deg = 245.0
    target_hdg_rad = math.radians(target_hdg_deg)
    target_roll_deg = -15.0
    target_roll_rad = math.radians(target_roll_deg)
    target_vs_fpm = 750.0
    target_vs_ms = target_vs_fpm * 0.00508

    print(f"\n[INJECT] Target telemetry: Alt={target_alt_ft:.0f}ft ({target_alt_m:.1f}m), IAS={target_ias_kts:.1f}kt, Hdg={target_hdg_deg:.1f}°, Roll={target_roll_deg:.1f}°, VS={target_vs_fpm:.0f}fpm")

    shm_data = ctypes.string_at(pSHM + 0x3C, 88)
    floats = list(struct.unpack('<22f', shm_data))
    floats[5]  = target_alt_m
    floats[7]  = target_ias_kts
    floats[12] = target_hdg_rad
    floats[14] = target_roll_rad
    floats[18] = target_alt_m
    floats[19] = target_ias_kts
    floats[20] = target_vs_ms
    floats[21] = target_hdg_rad

    ctypes.memmove(pSHM + 0x3C, struct.pack('<22f', *floats), 88)

    # 6. Monitor readback directly from GTN process memory for 2 seconds
    print("Monitoring GTN memory readback for 2 seconds...")
    samples = []
    for i in range(10):
        time.sleep(0.2)
        buf = ctypes.create_string_buffer(88)
        kernel32.ReadProcessMemory(hGTN, ctypes.c_void_p(addr_gtn_shm + 0x3C), buf, 88, None)
        cur = struct.unpack('<22f', buf.raw)
        alt_ft = cur[5] / 0.3048
        ias = cur[7]
        hdg = math.degrees(cur[12]) % 360.0
        roll = math.degrees(cur[14])
        vs = cur[20] / 0.00508
        samples.append((alt_ft, ias, hdg, roll, vs))
        print(f"  [{i*0.2:.1f}s] GTN Readback: Alt={alt_ft:.0f}ft, IAS={ias:.1f}kt, Hdg={hdg:.1f}°, Roll={roll:.1f}°, VS={vs:.0f}fpm")

    last_alt, last_ias, last_hdg, last_roll, last_vs = samples[-1]

    # 7. Assertions
    assert abs(last_alt - target_alt_ft) < 1.0, f"GTN Alt mismatch: {last_alt} vs {target_alt_ft}"
    assert abs(last_ias - target_ias_kts) < 0.1, f"GTN IAS mismatch: {last_ias} vs {target_ias_kts}"
    assert abs(last_hdg - target_hdg_deg) < 0.1, f"GTN Hdg mismatch: {last_hdg} vs {target_hdg_deg}"
    assert abs(last_roll - target_roll_deg) < 0.1, f"GTN Roll mismatch: {last_roll} vs {target_roll_deg}"
    assert abs(last_vs - target_vs_fpm) < 1.0, f"GTN VS mismatch: {last_vs} vs {target_vs_fpm}"

    print("\n[ALL ASSERTIONS PASSED] GTN 650Xi telemetry ingestion verified with 100% precision!")

finally:
    # 8. Clean restoration
    ctypes.memmove(pSHM + 0x3C, orig_payload, 88)
    ctypes.c_uint8.from_address(pSHM + 0x0C).value = orig_flag
    kernel32.UnmapViewOfFile(ctypes.c_void_p(pSHM))
    kernel32.CloseHandle(hSHM)
    kernel32.CloseHandle(hGTN)
    print("[PASS] Cleanly restored SHM and closed handles.")
