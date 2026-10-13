"""Analyze HSDB2N data from GarminHook logs to find AHRS/ADC sensor patterns."""
import re
import os
import glob
from collections import Counter

# Find latest GDU log
logs = glob.glob(r'C:\Users\Arnom\AppData\Roaming\GarminBridge\*.log')
logs.sort(key=os.path.getmtime, reverse=True)

# Use the two largest recent logs (GDU + GTN)
for log_path in logs[:2]:
    print(f"\n{'='*70}")
    print(f"ANALYZING: {os.path.basename(log_path)} ({os.path.getsize(log_path)} bytes)")
    print(f"{'='*70}")
    
    with open(log_path, 'r', errors='replace') as f:
        lines = f.readlines()

    # Extract HSDB2N_PUT entries with hex data
    hex_entries = []
    current_entry = None
    for line in lines:
        line = line.strip()
        if 'HSDB2N_PUT: p1=' in line:
            if current_entry and current_entry.get('hex'):
                hex_entries.append(current_entry)
            m = re.search(r'p1=0x([0-9A-Fa-f]+) p2=0x([0-9A-Fa-f]+)', line)
            if m:
                current_entry = {
                    'p1': int(m.group(1), 16),
                    'p2': int(m.group(2), 16),
                    'hex': {},
                    'time': line[:10]
                }
            else:
                current_entry = None
        elif current_entry and '  H2P ' in line and 'FLOATS' not in line:
            m = re.search(r'H2P ([0-9A-Fa-f]+): (.+)', line)
            if m:
                offset = int(m.group(1), 16)
                hex_bytes = m.group(2).strip().split()
                current_entry['hex'][offset] = hex_bytes
    
    if current_entry and current_entry.get('hex'):
        hex_entries.append(current_entry)

    print(f"Total HSDB2N_PUT entries with hex data: {len(hex_entries)}")
    
    if not hex_entries:
        continue

    # Group by p1 address
    p1_counts = Counter(e['p1'] for e in hex_entries)
    print(f"\nSource addresses (p1):")
    for addr, count in p1_counts.most_common():
        print(f"  0x{addr:08X}: {count} entries")

    # Analyze first byte patterns (could be ARINC 429 labels)
    print(f"\nFirst 2 bytes (label/type?):")
    type_counts = Counter()
    for e in hex_entries:
        if 0 in e['hex'] and len(e['hex'][0]) >= 2:
            first2 = ' '.join(e['hex'][0][:2])
            type_counts[first2] += 1
    for pattern, count in type_counts.most_common(15):
        print(f"  {pattern}: {count} occurrences")

    # Look for byte[0] patterns specifically
    print(f"\nByte[0] (ARINC label?):")
    b0_counts = Counter()
    for e in hex_entries:
        if 0 in e['hex'] and len(e['hex'][0]) >= 1:
            b0_counts[e['hex'][0][0]] += 1
    for val, count in b0_counts.most_common(10):
        print(f"  0x{val}: {count} occurrences")

    # Show unique hex patterns grouped by first 2 bytes
    print(f"\n=== Unique message patterns (by first 2 bytes) ===")
    seen_types = set()
    for e in hex_entries:
        if 0 not in e['hex']:
            continue
        first2 = ' '.join(e['hex'][0][:2]) if len(e['hex'][0]) >= 2 else '??'
        if first2 in seen_types:
            continue
        seen_types.add(first2)
        print(f"\n  Type [{first2}] from p1=0x{e['p1']:08X}:")
        for offset in sorted(e['hex'].keys()):
            hex_str = ' '.join(e['hex'][offset])
            print(f"    {offset:04X}: {hex_str}")
        # Also interpret as ARINC 429 words (32-bit, LSB first)
        if 0 in e['hex'] and len(e['hex'][0]) >= 4:
            raw = e['hex'][0]
            for i in range(0, min(len(raw), 16), 4):
                try:
                    word = int(raw[i+3], 16) << 24 | int(raw[i+2], 16) << 16 | int(raw[i+1], 16) << 8 | int(raw[i], 16)
                    label = word & 0xFF
                    sdi = (word >> 8) & 0x03
                    data = (word >> 10) & 0x7FFFF
                    ssm = (word >> 29) & 0x03
                    parity = (word >> 31) & 0x01
                    print(f"    ARINC word @{i}: label=0o{label:03o} SDI={sdi} data=0x{data:05X}({data}) SSM={ssm} P={parity}")
                except (ValueError, IndexError):
                    pass
        if len(seen_types) >= 10:
            break

    # Check for HSDB2N_GET too
    get_count = sum(1 for l in lines if 'HSDB2N_GET' in l)
    print(f"\n\nHSDB2N_GET entries: {get_count}")
    
    # Show STATS
    print(f"\n=== STATS ===")
    for line in lines:
        if 'STATS:' in line:
            print(f"  {line.strip()}")

    # Show SIM mode
    for line in lines:
        if 'SIM_get_mode' in line or 'SYS_pvg_get_mode' in line or 'lrus_online' in line:
            print(f"  {line.strip()}")
