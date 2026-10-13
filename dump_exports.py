"""Dump exported function names from a PE DLL using pefile or manual PE parsing."""
import struct, sys, os

def read_exports(dll_path):
    """Read export table from PE file manually (no pefile dependency)."""
    with open(dll_path, 'rb') as f:
        data = f.read()
    
    # DOS header
    if data[:2] != b'MZ':
        print("Not a valid PE file")
        return []
    
    pe_offset = struct.unpack_from('<I', data, 0x3C)[0]
    
    # PE signature
    if data[pe_offset:pe_offset+4] != b'PE\x00\x00':
        print("Invalid PE signature")
        return []
    
    # COFF header
    coff = pe_offset + 4
    num_sections = struct.unpack_from('<H', data, coff + 2)[0]
    opt_header_size = struct.unpack_from('<H', data, coff + 16)[0]
    
    # Optional header
    opt = coff + 20
    magic = struct.unpack_from('<H', data, opt)[0]
    
    if magic == 0x10b:  # PE32
        num_rva_and_sizes = struct.unpack_from('<I', data, opt + 92)[0]
        export_rva = struct.unpack_from('<I', data, opt + 96)[0]
        export_size = struct.unpack_from('<I', data, opt + 100)[0]
    elif magic == 0x20b:  # PE32+
        num_rva_and_sizes = struct.unpack_from('<I', data, opt + 108)[0]
        export_rva = struct.unpack_from('<I', data, opt + 112)[0]
        export_size = struct.unpack_from('<I', data, opt + 116)[0]
    else:
        print(f"Unknown PE magic: 0x{magic:04X}")
        return []
    
    if export_rva == 0:
        print("No export directory")
        return []
    
    # Section headers
    sections = []
    sec_offset = opt + opt_header_size
    for i in range(num_sections):
        s = sec_offset + i * 40
        name = data[s:s+8].rstrip(b'\x00').decode('ascii', errors='replace')
        vsize = struct.unpack_from('<I', data, s + 8)[0]
        va = struct.unpack_from('<I', data, s + 12)[0]
        raw_size = struct.unpack_from('<I', data, s + 16)[0]
        raw_ptr = struct.unpack_from('<I', data, s + 20)[0]
        sections.append((name, va, vsize, raw_ptr, raw_size))
    
    def rva_to_file(rva):
        for name, va, vsize, raw_ptr, raw_size in sections:
            if va <= rva < va + max(vsize, raw_size):
                return raw_ptr + (rva - va)
        return None
    
    # Export directory
    exp_file = rva_to_file(export_rva)
    if exp_file is None:
        print("Cannot resolve export directory RVA")
        return []
    
    num_functions = struct.unpack_from('<I', data, exp_file + 20)[0]
    num_names = struct.unpack_from('<I', data, exp_file + 24)[0]
    addr_functions = struct.unpack_from('<I', data, exp_file + 28)[0]
    addr_names = struct.unpack_from('<I', data, exp_file + 32)[0]
    addr_ordinals = struct.unpack_from('<I', data, exp_file + 36)[0]
    
    names_file = rva_to_file(addr_names)
    
    exports = []
    for i in range(num_names):
        name_rva = struct.unpack_from('<I', data, names_file + i * 4)[0]
        name_file = rva_to_file(name_rva)
        if name_file:
            end = data.index(b'\x00', name_file)
            name = data[name_file:end].decode('ascii', errors='replace')
            exports.append(name)
    
    return exports

# Scan all krnl.dll versions
dlls = [
    r'C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_50\app-3.50.20\bin\krnl.dll',
    r'C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_30\app-3.30.7\bin\krnl.dll',
]

for dll_path in dlls:
    if not os.path.exists(dll_path):
        continue
    print(f"\n{'='*80}")
    print(f"EXPORTS: {dll_path}")
    print(f"{'='*80}")
    exports = read_exports(dll_path)
    
    # Print ALL exports, highlighting interesting ones
    interesting = ['ahrs', 'adc', 'sim', 'sensor', 'att', 'head', 'air', 'pitch', 
                   'roll', 'speed', 'alt', 'nav', 'gps', 'imu', 'gyro', 'mag',
                   'pvg', 'tsk', 'reg', 'set', 'update', 'write', 'inject',
                   'flight', 'model', 'data', 'state', 'param', 'var']
    
    print(f"\nTotal exports: {len(exports)}")
    print(f"\n--- ALL EXPORTS ---")
    for name in sorted(exports):
        marker = ""
        name_lower = name.lower()
        for kw in interesting:
            if kw in name_lower:
                marker = f"  <== INTERESTING ({kw})"
                break
        print(f"  {name}{marker}")
