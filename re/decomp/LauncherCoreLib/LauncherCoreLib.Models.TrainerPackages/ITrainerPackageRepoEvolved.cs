using System;
using System.Threading.Tasks;
using LauncherCoreLib.Utility;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface ITrainerPackageRepoEvolved
{
	System.Threading.Tasks.Task LoadingComplete { get; }

	IObservable<DataChangeNotification<ITrainerPackage>> WhenPackageIsChanged { get; }

	IObservable<ITrainerPackage> GetAllPackages();

	System.Threading.Tasks.Task<PackageInstallationInfo> GetPackageByReleaseId(TrainerEmbeddedReleaseId embeddedReleaseId);

	System.Threading.Tasks.Task<ITrainerPackage?> GetPackageByLocationOrNull(string packageRootDirectory);

	System.Threading.Tasks.Task<ITrainerPackage> RegisterPackage(ITrainerPackage package);

	System.Threading.Tasks.Task<bool> UnRegisterPackage(ITrainerPackage package);
}
