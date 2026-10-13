import ctypes
import ctypes.wintypes as wt

user32 = ctypes.windll.user32

def cb(hwnd, lparam):
    if user32.IsWindowVisible(hwnd):
        title = ctypes.create_unicode_buffer(256)
        user32.GetWindowTextW(hwnd, title, 256)
        if title.value:
            pid = wt.DWORD()
            user32.GetWindowThreadProcessId(hwnd, ctypes.byref(pid))
            print(f"PID {pid.value} HWND {hex(hwnd)}: '{title.value}'")
    return True

WNDENUMPROC = ctypes.WINFUNCTYPE(ctypes.c_bool, wt.HWND, wt.LPARAM)
user32.EnumWindows(WNDENUMPROC(cb), 0)
