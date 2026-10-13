using System;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;

namespace LauncherCoreLib.Models.DAL;

public sealed class TemporaryUnitOfWork : IUnitOfWork, System.IDisposable
{
	private readonly TemporaryHangarDirectory _temporaryHangar;

	private readonly SettingsRepo _separateSettingsRepo;

	public IAircraftRepoEvolved Aircraft => _temporaryHangar;

	public ISimulatorRepository Simulators => _temporaryHangar;

	public IFamilyDataRepo FamilyData => _temporaryHangar;

	public IAircraftHistoryRepoV2 AircraftHistory => _temporaryHangar;

	public ISettingsRepo Settings => _separateSettingsRepo;

	[field: CompilerGenerated]
	public IUnitOfWork WrappedUnit
	{
		[CompilerGenerated]
		get;
	}

	public ILowLevelDal LowLevel => WrappedUnit.LowLevel;

	public IPowerStatsRepo PowerStats => WrappedUnit.PowerStats;

	public ITrainerPackageRepoEvolved Packages => WrappedUnit.Packages;

	public PackageRepositories PackageRepos => WrappedUnit.PackageRepos;

	public IAvionicsRepo Avionics => WrappedUnit.Avionics;

	public TemporaryUnitOfWork(IUnitOfWork unitOfWork)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		WrappedUnit = unitOfWork ?? throw new ArgumentNullException("unitOfWork");
		_temporaryHangar = TemporaryHangarDirectory.CreateNewUniqueHangar(unitOfWork.LowLevel);
		_separateSettingsRepo = new SettingsRepo(new GlobalSettingTable(), _temporaryHangar, _temporaryHangar, unitOfWork.Packages, _temporaryHangar);
	}

	public void Dispose()
	{
		_temporaryHangar.Dispose();
		_separateSettingsRepo.Dispose();
	}
}
