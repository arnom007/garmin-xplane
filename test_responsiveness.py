import ctypes
import ctypes.wintypes as wt

user32 = ctypes.windll.user32

def enum_win(hwnd, extra):
    pid = wt.DWORD()
    tid = user32.GetWindowThreadProcessId(hwnd, ctypes.byref(pid))
    if pid.value == 22540:
        length = user32.GetWindowTextLengthW(hwnd)
        buf = ctypes.create_unicode_buffer(length + 1)
        user32.GetWindowTextW(hwnd, buf, length + 1)
        
        # Test responsiveness with SendMessageTimeout
        result = wt.DWORD()
        # WM_NULL = 0x0000, SMTO_ABORTIFHUNG = 0x0002, timeout = 1000ms
        res = user32.SendMessageTimeoutW(hwnd, 0x0000, 0, 0, 0x0002, 1000, ctypes.byref(result))
        is_hung = user32.IsHungAppWindow(hwnd)
        print(f"HWND {hwnd} '{buf.value}': SendMessage={bool(res)}, isHung={bool(is_hung)}")
    return True

WNDENUMPROC = ctypes.WINFUNCTYPE(ctypes.c_bool, wt.HWND, wt.LPARAM)
user32.EnumWindows(WNDENUMPROC(enum_win), 0)
