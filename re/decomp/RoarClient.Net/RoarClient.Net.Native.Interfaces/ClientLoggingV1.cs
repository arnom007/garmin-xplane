using System.Runtime.InteropServices;

namespace RoarClient.Net.Native.Interfaces;

public static class ClientLoggingV1
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_adv_callback_register_log(ROAR_log_callback? on_log);
}
