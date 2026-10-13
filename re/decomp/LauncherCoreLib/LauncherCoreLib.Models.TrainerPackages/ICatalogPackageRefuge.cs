using System;
using System.Threading.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface ICatalogPackageRefuge
{
	System.Threading.Tasks.Task<IRescuedCatalogPackage?> GetRescuedPackageByGtiInfoOrNull(string gtiAppName, Version appCatalogedVersion);

	System.Threading.Tasks.Task<IRescuedCatalogPackage?> GetRescuedPackageByPathOrNull(string packagePath);

	IObservable<IRescuedCatalogPackage> ListRescuedPackages();

	System.Threading.Tasks.Task<IRescuedCatalogPackage> RescuePackage(Version catalogVersion, string gtiAppName, Version appCatalogedVersion, string catalogDirPath);

	System.Threading.Tasks.Task<IRescuedCatalogPackage> RecordAnotherCatalogVersion(IRescuedCatalogPackage rescuedPackage, Version catalogVersion);

	void MarkDeadPackage(PotentialPackageDirectory deadPackage);
}
