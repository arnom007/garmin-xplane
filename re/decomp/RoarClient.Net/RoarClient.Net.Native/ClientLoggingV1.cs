using System;
using System.CodeDom.Compiler;
using RoarClient.Net.Native.Interfaces;

namespace RoarClient.Net.Native;

[GeneratedCode("ClientInterfaces.tt", "0.1.0")]
public sealed class ClientLoggingV1 : INativeLibraryInterface
{
	public RoarClient.Net.Native.Interfaces.ClientLoggingV1.ROAR_adv_callback_register_log ROAR_adv_callback_register_log { get; }

	public ClientLoggingV1(RoarClient.Net.Native.Interfaces.ClientLoggingV1.ROAR_adv_callback_register_log ROAR_adv_callback_register_log)
	{
		this.ROAR_adv_callback_register_log = ROAR_adv_callback_register_log ?? throw new ArgumentNullException("ROAR_adv_callback_register_log");
	}

	public static bool IsSupportedByLibrary(IDynamicNativeLibraryLoader library)
	{
		return library.SupportsDelegate<RoarClient.Net.Native.Interfaces.ClientLoggingV1.ROAR_adv_callback_register_log>();
	}

	public static ClientLoggingV1 BindToLibrary(IDynamicNativeLibraryLoader library)
	{
		try
		{
			return new ClientLoggingV1(library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.ClientLoggingV1.ROAR_adv_callback_register_log>());
		}
		catch (EntryPointNotFoundException innerException)
		{
			throw new NotSupportedException("The native library does not have all of the functions for this interface.", innerException);
		}
	}
}
