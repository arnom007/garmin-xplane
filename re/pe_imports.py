import pefile, sys

paths = {
    "gdu_krnl_sim_3.86": r"C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_86\app-3.86.4\bin\krnl_sim.dll",
    "gtn_krnl_sim_21.3": r"C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__GTN_Xi__21_03\app-21.3.7\bin\krnl_sim.dll",
}
for tag, p in paths.items():
    pe = pefile.PE(p, fast_load=False)
    print(f"==== {tag}  machine=0x{pe.FILE_HEADER.Machine:X} imagebase=0x{pe.OPTIONAL_HEADER.ImageBase:X}")
    for entry in getattr(pe, "DIRECTORY_ENTRY_IMPORT", []):
        dll = entry.dll.decode()
        if dll.lower().startswith(("ws2_32", "wsock32", "mswsock")):
            names = [(imp.name.decode() if imp.name else f"ord{imp.ordinal}") for imp in entry.imports]
            print(f"  {dll}: {', '.join(names)}")
    exps = [e.name.decode() for e in pe.DIRECTORY_ENTRY_EXPORT.symbols if e.name] if hasattr(pe, "DIRECTORY_ENTRY_EXPORT") else []
    print(f"  exports: {len(exps)}")
    for kw in ("roar", "ROAR", "hsdb", "a429", "REG_pvg_lookup", "reg_read", "reg_write"):
        hits = [e for e in exps if kw.lower() in e.lower()]
        print(f"   [{kw}] {len(hits)}: {', '.join(hits[:25])}")
