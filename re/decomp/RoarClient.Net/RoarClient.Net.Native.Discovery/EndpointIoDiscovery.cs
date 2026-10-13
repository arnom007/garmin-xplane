using RoarClient.Net.Native.Adapters;

namespace RoarClient.Net.Native.Discovery;

public static class EndpointIoDiscovery
{
	public static IEndpointIoAdapter GetIoAdapter(IDynamicNativeLibraryLoader library)
	{
		return new EndpointIoV1Adapter(EndpointIoV1.BindToLibrary(library), HandlesV1.BindToLibrary(library));
	}
}
