using System;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IPackageInstallRequester
{
	IObservable<InstallRequestResultRecord> RequestPermissionToDownloadPackages(IPackageInstallRequest request);
}
