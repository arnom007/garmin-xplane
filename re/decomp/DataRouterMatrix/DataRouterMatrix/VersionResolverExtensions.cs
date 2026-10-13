using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DataRouterMatrix;

public static class VersionResolverExtensions
{
	public static ValueTuple<Version, Version> ChooseNewestAssemblies(this IDataRouterVersionResolver versionResolver, System.Collections.Generic.IReadOnlyCollection<Version> allTrainerClientVersions, System.Collections.Generic.IReadOnlyCollection<Version> allRoutersVersions, System.Collections.Generic.IReadOnlyCollection<Version> allLauncherClientVersions)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (versionResolver == null)
		{
			throw new ArgumentNullException("versionResolver");
		}
		if (allTrainerClientVersions == null)
		{
			throw new ArgumentNullException("allTrainerClientVersions");
		}
		if (allRoutersVersions == null)
		{
			throw new ArgumentNullException("allRoutersVersions");
		}
		if (allLauncherClientVersions == null)
		{
			throw new ArgumentNullException("allLauncherClientVersions");
		}
		System.Collections.Generic.IEnumerator<Version> enumerator = ((System.Collections.Generic.IEnumerable<Version>)Enumerable.OrderByDescending<Version, Version>((System.Collections.Generic.IEnumerable<Version>)versionResolver.GetRoutersThatSupportAllClients((System.Collections.Generic.IEnumerable<Version>)allRoutersVersions, (System.Collections.Generic.IEnumerable<Version>)allTrainerClientVersions), (Func<Version, Version>)((Version r) => r))).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				Version current = enumerator.Current;
				if (versionResolver.TryGetNewestClientSupportedByRouter(current, allLauncherClientVersions, out Version newestLauncherClientVersion))
				{
					return new ValueTuple<Version, Version>(current, newestLauncherClientVersion);
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		throw new NotSupportedException(Resources.NoSolutionExists);
	}

	public static bool TryGetNewestClientSupportedByRouter(this IDataRouterVersionResolver versionResolver, Version routerVersion, System.Collections.Generic.IReadOnlyCollection<Version> allLauncherClientVersions, [MaybeNullWhen(false)] out Version newestLauncherClientVersion)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (versionResolver == null)
		{
			throw new ArgumentNullException("versionResolver");
		}
		System.Collections.Generic.ICollection<Version> clientsCompatibleWithRouter = versionResolver.GetClientsCompatibleWithRouter(routerVersion, (System.Collections.Generic.IEnumerable<Version>)allLauncherClientVersions);
		newestLauncherClientVersion = Enumerable.FirstOrDefault<Version>((System.Collections.Generic.IEnumerable<Version>)Enumerable.OrderByDescending<Version, Version>((System.Collections.Generic.IEnumerable<Version>)clientsCompatibleWithRouter, (Func<Version, Version>)((Version v) => v)));
		return newestLauncherClientVersion != (Version)null;
	}
}
