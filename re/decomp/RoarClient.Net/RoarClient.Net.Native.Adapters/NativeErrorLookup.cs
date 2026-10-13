using System;
using System.Runtime.InteropServices;

namespace RoarClient.Net.Native.Adapters;

public class NativeErrorLookup : IHumanErrorLookup
{
	private readonly HumanErrorLookupV1 _nativeLookup;

	public NativeErrorLookup(HumanErrorLookupV1 nativeLookup)
	{
		_nativeLookup = nativeLookup ?? throw new ArgumentNullException("nativeLookup");
	}

	public string GetHumanReadableError(ROAR_err_type roarError)
	{
		IntPtr intPtr = _nativeLookup.ROAR_err_str_get(roarError);
		if (intPtr == IntPtr.Zero)
		{
			return string.Empty;
		}
		return Marshal.PtrToStringAnsi(intPtr);
	}
}
