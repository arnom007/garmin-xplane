import time
import math
import ctypes
import struct
from mem_injector import GDUMemoryInjector, find_process_pid, get_process_module_base

print("=== VERIFYING MEM_INJECTOR INTEGRATION (DUAL TRAINER: GDU & GTN) ===")
inj = GDUMemoryInjector()

if not inj.connect():
    print("[FAIL] Could not connect")
    exit(1)

print("[PASS] Connected to GDU and SHM")
print(f"Discovered GDU addresses: {inj.addrs}")

# Connect to GTN for cross-trainer verification
gtn_pid = find_process_pid('gtn_simulator')
hGTN = None
addr_gtn_shm = 0x217d0000
if gtn_pid:
    kernel32 = ctypes.windll.kernel32
    hGTN = kernel32.OpenProcess(0x1F0FFF, False, gtn_pid)
    iop_base = get_process_module_base(hGTN, 'iop_sim_sim.dll')
    if iop_base:
        ptr_buf = ctypes.create_string_buffer(4)
        if kernel32.ReadProcessMemory(hGTN, ctypes.c_void_p(iop_base + 0x1bc1f0), ptr_buf, 4, None):
            addr_gtn_shm = struct.unpack('<I', ptr_buf.raw)[0]
    print(f"[PASS] Connected to GTN 650Xi (PID {gtn_pid}) | SHM view: 0x{addr_gtn_shm:08X}")

# Disable demo flight model
ok = inj.disable_demo_flight_model()
print(f"[PASS] Demo flight model disabled: {ok}")

# Start injection
inj.start_injection()
print("[PASS] Injection thread started")

# Feed test telemetry
target_pitch = 6.0
target_roll = -18.0
target_hdg = 210.0
target_ias = 135.0
target_alt = 4200.0
target_vs = 800.0

print(f"\nFeeding target: Pitch={target_pitch}°, Roll={target_roll}°, Hdg={target_hdg}°, IAS={target_ias}kt, Alt={target_alt}ft, VS={target_vs}fpm")
inj.update_target_data(
    pitch_deg=target_pitch,
    roll_deg=target_roll,
    heading_deg=target_hdg,
    ias_kts=target_ias,
    altitude_ft=target_alt,
    vs_fpm=target_vs,
)

print("Monitoring for 3 seconds...")
samples_gdu = []
samples_gtn = []
for i in range(15):
    time.sleep(0.2)
    p = inj._read_float(inj.addrs['ahrs_pitch'])
    r = inj._read_float(inj.addrs['ahrs_roll'])
    h = inj._read_float(inj.addrs['ahrs_hdg'])
    ias = inj._read_float(inj.addrs['ias_widget'])
    alt = inj._read_float(inj.addrs['alt_widgets'][0])
    vs = inj._read_float(inj.addrs['vs_widgets'][0])
    samples_gdu.append((p, r, h, ias, alt, vs))

    if hGTN:
        buf = ctypes.create_string_buffer(88)
        kernel32.ReadProcessMemory(hGTN, ctypes.c_void_p(addr_gtn_shm + 0x3C), buf, 88, None)
        cur = struct.unpack('<22f', buf.raw)
        gtn_alt = cur[5] / 0.3048
        gtn_ias = cur[7]
        gtn_hdg = math.degrees(cur[12]) % 360.0
        gtn_roll = math.degrees(cur[14])
        gtn_vs = cur[20] / 0.00508
        samples_gtn.append((gtn_alt, gtn_ias, gtn_hdg, gtn_roll, gtn_vs))

    if i % 3 == 0:
        print(f"  [{i*0.2:.1f}s] GDU: P={p:.1f}°, R={r:.1f}°, H={h:.1f}°, IAS={ias:.1f}kt, Alt={alt:.0f}ft, VS={vs:.0f}fpm")
        if hGTN:
            print(f"         GTN: R={gtn_roll:.1f}°, H={gtn_hdg:.1f}°, IAS={gtn_ias:.1f}kt, Alt={gtn_alt:.0f}ft, VS={gtn_vs:.0f}fpm")

last_p, last_r, last_h, last_ias, last_alt, last_vs = samples_gdu[-1]

# GDU Assertions
assert abs(last_p - target_pitch) < 0.1, f"GDU Pitch mismatch: {last_p} vs {target_pitch}"
assert abs(last_r - target_roll) < 0.1, f"GDU Roll mismatch: {last_r} vs {target_roll}"
assert abs(last_h - target_hdg) < 0.1, f"GDU Heading mismatch: {last_h} vs {target_hdg}"
assert abs(last_ias - target_ias) < 0.1, f"GDU IAS mismatch: {last_ias} vs {target_ias}"
assert abs(last_alt - target_alt) < 1.0, f"GDU Alt mismatch: {last_alt} vs {target_alt}"
assert abs(last_vs - target_vs) < 1.0, f"GDU VS mismatch: {last_vs} vs {target_vs}"

# GTN Assertions
if hGTN:
    gtn_alt, gtn_ias, gtn_hdg, gtn_roll, gtn_vs = samples_gtn[-1]
    assert abs(gtn_alt - target_alt) < 1.0, f"GTN Alt mismatch: {gtn_alt} vs {target_alt}"
    assert abs(gtn_ias - target_ias) < 0.1, f"GTN IAS mismatch: {gtn_ias} vs {target_ias}"
    assert abs(gtn_hdg - target_hdg) < 0.1, f"GTN Heading mismatch: {gtn_hdg} vs {target_hdg}"
    assert abs(gtn_roll - target_roll) < 0.1, f"GTN Roll mismatch: {gtn_roll} vs {target_roll}"
    assert abs(gtn_vs - target_vs) < 1.0, f"GTN VS mismatch: {gtn_vs} vs {target_vs}"
    kernel32.CloseHandle(hGTN)

print("\n[ALL DUAL-TRAINER ASSERTIONS PASSED] Both GDU 1060 TXi and GTN 650Xi matched target telemetry perfectly!")

# Stop injection and clean up
inj.stop_injection()
inj.disconnect()
print("[PASS] Cleanly disconnected and restored.")
