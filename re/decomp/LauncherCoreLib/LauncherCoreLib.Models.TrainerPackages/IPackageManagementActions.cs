using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IPackageManagementActions
{
	void StartPackageInstall(ITrainerPackage package);

	void StartPackageUninstall(ITrainerPackage package);
}
