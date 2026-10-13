using System;
using System.Collections.Generic;
using System.Linq;
using DataRouterMatrix;
using LOLA.Extensions;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class SimSessionRouterInstallAdapter : ISimSessionRouterInstallationAdapter
{
	private readonly IDataRouterInstallationResolver _dataRouterInstallationResolver;

	public SimSessionRouterInstallAdapter(IDataRouterInstallationResolver dataRouterInstallationResolver)
	{
		_dataRouterInstallationResolver = dataRouterInstallationResolver;
	}

	public ValueTuple<DataRouterAssemblyInfo, DataRouterAssemblyInfo> ResolveRouterAssembliesForEnvironment(ISessionEnvironment environment)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		Version val = default(Version);
		List<Version> trainerClientVersions = Enumerable.ToList<Version>(Enumerable.Distinct<Version>(Enumerable.Select<string, Version>(Enumerable.Select<KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>, string>((System.Collections.Generic.IEnumerable<KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>>)environment.Inputs.CreationRequestOrThrow().Packages, (Func<KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>, string>)((KeyValuePair<TrainerPackageIdentifier, ITrainerPackage> p) => p.Value.BaseInfo.Manifest.CopyDataFormats()?.DataRouterClientVersion)), (Func<string, Version>)((string clientVersion) => (!Version.TryParse(clientVersion, ref val)) ? null : val)).WhereNotNull()));
		return _dataRouterInstallationResolver.ResolveBestRouterAssemblies((System.Collections.Generic.IEnumerable<Version>)trainerClientVersions);
	}
}
