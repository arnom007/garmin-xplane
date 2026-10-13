using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DataRouterMatrix;

public class DataRouterInstallationCompatabilityChecker : IDataRouterCompatabilityChecker
{
	private readonly IDataRouterVersionLookupProvider _routerResolverProvider;

	public DataRouterInstallationCompatabilityChecker(IDataRouterVersionLookupProvider routerBasedVersionResolverProvider)
	{
		_routerResolverProvider = routerBasedVersionResolverProvider;
	}

	public System.Threading.Tasks.Task<Compatibility> GetClientCompatibility(Version clientVersion)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		DataRouterVersionLookup lookup = _routerResolverProvider.Lookup;
		if (lookup.Format == VersionLookupFormat.RouterBased)
		{
			return System.Threading.Tasks.Task.FromResult<Compatibility>(lookup.Lookup.GetCompatibilityOfVersion(clientVersion));
		}
		if (lookup.Format == VersionLookupFormat.ClientBased)
		{
			return System.Threading.Tasks.Task.FromResult<Compatibility>(GetClientBasedCompatability(clientVersion, (System.Collections.Generic.IEnumerable<Version>)lookup.Lookup.Keys));
		}
		throw new NotSupportedException(((object)lookup.Format).ToString());
		[CompilerGenerated]
		static Compatibility GetClientBasedCompatability(Version clientVersion, System.Collections.Generic.IEnumerable<Version> clientLookupKeys)
		{
			List<Version> val = Enumerable.ToList<Version>((System.Collections.Generic.IEnumerable<Version>)Enumerable.OrderBy<Version, Version>(clientLookupKeys, (Func<Version, Version>)((Version v) => v)));
			if (val.Count == 0)
			{
				return Compatibility.Incompatible;
			}
			if (clientVersion < val[0])
			{
				return Compatibility.TooOld;
			}
			if (clientVersion > val[val.Count - 1])
			{
				return Compatibility.TooNew;
			}
			if (val.Contains(clientVersion))
			{
				return Compatibility.Supported;
			}
			return Compatibility.Incompatible;
		}
	}
}
