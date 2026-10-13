import ctypes, struct

kernel32 = ctypes.windll.kernel32
hGDU = kernel32.OpenProcess(0x1F0FFF, False, 22540)

# We want to find references to the string 'attitude_ladder' or 'artificial_horizon' or 'attitude_indicator'
target_strings = [
    b'attitude_indicator\x00',
    b'attitude_ladder\x00',
    b'artificial_horizon\x00',
    b'attitude_roll_and_slip_skid\x00',
    b'eh_attitude\x00',
]

class MBI(ctypes.Structure):
    _fields_ = [
        ('BaseAddress', ctypes.c_void_p), ('AllocationBase', ctypes.c_void_p),
        ('AllocationProtect', ctypes.c_uint32), ('RegionSize', ctypes.c_size_t),
        ('State', ctypes.c_uint32), ('Protect', ctypes.c_uint32), ('Type', ctypes.c_uint32)
    ]

mbi = MBI()
addr = 0x10000
regions = []

while addr < 0x7FFE0000:
    if kernel32.VirtualQueryEx(hGDU, ctypes.c_void_p(addr), ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
        addr += 0x10000
        continue
    if mbi.State == 0x1000 and mbi.Protect == 0x04 and mbi.RegionSize < 50*1024*1024:
        buf = ctypes.create_string_buffer(mbi.RegionSize)
        br = ctypes.c_size_t(0)
        if kernel32.ReadProcessMemory(hGDU, ctypes.c_void_p(mbi.BaseAddress), buf, mbi.RegionSize, ctypes.byref(br)):
            data = buf.raw[:br.value]
            regions.append((mbi.BaseAddress, data))
    addr = mbi.BaseAddress + mbi.RegionSize

print(f"Read {len(regions)} committed regions.")

# 1. Find string addresses
str_addrs = {}
for base, data in regions:
    for s in target_strings:
        idx = 0
        while True:
            p = data.find(s, idx)
            if p == -1: break
            str_name = s.decode('ascii', errors='ignore').rstrip('\x00')
            if str_name not in str_addrs:
                str_addrs[str_name] = []
            str_addrs[str_name].append(base + p)
            idx = p + len(s)

print("Found string addresses:")
for k, v in str_addrs.items():
    print(f"  {k}: {[hex(x) for x in v]}")

# 2. Find pointers to these strings (which are widget instances!)
widget_instances = {}
for name, s_addrs in str_addrs.items():
    widget_instances[name] = []
    for s_addr in s_addrs:
        needle = struct.pack('<I', s_addr)
        for base, data in regions:
            idx = 0
            while True:
                p = data.find(needle, idx)
                if p == -1: break
                ptr_addr = base + p
                widget_instances[name].append(ptr_addr)
                idx = p + 4

print("\nFound widget object pointers to strings:")
for k, v in widget_instances.items():
    print(f"  {k}: {[hex(x) for x in v]}")

# For each widget instance, dump 128 bytes around it
for name, ptrs in widget_instances.items():
    for ptr in ptrs[:3]:
        buf = ctypes.create_string_buffer(128)
        kernel32.ReadProcessMemory(hGDU, ctypes.c_void_p(ptr - 16), buf, 128, None)
        print(f"\n--- Context of {name} at 0x{ptr:08X} ---")
        data = buf.raw
        for i in range(0, len(data), 4):
            f = struct.unpack_from('<f', data, i)[0]
            u = struct.unpack_from('<I', data, i)[0]
            print(f"  offset {i-16:+3d} (0x{ptr - 16 + i:08X}): {f:12.4f} | 0x{u:08X}")
