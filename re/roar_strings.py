import pefile, re, sys

base = r"C:\Program Files (x86)\Garmin\Trainers\Launcher\roar.19.5.1.0"
for name in ("roar_pub.dll", "roar.exe"):
    p = base + "\\" + name
    pe = pefile.PE(p)
    print(f"===== {name} machine=0x{pe.FILE_HEADER.Machine:X}")
    if hasattr(pe, "DIRECTORY_ENTRY_EXPORT"):
        for e in pe.DIRECTORY_ENTRY_EXPORT.symbols:
            print("  EXPORT", e.ordinal, e.name.decode() if e.name else None)
    for entry in getattr(pe, "DIRECTORY_ENTRY_IMPORT", []):
        names = [(i.name.decode() if i.name else f"ord{i.ordinal}") for i in entry.imports]
        print(f"  IMPORT {entry.dll.decode()}: {', '.join(names[:40])}{' ...' if len(names) > 40 else ''}")
    data = open(p, "rb").read()
    strs = set()
    for m in re.finditer(rb"[\x20-\x7e]{6,}", data):
        s = m.group().decode()
        if re.search(r"(?i)roar|endpoint|port|udp|tcp|hsdb|a429|arinc|json|config|route|lru|sim|master|client|register|connect|xplane|external|plugin|pipe|--", s):
            strs.add(s)
    # wide strings too
    for m in re.finditer(rb"(?:[\x20-\x7e]\x00){6,}", data):
        s = m.group().decode("utf-16le")
        if re.search(r"(?i)roar|endpoint|port|udp|tcp|hsdb|json|config|route|lru|master|client|--", s):
            strs.add("W:" + s)
    print(f"  --- interesting strings ({len(strs)}) ---")
    for s in sorted(strs):
        print("   ", s[:160])
