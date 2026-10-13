using RoarClient.Net.Native.Adapters;

namespace RoarClient.Net.Native.Discovery;

public static class ErrorLookupDiscovery
{
	public static IHumanErrorLookup GetErrorAdapter(IDynamicNativeLibraryLoader library)
	{
		if (HumanErrorLookupV1.IsSupportedByLibrary(library))
		{
			return new NativeErrorLookup(HumanErrorLookupV1.BindToLibrary(library));
		}
		return new NoNativeErrors();
	}
}
