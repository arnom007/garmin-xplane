using System;
using System.Collections.Generic;

namespace DataRouterMatrix;

public interface IDataRouterVersionResolver
{
	System.Collections.Generic.ICollection<Version> GetRoutersThatSupportAllClients(System.Collections.Generic.IEnumerable<Version> availableRouters, System.Collections.Generic.IEnumerable<Version> clientVersions);

	System.Collections.Generic.ICollection<Version> GetClientsCompatibleWithRouter(Version routerVersion, System.Collections.Generic.IEnumerable<Version> clientVersions);
}
