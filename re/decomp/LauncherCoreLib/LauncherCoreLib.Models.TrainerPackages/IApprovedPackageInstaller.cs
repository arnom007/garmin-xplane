using System;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IApprovedPackageInstaller
{
	IObservable<TrainerInstallationStatus> DownloadAndInstall(IPackageInstallApproval approval);
}
