using System;
using System.Collections.Generic;

namespace DataRouterMatrix;

public static class RouterResolvers
{
	public delegate System.Collections.Generic.IEnumerable<Version> Strategy(VersionRangeSelector<Version> versionSelector, System.Collections.Generic.IEnumerable<Version> routerVersions, System.Collections.Generic.IEnumerable<Version> clientVersions);

	public static System.Collections.Generic.IEnumerable<Version> RoutersThatWorkWithAllClientsFromRouterLookup(VersionRangeSelector<Version> routerBasedLookup, System.Collections.Generic.IEnumerable<Version> routerVersions, System.Collections.Generic.IEnumerable<Version> clientVersions)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (routerBasedLookup == null)
		{
			throw new ArgumentNullException("routerBasedLookup");
		}
		return routerBasedLookup.SelectKeysThatSupportVersions(routerVersions, clientVersions);
	}

	public static System.Collections.Generic.IEnumerable<Version> RoutersThatWorkWithAllClientsFromClientLookup(VersionRangeSelector<Version> clientBasedLookup, System.Collections.Generic.IEnumerable<Version> routerVersions, System.Collections.Generic.IEnumerable<Version> clientVersions)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (clientBasedLookup == null)
		{
			throw new ArgumentNullException("clientBasedLookup");
		}
		return clientBasedLookup.SelectVersionsSupportedByKeys(routerVersions, clientVersions);
	}

	public static System.Collections.Generic.IEnumerable<Version> RoutersThatWorkWithAllClients(DataRouterVersionLookup lookup, System.Collections.Generic.IEnumerable<Version> routerVersions, System.Collections.Generic.IEnumerable<Version> clientVersions)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (lookup == null)
		{
			throw new ArgumentNullException("lookup");
		}
		return ChooseStrategy(lookup.Format)(lookup.Lookup, routerVersions, clientVersions);
	}

	public static Strategy ChooseStrategy(VersionLookupFormat format)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		return format switch
		{
			VersionLookupFormat.RouterBased => RoutersThatWorkWithAllClientsFromRouterLookup, 
			VersionLookupFormat.ClientBased => RoutersThatWorkWithAllClientsFromClientLookup, 
			_ => throw new NotSupportedException(((object)format).ToString()), 
		};
	}
}
