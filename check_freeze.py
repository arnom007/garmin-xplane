import ctypes
import ctypes.wintypes as wt

user32 = ctypes.windll.user32

def enum_win(hwnd, extra):
    if user32.IsWindowVisible(hwnd):
        length = user32.GetWindowTextLengthW(hwnd)
        if length > 0:
            buf = ctypes.create_unicode_buffer(length + 1)
            user32.GetWindowTextW(hwnd, buf, length + 1)
            title = buf.value
            pid = wt.DWORD()
            user32.GetWindowThreadProcessId(hwnd, ctypes.byref(pid))
            is_hung = user32.IsHungAppWindow(hwnd)
            print(f"PID={pid.value:6d} | HWND={hwnd} | isHung={is_hung} | Title='{title}'")
    return True

WNDENUMPROC = ctypes.WINFUNCTYPE(ctypes.c_bool, wt.HWND, wt.LPARAM)
user32.EnumWindows(WNDENUMPROC(enum_win), 0)
