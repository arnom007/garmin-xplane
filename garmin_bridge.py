"""
garmin_bridge.py — Central Middleware for Garmin Trainer ↔ X-Plane 11 Integration

This is the main entry point. It:
1. Creates shared memory regions for DLL communication
2. Reads flight data from X-Plane via XPlaneConnect
3. Writes data to shared memory for injected GarminHook DLLs
4. Reads Garmin output (radio freqs, etc.) and sends back to X-Plane
5. Manages DLL injection into Garmin Trainer processes
6. Provides a GUI for monitoring and control

"""

import os
import sys
import time
import json
import threading
import subprocess
import ctypes
import struct
import socket
import traceback
from pathlib import Path

from shared_memory import GarminBridgeSharedMemory
from xplane_datarefs import XPlaneDataReader
from mem_injector import GDUMemoryInjector


# ============================================================
# Configuration
# ============================================================

CONFIG_FILE = os.path.join(os.path.dirname(__file__), 'garmin_bridge_config.json')

# Auto-detect build paths
_SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
_BUILD_DIR = os.path.join(_SCRIPT_DIR, 'GarminHook_v2', 'build')

DEFAULT_CONFIG = {
    'xplane_host': '127.0.0.1',
    'xplane_port': 49002,   # Changed to match Lua script (49002)
    'update_rate_hz': 120,
    'garmin_trainer_path': r'C:\Program Files (x86)\Garmin\Trainers\Launcher\Aviation_Trainer_Launcher.exe',
    'dll_injector_path': os.path.join(_BUILD_DIR, 'DllInjector.exe'),
    'garmin_hook_dll_path': os.path.join(_BUILD_DIR, 'GarminHook.dll'),
    'txi_process_name': 'gdu_7xx_simulator.exe',
    'gtn_process_name': 'gtn_simulator.exe',
    'txi_kernel_dll': 'krnl.dll',
    'gtn_kernel_dll': 'KRNL.dll',
    'auto_inject': True,
    'auto_launch_trainer': False,
    'log_to_file': True,
    'log_dir': os.path.join(os.path.expandvars('%APPDATA%'), 'GarminBridge'),
}


def load_config():
    """Load configuration from file, using defaults for missing values."""
    config = dict(DEFAULT_CONFIG)
    if os.path.exists(CONFIG_FILE):
        try:
            with open(CONFIG_FILE, 'r') as f:
                saved = json.load(f)
                config.update(saved)
        except Exception as e:
            print(f"[Config] Erro ao carregar config: {e}")
    return config


def save_config(config):
    """Save configuration to file."""
    try:
        with open(CONFIG_FILE, 'w') as f:
            json.dump(config, f, indent=2)
    except Exception as e:
        print(f"[Config] Erro ao salvar config: {e}")


# ============================================================
# Process Management
# ============================================================

def find_process(process_name):
    """Find a running process by name. Returns PID or None."""
    try:
        import ctypes
        from ctypes import wintypes

        PROCESS_QUERY_INFORMATION = 0x0400
        TH32CS_SNAPPROCESS = 0x2

        class PROCESSENTRY32(ctypes.Structure):
            _fields_ = [
                ('dwSize', wintypes.DWORD),
                ('cntUsage', wintypes.DWORD),
                ('th32ProcessID', wintypes.DWORD),
                ('th32DefaultHeapID', ctypes.POINTER(ctypes.c_ulong)),
                ('th32ModuleID', wintypes.DWORD),
                ('cntThreads', wintypes.DWORD),
                ('th32ParentProcessID', wintypes.DWORD),
                ('pcPriClassBase', ctypes.c_long),
                ('dwFlags', wintypes.DWORD),
                ('szExeFile', ctypes.c_char * 260),
            ]

        kernel32 = ctypes.WinDLL('kernel32')
        hSnap = kernel32.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0)

        pe = PROCESSENTRY32()
        pe.dwSize = ctypes.sizeof(PROCESSENTRY32)

        pids = []
        if kernel32.Process32First(hSnap, ctypes.byref(pe)):
            while True:
                name = pe.szExeFile.decode('utf-8', errors='ignore')
                if name.lower() == process_name.lower():
                    pids.append(pe.th32ProcessID)
                if not kernel32.Process32Next(hSnap, ctypes.byref(pe)):
                    break

        kernel32.CloseHandle(hSnap)
        return pids

    except Exception as e:
        print(f"[Process] Erro ao buscar processo: {e}")
        return []


def inject_dll(process_name, dll_path, injector_path):
    """Inject a DLL into a process using the v2 DllInjector."""
    if not os.path.exists(injector_path):
        print(f"[Inject] DllInjector nao encontrado: {injector_path}")
        return False
    if not os.path.exists(dll_path):
        print(f"[Inject] DLL nao encontrada: {dll_path}")
        return False

    try:
        injector_dir = os.path.dirname(injector_path)

        # Ensure GarminHook.dll is in the same folder as DllInjector.exe
        dll_dest = os.path.join(injector_dir, 'GarminHook.dll')
        if os.path.abspath(dll_path) != os.path.abspath(dll_dest):
            import shutil
            shutil.copy2(dll_path, dll_dest)

        # Also ensure EasyHook32.dll is present
        easyhook_dest = os.path.join(injector_dir, 'EasyHook32.dll')
        if not os.path.exists(easyhook_dest):
            # Try to find it from old project
            old_eh = r'C:\Users\Arnom\Desktop\X-Plane 11.5 CSAV\Aircraft\CSAV\emb312M\garminDll\DllInjector\Release\EasyHook32.dll'
            if os.path.exists(old_eh):
                import shutil
                shutil.copy2(old_eh, easyhook_dest)

        import ctypes
        try:
            is_admin = ctypes.windll.shell32.IsUserAnAdmin()
        except Exception:
            is_admin = False

        if not is_admin:
            print("[Inject] AVISO: Nao executando como Administrador. Solicitando elevacao UAC...")
            # lpOperation = "runas", lpFile = injector_path, lpParameters = process_name, lpDirectory = injector_dir, nShowCmd = 0 (SW_HIDE)
            # Use ShellExecuteW to launch elevated DllInjector.exe
            ret = ctypes.windll.shell32.ShellExecuteW(
                None,
                "runas",
                injector_path,
                process_name,
                injector_dir,
                0 # SW_HIDE
            )
            if ret > 32:
                print("[Inject] Processo de injecao elevado iniciado! Aceite a confirmacao UAC na barra de tarefas.")
                # Since we elevated and cannot capture stdout, wait a bit for it to run
                time.sleep(2.5)
                return True
            else:
                print(f"[Inject] Falha ao solicitar elevacao UAC (codigo {ret}).")
                return False

        # Run DllInjector with the target process name as argument
        # DllInjector does not call system("pause") anymore when successful
        proc = subprocess.Popen(
            [injector_path, process_name],
            cwd=injector_dir,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            stdin=subprocess.PIPE,
            text=True,
            creationflags=subprocess.CREATE_NO_WINDOW
        )

        # Wait up to 15 seconds for injection to complete
        try:
            stdout, stderr = proc.communicate(timeout=15)
        except subprocess.TimeoutExpired:
            proc.kill()
            stdout, stderr = proc.communicate()

        output = (stdout or '') + (stderr or '')
        print(f"[Inject] {output.strip()}")

        # Check if injection succeeded
        return 'SUCCESS' in output or 'injected successfully' in output.lower()

    except Exception as e:
        print(f"[Inject] Erro na injecao: {e}")
        import traceback
        traceback.print_exc()
        return False


# ============================================================
# Bridge Core
# ============================================================

class GarminBridge:
    """
    Main bridge controller that orchestrates all components.
    """

    def __init__(self, config=None):
        self.config = config or load_config()
        self.running = False
        self._thread = None

        # Components
        self.shm = None
        self.xp_reader = None
        self.mem_injector = GDUMemoryInjector(log_func=lambda msg: self._log(msg))

        # Status tracking
        self.status = {
            'xplane_connected': False,
            'shm_active': False,
            'txi_injected': False,
            'gtn_injected': False,
            'txi_running': False,
            'gtn_running': False,
            'mem_inject_active': False,
            'mem_inject_addrs': 0,
            'update_count': 0,
            'updates_per_sec': 0.0,
            'last_error': '',
            'last_data': {},
        }

        # Callbacks for GUI updates
        self._status_callbacks = []
        self._log_callbacks = []

    def on_status_update(self, callback):
        """Register a callback for status updates."""
        self._status_callbacks.append(callback)

    def on_log(self, callback):
        """Register a callback for log messages."""
        self._log_callbacks.append(callback)

    def _log(self, msg):
        """Log a message."""
        timestamp = time.strftime('%H:%M:%S')
        full_msg = f"[{timestamp}] {msg}"
        print(full_msg)
        for cb in self._log_callbacks:
            try:
                cb(full_msg)
            except:
                pass

    def _update_status(self):
        """Notify status callbacks."""
        for cb in self._status_callbacks:
            try:
                cb(dict(self.status))
            except:
                pass

    def start(self):
        """Start the bridge."""
        if self.running:
            return

        self._log("Iniciando GarminBridge...")

        # Create shared memory
        try:
            self.shm = GarminBridgeSharedMemory()
            self.status['shm_active'] = True
            self._log("Shared memory criada com sucesso")
        except Exception as e:
            self._log(f"ERRO ao criar shared memory: {e}")
            self.status['last_error'] = str(e)
            return

        # Create UDP Bridge to listen to Lua script
        self.xp_reader = UDPBridge(listen_port=self.config['xplane_port'], send_port=self.config['xplane_port'] + 1)
        self.xp_reader.start()

        # Start main loop thread
        self.running = True
        self._thread = threading.Thread(target=self._main_loop, daemon=True)
        self._thread.start()

        self._log("GarminBridge iniciado!")

    def stop(self):
        """Stop the bridge."""
        self._log("Parando GarminBridge...")
        self.running = False

        # Kill any directly launched simulators or coordinator processes
        self.kill_direct_processes()

        if self._thread:
            self._thread.join(timeout=3.0)
            self._thread = None

        if self.xp_reader:
            self.xp_reader.disconnect()
            self.xp_reader = None

        if self.shm:
            self.shm.close()
            self.shm = None

        if hasattr(self, 'mem_injector') and self.mem_injector:
            self.mem_injector.disconnect()
            self.status['mem_inject_active'] = False
            self.status['mem_inject_addrs'] = 0

        self.status['xplane_connected'] = False
        self.status['shm_active'] = False
        self._update_status()
        self._log("GarminBridge parado.")

    def _main_loop(self):
        """Main bridge loop — runs in a separate thread."""
        interval = 1.0 / self.config['update_rate_hz']
        update_count = 0
        last_stats_time = time.time()
        last_process_check = 0

        while self.running:
            loop_start = time.time()

            try:
                # Check Garmin processes periodically (every 2 seconds)
                if time.time() - last_process_check > 2.0:
                    self._check_garmin_processes()
                    self._check_mem_injector()
                    last_process_check = time.time()

                # Read X-Plane data from Lua (UDP)
                data = self.xp_reader.receive()

                if data:
                    self._last_flight_data = data
                    self._last_data_time = time.time()

                    # Write to shared memory for Garmin DLLs
                    if self.status['shm_active']:
                        self.shm.write_xplane_data(**data)

                        # Read Garmin data back
                        garmin_data = self.shm.read_garmin_data()
                        if garmin_data:
                            # Write radio changes back to X-Plane via Lua
                            self.xp_reader.write_radio_to_xplane(garmin_data)

                    update_count += 1
                    self.status['update_count'] = update_count
                    self.status['last_data'] = {
                        'lat': data.get('latitude', 0),
                        'lon': data.get('longitude', 0),
                        'alt': data.get('altitude_msl_m', 0),
                        'hdg': data.get('mag_heading_deg', 0),
                        'ias': data.get('ias_kts', 0),
                        'vs': data.get('vs_fpm', 0),
                        'n1': data.get('n1_percent', 0),
                    }
                
                # Check connection timeout (2 seconds without data)
                is_connected = hasattr(self, '_last_data_time') and (time.time() - self._last_data_time < 2.0)
                self.status['xplane_connected'] = is_connected

                # DIRECT MEMORY INJECTION — update target data for injection thread
                if self.mem_injector.scan_complete:
                    if is_connected:
                        if not self.mem_injector.demo_killed:
                            self.mem_injector.disable_demo_flight_model()
                            self._log("[MemInject] X-Plane conectado: Demo flight model desativado!")
                        flight_data = self._last_flight_data
                        alt_ft = flight_data.get('altitude_msl_m', 0) * 3.28084  # m -> ft
                        self.mem_injector.update_target_data(
                            ias_kts=flight_data.get('ias_kts'),
                            heading_deg=flight_data.get('mag_heading_deg'),
                            altitude_ft=alt_ft,
                            gs_kts=flight_data.get('gs_kts'),
                            pitch_deg=flight_data.get('pitch_deg'),
                            roll_deg=flight_data.get('roll_deg'),
                            vs_fpm=flight_data.get('vs_fpm'),
                        )
                    else:
                        if self.mem_injector.demo_killed:
                            self.mem_injector.resume_demo()
                            self._log("[MemInject] X-Plane desconectado: Demo flight model liberado.")

                # Update stats every second
                now = time.time()
                if now - last_stats_time >= 1.0:
                    elapsed = now - last_stats_time
                    self.status['updates_per_sec'] = update_count / elapsed if elapsed > 0 else 0
                    update_count = 0
                    last_stats_time = now
                    self._update_status()

            except Exception as e:
                self.status['last_error'] = str(e)
                self._log(f"Erro no loop: {e}")
                traceback.print_exc()
                time.sleep(1.0)  # Slow down on error

            # Sleep to maintain target rate
            elapsed = time.time() - loop_start
            sleep_time = interval - elapsed
            if sleep_time > 0:
                time.sleep(sleep_time)

    def _check_garmin_processes(self):
        """Check if Garmin Trainer processes are running."""
        txi_pids = find_process(self.config['txi_process_name'])
        gtn_pids = find_process(self.config['gtn_process_name'])

        self.status['txi_running'] = len(txi_pids) > 0
        self.status['gtn_running'] = len(gtn_pids) > 0

        # Auto DLL injection if enabled
        if self.config.get('auto_inject', False):
            import threading
            # If GDU is running and not injected yet, auto-inject in a background thread
            if self.status['txi_running'] and not self.status['txi_injected']:
                if not getattr(self, '_txi_injecting', False):
                    self._txi_injecting = True
                    self._log("[AutoInject] GDU TXi detectado. Iniciando auto-injecao...")
                    
                    def run_txi_inject():
                        self.inject_garmin('txi')
                        self._txi_injecting = False
                        
                    threading.Thread(target=run_txi_inject, daemon=True).start()

            # If GTN is running and not injected yet, auto-inject in a background thread
            if self.status['gtn_running'] and not self.status['gtn_injected']:
                if not getattr(self, '_gtn_injecting', False):
                    self._gtn_injecting = True
                    self._log("[AutoInject] GTN Xi detectado. Iniciando auto-injecao...")
                    
                    def run_gtn_inject():
                        self.inject_garmin('gtn')
                        self._gtn_injecting = False
                        
                    threading.Thread(target=run_gtn_inject, daemon=True).start()

    def _check_mem_injector(self):
        """Auto-connect and enable direct memory injection with demo flight model bypass."""
        if not self.status['txi_running']:
            # GDU not running, disconnect if connected
            if self.mem_injector.hProcess:
                self.mem_injector.disconnect()
                self.status['mem_inject_active'] = False
                self.status['mem_inject_addrs'] = 0
            return

        # GDU is running — connect if not yet connected
        if not self.mem_injector.hProcess:
            if self.mem_injector.connect():
                self._log("[MemInject] Connected to GDU and Trainer SHM.")
                status = self.mem_injector.get_status()
                self.status['mem_inject_active'] = True
                self.status['mem_inject_addrs'] = status['addrs_found']
                self._log(f"[MemInject] Pronto para injeção! {status['addrs_found']} endereços ativos.")
                
                # Start injection thread at clean 50Hz
                self.mem_injector.start_injection()
            return

        # Already connected — verify addresses periodically
        if self.mem_injector.scan_complete:
            if not self.mem_injector.verify_addresses():
                self._log("[MemInject] Enderecos desatualizados, re-escaneando...")
                self.mem_injector.stop_injection()
                self.mem_injector.scan()
                status = self.mem_injector.get_status()
                self.status['mem_inject_addrs'] = status['addrs_found']
                if self.mem_injector.scan_complete:
                    self.mem_injector.start_injection()
        elif self.mem_injector.hProcess:
            # Connected but scan not done — retry scan
            self.mem_injector.scan()

    def inject_garmin(self, target='both'):
        """
        Inject GarminHook.dll into Garmin Trainer process(es).
        target: 'txi', 'gtn', or 'both'
        """
        injector = self.config.get('dll_injector_path', '')
        hook_dll = self.config.get('garmin_hook_dll_path', '')

        if not injector or not os.path.exists(injector):
            self._log(f"ERRO: DllInjector nao encontrado: {injector}")
            self._log("Verifique se o build foi feito (GarminHook_v2/build.bat)")
            return False

        if not hook_dll or not os.path.exists(hook_dll):
            self._log(f"ERRO: GarminHook.dll nao encontrada: {hook_dll}")
            return False

        success = True

        if target in ('txi', 'both'):
            txi_pids = find_process(self.config['txi_process_name'])
            if txi_pids:
                self._log(f"Injetando no GDU TXi (PID: {txi_pids[0]})...")
                if inject_dll(self.config['txi_process_name'], hook_dll, injector):
                    self.status['txi_injected'] = True
                    self._log("GDU TXi: Injecao OK!")
                else:
                    self._log("GDU TXi: FALHA na injecao!")
                    success = False
            else:
                self._log(f"GDU TXi ({self.config['txi_process_name']}) nao encontrado!")
                success = False

        if target in ('gtn', 'both'):
            gtn_pids = find_process(self.config['gtn_process_name'])
            if gtn_pids:
                self._log(f"Injetando no GTN Xi (PID: {gtn_pids[0]})...")
                if inject_dll(self.config['gtn_process_name'], hook_dll, injector):
                    self.status['gtn_injected'] = True
                    self._log("GTN Xi: Injecao OK!")
                else:
                    self._log("GTN Xi: FALHA na injecao!")
                    success = False
            else:
                self._log(f"GTN Xi ({self.config['gtn_process_name']}) nao encontrado!")
                success = False

        self._update_status()
        return success

    def launch_trainer(self):
        """Launch the Garmin Aviation Trainer Launcher."""
        path = self.config.get('garmin_trainer_path', '')
        if not os.path.exists(path):
            self._log(f"Garmin Trainer Launcher não encontrado: {path}")
            return False

        try:
            subprocess.Popen([path], cwd=os.path.dirname(path))
            self._log("Garmin Trainer Launcher iniciado!")
            return True
        except Exception as e:
            self._log(f"Erro ao iniciar Trainer: {e}")
            return False

    def launch_direct(self):
        """Launch GDU and GTN simulators directly, bypassing the Garmin Trainer."""
        import shutil
        import zipfile

        self._log("[DirectLaunch] Iniciando lancamento direto dos simuladores...")

        # --- Clean up stale processes from previous runs to avoid port conflicts ---
        for proc_name in [self.config['txi_process_name'], self.config['gtn_process_name'], 'roar.exe']:
            try:
                pids = find_process(proc_name)
                for pid in pids:
                    self._log(f"[DirectLaunch] Encerrando processo antigo {proc_name} (PID: {pid})...")
                    subprocess.run(['taskkill', '/F', '/PID', str(pid)], capture_output=True)
            except Exception as e:
                pass

        # --- Find simulator executables (prioritizing official versions used by selected aircraft config) ---
        gdu_search_paths = [
            r'C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_86\app-3.86.4\bin',
            r'C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_50\app-3.50.20\bin',
            r'C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_62\app-3.62.2\bin',
            r'C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_30\app-3.30.7\bin',
        ]
        gtn_search_paths = [
            r'C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__GTN_Xi__21_03\app-21.3.7\bin',
            r'C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__GTN_Xi__20_42\app-20.42.2\bin',
            r'C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__GTN_Xi__20_20\app-20.20.13\bin',
        ]

        gdu_exe = None
        for p in gdu_search_paths:
            candidate = os.path.join(p, 'gdu_7xx_simulator.exe')
            if os.path.exists(candidate):
                gdu_exe = candidate
                break
        if not gdu_exe:
            self._log("[DirectLaunch] ERRO: gdu_7xx_simulator.exe nao encontrado!")
            self._log(f"  Caminhos verificados: {gdu_search_paths}")
            return False

        gtn_exe = None
        for p in gtn_search_paths:
            candidate = os.path.join(p, 'gtn_simulator.exe')
            if os.path.exists(candidate):
                gtn_exe = candidate
                break
        if not gtn_exe:
            self._log("[DirectLaunch] ERRO: gtn_simulator.exe nao encontrado!")
            self._log(f"  Caminhos verificados: {gtn_search_paths}")
            return False

        self._log(f"[DirectLaunch] GDU: {gdu_exe}")
        self._log(f"[DirectLaunch] GTN: {gtn_exe}")

        # --- Find roar.exe ---
        roar_exe = None
        roar_search_paths = [
            r'C:\Program Files (x86)\Garmin\Trainers\Launcher',
        ]
        for p in roar_search_paths:
            if os.path.exists(p):
                candidates = []
                for root, dirs, files in os.walk(p):
                    if 'roar.exe' in files:
                        candidates.append(os.path.join(root, 'roar.exe'))
                if candidates:
                    candidates.sort()
                    roar_exe = candidates[-1]
                    break
        if not roar_exe:
            self._log("[DirectLaunch] AVISO: roar.exe nao encontrado! Alguns simuladores podem falhar na conexao.")
        else:
            self._log(f"[DirectLaunch] ROAR: {roar_exe}")

        # --- Create session directory in the official Garmin Temp folder ---
        sessions_base = r"C:\Users\Arnom\AppData\Local\Temp\Garmin\Trainers\Launcher\Sessions"
        session_id = f"999_{int(time.time())}"
        session_dir = os.path.join(sessions_base, session_id)

        try:
            os.makedirs(session_dir, exist_ok=True)
            # Create crash report subdirs
            os.makedirs(os.path.join(session_dir, 'TXi_1'), exist_ok=True)
            os.makedirs(os.path.join(session_dir, 'GTN_Xi_1'), exist_ok=True)
            self._log(f"[DirectLaunch] Diretorio de sessao: {session_dir}")
        except Exception as e:
            self._log(f"[DirectLaunch] ERRO ao criar diretorio de sessao: {e}")
            return False

        # --- Copy and customize sim_session.json ---
        template_path = os.path.join(_SCRIPT_DIR, 'sim_session_template.json')
        if not os.path.exists(template_path):
            self._log(f"[DirectLaunch] ERRO: Template nao encontrado: {template_path}")
            return False

        try:
            with open(template_path, 'r') as f:
                session_data = json.load(f)

            # Inject CrashReportDirectoryPath into each trainer's settings
            for trainer_id in ('TXi_1', 'GTN_Xi_1'):
                if trainer_id in session_data.get('trainers', {}):
                    crash_dir = os.path.join(session_dir, trainer_id)
                    session_data['trainers'][trainer_id]['settings']['CrashReportDirectoryPath'] = crash_dir

            session_json_path = os.path.join(session_dir, 'sim_session.json')
            with open(session_json_path, 'w') as f:
                json.dump(session_data, f, indent=2)
            self._log(f"[DirectLaunch] sim_session.json criado com CrashReportDirectoryPath")
        except Exception as e:
            self._log(f"[DirectLaunch] ERRO ao criar sim_session.json: {e}")
            return False

        # --- Copy and uncompress winfs .gca files ---
        backup_dir = os.path.join(_SCRIPT_DIR, 'garmin_session_backup')
        winfs_dest = os.path.join(session_dir, 'winfs', 'preflight')

        try:
            os.makedirs(winfs_dest, exist_ok=True)
            gca_copied = 0
            if os.path.isdir(backup_dir):
                for fname in os.listdir(backup_dir):
                    if fname.endswith('.gca'):
                        src = os.path.join(backup_dir, fname)
                        dst = os.path.join(winfs_dest, fname)
                        shutil.copy2(src, dst)
                        gca_copied += 1
                        self._log(f"[DirectLaunch] Copiado: {fname}")

                        # Extract to specific trainer directory
                        if 'TXi' in fname:
                            extract_dir = os.path.join(winfs_dest, 'TXi_1')
                            self._log(f"[DirectLaunch] Extraindo {fname} para {extract_dir}...")
                            with zipfile.ZipFile(src, 'r') as z:
                                z.extractall(extract_dir)
                            self._log("[DirectLaunch] TXi_1 extraído com sucesso")
                        elif 'GTN_Xi' in fname:
                            extract_dir = os.path.join(winfs_dest, 'GTN_Xi_1')
                            self._log(f"[DirectLaunch] Extraindo {fname} para {extract_dir}...")
                            with zipfile.ZipFile(src, 'r') as z:
                                z.extractall(extract_dir)
                            self._log("[DirectLaunch] GTN_Xi_1 extraído com sucesso")

            if gca_copied == 0:
                self._log("[DirectLaunch] AVISO: Nenhum arquivo .gca encontrado no backup!")
                self._log(f"  Verificar: {backup_dir}")
            else:
                self._log(f"[DirectLaunch] {gca_copied} arquivo(s) .gca copiado(s) e extraido(s) para winfs/preflight/")
        except Exception as e:
            self._log(f"[DirectLaunch] ERRO ao copiar e extrair arquivos .gca: {e}")
            return False

        # --- Launch simulators ---
        if not hasattr(self, '_direct_processes'):
            self._direct_processes = []

        try:
            # 1. Launch ROAR master first so coordinate mapping works immediately
            if roar_exe:
                self._log("[DirectLaunch] Iniciando ROAR master...")
                roar_proc = subprocess.Popen(
                    [roar_exe],
                    cwd=os.path.dirname(roar_exe),
                    close_fds=True,
                )
                self._direct_processes.append(('ROAR', roar_proc))
                self._log(f"[DirectLaunch] ROAR master iniciado (PID: {roar_proc.pid})")
                
                # Give ROAR time to set up and bind socket ports
                time.sleep(1.5)

            session_dir = os.path.dirname(session_json_path)

            # 2. Launch GTN simulator
            self._log("[DirectLaunch] Iniciando GTN Xi...")
            env_gtn = os.environ.copy()
            gtn_dir = os.path.dirname(gtn_exe)
            env_gtn['PATH'] = gtn_dir + os.pathsep + env_gtn.get('PATH', '')
            gtn_proc = subprocess.Popen(
                ['gtn_simulator.exe', '--configpath', session_json_path, '--endpt', 'GTN_Xi_1'],
                executable=gtn_exe,
                cwd=session_dir,
                env=env_gtn,
                close_fds=True,
            )
            self._direct_processes.append(('GTN_Xi', gtn_proc))
            self._log(f"[DirectLaunch] GTN Xi iniciado (PID: {gtn_proc.pid})")

            # Small delay between simulator launches
            time.sleep(1.0)

            # 3. Launch GDU simulator
            self._log("[DirectLaunch] Iniciando GDU TXi...")
            env_gdu = os.environ.copy()
            gdu_dir = os.path.dirname(gdu_exe)
            env_gdu['PATH'] = gdu_dir + os.pathsep + env_gdu.get('PATH', '')
            gdu_proc = subprocess.Popen(
                ['gdu_7xx_simulator.exe', '--configpath', session_json_path, '--endpt', 'TXi_1'],
                executable=gdu_exe,
                cwd=session_dir,
                env=env_gdu,
                close_fds=True,
            )
            self._direct_processes.append(('GDU_TXi', gdu_proc))
            self._log(f"[DirectLaunch] GDU TXi iniciado (PID: {gdu_proc.pid})")

        except Exception as e:
            self._log(f"[DirectLaunch] ERRO ao lancar simuladores: {e}")
            traceback.print_exc()
            return False

        self._log("[DirectLaunch] ✅ Simuladores lancados com sucesso! Aguardando processos aparecerem...")
        return True

    def kill_direct_processes(self):
        """Terminate any directly-launched simulator processes."""
        if not hasattr(self, '_direct_processes') or not self._direct_processes:
            self._log("[DirectLaunch] Nenhum processo direto para encerrar.")
            return

        for name, proc in self._direct_processes:
            try:
                if proc.poll() is None:
                    proc.terminate()
                    self._log(f"[DirectLaunch] Terminando {name} (PID: {proc.pid})")
            except Exception as e:
                self._log(f"[DirectLaunch] Erro ao terminar {name}: {e}")

        self._direct_processes.clear()
        self._log("[DirectLaunch] Processos diretos encerrados.")


# ============================================================
# Standalone UDP Bridge (alternative to XPlaneConnect)
# ============================================================

class UDPBridge:
    """
    Alternative data bridge using direct UDP, matching the X-Plane
    DATA output format that the old GarminHook already understood.
    
    This can be used if XPlaneConnect is not available or if you
    want to use X-Plane's built-in UDP data output.
    """

    def __init__(self, listen_port=49002, send_port=49003):
        self.listen_port = listen_port
        self.send_port = send_port
        self.sock = None

    def start(self):
        """Start listening for X-Plane UDP data."""
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sock.bind(('127.0.0.1', self.listen_port))
        self.sock.setblocking(False)  # Non-blocking for high frequency loop

    def disconnect(self):
        """Close the UDP socket."""
        if self.sock:
            try:
                self.sock.close()
            except Exception:
                pass
            self.sock = None

    def receive(self):
        """
        Receive and parse GXPL packet from Lua script.
        Returns dict with parsed values.
        """
        try:
            data, addr = self.sock.recvfrom(2048)
        except BlockingIOError:
            return None
        except socket.timeout:
            return None

        try:
            if not data or len(data) < 8:
                return None

            header = data[:4].decode('ascii', errors='ignore')
            if header != 'GXPL':
                return None

            # Format from XPGarminBridge.lua
            # 4 bytes 'GXPL'
            # 4 bytes magic
            # 4 bytes sequence
            # 8 bytes timestamp (double)
            # -> offset = 20

            result = {}
            magic, seq = struct.unpack_from('<II', data, 4)
            timestamp = struct.unpack_from('<d', data, 12)[0]

            offset = 20
            # GPS
            result['latitude'], result['longitude'] = struct.unpack_from('<dd', data, offset)
            offset += 16
            result['altitude_msl_m'], result['altitude_agl_m'], result['groundspeed_ms'] = struct.unpack_from('<fff', data, offset)
            offset += 12

            # Attitude
            result['pitch_deg'], result['roll_deg'], result['mag_heading_deg'], result['true_heading_deg'] = struct.unpack_from('<ffff', data, offset)
            offset += 16

            # Air Data
            result['ias_kts'], result['tas_kts'], result['vs_fpm'], result['baro_inhg'], result['indicated_alt_ft'], result['oat_deg_c'] = struct.unpack_from('<ffffff', data, offset)
            offset += 24

            # Slip/Turn
            result['slip_deg'], result['turn_rate_deg_s'] = struct.unpack_from('<ff', data, offset)
            offset += 8

            # Engine
            result['n1_percent'], result['n2_percent'], result['torque_ftlb'], result['torque_percent'], \
            result['itt_deg_c'], result['oil_press_psi'], result['oil_temp_deg_c'], result['fuel_flow_pph'], \
            result['fuel_qty_kg_l'], result['fuel_qty_kg_r'], result['prop_rpm'] = struct.unpack_from('<fffffffffff', data, offset)
            offset += 44
            
            result['eng_running'] = struct.unpack_from('<I', data, offset)[0]
            offset += 4

            # Electrical
            result['bus_volts'], result['gen_amps'] = struct.unpack_from('<ff', data, offset)
            offset += 8

            # Radio
            result['com1_active_khz'], result['com1_standby_khz'], result['com2_active_khz'], result['com2_standby_khz'], \
            result['nav1_active_khz'], result['nav1_standby_khz'], result['nav2_active_khz'], result['nav2_standby_khz'] = struct.unpack_from('<IIIIIIII', data, offset)
            offset += 32

            # Navigation
            result['nav1_hdef_dots'], result['nav1_vdef_dots'], result['gps_track_deg'] = struct.unpack_from('<fff', data, offset)
            offset += 12

            # Time
            result['utc_hours'], result['utc_minutes'], result['utc_seconds'] = struct.unpack_from('<fff', data, offset)
            offset += 12

            # Flags
            result['data_valid'] = struct.unpack_from('<I', data, offset)[0]

            return result

        except socket.timeout:
            return None
        except Exception as e:
            print(f"Erro UDP: {e}")
            return None

    def write_radio_to_xplane(self, garmin_data):
        """
        Write radio frequency changes from Garmin back to Lua script via UDP.
        """
        if not self.sock or not garmin_data:
            return
            
        try:
            # Only write if Garmin has valid data
            if garmin_data.get('data_valid', 0) == 0:
                return
                
            # Build GBRX packet for Lua
            # Format: 'GBRX' (4) + cmd_type (4) + com1_act (4) + com1_stby (4) + nav1_act (4) + nav1_stby (4)
            cmd_type = 1 # Radio change
            
            pkt = struct.pack('<4sIIIIII', 
                b'GBRX', 
                cmd_type,
                garmin_data.get('com1_active_khz', 0),
                garmin_data.get('com1_standby_khz', 0),
                garmin_data.get('nav1_active_khz', 0),
                garmin_data.get('nav1_standby_khz', 0)
            )
            self.sock.sendto(pkt, ('127.0.0.1', self.send_port))
            
        except Exception as e:
            pass

    def stop(self):
        if self.sock:
            self.sock.close()
            self.sock = None


# ============================================================
# Main Entry Point
# ============================================================

def main():
    """Main entry point — launches GUI or CLI."""
    config = load_config()
    save_config(config)  # Save defaults if first run

    # Ensure log directory exists
    os.makedirs(config.get('log_dir', '.'), exist_ok=True)

    # Try to launch GUI
    try:
        from bridge_gui import run_gui
        run_gui(config)
    except ImportError:
        print("GUI não disponível, rodando em modo CLI...")
        run_cli(config)


def run_cli(config):
    """Run bridge in CLI mode (no GUI)."""
    bridge = GarminBridge(config)

    def log_handler(msg):
        pass  # Already printed by _log

    bridge.on_log(log_handler)
    bridge.start()

    print("\nGarminBridge rodando. Pressione Ctrl+C para parar.\n")
    print("Comandos: 'inject' = injetar DLLs, 'status' = ver status, 'quit' = sair\n")

    try:
        while True:
            cmd = input("> ").strip().lower()
            if cmd == 'quit' or cmd == 'q':
                break
            elif cmd == 'inject' or cmd == 'i':
                bridge.inject_garmin()
            elif cmd == 'status' or cmd == 's':
                s = bridge.status
                print(f"  X-Plane: {'Conectado' if s['xplane_connected'] else 'Desconectado'}")
                print(f"  GDU TXi: {'Rodando' if s['txi_running'] else 'Parado'}"
                      f" | Injetado: {'Sim' if s['txi_injected'] else 'Não'}")
                print(f"  GTN Xi:  {'Rodando' if s['gtn_running'] else 'Parado'}"
                      f" | Injetado: {'Sim' if s['gtn_injected'] else 'Não'}")
                print(f"  Updates/s: {s['updates_per_sec']:.1f}")
                if s.get('last_data'):
                    d = s['last_data']
                    print(f"  Posição: {d.get('lat', 0):.4f}, {d.get('lon', 0):.4f}")
                    print(f"  Alt: {d.get('alt', 0):.0f}m  HDG: {d.get('hdg', 0):.0f}°"
                          f"  IAS: {d.get('ias', 0):.0f}kt  N1: {d.get('n1', 0):.1f}%")
            elif cmd == 'launch' or cmd == 'l':
                bridge.launch_trainer()
            elif cmd == 'help' or cmd == 'h':
                print("  inject (i)  — Injetar DLLs nos processos Garmin")
                print("  status (s)  — Ver status da bridge")
                print("  launch (l)  — Abrir Garmin Trainer Launcher")
                print("  quit (q)    — Sair")
            else:
                print("  Comando desconhecido. Digite 'help' para ajuda.")

    except KeyboardInterrupt:
        pass

    bridge.stop()
    print("GarminBridge finalizado.")


if __name__ == '__main__':
    main()
