using System;
using System.CodeDom.Compiler;
using RoarClient.Net.Native.Interfaces;

namespace RoarClient.Net.Native;

[GeneratedCode("ClientInterfaces.tt", "0.1.0")]
public sealed class InitV2 : INativeLibraryInterface
{
	public RoarClient.Net.Native.Interfaces.InitV2.ROAR_client_init ROAR_client_init { get; }

	public RoarClient.Net.Native.Interfaces.InitV2.ROAR_client_init_callback ROAR_client_init_callback { get; }

	public RoarClient.Net.Native.Interfaces.InitV2.ROAR_client_close ROAR_client_close { get; }

	public InitV2(RoarClient.Net.Native.Interfaces.InitV2.ROAR_client_init ROAR_client_init, RoarClient.Net.Native.Interfaces.InitV2.ROAR_client_init_callback ROAR_client_init_callback, RoarClient.Net.Native.Interfaces.InitV2.ROAR_client_close ROAR_client_close)
	{
		this.ROAR_client_init = ROAR_client_init ?? throw new ArgumentNullException("ROAR_client_init");
		this.ROAR_client_init_callback = ROAR_client_init_callback ?? throw new ArgumentNullException("ROAR_client_init_callback");
		this.ROAR_client_close = ROAR_client_close ?? throw new ArgumentNullException("ROAR_client_close");
	}

	public static bool IsSupportedByLibrary(IDynamicNativeLibraryLoader library)
	{
		if (library.SupportsDelegate<RoarClient.Net.Native.Interfaces.InitV2.ROAR_client_init>() && library.SupportsDelegate<RoarClient.Net.Native.Interfaces.InitV2.ROAR_client_init_callback>())
		{
			return library.SupportsDelegate<RoarClient.Net.Native.Interfaces.InitV2.ROAR_client_close>();
		}
		return false;
	}

	public static InitV2 BindToLibrary(IDynamicNativeLibraryLoader library)
	{
		try
		{
			return new InitV2(library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.InitV2.ROAR_client_init>(), library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.InitV2.ROAR_client_init_callback>(), library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.InitV2.ROAR_client_close>());
		}
		catch (EntryPointNotFoundException innerException)
		{
			throw new NotSupportedException("The native library does not have all of the functions for this interface.", innerException);
		}
	}
}
