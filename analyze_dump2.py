"""
Detailed timeline analysis of the most promising offsets in TYPE 0x2c.

From first pass:
- 0x18: 0 -> 914.4  (looks like altitude in meters! 3000ft = 914.4m)
- 0x1C: 0 -> 103.87 (looks like speed! 202kt = 103.9 m/s) 
- 0x20: varies widely, includes negative values (attitude related?)
- 0x24: 0 -> 5.56, includes 3.2 (not clear)
- 0x28: -5.26 -> 908.97 (altitude related, slightly different from 0x18)
- 0x30: -1.69 -> 0.00 (GPS latitude as float? -1.69 rad = -96.8° NO, too big)
- 0x38: -1.83 -> 0.00 (GPS longitude as float?)

Key insight: -1.6913 and -1.8309 are NOT GPS coordinates in radians.
-22° lat = -0.384 rad, -43° lon = -0.750 rad

But wait - what if the dumper is reading the WRONG bytes as floats?
The AHRS magic is at offset 0x08. So the "absolute" offsets in the dump
are from addr_msg, not from the magic. Let me re-check.

Let me track the 0x2c offsets as a complete timeline to match maneuvers.
"""

import re
from collections import defaultdict

LOG_PATH = r'C:\Users\Arnom\AppData\Roaming\GarminBridge\GarminHook_V7_20252.log'

def parse_snapshots_0x2c(log_text):
    """Parse 0x2c snapshots with full raw byte data."""
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
            if not line:
                continue
            
            tm = re.match(r'\[(\d+:\d+:\d+)\]', line)
            if tm and timestamp is None:
                timestamp = tm.group(1)
            
            # Float values
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
            snapshots.append({
                'time': timestamp,
                'floats': float_vals,
            })
    
    return snapshots

def time_to_sec(ts):
    p = ts.split(':')
    return int(p[0])*3600 + int(p[1])*60 + int(p[2])

def main():
    with open(LOG_PATH, 'r') as f:
        text = f.read()
    
    snaps = parse_snapshots_0x2c(text)
    print(f"Parsed {len(snaps)} snapshots of TYPE 0x2c")
    
    # The user's maneuvers timeline (approximate):
    # Session starts ~21:58
    # Phase 1: 130kt climb to 2500ft, HDG 320, pitch ~6.25°
    # Phase 2: Level 2500ft, accel to 202kt (settled ~195), HDG 320, pitch ~0°  
    # Phase 3: Climb to 3000ft, 193-194kt, VS 900fpm
    # Phase 4: Turn to HDG 060, bank ~27°, level 3000ft
    
    # Track the key offsets over time
    offsets = [
        (0x18, "0x18"),
        (0x1C, "0x1C"),
        (0x20, "0x20"),
        (0x24, "0x24"),
        (0x28, "0x28"),
        (0x2C, "0x2C"),
        (0x30, "0x30"),
        (0x34, "0x34"),
        (0x38, "0x38"),
        (0x3C, "0x3C"),
        (0x60, "0x60"),
        (0x78, "0x78"),
        (0x7C, "0x7C"),
    ]
    
    # Print header
    print(f"\n{'Time':>10s}", end="")
    for off, name in offsets:
        print(f" | {name:>12s}", end="")
    print()
    print("-" * (10 + 15 * len(offsets)))
    
    # Print every 5th sample to keep output manageable
    prev_sec = 0
    for idx, s in enumerate(snaps):
        sec = time_to_sec(s['time'])
        
        # Print every sample but mark phase transitions
        if idx % 5 != 0 and idx != 0 and idx != len(snaps)-1:
            continue
        
        print(f"{s['time']:>10s}", end="")
        for off, name in offsets:
            v = s['floats'].get(off)
            if v is None:
                print(f" | {'N/A':>12s}", end="")
            elif abs(v) > 1e9:
                print(f" | {'GARB':>12s}", end="")
            else:
                print(f" | {v:>12.4f}", end="")
        print()
        prev_sec = sec

    # Now let's do the critical analysis:
    # What VALUE appears at each offset when we know the flight state?
    print("\n\n" + "="*80)
    print("CRITICAL CORRELATION ANALYSIS")
    print("="*80)
    
    # Expected values at different phases:
    # Phase 1 (early): alt=762m(2500ft), ias=66.9m/s(130kt), hdg=5.585rad(320°), pitch=0.109rad(6.25°)
    # Phase 2 (mid):   alt=762m, ias=100.3m/s(195kt), hdg=5.585rad(320°), pitch=0
    # Phase 3 (climb): alt→914m, ias=99.3m/s(193kt), hdg=5.585rad(320°), pitch>0
    # Phase 4 (turn):  alt=914m, ias=99.3m/s, hdg→1.047rad(060°), roll=0.471rad(27°)
    
    targets = {
        'ALTITUDE (m)': [(762.0, '2500ft'), (914.4, '3000ft')],
        'AIRSPEED (m/s)': [(66.9, '130kt'), (100.3, '195kt'), (103.9, '202kt')],
        'AIRSPEED (kt)': [(130.0, '130kt'), (195.0, '195kt'), (202.0, '202kt')],
        'HEADING (rad)': [(5.585, '320°'), (1.047, '060°')],
        'HEADING (deg)': [(320.0, '320°'), (60.0, '060°')],
        'PITCH (rad)': [(0.109, '6.25°'), (0.0, '0°')],
        'ROLL (rad)': [(0.471, '27°'), (0.0, '0°')],
        'VS (m/s)': [(4.572, '900fpm'), (0.0, '0fpm')],
    }
    
    all_offsets = set()
    for s in snaps:
        all_offsets.update(s['floats'].keys())
    
    for param_name, target_vals in targets.items():
        print(f"\n--- Searching for {param_name} ---")
        for target, label in target_vals:
            if target == 0:
                continue  # zero matches too many things
            tolerance = abs(target) * 0.05 + 0.5  # 5% + 0.5 absolute
            
            matches = defaultdict(int)
            for s in snaps:
                for off in sorted(all_offsets):
                    v = s['floats'].get(off)
                    if v is not None and abs(v) < 1e10:
                        if abs(v - target) < tolerance:
                            matches[off] += 1
            
            if matches:
                sorted_matches = sorted(matches.items(), key=lambda x: -x[1])
                top = sorted_matches[:5]
                print(f"  {label} ({target:.2f}): ", end="")
                for off, count in top:
                    print(f"0x{off:02X}({count} hits) ", end="")
                print()
            else:
                print(f"  {label} ({target:.2f}): NO MATCHES")

    # Extra: check the early snapshots vs late snapshots  
    print("\n\n" + "="*80)
    print("PHASE COMPARISON: First 10 vs Last 10 snapshots (only 0x2c)")
    print("="*80)
    
    early = snaps[:10]
    late = snaps[-10:]
    
    for off in sorted(all_offsets):
        early_vals = [s['floats'].get(off) for s in early if s['floats'].get(off) is not None and abs(s['floats'].get(off, 1e20)) < 1e9]
        late_vals = [s['floats'].get(off) for s in late if s['floats'].get(off) is not None and abs(s['floats'].get(off, 1e20)) < 1e9]
        
        if not early_vals or not late_vals:
            continue
        
        early_avg = sum(early_vals) / len(early_vals)
        late_avg = sum(late_vals) / len(late_vals)
        
        if abs(early_avg - late_avg) > 0.001:
            print(f"  0x{off:02X}: early_avg={early_avg:>12.4f}, late_avg={late_avg:>12.4f}, delta={late_avg - early_avg:>12.4f}")

if __name__ == '__main__':
    main()
