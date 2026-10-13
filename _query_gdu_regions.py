import ctypes
from ctypes import wintypes

kernel32 = ctypes.windll.kernel32

hGDU = kernel32.OpenProcess(0x1F0FFF, False, 6452)

class MBI(ctypes.Structure):
    _fields_ = [
        ("BaseAddress", ctypes.c_void_p),
        ("AllocationBase", ctypes.c_void_p),
        ("AllocationProtect", wintypes.DWORD),
        ("RegionSize", ctypes.c_size_t),
        ("State", wintypes.DWORD),
        ("Protect", wintypes.DWORD),
        ("Type", wintypes.DWORD),
    ]

mbi = MBI()
for addr, name in [(0x312BB9F0, "GDU AHRS Heading"), (0x2D3BFE1C, "GDU IAS Widget")]:
    kernel32.VirtualQueryEx(hGDU, ctypes.c_void_p(addr), ctypes.byref(mbi), ctypes.sizeof(mbi))
    print(f"{name} (0x{addr:08x}):")
    print(f"  BaseAddress: 0x{mbi.BaseAddress:08x}")
    print(f"  AllocationBase: 0x{mbi.AllocationBase:08x}")
    print(f"  RegionSize: 0x{mbi.RegionSize:x} ({mbi.RegionSize} bytes)")
    print(f"  State: 0x{mbi.State:x}, Protect: 0x{mbi.Protect:x}, Type: 0x{mbi.Type:x}")

kernel32.CloseHandle(hGDU)
