import os
import json
import subprocess
import shutil

SESSION_DIR = os.path.join(os.environ['LOCALAPPDATA'], "Temp", "Garmin", "Trainers", "Launcher", "Sessions", "GarminBridge_Standalone")
JSON_PATH = os.path.join(SESSION_DIR, "sim_session.json")

def create_session():
    if os.path.exists(SESSION_DIR):
        shutil.rmtree(SESSION_DIR, ignore_errors=True)
    os.makedirs(SESSION_DIR, exist_ok=True)
    
    # We create the required directories for Crash Reports as Garmin expects them
    os.makedirs(os.path.join(SESSION_DIR, "GTN_Xi_1"), exist_ok=True)
    os.makedirs(os.path.join(SESSION_DIR, "TXi_1"), exist_ok=True)

    session_data = {
      "session_number": 999,
      "configuration_name": "GarminBridge_NoDemo",
      "environment_settings": {
        "airframe": "SINGLETURBO",
        "auto_poweron": True
      },
      "trainers": {
        "GTN_Xi_1": {
          "id": "GTN_Xi_1",
          "slot": {
            "category": "NAVIGATOR",
            "instance_id": 1
          },
          "family_name": "GTN_Xi",
          "settings": {
            "instance_id": 1,
            "model": "650",
            "CrashReportDirectoryPath": os.path.join(SESSION_DIR, "GTN_Xi_1")
          }
        },
        "TXi_1": {
          "id": "TXi_1",
          "slot": {
            "category": "DISPLAY",
            "instance_id": 1
          },
          "family_name": "TXi",
          "settings": {
            "instance_id": 1,
            "model": "1060",
            "CrashReportDirectoryPath": os.path.join(SESSION_DIR, "TXi_1"),
            "SingleTurboAirframeConfig": "SOCATA (Daher),TBM 700N (TBM 850),Pratt & Whitney,PT6A,-66D",
            "EisAlignment": "-EIS_RIGHT",
            "PfdAlignment": "-PFD_LEFT",
            "Layout": "-PFD ON -MFD ON -EIS ON"
          }
        }
      },
      "install_slots": {
        "NAVIGATOR": { "1": "GTN_Xi_1" },
        "DISPLAY": { "1": "TXi_1" }
      }
    }
    
    with open(JSON_PATH, "w") as f:
        json.dump(session_data, f, indent=2)
        
    return JSON_PATH

def launch():
    json_path = create_session()
    print(f"Sessǜo Customizada Criada: {json_path}")
    
    # Caminhos mais recentes dos executáveis (vistos no sistema)
    gtn_path = r"C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__GTN_Xi__20_42\app-20.42.2\bin\gtn_simulator.exe"
    gdu_path = r"C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_62\app-3.62.2\bin\gdu_7xx_simulator.exe"
    
    print("Iniciando GTN Xi (Standalone)...")
    subprocess.Popen([gtn_path, "--configpath", json_path], cwd=os.path.dirname(gtn_path), close_fds=True)
    
    print("Iniciando GDU 1060 TXi (Standalone)...")
    subprocess.Popen([gdu_path, "--configpath", json_path], cwd=os.path.dirname(gdu_path), close_fds=True)
    
    print("Executáveis iniciados sem o Tracker / Trainer Navigation Control!")

if __name__ == "__main__":
    launch()
