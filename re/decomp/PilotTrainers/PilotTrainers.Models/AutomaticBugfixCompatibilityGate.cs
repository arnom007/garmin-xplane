using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureMatrix;
using LOLA.Extensions;
using LauncherCoreLib.Models.TrainerPackages;
using LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;
using LauncherFeatureMatrix;
using TrainerPackageLib;

namespace PilotTrainers.Models;

public class AutomaticBugfixCompatibilityGate : IPackageReferenceFilter
{
	private readonly IPackageFeatureCompatibilityResolverProvider _resolverProvider;

	public AutomaticBugfixCompatibilityGate(IPackageFeatureCompatibilityResolverProvider resolverProvider)
	{
		_resolverProvider = resolverProvider;
	}

	public async Task<PackageReferenceUpgradeInfo?> RemoveUnsupportedPackages(PackageReferenceUpgradeInfo packageReference)
	{
		IPackageFeatureCompatibilityResolver resolver = await _resolverProvider.LoadResolver().ConfigureAwait(continueOnCapturedContext: false);
		List<ITrainerPackage> list = (from t in await Task.WhenAll(packageReference.InstallInfo.Packages.Select(async (ITrainerPackage p) => (p: p, compatibility: await resolver.GetPackageFeatureCompatability(p).ConfigureAwait(continueOnCapturedContext: false)))).ConfigureAwait(continueOnCapturedContext: false)
			where t.compatibility == AggFeatureCompatibility.Supported
			select t.p).ToList();
		if (list.SetEquals(packageReference.InstallInfo.Packages))
		{
			return packageReference;
		}
		if (list.Count == 0)
		{
			return null;
		}
		return new PackageReferenceUpgradeInfo(packageReference.CurrentId, new PackageInstallationInfo(list, MissingCatalogReason.Discontinued, packageReference.InstallInfo.LatestListingInfo));
	}
}
