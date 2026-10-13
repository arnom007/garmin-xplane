"""
FINAL DEFINITIVE ANALYSIS.

From correlation analysis, the 0x2c message structure (offsets from addr_msg) is:

CONFIRMED:
- 0x18: ALTITUDE (float, meters). Goes from ~0 -> 762 (2500ft) -> 914.4 (3000ft). 351+58=409 hits.
- 0x1C: AIRSPEED (float, m/s). Goes from ~0 -> ~66.9 (130kt) -> ~103.8 (202kt). 55+393=448 hits.
- 0x20: HEADING (float, radians). HDG search: 320°=5.585rad had 9 hits, 060°=1.047rad had 319 hits.
         Early avg=4.5561, Late avg=0.6634. But wait - late value 0.6634 is NOT 1.047 (060°).
         0.6634 rad = 38°. Hmm, that's not right either...
         Let me check: could 0x20 be something else?
- 0x24: Varied from 0.4158 (early) to -0.0002 (late). 
        Got 61 hits for heading 5.585 rad (320°). 
        Also got 53 hits for VS 4.572 m/s (900 fpm).
        These are very different values - need to check timeline.
- 0x28: ALTITUDE (float, meters). Similar to 0x18 but slightly different values (908.9 vs 914.4).
        This is probably the CURRENT altitude (barometric) while 0x18 is the TARGET altitude.

NEED TO VERIFY:
- 0x20 with detailed timeline to see if it follows heading changes
- 0x24 with detailed timeline to see if it's VS or heading
- 0x30, 0x38: slowly drifting negative values (-1.69, -1.83) - probably lat/lon

Let me print detailed timeline for 0x20 and 0x24 every 4 seconds.
"""

import re
from collections import defaultdict

LOG_PATH = r'C:\Users\Arnom\AppData\Roaming\GarminBridge\GarminHook_V7_20252.log'

def parse_snapshots_0x2c(log_text):
    snapshots = []
    blocks = log_text.split('=== DUMP SNAPSHOT ')
    
    for block in blocks[1:]:
        lines = block.strip().split('\n')
        header = lines[0]
        
        m = re.search(r'TYPE: 0x([0-9a-fA-F]+)', header)
        if not m:
            continue
        msg_type = int(m.group(1), 16)
        if msg_type != 0x2c:
            continue
        
        timestamp = None
        float_vals = {}
        
        for line in lines[1:]:
            line = line.strip()
            tm = re.match(r'\[(\d+:\d+:\d+)\]', line)
            if tm and timestamp is None:
                timestamp = tm.group(1)
            
            fm = re.match(r'\[\d+:\d+:\d+\]\s+F([0-9A-Fa-f]{4}):\s+(.*)', line)
            if fm:
                offset = int(fm.group(1), 16)
                vals = fm.group(2).strip().split()
                for i, vs in enumerate(vals):
                    try:
                        float_vals[offset + i*4] = float(vs)
                    except ValueError:
                        float_vals[offset + i*4] = None
        
        if timestamp:
            snapshots.append({'time': timestamp, 'floats': float_vals})
    
    return snapshots

def fmt(v):
    """Format a float value safely."""
    if v is None:
        return f"{'N/A':>10s}"
    if abs(v) > 1e9:
        return f"{'GARB':>10s}"
    return f"{v:>10.4f}"

import math

def main():
    with open(LOG_PATH, 'r') as f:
        text = f.read()
    
    snaps = parse_snapshots_0x2c(text)
    
    print("="*120)
    print("DETAILED TIMELINE: 0x2c offsets 0x18-0x28 (every 2nd sample)")  
    print("="*120)
    print(f"{'Time':>10s} | {'0x18 ALT?':>10s} | {'0x1C IAS?':>10s} | {'0x20 HDG?':>10s} | {'0x20°':>8s} | {'0x24 ???':>10s} | {'0x24°':>8s} | {'0x28 ALT2?':>10s}")
    print("-"*100)
    
    for idx, s in enumerate(snaps):
        if idx % 2 != 0:
            continue
        
        v18 = s['floats'].get(0x18)
        v1c = s['floats'].get(0x1C)
        v20 = s['floats'].get(0x20)
        v24 = s['floats'].get(0x24)
        v28 = s['floats'].get(0x28)
        
        # Convert to degrees for readability
        v20_deg = ""
        if v20 is not None and abs(v20) < 100:
            v20_deg = f"{math.degrees(v20):>7.1f}°"
        
        v24_deg = ""
        if v24 is not None and abs(v24) < 100:
            v24_deg = f"{math.degrees(v24):>7.1f}°"
        
        print(f"{s['time']:>10s} | {fmt(v18)} | {fmt(v1c)} | {fmt(v20)} | {v20_deg:>8s} | {fmt(v24)} | {v24_deg:>8s} | {fmt(v28)}")

    # Now let's look at the 0x30 and 0x38 as potential lat/lon
    print("\n\n" + "="*80)
    print("GPS ANALYSIS: 0x30 and 0x38 as lat/lon (float values)")
    print("="*80)
    print("NOTE: Expected lat=-22°=-0.384rad, lon=-43°=-0.750rad")
    print("But we see 0x30=-1.69, 0x38=-1.83. These are NOT radians of GPS.")
    print("In DEGREES: -1.69rad = -96.8°, -1.83rad = -104.8°")
    print()
    
    # Check if maybe they're in a different unit
    # What if they're in Garmin semicircles? 1 semicircle = 180°/2^31
    # Or maybe they're in a scaled integer format?
    # Let's just look at the raw hex at those offsets
    print("Could 0x30 and 0x38 be 2x floats that form a double (word-swapped)?")
    print("0x30 float = -1.6913 (bytes: BF D8 7C 85 in little-endian)")
    print("0x34 float = 426.0332 (bytes: 43 D5 04 42 in little-endian)")
    print("If swapped: the double at [0x34..0x37][0x30..0x33] = ?")
    
    import struct
    # -1.6913 as float bytes
    b30 = struct.pack('<f', -1.6913)  # 85 7C D8 BF
    # 426.0332 as float bytes  
    b34 = struct.pack('<f', 426.0332) # 42 04 D5 43
    
    # Normal order: b30 + b34
    d_normal = struct.unpack('<d', b30 + b34)[0]
    # Swapped: b34 + b30
    d_swapped = struct.unpack('<d', b34 + b30)[0]
    
    print(f"  Normal double (0x30..0x37): {d_normal}")
    print(f"  Swapped double (0x34+0x30): {d_swapped}")
    print(f"  Normal in degrees: {math.degrees(d_normal)}")
    print(f"  Swapped in degrees: {math.degrees(d_swapped)}")
    
    # Same for 0x38
    b38 = struct.pack('<f', -1.8309)
    b3c = struct.pack('<f', 0.0)  # 0x3C is usually 0
    d38_normal = struct.unpack('<d', b38 + b3c)[0]
    d38_swapped = struct.unpack('<d', b3c + b38)[0]
    print(f"\n  0x38 Normal double: {d38_normal}")
    print(f"  0x38 Swapped double: {d38_swapped}")

    # Also check: what if 0x30-0x37 is a SINGLE double?
    # The dumper reports it as F0030: -1.6913  GARB  -1.8309  0.0000
    # That means F0030[0]=float@0x30=-1.6913, F0030[1]=float@0x34=GARBAGE
    # So the double at 0x30 would be combining those two floats' bytes.
    # Let me read actual hex from the log
    
    print("\n\nLet me look at actual hex bytes at offset 0x30-0x3F:")
    blocks = text.split('=== DUMP SNAPSHOT ')
    count = 0
    for block in blocks[1:]:
        if 'TYPE: 0x2c' not in block:
            continue
        lines = block.split('\n')
        for line in lines:
            if '0030:' in line and 'F0030' not in line and 'D0030' not in line:
                # Raw hex line
                parts = line.strip().split()
                # Find the hex bytes after "0030:"
                try:
                    idx = parts.index('0030:')
                    hex_str = ' '.join(parts[idx+1:idx+17])
                    
                    # Parse actual bytes
                    byte_list = []
                    for b in parts[idx+1:idx+17]:
                        if len(b) == 2:
                            try:
                                byte_list.append(int(b, 16))
                            except ValueError:
                                break
                    
                    if len(byte_list) >= 16:
                        raw = bytes(byte_list)
                        f0 = struct.unpack_from('<f', raw, 0)[0]
                        f1 = struct.unpack_from('<f', raw, 4)[0]
                        f2 = struct.unpack_from('<f', raw, 8)[0]
                        f3 = struct.unpack_from('<f', raw, 12)[0]
                        d0 = struct.unpack_from('<d', raw, 0)[0]
                        d1 = struct.unpack_from('<d', raw, 8)[0]
                        
                        if count < 5:
                            ts = ""
                            tm = re.match(r'\[(\d+:\d+:\d+)\]', line.strip())
                            if tm:
                                ts = tm.group(1)
                            print(f"  [{ts}] hex: {hex_str}")
                            print(f"    floats: {f0:.4f}, {f1:.4f}, {f2:.4f}, {f3:.4f}")
                            print(f"    doubles: {d0:.10f}, {d1:.10f}")
                            print(f"    d0_deg: {math.degrees(d0):.6f}°, d1_deg: {math.degrees(d1):.6f}°")
                        count += 1
                except (ValueError, IndexError):
                    pass
    print(f"  (Total 0x2c samples with hex at 0x30: {count})")


if __name__ == '__main__':
    main()
