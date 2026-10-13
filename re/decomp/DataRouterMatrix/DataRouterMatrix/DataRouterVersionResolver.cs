using System;
using System.Collections.Generic;
using System.Linq;

namespace DataRouterMatrix;

public class DataRouterVersionResolver : IDataRouterVersionResolver
{
	private readonly IDataRouterVersionLookupProvider _lookupProvider;

	public DataRouterVersionResolver(IDataRouterVersionLookupProvider lookupProvider)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_lookupProvider = lookupProvider ?? throw new ArgumentNullException("lookupProvider");
	}

	public System.Collections.Generic.ICollection<Version> GetClientsCompatibleWithRouter(Version routerVersion, System.Collections.Generic.IEnumerable<Version> clientVersions)
	{
		return (System.Collections.Generic.ICollection<Version>)Enumerable.ToList<Version>(ClientResolvers.ClientsThatWorkWithRouter(_lookupProvider.Lookup, routerVersion, clientVersions));
	}

	public System.Collections.Generic.ICollection<Version> GetRoutersThatSupportAllClients(System.Collections.Generic.IEnumerable<Version> availableRouters, System.Collections.Generic.IEnumerable<Version> clientVersions)
	{
		return (System.Collections.Generic.ICollection<Version>)Enumerable.ToList<Version>(RouterResolvers.RoutersThatWorkWithAllClients(_lookupProvider.Lookup, availableRouters, clientVersions));
	}
}
