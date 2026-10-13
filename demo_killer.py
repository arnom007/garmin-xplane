"""
demo_killer.py — Standalone Demo Thread Killer for Garmin GDU

Opens the GDU process, scans for the IAS widget, then systematically
tests each thread to find which one drives the demo mode.
Suspends the demo thread so external injection can work cleanly.

Usage: Run as admin while GDU trainer is open.
"""

import ctypes
import ctypes.wintypes as wt
import struct
import time
import sys
import os

kernel32 = ctypes.windll.kernel32

# Windows API constants
PROCESS_VM_READ = 0x0010
PROCESS_VM_WRITE = 0x0020
PROCESS_VM_OPERATION = 0x0008
PROCESS_QUERY_INFORMATION = 0x0400
MEM_COMMIT = 0x1000
PAGE_NOACCESS = 0x01
PAGE_GUARD = 0x100
TH32CS_SNAPPROCESS = 0x02
TH32CS_SNAPTHREAD = 0x00000004
THREAD_SUSPEND_RESUME = 0x0002

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

class THREADENTRY32(ctypes.Structure):
    _fields_ = [
        ("dwSize", wt.DWORD), ("cntUsage", wt.DWORD),
        ("th32ThreadID", wt.DWORD), ("th32OwnerProcessID", wt.DWORD),
        ("tpBasePri", ctypes.c_long), ("tpDeltaPri", ctypes.c_long),
        ("dwFlags", wt.DWORD),
    ]


def find_gdu():
    """Find GDU process PID."""
    snap = kernel32.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0)
    pe = PROCESSENTRY32()
    pe.dwSize = ctypes.sizeof(PROCESSENTRY32)
    result = None
    if kernel32.Process32First(snap, ctypes.byref(pe)):
        while True:
            exe = pe.szExeFile.decode('utf-8', errors='replace').lower()
            if 'gdu_7xx' in exe:
                result = pe.th32ProcessID
                break
            if not kernel32.Process32Next(snap, ctypes.byref(pe)):
                break
    kernel32.CloseHandle(snap)
    return result


def read_float(hProcess, addr):
    buf = ctypes.create_string_buffer(4)
    br = ctypes.c_size_t(0)
    if kernel32.ReadProcessMemory(hProcess, ctypes.c_void_p(addr), buf, 4, ctypes.byref(br)):
        return struct.unpack('<f', buf.raw[:4])[0]
    return None


def write_float(hProcess, addr, value):
    data = struct.pack('<f', value)
    bw = ctypes.c_size_t(0)
    return kernel32.WriteProcessMemory(hProcess, ctypes.c_void_p(addr), data, 4, ctypes.byref(bw))


def find_ias_widget(hProcess):
    """Find IAS widget by [IAS] [20.0] [999.0] signature."""
    print("  Scanning for IAS widget...")
    
    try:
        import numpy as np
        use_numpy = True
    except ImportError:
        use_numpy = False
    
    candidates = []
    mbi = MEMORY_BASIC_INFORMATION()
    addr = 0x10000
    
    while addr < 0x7FFE0000:
        if kernel32.VirtualQueryEx(hProcess, ctypes.c_void_p(addr),
                                   ctypes.byref(mbi), ctypes.sizeof(mbi)) == 0:
            addr += 0x10000
            continue
        if (mbi.State == MEM_COMMIT and mbi.Protect and
            not (mbi.Protect & PAGE_NOACCESS) and
            not (mbi.Protect & PAGE_GUARD) and
            mbi.RegionSize < 50 * 1024 * 1024):
            buf = ctypes.create_string_buffer(mbi.RegionSize)
            br = ctypes.c_size_t(0)
            if kernel32.ReadProcessMemory(hProcess, ctypes.c_void_p(mbi.BaseAddress),
                                          buf, mbi.RegionSize, ctypes.byref(br)):
                data = buf.raw[:br.value]
                base = mbi.BaseAddress
                
                if use_numpy and len(data) >= 12:
                    farr = np.frombuffer(data[:len(data) - len(data) % 4], dtype=np.float32)
                    twenty_idxs = np.where(np.abs(farr - 20.0) < 0.01)[0]
                    for idx in twenty_idxs:
                        idx = int(idx)
                        if 1 <= idx and idx + 1 < len(farr):
                            if abs(float(farr[idx + 1]) - 999.0) < 0.01:
                                ias_val = float(farr[idx - 1])
                                if 0 <= ias_val <= 500:
                                    candidates.append((base + (idx - 1) * 4, ias_val))
                else:
                    for off in range(4, len(data) - 8, 4):
                        fval = struct.unpack_from('<f', data, off)[0]
                        if abs(fval - 20.0) < 0.01:
                            f_999 = struct.unpack_from('<f', data, off + 4)[0]
                            if abs(f_999 - 999.0) < 0.01:
                                ias_val = struct.unpack_from('<f', data, off - 4)[0]
                                if 0 <= ias_val <= 500:
                                    candidates.append((base + off - 4, ias_val))
        addr = mbi.BaseAddress + mbi.RegionSize
    
    if candidates:
        best = max(candidates, key=lambda x: x[1] if 50 < x[1] < 400 else -1)
        return best[0], best[1]
    return None, None


def enumerate_threads(pid):
    """Get all thread IDs for a process."""
    snap = kernel32.CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0)
    te = THREADENTRY32()
    te.dwSize = ctypes.sizeof(THREADENTRY32)
    threads = []
    if kernel32.Thread32First(snap, ctypes.byref(te)):
        while True:
            if te.th32OwnerProcessID == pid:
                threads.append(te.th32ThreadID)
            if not kernel32.Thread32Next(snap, ctypes.byref(te)):
                break
    kernel32.CloseHandle(snap)
    return threads


# ============================================================
# Main
# ============================================================

print("=" * 60)
print("  Garmin GDU — Demo Thread Killer")
print("=" * 60)

# Find GDU
pid = find_gdu()
if not pid:
    print("\n  ERRO: GDU (gdu_7xx_simulator.exe) nao encontrado!")
    print("  Abra o Garmin Trainer primeiro.")
    input("\nPress Enter to exit...")
    sys.exit(1)

print(f"\n  GDU encontrado: PID={pid}")

# Open process
hProcess = kernel32.OpenProcess(
    PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_QUERY_INFORMATION,
    False, pid
)
if not hProcess:
    print(f"\n  ERRO: Nao foi possivel abrir o processo (erro {ctypes.GetLastError()})")
    print("  Execute como Administrador!")
    input("\nPress Enter to exit...")
    sys.exit(1)

# Find IAS widget
print("\n  [1/3] Procurando IAS widget...")
t0 = time.time()
ias_addr, ias_val = find_ias_widget(hProcess)
dt = time.time() - t0

if not ias_addr:
    print(f"  ERRO: IAS widget nao encontrado (scan levou {dt:.1f}s)")
    kernel32.CloseHandle(hProcess)
    input("\nPress Enter to exit...")
    sys.exit(1)

print(f"  IAS widget: 0x{ias_addr:08X} = {ias_val:.1f} kts (scan: {dt:.1f}s)")

# Enumerate threads
print(f"\n  [2/3] Enumerando threads...")
threads = enumerate_threads(pid)
main_tid = min(threads)
test_threads = [t for t in threads if t != main_tid]
print(f"  Total: {len(threads)} threads (main TID={main_tid})")
print(f"  Testando {len(test_threads)} threads candidatas...\n")

# Test each thread
SENTINEL = 77777.0
demo_tid = None
demo_handle = None

for i, tid in enumerate(test_threads):
    hThread = kernel32.OpenThread(THREAD_SUSPEND_RESUME, False, tid)
    if not hThread:
        print(f"  [{i+1}/{len(test_threads)}] TID={tid}: SKIP (no access)")
        continue
    
    # Write sentinel
    write_float(hProcess, ias_addr, SENTINEL)
    
    # Suspend thread
    kernel32.SuspendThread(hThread)
    
    # Wait for demo to overwrite
    time.sleep(0.15)
    
    # Read back
    val = read_float(hProcess, ias_addr)
    
    if val is not None and abs(val - SENTINEL) < 1.0:
        print(f"  [{i+1}/{len(test_threads)}] TID={tid}: *** DEMO THREAD FOUND! ***")
        print(f"           Sentinel {SENTINEL:.0f} survived 150ms (readback={val:.1f})")
        demo_tid = tid
        demo_handle = hThread
        break
    else:
        kernel32.ResumeThread(hThread)
        kernel32.CloseHandle(hThread)
        print(f"  [{i+1}/{len(test_threads)}] TID={tid}: not demo (val={f'{val:.1f}' if val is not None else 'None'})")

# Result
print(f"\n  {'='*50}")
if demo_tid:
    print(f"  SUCESSO! Demo thread suspensa: TID={demo_tid}")
    print(f"  Os ponteiros da GDU devem ter PARADO de se mover.")
    print()
    print(f"  [3/3] Teste de escrita com demo morto:")
    
    # Test that writes now stick
    for test_val in [100.0, 200.0, 0.0]:
        write_float(hProcess, ias_addr, test_val)
        time.sleep(0.1)
        readback = read_float(hProcess, ias_addr)
        ok = "OK" if readback and abs(readback - test_val) < 1.0 else "FAIL"
        print(f"    IAS={test_val:.0f} → readback={readback:.1f}  [{ok}]")
    
    print()
    choice = input("  Manter demo suspenso? (S/n): ").strip().lower()
    if choice == 'n':
        kernel32.ResumeThread(demo_handle)
        kernel32.CloseHandle(demo_handle)
        print("  Demo thread RESUMIDA.")
    else:
        print("  Demo thread permanece SUSPENSA.")
        print("  O GDU vai manter os valores que voce injetar.")
        print("  FECHE esta janela quando terminar.")
        # Keep running so the handle stays valid
        try:
            input("\n  Press Enter para RESUMIR o demo e sair...")
        except KeyboardInterrupt:
            pass
        kernel32.ResumeThread(demo_handle)
        kernel32.CloseHandle(demo_handle)
        print("  Demo thread resumida.")
else:
    print("  FALHA: Nao foi possivel identificar a thread do demo.")
    print("  O demo pode rodar na thread principal (UI).")
    print("  Nesse caso, usaremos burst mode (alta frequencia).")

kernel32.CloseHandle(hProcess)
print("\nDone!")
input("Press Enter to exit...")
