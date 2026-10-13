import pefile

dll_path = r'C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_86\app-3.86.4\bin\cdp_pfd_b_sim.dll'
pe = pefile.PE(dll_path)
print("Sections:", [s.Name.decode(errors='ignore').strip() for s in pe.sections])

if hasattr(pe, 'DIRECTORY_ENTRY_EXPORT'):
    print(f"Total exports: {len(pe.DIRECTORY_ENTRY_EXPORT.symbols)}")
    for exp in pe.DIRECTORY_ENTRY_EXPORT.symbols[:60]:
        name = exp.name.decode('utf-8', errors='ignore') if exp.name else f"ord_{exp.ordinal}"
        print(f"  {name} at RVA 0x{exp.address:X}")
else:
    print("No export directory")
