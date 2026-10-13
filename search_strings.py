import re

dll_path = r'C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_86\app-3.86.4\bin\cdp_pfd_b_sim.dll'
with open(dll_path, 'rb') as f:
    data = f.read()

keywords = [b'pitch', b'roll', b'attitude', b'horizon', b'heading', b'compass', b'adi_']
matches = {}
for kw in keywords:
    matches[kw.decode()] = []
    for m in re.finditer(re.escape(kw), data, re.IGNORECASE):
        # Extract surrounding string
        start = max(0, m.start() - 20)
        end = min(len(data), m.end() + 40)
        chunk = data[start:end]
        # find null-terminated string boundaries
        s0 = data.rfind(b'\x00', 0, m.start()) + 1
        s1 = data.find(b'\x00', m.end())
        if s1 != -1 and s1 - s0 < 100:
            str_val = data[s0:s1].decode('ascii', errors='ignore')
            if str_val not in matches[kw.decode()]:
                matches[kw.decode()].append(str_val)

for kw, strs in matches.items():
    print(f"=== Keyword: {kw} ({len(strs)} matches) ===")
    for s in strs[:15]:
        print(f"  '{s}'")
