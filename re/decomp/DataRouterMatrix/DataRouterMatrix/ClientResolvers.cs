using System;
using System.Collections.Generic;
using System.Linq;

namespace DataRouterMatrix;

public static class ClientResolvers
{
	public delegate System.Collections.Generic.IEnumerable<Version> Strategy(VersionRangeSelector<Version> versionSelector, Version routerVersion, System.Collections.Generic.IEnumerable<Version> clientVersions);

	public static System.Collections.Generic.IEnumerable<Version> ClientsThatWorkWithRouterFromRouterLookup(VersionRangeSelector<Version> routerBasedLookup, Version routerVersion, System.Collections.Generic.IEnumerable<Version> clientVersions)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		VersionRangeSelector<Version> routerBasedLookup2 = routerBasedLookup;
		Version routerVersion2 = routerVersion;
		if (routerBasedLookup2 == null)
		{
			throw new ArgumentNullException("routerBasedLookup");
		}
		return Enumerable.Where<Version>(clientVersions, (Func<Version, bool>)((Version cv) => routerBasedLookup2.DoesKeySupportVersion(routerVersion2, cv)));
	}

	public static System.Collections.Generic.IEnumerable<Version> ClientsThatWorkWithRouterFromClientLookup(VersionRangeSelector<Version> clientBasedLookup, Version routerVersion, System.Collections.Generic.IEnumerable<Version> clientVersions)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (clientBasedLookup == null)
		{
			throw new ArgumentNullException("clientBasedLookup");
		}
		return Enumerable.Intersect<Version>(clientBasedLookup.SelectKeysThatSupportVersion(routerVersion), clientVersions);
	}

	public static System.Collections.Generic.IEnumerable<Version> ClientsThatWorkWithRouter(DataRouterVersionLookup lookup, Version routerVersion, System.Collections.Generic.IEnumerable<Version> clientVersions)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (lookup == null)
		{
			throw new ArgumentNullException("lookup");
		}
		return ChooseStrategy(lookup.Format)(lookup.Lookup, routerVersion, clientVersions);
	}

	public static Strategy ChooseStrategy(VersionLookupFormat format)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		return format switch
		{
			VersionLookupFormat.RouterBased => ClientsThatWorkWithRouterFromRouterLookup, 
			VersionLookupFormat.ClientBased => ClientsThatWorkWithRouterFromClientLookup, 
			_ => throw new NotSupportedException(((object)format).ToString()), 
		};
	}
}
