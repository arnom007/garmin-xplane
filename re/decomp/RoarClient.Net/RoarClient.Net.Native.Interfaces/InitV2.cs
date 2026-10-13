using System.Runtime.InteropServices;

namespace RoarClient.Net.Native.Interfaces;

public static class InitV2
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_client_init();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_client_init_callback(ROAR_data_rx_callback on_rx);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate void ROAR_client_close();
}
