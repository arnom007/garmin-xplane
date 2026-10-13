using System;
using System.CodeDom.Compiler;
using RoarClient.Net.Native.Interfaces;

namespace RoarClient.Net.Native;

[GeneratedCode("ClientInterfaces.tt", "0.1.0")]
public sealed class EndpointIoV1 : INativeLibraryInterface
{
	public RoarClient.Net.Native.Interfaces.EndpointIoV1.ROAR_endpt_open ROAR_endpt_open { get; }

	public RoarClient.Net.Native.Interfaces.EndpointIoV1.ROAR_endpt_read ROAR_endpt_read { get; }

	public RoarClient.Net.Native.Interfaces.EndpointIoV1.ROAR_endpt_write ROAR_endpt_write { get; }

	public EndpointIoV1(RoarClient.Net.Native.Interfaces.EndpointIoV1.ROAR_endpt_open ROAR_endpt_open, RoarClient.Net.Native.Interfaces.EndpointIoV1.ROAR_endpt_read ROAR_endpt_read, RoarClient.Net.Native.Interfaces.EndpointIoV1.ROAR_endpt_write ROAR_endpt_write)
	{
		this.ROAR_endpt_open = ROAR_endpt_open ?? throw new ArgumentNullException("ROAR_endpt_open");
		this.ROAR_endpt_read = ROAR_endpt_read ?? throw new ArgumentNullException("ROAR_endpt_read");
		this.ROAR_endpt_write = ROAR_endpt_write ?? throw new ArgumentNullException("ROAR_endpt_write");
	}

	public static bool IsSupportedByLibrary(IDynamicNativeLibraryLoader library)
	{
		if (library.SupportsDelegate<RoarClient.Net.Native.Interfaces.EndpointIoV1.ROAR_endpt_open>() && library.SupportsDelegate<RoarClient.Net.Native.Interfaces.EndpointIoV1.ROAR_endpt_read>())
		{
			return library.SupportsDelegate<RoarClient.Net.Native.Interfaces.EndpointIoV1.ROAR_endpt_write>();
		}
		return false;
	}

	public static EndpointIoV1 BindToLibrary(IDynamicNativeLibraryLoader library)
	{
		try
		{
			return new EndpointIoV1(library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.EndpointIoV1.ROAR_endpt_open>(), library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.EndpointIoV1.ROAR_endpt_read>(), library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.EndpointIoV1.ROAR_endpt_write>());
		}
		catch (EntryPointNotFoundException innerException)
		{
			throw new NotSupportedException("The native library does not have all of the functions for this interface.", innerException);
		}
	}
}
