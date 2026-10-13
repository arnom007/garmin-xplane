using System.Runtime.InteropServices;

namespace RoarClient.Net.Native;

[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
public delegate void ROAR_log_callback([MarshalAs(UnmanagedType.LPStr)] string message, int message_sz, byte is_error);
