using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IPackageInstallApprovalCollector
{
	void GrantPermissionToDownloadPackage(TrainerPackageIdentifier trainerPackage);

	void RevokePermissionToDownloadPackage(TrainerPackageIdentifier trainerPackage);
}
