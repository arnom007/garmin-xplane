"""
Garmin GDU Memory Scanner - Interactive Console
Just run this and follow the prompts!
"""

import builtins
import datetime
import os

log_dir = os.path.join(os.path.expandvars(r'%APPDATA%'), 'GarminBridge')
os.makedirs(log_dir, exist_ok=True)
log_path = os.path.join(log_dir, 'memscan_interactive.log')
_log_f = open(log_path, 'a', encoding='utf-8')
_log_f.write(f'\n\n=== NEW SESSION: {datetime.datetime.now().isoformat()} ===\n')

def L(*args, **kwargs):
    builtins.print(*args, **kwargs)
    end = kwargs.get('end', '\n')
    msg = " ".join(str(a) for a in args) + end
    _log_f.write(msg)
    _log_f.flush()


_orig_input = builtins.input
def I(prompt=''):
    builtins.print(prompt, end='', flush=True)
    _log_f.write(prompt)
    _log_f.flush()
    val = _orig_input()
    _log_f.write(val + '\n')
    _log_f.flush()
    return val

import ctypes
import ctypes.wintypes as wt
import struct
import sys
import os
import json
import time



kernel32 = ctypes.windll.kernel32
PROCESS_VM_READ = 0x0010
PROCESS_QUERY_INFORMATION = 0x0400
MEM_COMMIT = 0x1000
PAGE_NOACCESS = 0x01
PAGE_GUARD = 0x100
TH32CS_SNAPPROCESS = 0x02
SAVE_FILE = os.path.join(os.path.dirname(os.path.abspath(__file__)), '_memscan_state.json')

class PROCESSENTRY32(ctypes.Structure):
    _fields_ = [
        ("dwSize", wt.DWORD), ("cntUsage", wt.DWORD),
        ("th32ProcessID", wt.DWORD), ("th32DefaultHeapID", ctypes.c_void_p),
        ("th32ModuleID", wt.DWORD), ("cntThreads", wt.DWORD),
        ("th32ParentProcessID", wt.DWORD), ("pcPriClassBase", ctypes.c_long),
        ("dwFlags", wt.DWORD), ("szExeFile", ctypes.c_char * 260),
    ]

class MEMORY_BASIC_INFORMATION(ctypes.Structure):
    _fields_ = [
        ("BaseAddress", ctypes.c_void_p), ("AllocationBase", ctypes.c_void_p),
        ("AllocationProtect", wt.DWORD), ("RegionSize", ctypes.c_size_t),
        ("State", wt.DWORD), ("Protect", wt.DWORD), ("Type", wt.DWORD),
    ]

def find_gdu():
    snap = kernel32.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0)
    pe = PROCESSENTRY32()
    pe.dwSize = ctypes.sizeof(PROCESSENTRY32)
    result = None
    if kernel32.Process32First(snap, ctypes.byref(pe)):
        while True:
            exe = pe.szExeFile.decode('utf-8', errors='replace').lower()
            if 'gdu_7xx' in exe:
                result = (pe.th32ProcessID, exe)
                break
            if not kernel32.Process32Next(snap, ctypes.byref(pe)):
                break
    kernel32.CloseHandle(snap)
    return result

def scan_all(pid, target, tol):
    hProc = kernel32.OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, False, pid)
    if not hProc:
        L(f"  ERRO: Nao conseguiu abrir processo. Rode como Administrador!")
        return []
    results = []
    mbi = MEMORY_BASIC_INFORMATION()
    addr = 0x10000
    scanned = 0
    while addr < 0x7FFE0000:
        if kernel32.VirtualQueryEx(hProc, ctypes.c_void_p(addr), ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
            addr += 0x10000
            continue
        if (mbi.State == MEM_COMMIT and mbi.Protect and
            not (mbi.Protect & PAGE_NOACCESS) and not (mbi.Protect & PAGE_GUARD) and
            mbi.RegionSize < 50*1024*1024):
            buf = ctypes.create_string_buffer(mbi.RegionSize)
            br = ctypes.c_size_t(0)
            if kernel32.ReadProcessMemory(hProc, ctypes.c_void_p(mbi.BaseAddress), buf, mbi.RegionSize, ctypes.byref(br)):
                data = buf.raw[:br.value]
                for off in range(0, len(data)-3, 4):
                    val = struct.unpack_from('<f', data, off)[0]
                    if abs(val - target) <= tol:
                        results.append(mbi.BaseAddress + off)
                scanned += br.value
        addr = mbi.BaseAddress + mbi.RegionSize
    kernel32.CloseHandle(hProc)
    L(f"  Escaneou {scanned / 1024 / 1024:.0f} MB de memoria")
    return results

def read_addrs(pid, addrs, target, tol):
    hProc = kernel32.OpenProcess(PROCESS_VM_READ, False, pid)
    if not hProc:
        return []
    matched = []
    for a in addrs:
        buf = ctypes.create_string_buffer(4)
        br = ctypes.c_size_t(0)
        if kernel32.ReadProcessMemory(hProc, ctypes.c_void_p(a), buf, 4, ctypes.byref(br)):
            val = struct.unpack('<f', buf.raw[:4])[0]
            if abs(val - target) <= tol:
                matched.append((a, val))
    kernel32.CloseHandle(hProc)
    return matched

def read_context(pid, address):
    hProc = kernel32.OpenProcess(PROCESS_VM_READ, False, pid)
    if not hProc:
        return
    buf = ctypes.create_string_buffer(160)
    br = ctypes.c_size_t(0)
    start = address - 32
    if kernel32.ReadProcessMemory(hProc, ctypes.c_void_p(start), buf, 160, ctypes.byref(br)):
        data = buf.raw[:br.value]
        for i in range(0, len(data)-3, 4):
            fval = struct.unpack_from('<f', data, i)[0]
            marker = " <<<< AQUI" if start+i == address else ""
            if abs(fval) < 1e8 and (abs(fval) > 1e-8 or fval == 0):
                L(f"    0x{start+i:08X}: {fval:12.4f}{marker}")
            else:
                ival = struct.unpack_from('<I', data, i)[0]
                L(f"    0x{start+i:08X}: 0x{ival:08X}{marker}")
    kernel32.CloseHandle(hProc)

def monitor_addr(pid, address):
    hProc = kernel32.OpenProcess(PROCESS_VM_READ, False, pid)
    if not hProc:
        return
    L(f"  Monitorando 0x{address:08X}... (Ctrl+C para parar)")
    try:
        while True:
            buf = ctypes.create_string_buffer(4)
            br = ctypes.c_size_t(0)
            kernel32.ReadProcessMemory(hProc, ctypes.c_void_p(address), buf, 4, ctypes.byref(br))
            val = struct.unpack('<f', buf.raw[:4])[0]
            L(f"\r  Valor: {val:12.4f}", end='', flush=True)
            time.sleep(0.1)
    except KeyboardInterrupt:
        L()
    kernel32.CloseHandle(hProc)

def banner():
    L()
    L("=" * 55)
    L("  GARMIN GDU - Memory Scanner")
    L("  Encontra variaveis do flight model na memoria")
    L("=" * 55)
    L()

def main():
    banner()
    
    # Check GDU
    proc = find_gdu()
    if not proc:
        L("  [!] GDU simulator NAO encontrado!")
        L("  [!] Abra o Garmin Trainer primeiro.")
        I("\n  Pressione ENTER para sair...")
        return
    
    pid, name = proc
    L(f"  [OK] GDU encontrado: {name} (PID={pid})")
    L()

    while True:
        L("-" * 55)
        L("  OPCOES:")
        L("    1 - SCAN 1: Primeira varredura (valor atual)")
        L("    2 - SCAN 2: Segunda varredura (apos mudar valor)")
        L("    3 - MONITORAR um endereco")
        L("    4 - VER contexto de um endereco")
        L("    5 - SCAN RAPIDO (heading ~0-360)")
        L("    0 - Sair")
        L("-" * 55)
        
        try:
            choice = I("  Escolha: ").strip()
        except (EOFError, KeyboardInterrupt):
            break
        
        if choice == '0':
            break
        
        elif choice == '1':
            L()
            L("  === SCAN 1 ===")
            L("  Olhe o PFD da GDU e anote um valor aproximado.")
            L("  Pode ser IAS, GS, altitude, heading, etc.")
            L()
            try:
                val_str = I("  Valor aproximado no PFD (ex: 160): ").strip()
                target = float(val_str)
                tol_str = I("  Tolerancia (padrao 5): ").strip()
                tol = float(tol_str) if tol_str else 5.0
            except ValueError:
                L("  Valor invalido!")
                continue
            
            L(f"\n  Escaneando memoria por ~{target:.1f} +/- {tol:.1f}...")
            t0 = time.time()
            addrs = scan_all(pid, target, tol)
            dt = time.time() - t0
            L(f"  Encontrou {len(addrs)} enderecos em {dt:.1f}s")
            
            state = {'addrs': addrs, 'target': target, 'tol': tol, 'pid': pid}
            with open(SAVE_FILE, 'w') as f:
                json.dump(state, f)
            L(f"  Salvo!")
            L()
            L("  >>> Agora MUDE o valor no demo (Navigation Control)")
            L("  >>> Ex: mude a velocidade de 160 para 250")
            L("  >>> Depois escolha opcao 2")
            L()
        
        elif choice == '2':
            L()
            L("  === SCAN 2 ===")
            if not os.path.exists(SAVE_FILE):
                L("  [!] Faca o SCAN 1 primeiro!")
                continue
            
            with open(SAVE_FILE, 'r') as f:
                state = json.load(f)
            
            old_addrs = state['addrs']
            old_target = state['target']
            L(f"  Scan 1 tinha {len(old_addrs)} enderecos com ~{old_target:.1f}")
            L()
            
            try:
                val_str = I("  Novo valor no PFD (ex: 250): ").strip()
                new_target = float(val_str)
                tol_str = I("  Tolerancia (padrao 5): ").strip()
                tol = float(tol_str) if tol_str else 5.0
            except ValueError:
                L("  Valor invalido!")
                continue
            
            L(f"\n  Verificando {len(old_addrs)} enderecos...")
            matched = read_addrs(pid, old_addrs, new_target, tol)
            L(f"\n  === RESULTADO ===")
            L(f"  Enderecos que mudaram de ~{old_target:.1f} para ~{new_target:.1f}: {len(matched)}")
            
            if len(matched) > 0:
                # Sempre salvar a lista menor para o próximo scan2
                state['addrs'] = [a for a, v in matched]
                state['scan2_results'] = [(a, v) for a, v in matched]
                with open(SAVE_FILE, 'w') as f:
                    json.dump(state, f)

                if len(matched) <= 100:
                    L()
                    for addr, val in matched:
                        L(f"    0x{addr:08X}: {val:.4f}")
                    
                    if len(matched) <= 10:
                        L(f"\n  === Contexto dos matches ===")
                        for addr, val in matched[:5]:
                            L(f"\n  --- 0x{addr:08X} = {val:.4f} ---")
                            read_context(pid, addr)
                    
                    if len(matched) > 10:
                        L("\n  Ainda muitos! Mude o valor de novo e faca outro scan2.")
                    else:
                        L("\n  [!] Esses sao os candidatos! Use opcao 3 para monitorar.")
                else:
                    L(f"  Muitos matches ({len(matched)}). Mude o valor mais e tente scan2 de novo.")
                    for addr, val in matched[:20]:
                        L(f"    0x{addr:08X}: {val:.4f}")
            else:
                L("  Nenhum match! O valor pode ter sido armazenado diferente.")
                L("  Tente com tolerancia maior ou outro parametro.")
        
        elif choice == '3':
            L()
            try:
                addr_str = I("  Endereco (ex: 0x00F11D3C): ").strip()
                addr = int(addr_str, 16) if addr_str.startswith('0x') else int(addr_str)
            except ValueError:
                L("  Endereco invalido!")
                continue
            monitor_addr(pid, addr)
        
        elif choice == '4':
            L()
            try:
                addr_str = I("  Endereco (ex: 0x00F11D3C): ").strip()
                addr = int(addr_str, 16) if addr_str.startswith('0x') else int(addr_str)
            except ValueError:
                L("  Endereco invalido!")
                continue
            read_context(pid, addr)
        
        elif choice == '5':
            L()
            L("  === SCAN RAPIDO: Heading ===")
            try:
                hdg_str = I("  Heading atual no PFD (ex: 270): ").strip()
                hdg = float(hdg_str)
            except ValueError:
                L("  Valor invalido!")
                continue
            
            L(f"  Escaneando por heading ~{hdg:.0f} +/- 3...")
            addrs = scan_all(pid, hdg, 3.0)
            L(f"  Float matches: {len(addrs)}")
            
            # Also scan for radians
            hdg_rad = hdg * 3.14159265 / 180.0
            L(f"  Escaneando por {hdg:.0f} em radianos ({hdg_rad:.4f}) +/- 0.05...")
            addrs_rad = scan_all(pid, hdg_rad, 0.05)
            L(f"  Radian matches: {len(addrs_rad)}")
            
            all_addrs = addrs + addrs_rad
            state = {'addrs': all_addrs, 'target': hdg, 'tol': 3.0, 'pid': pid, 'type': 'heading'}
            with open(SAVE_FILE, 'w') as f:
                json.dump(state, f)
            L(f"  Total salvo: {len(all_addrs)} enderecos")
            L("  Mude o heading e faca scan2!")
        
        L()

    L("\n  Bye!")

if __name__ == '__main__':
    main()
