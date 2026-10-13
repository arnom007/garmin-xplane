using System;
using System.CodeDom.Compiler;
using RoarClient.Net.Native.Interfaces;

namespace RoarClient.Net.Native;

[GeneratedCode("ClientInterfaces.tt", "0.1.0")]
public sealed class HumanErrorLookupV1 : INativeLibraryInterface
{
	public RoarClient.Net.Native.Interfaces.HumanErrorLookupV1.ROAR_err_str_get ROAR_err_str_get { get; }

	public HumanErrorLookupV1(RoarClient.Net.Native.Interfaces.HumanErrorLookupV1.ROAR_err_str_get ROAR_err_str_get)
	{
		this.ROAR_err_str_get = ROAR_err_str_get ?? throw new ArgumentNullException("ROAR_err_str_get");
	}

	public static bool IsSupportedByLibrary(IDynamicNativeLibraryLoader library)
	{
		return library.SupportsDelegate<RoarClient.Net.Native.Interfaces.HumanErrorLookupV1.ROAR_err_str_get>();
	}

	public static HumanErrorLookupV1 BindToLibrary(IDynamicNativeLibraryLoader library)
	{
		try
		{
			return new HumanErrorLookupV1(library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.HumanErrorLookupV1.ROAR_err_str_get>());
		}
		catch (EntryPointNotFoundException innerException)
		{
			throw new NotSupportedException("The native library does not have all of the functions for this interface.", innerException);
		}
	}
}
