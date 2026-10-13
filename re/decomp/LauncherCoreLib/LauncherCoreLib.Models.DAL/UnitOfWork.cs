using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;

namespace LauncherCoreLib.Models.DAL;

public class UnitOfWork : IUnitOfWork
{
	[field: CompilerGenerated]
	public ILowLevelDal LowLevel
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public IAircraftRepoEvolved Aircraft
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public ISimulatorRepository Simulators
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public IFamilyDataRepo FamilyData
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public IAircraftHistoryRepoV2 AircraftHistory
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public IPowerStatsRepo PowerStats
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public ITrainerPackageRepoEvolved Packages
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public PackageRepositories PackageRepos
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public IAvionicsRepo Avionics
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public ISettingsRepo Settings
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}
}
