using System.Runtime.InteropServices;

namespace RoarClient.Net.Native.Interfaces;

public static class InitV1
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_client_init(ROAR_data_rx_callback? on_rx = null);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate void ROAR_client_close();
}
