using System;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;

namespace LauncherCoreLib.Models.TrainerPackages;

public class PackageRepositories
{
	[field: CompilerGenerated]
	public ITrainerPackageRepoEvolved Packages
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IPackageUpgradeDecisionRepo UpgradeDecisions
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IPackageInstallApprovalCollector DownloadApproval
	{
		[CompilerGenerated]
		get;
	}

	public PackageRepositories(ITrainerPackageRepoEvolved registeredPacks, IPackageUpgradeDecisionRepo upgradeDecisions, IPackageInstallApprovalCollector downloadPermissions)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Packages = registeredPacks ?? throw new ArgumentNullException("registeredPacks");
		UpgradeDecisions = upgradeDecisions ?? throw new ArgumentNullException("upgradeDecisions");
		DownloadApproval = downloadPermissions ?? throw new ArgumentNullException("downloadPermissions");
	}
}
