using System;

namespace RoarClient.Net.Native;

public interface IDynamicNativeLibraryLoader : IDisposable
{
	string LibraryName { get; }

	bool HasFunctionName(string functionName);

	Delegate GetFunctionDelegate(string functionName, Type delegateType);
}
