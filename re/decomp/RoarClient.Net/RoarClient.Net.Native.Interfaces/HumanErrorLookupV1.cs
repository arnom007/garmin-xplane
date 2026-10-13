using System;
using System.Runtime.InteropServices;

namespace RoarClient.Net.Native.Interfaces;

public static class HumanErrorLookupV1
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate IntPtr ROAR_err_str_get(ROAR_err_type error);
}
