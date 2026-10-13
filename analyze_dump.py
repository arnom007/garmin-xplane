"""
Analyze GarminHook dumper logs to find exact memory offsets for injection.

User maneuvers during dump session:
1. 130kt, climb to 2500ft, pitch ~6.25°, VS 900fpm, HDG 320
2. Level at 2500ft, command 202kt (settled ~195kt), pitch ~0°, HDG 320
3. Climb to 3000ft, speed dropping to 193-194kt, VS 900fpm
4. Turn to HDG 060, bank ~27°, level at 3000ft, 193kt, TAS/GS=202

Expected values in metric:
- Alt: 2500ft=762.0m, 3000ft=914.4m
- IAS: 130kt=66.9m/s, 195kt=100.3m/s, 202kt=103.9m/s
- IAS in kt: 130, 195, 202
- HDG: 320°=5.585rad, 060°=1.047rad
- Pitch: 6.25°=0.109rad
- Roll: 27°=0.471rad  
- VS: 900fpm=4.572m/s
- TAS/GS: 202kt=103.9m/s
"""

import struct
import re
import sys
from collections import defaultdict

LOG_PATH = r'C:\Users\Arnom\AppData\Roaming\GarminBridge\GarminHook_V7_20252.log'

def parse_snapshots(log_text):
    """Parse all DUMP SNAPSHOT blocks from the log."""
    snapshots = []
    blocks = log_text.split('=== DUMP SNAPSHOT ')
    
    for block in blocks[1:]:  # skip first empty split
        lines = block.strip().split('\n')
        header = lines[0]
        
        # Extract type
        m = re.search(r'TYPE: 0x([0-9a-fA-F]+)', header)
        if not m:
            continue
        msg_type = int(m.group(1), 16)
        
        # Extract timestamp from first hex line
        timestamp = None
        hex_bytes = []
        float_vals = {}
        double_vals = {}
        
        for line in lines[1:]:
            line = line.strip()
            if not line:
                continue
                
            # Timestamp extraction
            tm = re.match(r'\[(\d+:\d+:\d+)\]', line)
            if tm and timestamp is None:
                timestamp = tm.group(1)
            
            # Hex bytes: [HH:MM:SS]   0000: XX XX XX ...
            hm = re.match(r'\[\d+:\d+:\d+\]\s+([0-9A-Fa-f]{4}):\s+((?:[0-9A-Fa-f?]{2}\s+)+)', line)
            if hm:
                offset = int(hm.group(1), 16)
                byte_strs = hm.group(2).strip().split()
                for i, bs in enumerate(byte_strs):
                    if bs != '??':
                        try:
                            hex_bytes.append((offset + i, int(bs, 16)))
                        except ValueError:
                            pass
            
            # Float values: [HH:MM:SS]   F0000: val val val val
            fm = re.match(r'\[\d+:\d+:\d+\]\s+F([0-9A-Fa-f]{4}):\s+(.*)', line)
            if fm:
                offset = int(fm.group(1), 16)
                vals = fm.group(2).strip().split()
                for i, vs in enumerate(vals):
                    try:
                        float_vals[offset + i*4] = float(vs)
                    except ValueError:
                        pass
            
            # Double values: [HH:MM:SS]   D0000: val val
            dm = re.match(r'\[\d+:\d+:\d+\]\s+D([0-9A-Fa-f]{4}):\s+(.*)', line)
            if dm:
                offset = int(dm.group(1), 16)
                vals = dm.group(2).strip().split()
                for i, vs in enumerate(vals):
                    try:
                        double_vals[offset + i*8] = float(vs)
                    except ValueError:
                        pass
        
        if timestamp:
            raw_bytes = bytearray(128)
            for off, b in hex_bytes:
                if off < 128:
                    raw_bytes[off] = b
            
            snapshots.append({
                'type': msg_type,
                'time': timestamp,
                'floats': float_vals,
                'doubles': double_vals,
                'raw': bytes(raw_bytes),
                'hex_bytes': dict(hex_bytes),
            })
    
    return snapshots

def time_to_seconds(ts):
    parts = ts.split(':')
    return int(parts[0])*3600 + int(parts[1])*60 + int(parts[2])

def analyze_type(snapshots, msg_type, label):
    """Analyze all snapshots of a given type to find changing values."""
    typed = [s for s in snapshots if s['type'] == msg_type]
    if not typed:
        print(f"\n{'='*60}")
        print(f"NO SNAPSHOTS FOR TYPE 0x{msg_type:02x} ({label})")
        return
    
    print(f"\n{'='*60}")
    print(f"TYPE 0x{msg_type:02x} ({label}) — {len(typed)} snapshots")
    print(f"Time range: {typed[0]['time']} to {typed[-1]['time']}")
    print(f"{'='*60}")
    
    # Collect all float offsets that have non-zero, non-garbage values
    all_float_offsets = set()
    for s in typed:
        for off, val in s['floats'].items():
            all_float_offsets.add(off)
    
    # For each float offset, track the value over time
    print(f"\n--- FLOAT VALUE TRACKING (offsets with meaningful changes) ---")
    
    interesting_offsets = []
    for off in sorted(all_float_offsets):
        values = []
        for s in typed:
            if off in s['floats']:
                values.append(s['floats'][off])
            else:
                values.append(None)
        
        # Filter: only show offsets where values change AND are in reasonable range
        non_none = [v for v in values if v is not None]
        if not non_none:
            continue
        
        unique = set()
        for v in non_none:
            if abs(v) < 1e10:  # filter out garbage huge values
                unique.add(round(v, 4))
        
        if len(unique) > 1:
            min_v = min(v for v in non_none if abs(v) < 1e10) if any(abs(v) < 1e10 for v in non_none) else 0
            max_v = max(v for v in non_none if abs(v) < 1e10) if any(abs(v) < 1e10 for v in non_none) else 0
            interesting_offsets.append((off, min_v, max_v, non_none))
    
    for off, min_v, max_v, values in interesting_offsets:
        # Annotate with possible meanings
        annotations = []
        
        # Altitude checks (meters)
        if 700 < max_v < 1000 and min_v > 400:
            annotations.append("*** ALTITUDE (m)? 2500ft=762m, 3000ft=914m ***")
        
        # IAS checks (m/s)
        if 50 < max_v < 120 and min_v > 30:
            annotations.append("*** AIRSPEED (m/s)? 130kt=66.9, 195kt=100.3, 202kt=103.9 ***")
        
        # IAS checks (knots) 
        if 120 < max_v < 220 and min_v > 100:
            annotations.append("*** AIRSPEED (kt)? 130, 195, 202 ***")
        
        # Heading checks (radians)
        if 0.5 < max_v < 6.5 and min_v > 0.3:
            rng = max_v - min_v
            if rng > 1.0:
                annotations.append(f"*** HEADING (rad)? 320°=5.585, 060°=1.047 ***")
        
        # Heading checks (degrees)
        if 50 < max_v < 370 and min_v > 30:
            rng = max_v - min_v
            if rng > 100:
                annotations.append(f"*** HEADING (deg)? 320, 060 ***")
        
        # Pitch checks (radians)
        if -0.2 < min_v < 0.2 and 0.05 < max_v < 0.3:
            annotations.append("*** PITCH (rad)? 6.25°=0.109 ***")
        
        # Roll checks (radians)
        if -0.6 < min_v and max_v < 0.6:
            if max_v > 0.3 or min_v < -0.3:
                annotations.append("*** ROLL (rad)? 27°=0.471 ***")
        
        # VS checks (m/s)
        if -1 < min_v < 1 and 3 < max_v < 10:
            annotations.append("*** VERTICAL SPEED (m/s)? 900fpm=4.572 ***")
        
        ann = " ".join(annotations) if annotations else ""
        
        # Sample some values across time
        sample_indices = [0, len(values)//4, len(values)//2, 3*len(values)//4, len(values)-1]
        sample_indices = list(set(min(i, len(values)-1) for i in sample_indices))
        sample_indices.sort()
        samples = [(i, values[i]) for i in sample_indices if abs(values[i]) < 1e10]
        
        sample_str = ", ".join(f"[{i}]={v:.4f}" for i, v in samples)
        
        print(f"  Offset 0x{off:02X}: range [{min_v:.4f} .. {max_v:.4f}] ({len(unique)} unique) {ann}")
        print(f"    samples: {sample_str}")
    
    # Also analyze doubles
    print(f"\n--- DOUBLE VALUE TRACKING (offsets with meaningful changes) ---")
    all_double_offsets = set()
    for s in typed:
        for off, val in s['doubles'].items():
            all_double_offsets.add(off)
    
    for off in sorted(all_double_offsets):
        values = []
        for s in typed:
            if off in s['doubles']:
                values.append(s['doubles'][off])
            else:
                values.append(None)
        
        non_none = [v for v in values if v is not None]
        if not non_none:
            continue
        
        unique = set()
        for v in non_none:
            if abs(v) < 1e10:
                unique.add(round(v, 6))
        
        if len(unique) > 1:
            reasonable = [v for v in non_none if abs(v) < 1e10]
            if not reasonable:
                continue
            min_v = min(reasonable)
            max_v = max(reasonable)
            
            annotations = []
            # Lat check (radians): ~-0.384 for -22° (Rio area)
            if -0.5 < min_v < 0 and -0.5 < max_v < 0:
                annotations.append("*** LATITUDE (rad)? -22°=-0.384 ***")
            # Lon check (radians): ~-0.76 for -43° (Rio area)
            if -1.0 < min_v < -0.5 and -1.0 < max_v < -0.5:
                annotations.append("*** LONGITUDE (rad)? -43°=-0.750 ***")
            
            ann = " ".join(annotations) if annotations else ""
            
            sample_indices = [0, len(values)//4, len(values)//2, 3*len(values)//4, len(values)-1]
            sample_indices = list(set(min(i, len(values)-1) for i in sample_indices))
            sample_indices.sort()
            samples = [(i, values[i]) for i in sample_indices if values[i] is not None and abs(values[i]) < 1e10]
            
            sample_str = ", ".join(f"[{i}]={v:.6f}" for i, v in samples)
            print(f"  Offset 0x{off:02X}: range [{min_v:.6f} .. {max_v:.6f}] ({len(unique)} unique) {ann}")
            print(f"    samples: {sample_str}")


def detailed_timeline(snapshots, msg_type, offsets_of_interest, label):
    """Print detailed timeline for specific offsets."""
    typed = [s for s in snapshots if s['type'] == msg_type]
    if not typed:
        return
    
    print(f"\n{'='*60}")
    print(f"DETAILED TIMELINE: TYPE 0x{msg_type:02x} ({label})")
    print(f"{'='*60}")
    
    header = "Time       "
    for off, name in offsets_of_interest:
        header += f" | {name} (0x{off:02X})"
    print(header)
    print("-" * len(header))
    
    prev_time = None
    for s in typed:
        if prev_time and time_to_seconds(s['time']) - time_to_seconds(prev_time) > 10:
            print("  ... gap ...")
        prev_time = s['time']
        
        line = f"{s['time']}  "
        for off, name in offsets_of_interest:
            if off in s['floats']:
                v = s['floats'][off]
                if abs(v) < 1e10:
                    line += f" | {v:>12.4f}"
                else:
                    line += f" | {'(garbage)':>12s}"
            elif off in s['doubles']:
                v = s['doubles'][off]
                if abs(v) < 1e10:
                    line += f" | {v:>12.6f}"
                else:
                    line += f" | {'(garbage)':>12s}"
            else:
                line += f" | {'N/A':>12s}"
        print(line)


def main():
    print("Loading log file...")
    with open(LOG_PATH, 'r') as f:
        text = f.read()
    
    print(f"Log size: {len(text)} bytes")
    
    snapshots = parse_snapshots(text)
    print(f"Total snapshots parsed: {len(snapshots)}")
    
    # Count by type
    type_counts = defaultdict(int)
    for s in snapshots:
        type_counts[s['type']] += 1
    
    print("\nSnapshot counts by message type:")
    for t in sorted(type_counts.keys()):
        print(f"  0x{t:02x}: {type_counts[t]} snapshots")
    
    # Analyze each relevant type
    analyze_type(snapshots, 0x2c, "GDU AHRS")
    analyze_type(snapshots, 0x23, "ADC Airspeed")
    analyze_type(snapshots, 0x11, "GPS Message")
    
    print("\n\nDONE.")


if __name__ == '__main__':
    main()
