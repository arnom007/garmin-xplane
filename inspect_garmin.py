import re
import os

files_to_check = [
    r"C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_62\app-3.62.2\bin\gdu_7xx_simulator.exe",
    r"C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_62\app-3.62.2\bin\krnl_sim.dll",
    r"C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__GTN_Xi__20_42\app-20.42.2\bin\gtn_simulator.exe",
    r"C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__GTN_Xi__20_42\app-20.42.2\bin\KRNL.dll",
]

keywords = [
    b"sim_session", b"configpath", b"session_number", b"environment_settings",
    b"airframe", b"FlightSimulator", b"Demo", b"demo", b"simulation_mode",
    b"standalone", b"joystick", b"external", b"ext_sim", b"BaseSim",
    b"Trainer Navigation", b"TrainerNavigation", b"FDM"
]

for fp in files_to_check:
    print("=" * 60)
    print("FILE:", os.path.basename(fp))
    with open(fp, "rb") as f:
        data = f.read()
    
    # Check keyword matches with surrounding context
    for kw in keywords:
        matches = [m.start() for m in re.finditer(re.escape(kw), data, re.IGNORECASE)]
        if matches:
            print(f"  Keyword '{kw.decode()}': {len(matches)} matches")
            for pos in matches[:3]:
                snippet = data[max(0, pos-20) : min(len(data), pos+60)]
                # extract printable ascii
                clean = "".join(chr(b) if 32 <= b <= 126 else "." for b in snippet)
                print(f"    [0x{pos:08x}]: {clean}")
