using System.Runtime.InteropServices;

namespace RoarClient.Net.Native.Interfaces;

public static class HandlesV1
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_hndl_is_err(ROAR_hndl hndl);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_hndl_close(ROAR_hndl hndl);
}
