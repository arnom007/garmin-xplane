using System;
using System.CodeDom.Compiler;
using RoarClient.Net.Native.Interfaces;

namespace RoarClient.Net.Native;

[GeneratedCode("ClientInterfaces.tt", "0.1.0")]
public sealed class HandlesV1 : INativeLibraryInterface
{
	public RoarClient.Net.Native.Interfaces.HandlesV1.ROAR_hndl_is_err ROAR_hndl_is_err { get; }

	public RoarClient.Net.Native.Interfaces.HandlesV1.ROAR_hndl_close ROAR_hndl_close { get; }

	public HandlesV1(RoarClient.Net.Native.Interfaces.HandlesV1.ROAR_hndl_is_err ROAR_hndl_is_err, RoarClient.Net.Native.Interfaces.HandlesV1.ROAR_hndl_close ROAR_hndl_close)
	{
		this.ROAR_hndl_is_err = ROAR_hndl_is_err ?? throw new ArgumentNullException("ROAR_hndl_is_err");
		this.ROAR_hndl_close = ROAR_hndl_close ?? throw new ArgumentNullException("ROAR_hndl_close");
	}

	public static bool IsSupportedByLibrary(IDynamicNativeLibraryLoader library)
	{
		if (library.SupportsDelegate<RoarClient.Net.Native.Interfaces.HandlesV1.ROAR_hndl_is_err>())
		{
			return library.SupportsDelegate<RoarClient.Net.Native.Interfaces.HandlesV1.ROAR_hndl_close>();
		}
		return false;
	}

	public static HandlesV1 BindToLibrary(IDynamicNativeLibraryLoader library)
	{
		try
		{
			return new HandlesV1(library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.HandlesV1.ROAR_hndl_is_err>(), library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.HandlesV1.ROAR_hndl_close>());
		}
		catch (EntryPointNotFoundException innerException)
		{
			throw new NotSupportedException("The native library does not have all of the functions for this interface.", innerException);
		}
	}
}
