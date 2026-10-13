import struct, math

# Real hex at 0x30-0x3F from the dump:
# CB 9A D8 BF 41 04 D5 43 01 55 EA BF 04 15 EA 07
raw = bytes([0xCB, 0x9A, 0xD8, 0xBF, 0x41, 0x04, 0xD5, 0x43, 0x01, 0x55, 0xEA, 0xBF, 0x04, 0x15, 0xEA, 0x07])

print('=== OFFSET 0x30-0x37 (potential latitude) ===')
f0 = struct.unpack_from('<f', raw, 0)[0]
f1 = struct.unpack_from('<f', raw, 4)[0]
d_normal = struct.unpack_from('<d', raw, 0)[0]
print(f'  Float at 0x30: {f0}')
print(f'  Float at 0x34: {f1}')
print(f'  Double at 0x30 (LE): {d_normal} (garbage)')

# Word-swapped: swap the two 4-byte halves
swapped = raw[4:8] + raw[0:4]
d_swap = struct.unpack_from('<d', swapped, 0)[0]
print(f'  Double WORD-SWAPPED: {d_swap}')
print(f'  WORD-SWAPPED in degrees: {math.degrees(d_swap):.6f} deg')
print(f'  Expected latitude: ~-22 deg (Brazil)')

print()
print('=== OFFSET 0x38-0x3F (potential longitude) ===')
f2 = struct.unpack_from('<f', raw, 8)[0]
f3 = struct.unpack_from('<f', raw, 12)[0]
d_normal2 = struct.unpack_from('<d', raw, 8)[0]
print(f'  Float at 0x38: {f2}')
print(f'  Float at 0x3C: {f3}')
print(f'  Double at 0x38 (LE): {d_normal2} (garbage)')

# Word-swapped
swapped2 = raw[12:16] + raw[8:12]
d_swap2 = struct.unpack_from('<d', swapped2, 0)[0]
print(f'  Double WORD-SWAPPED: {d_swap2}')
print(f'  WORD-SWAPPED in degrees: {math.degrees(d_swap2):.6f} deg')
print(f'  Expected longitude: ~-43 deg (Brazil)')

print()
print('=== CHECKING: Is the double stored as Big-Endian? ===')
d_be = struct.unpack_from('>d', raw, 0)[0]
print(f'  Big-Endian double at 0x30: {d_be} -> {math.degrees(d_be):.6f} deg')
d_be2 = struct.unpack_from('>d', raw, 8)[0]
print(f'  Big-Endian double at 0x38: {d_be2} -> {math.degrees(d_be2):.6f} deg')

print()
print('=== FINAL CHECK: Could these be Garmin semicircles? ===')
# Garmin semicircle = degrees * (2^31 / 180)
# Read as int32
i0 = struct.unpack_from('<i', raw, 0)[0]
i1 = struct.unpack_from('<i', raw, 4)[0]
print(f'  Int32 at 0x30: {i0} -> {i0 * 180.0 / (2**31):.6f} deg')
print(f'  Int32 at 0x34: {i1} -> {i1 * 180.0 / (2**31):.6f} deg')
i2 = struct.unpack_from('<i', raw, 8)[0]
i3 = struct.unpack_from('<i', raw, 12)[0]
print(f'  Int32 at 0x38: {i2} -> {i2 * 180.0 / (2**31):.6f} deg')
print(f'  Int32 at 0x3C: {i3} -> {i3 * 180.0 / (2**31):.6f} deg')

print()
print('=== NOW: What about the 0x20 offset? ===')
print('0x20 at end of session: ~0.6634 rad = 38.0 deg')
print('Expected heading: 060 deg = 1.047 rad')
print('This is NOT heading.')
print()
print('What IS 0.6634?')
print(f'  0.6634 rad = {math.degrees(0.6634):.1f} deg  -- NOT heading')
print(f'  Maybe its pitch (in some scaled unit)?')
print(f'  Or maybe roll?')
print()
print('=== Early values of 0x20 ===')
print('  At 21:58:41: 5.1844 rad = 297.0 deg  (user said HDG 320)')
print('  At 22:00:23: -1.0811 rad = -61.9 deg')
print('  At 22:03:34: -0.4753 rad = -27.2 deg  (user said 27 deg bank!)')
print('  At 22:03:52:  0.4776 rad =  27.4 deg  (opposite bank!)') 
print('  At 22:04:53:  0.6645 rad =  38.1 deg')
print()
print('*** CONCLUSION for 0x20: This looks like ROLL (bank angle)! ***')
print('  -0.4753 matches the 27 deg bank during the turn to 060.')
print('  0.4776 matches the bank in the opposite direction.')
print('  But 0.6634 at the end is 38 deg -- perhaps wind correction or drift?')
print()

# Let me check if the heading might be encoded differently
# User said HDG went from 320 to 060
# 0x24 went from ~5.5657 to -0.0002
# 5.5657 rad = 318.9 deg -- VERY close to 320 deg!
# -0.0002 rad = -0.01 deg -- close to 0, not 060
print('=== CHECKING 0x24 as heading ===')
print(f'  0x24 early: 5.5657 rad = {math.degrees(5.5657):.1f} deg  (user said HDG 320!) YES!')
print(f'  0x24 late: -0.0002 rad = {math.degrees(-0.0002):.2f} deg  (user said HDG 060) NO, 0 not 60')
print()
print('Hmm, 0x24 starts at ~320 deg but ends at ~0 deg, not 060.')
print('But the user said HDG 060... maybe the Garmin heading wraps?')
print('Or maybe the heading is elsewhere.')
print()

# Check the user's timeline again:
# Phase 1: HDG 320, climbing, pitch 6.25°
# At 21:58: 0x24 = 0.0000 (was heading not yet set?)
# At 22:00: 0x24 = 5.5657 = 319° (HDG 320 confirmed!)
# Then 0x24 drops during turn...
# At ~22:04 after turn to 060: 0x24 = 0.0
# But 060° = 1.047 rad... the value is 0, not 1.047.
# 
# Unless the AHRS message only contains SOME data and the rest is in the 
# second or third group (the 0x2c message sometimes has multiple sections?)

# Let me check if the AHRS magic position matters
print('=== KEY QUESTION: Does the AHRS magic scan offset affect things? ===')
print('The code scans for AHRS_MAGIC at offsets 0..64 in 4-byte steps.')
print('AHRS_MAGIC = 0x003f900c')
print('If magic is at offset 0x08, then offset 0x18 from addr_msg = magic+0x10')
print('If magic is at offset 0x0C, then offset 0x18 from addr_msg = magic+0x0C')
print()
print('The previous code injected relative to the magic position (base = addr_msg + scan)')
print('But the current code injects at ABSOLUTE positions from addr_msg.')
print('This means our injection is ONLY correct if magic is at offset 0x00!')
print()
print('*** THIS IS THE BUG! ***')
print('If magic is at offset 0x08 (GDU), then we need to add 0x08 to all offsets.')
print('Or we should inject relative to the magic position, not absolute.')
