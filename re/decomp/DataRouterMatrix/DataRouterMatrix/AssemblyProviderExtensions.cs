using System;
using System.Collections.Generic;
using System.Linq;

namespace DataRouterMatrix;

public static class AssemblyProviderExtensions
{
	public static DataRouterAssemblyInfo GetByVersion(this IDataRouterAssemblyProvider provider, Version nativeVersion)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Version nativeVersion2 = nativeVersion;
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		return Enumerable.FirstOrDefault<DataRouterAssemblyInfo>(provider.GetAll(), (Func<DataRouterAssemblyInfo, bool>)((DataRouterAssemblyInfo a) => a.NativeVersion == nativeVersion2)) ?? throw new KeyNotFoundException($"No assembly found with version: {nativeVersion2}");
	}
}
