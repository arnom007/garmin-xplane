using System;
using RoarClient.Net.Native.Adapters;

namespace RoarClient.Net.Native.Discovery;

public static class InitDiscovery
{
	public static IClientInitAdapter GetInitAdapter(IDynamicNativeLibraryLoader library)
	{
		if (InitV2.IsSupportedByLibrary(library))
		{
			return new InitV2Adapter(InitV2.BindToLibrary(library));
		}
		if (InitV1.IsSupportedByLibrary(library))
		{
			return new InitV1Adapter(InitV1.BindToLibrary(library));
		}
		throw new NotSupportedException("This library doesn't have support any ROAR client init interfaces.");
	}
}
