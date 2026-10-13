using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;

namespace LauncherCoreLib.Models.DAL;

public interface IUnitOfWork
{
	ILowLevelDal LowLevel { get; }

	IAircraftRepoEvolved Aircraft { get; }

	ISimulatorRepository Simulators { get; }

	IFamilyDataRepo FamilyData { get; }

	IAircraftHistoryRepoV2 AircraftHistory { get; }

	IPowerStatsRepo PowerStats { get; }

	ITrainerPackageRepoEvolved Packages { get; }

	PackageRepositories PackageRepos { get; }

	IAvionicsRepo Avionics { get; }

	ISettingsRepo Settings { get; }
}
