#!/usr/bin/env python3
"""
ARINC 429 UDP Test Script for Garmin TXi Trainer
=================================================
Tests if the Garmin GDU 1060 TXi trainer accepts ARINC 429 data
via the GSIM UDP protocol discovered in philhug/gsim.

Usage:
  1. Start the GDU trainer (let it boot to demo mode)
  2. Run this script: python test_arinc_udp.py
  3. Watch the PFD — if heading/pitch/altitude change, it works!

Protocol format (from gsim):
  Header (12 bytes LE):
    [0..3]  uint32: LRU Source ID
    [4..7]  uint32: Channel/Port
    [8..11] uint32: ARINC Word Count (N)
  Body:
    N x uint32: ARINC 429 words

ARINC 429 word (32 bits):
    Bit 0..7:   Label (8 bits, bit-reversed octal)
    Bit 8..9:   SDI (Source/Destination Identifier)
    Bit 10..28: Data (19 bits, BNR or BCD)
    Bit 29..30: SSM (Sign/Status Matrix: 0=Fail, 3=Normal)
    Bit 31:     Parity (odd)
"""

import socket
import struct
import time
import math
import sys

# ============================================================
# ARINC 429 Encoding
# ============================================================

def reverse_bits_8(b):
    """Reverse bit order of an 8-bit value (ARINC 429 label convention)."""
    b = ((b & 0xF0) >> 4) | ((b & 0x0F) << 4)
    b = ((b & 0xCC) >> 2) | ((b & 0x33) << 2)
    b = ((b & 0xAA) >> 1) | ((b & 0x55) << 1)
    return b & 0xFF

def odd_parity(word):
    """Calculate odd parity bit for a 31-bit value."""
    return 1 if (bin(word & 0x7FFFFFFF).count('1') % 2 == 0) else 0

def encode_arinc429(label_octal, sdi, data_19bit, ssm):
    """
    Encode a 32-bit ARINC 429 word.
    
    Args:
        label_octal: Label in octal (e.g., 0o324 for pitch)
        sdi: Source/Destination Identifier (0-3)
        data_19bit: 19-bit data payload (BNR encoded)
        ssm: Sign/Status Matrix (0=Fail/NCD, 1=No data, 2=Test, 3=Normal)
    
    Returns:
        32-bit unsigned integer
    """
    raw_label = reverse_bits_8(label_octal & 0xFF)
    word = (raw_label & 0xFF) | \
           ((sdi & 0x03) << 8) | \
           ((data_19bit & 0x7FFFF) << 10) | \
           ((ssm & 0x03) << 29)
    word |= (odd_parity(word) << 31)
    return word & 0xFFFFFFFF

# ============================================================
# BNR (Binary) Encoding for common ARINC 429 labels
# ============================================================

def angle_to_bnr(angle_deg, max_range, bits=19):
    """
    Encode an angle to BNR (Binary Number Representation).
    
    ARINC 429 BNR encoding uses 2's complement for signed values.
    Resolution = max_range / 2^(bits-1)
    
    Args:
        angle_deg: Angle in degrees
        max_range: Maximum range of the label (e.g., 180 for heading, 90 for pitch)
        bits: Number of data bits (default 19)
    """
    # Clamp to range
    angle_deg = max(-max_range, min(max_range - (max_range / (2**(bits-1))), angle_deg))
    
    # Scale to integer
    resolution = max_range / (2 ** (bits - 1))
    raw = int(angle_deg / resolution)
    
    # 2's complement for negative values
    if raw < 0:
        raw = (1 << bits) + raw
    
    return raw & ((1 << bits) - 1)

def altitude_to_bnr(alt_ft, bits=19):
    """
    Encode altitude in feet to BNR.
    Label 203: Range typically -1000 to +131071 ft
    Resolution: 1 ft per bit (depends on installation)
    Some use 0.5 ft resolution with 20-bit field.
    """
    # Simple encoding: 1 ft per LSB, signed
    raw = int(alt_ft)
    if raw < 0:
        raw = (1 << bits) + raw
    return raw & ((1 << bits) - 1)

def speed_to_bnr(speed_kts, max_range=512, bits=19):
    """
    Encode speed in knots to BNR.
    Label 206: IAS, range 0-512 kts typical
    """
    resolution = max_range / (2 ** (bits - 1))
    raw = int(speed_kts / resolution)
    if raw < 0:
        raw = (1 << bits) + raw
    return raw & ((1 << bits) - 1)

def vspeed_to_bnr(vs_fpm, max_range=32768, bits=19):
    """
    Encode vertical speed in ft/min to BNR.
    Label 212: VS, signed
    """
    resolution = max_range / (2 ** (bits - 1))
    raw = int(vs_fpm / resolution)
    if raw < 0:
        raw = (1 << bits) + raw
    return raw & ((1 << bits) - 1)

# ============================================================
# ARINC 429 Labels (Octal)
# ============================================================

LABEL_ALTITUDE      = 0o203   # Barometric Altitude
LABEL_IAS           = 0o206   # Indicated Airspeed
LABEL_MACH          = 0o207   # Mach Number
LABEL_BARO_ALT      = 0o210   # Baro-Corrected Altitude  
LABEL_VERT_SPEED    = 0o212   # Vertical Speed
LABEL_TRUE_HDG      = 0o314   # True Heading
LABEL_MAG_HDG       = 0o310   # Magnetic Heading
LABEL_PITCH         = 0o324   # Pitch Angle
LABEL_ROLL          = 0o325   # Roll Angle
LABEL_BODY_YAW_RATE = 0o331   # Body Yaw Rate
LABEL_BODY_PITCH_RT = 0o326   # Body Pitch Rate
LABEL_BODY_ROLL_RT  = 0o327   # Body Roll Rate
LABEL_ACCEL_NORMAL  = 0o332   # Normal Acceleration
LABEL_ACCEL_LATERAL = 0o333   # Lateral Acceleration
LABEL_ACCEL_LONG    = 0o334   # Longitudinal Acceleration

# ============================================================
# GSIM Packet Builder
# ============================================================

def build_gsim_packet(lru_id, channel, arinc_words):
    """
    Build a GSIM UDP packet.
    
    Args:
        lru_id: LRU Source ID (uint32)
        channel: Channel/port number (uint32)
        arinc_words: List of 32-bit ARINC 429 encoded words
    
    Returns:
        bytes
    """
    header = struct.pack('<III', lru_id, channel, len(arinc_words))
    body = b''.join(struct.pack('<I', w) for w in arinc_words)
    return header + body

# ============================================================
# Main Test
# ============================================================

def main():
    print("=" * 60)
    print("  Garmin TXi ARINC 429 UDP Test")
    print("  Protocol: GSIM (Garmin Simulator)")
    print("=" * 60)
    
    # Try multiple possible ports
    PORTS_TO_TRY = [4469, 4470, 4471, 4472, 4473, 4474, 4475, 4480, 4490, 4500]
    HOST = '127.0.0.1'
    
    # Virtual LRU IDs to try
    LRU_IDS = [1, 2, 3, 99]
    
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    
    # Test values — distinctive enough to notice on instruments
    test_heading = 0.0      # Due North (demo mode shows ~173)
    test_pitch = 15.0       # 15 degrees nose up  
    test_roll = -20.0       # 20 degrees left bank
    test_altitude = 10000   # 10,000 ft (demo shows ~1400)
    test_ias = 200          # 200 kts
    test_vs = 500           # 500 ft/min climb
    
    print(f"\nTest values:")
    print(f"  Heading:  {test_heading}° (demo ~173°)")
    print(f"  Pitch:    {test_pitch}° nose up")
    print(f"  Roll:     {test_roll}° left bank")
    print(f"  Altitude: {test_altitude} ft (demo ~1400)")
    print(f"  IAS:      {test_ias} kts")
    print(f"  VS:       {test_vs} ft/min")
    
    # Build ARINC 429 words
    words = [
        encode_arinc429(LABEL_MAG_HDG,    0, angle_to_bnr(test_heading, 180), 3),
        encode_arinc429(LABEL_PITCH,      0, angle_to_bnr(test_pitch, 90), 3),
        encode_arinc429(LABEL_ROLL,       0, angle_to_bnr(test_roll, 180), 3),
        encode_arinc429(LABEL_ALTITUDE,   0, altitude_to_bnr(test_altitude), 3),
        encode_arinc429(LABEL_IAS,        0, speed_to_bnr(test_ias), 3),
        encode_arinc429(LABEL_VERT_SPEED, 0, vspeed_to_bnr(test_vs), 3),
    ]
    
    print(f"\nEncoded {len(words)} ARINC 429 words:")
    for i, w in enumerate(words):
        label = w & 0xFF
        data = (w >> 10) & 0x7FFFF
        ssm = (w >> 29) & 0x03
        print(f"  Word {i}: 0x{w:08X} (label=0x{label:02X} data={data} ssm={ssm})")
    
    # Phase 1: Port scan — send one packet to each port and see if any respond
    print(f"\n--- Phase 1: Scanning ports ---")
    for port in PORTS_TO_TRY:
        for lru_id in [1, 99]:
            pkt = build_gsim_packet(lru_id, 0, words)
            try:
                sock.sendto(pkt, (HOST, port))
            except Exception as e:
                print(f"  Port {port} LRU {lru_id}: ERROR {e}")
    print(f"  Sent test packets to ports: {PORTS_TO_TRY}")
    
    # Phase 2: Continuous stream at 20Hz to all candidate ports
    print(f"\n--- Phase 2: Streaming at 20Hz for 30 seconds ---")
    print(f"  Watch the Garmin instruments!")
    print(f"  Press Ctrl+C to stop")
    
    start = time.time()
    count = 0
    
    try:
        while time.time() - start < 30:
            # Slowly vary the heading to make it obvious if it's working
            elapsed = time.time() - start
            current_heading = (elapsed * 36) % 360  # 36°/s = full circle in 10s
            current_pitch = 10 * math.sin(elapsed * 0.5)  # oscillate pitch
            current_roll = 15 * math.sin(elapsed * 0.3)   # oscillate roll
            current_alt = 5000 + 2000 * math.sin(elapsed * 0.2)  # oscillate alt
            
            words = [
                encode_arinc429(LABEL_MAG_HDG,    0, angle_to_bnr(current_heading, 180), 3),
                encode_arinc429(LABEL_TRUE_HDG,   0, angle_to_bnr(current_heading, 180), 3),
                encode_arinc429(LABEL_PITCH,      0, angle_to_bnr(current_pitch, 90), 3),
                encode_arinc429(LABEL_ROLL,       0, angle_to_bnr(current_roll, 180), 3),
                encode_arinc429(LABEL_ALTITUDE,   0, altitude_to_bnr(current_alt), 3),
                encode_arinc429(LABEL_BARO_ALT,   0, altitude_to_bnr(current_alt), 3),
                encode_arinc429(LABEL_IAS,        0, speed_to_bnr(test_ias), 3),
                encode_arinc429(LABEL_VERT_SPEED, 0, vspeed_to_bnr(test_vs), 3),
            ]
            
            for port in PORTS_TO_TRY:
                for lru_id in [1, 2, 99]:
                    for channel in [0, 1]:
                        pkt = build_gsim_packet(lru_id, channel, words)
                        try:
                            sock.sendto(pkt, (HOST, port))
                            count += 1
                        except:
                            pass
            
            time.sleep(0.05)  # 20Hz
            
            if count % 200 == 0:
                print(f"  [{elapsed:.0f}s] Sent {count} packets, HDG={current_heading:.0f}° Pitch={current_pitch:.1f}° Alt={current_alt:.0f}ft")
    
    except KeyboardInterrupt:
        print("\n  Stopped by user")
    
    print(f"\n  Total packets sent: {count}")
    print(f"  Duration: {time.time() - start:.1f}s")
    
    sock.close()
    print("\nDone. Did any instruments change?")
    print("If not, the TXi might use a different port or need ROAR registration first.")

if __name__ == '__main__':
    main()
