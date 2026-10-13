using System;
using System.Threading.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class AircraftRepoWrapper : IAircraftRepoEvolved, IFlushable
{
	private readonly IAircraftRepoEvolved _actualRepo;

	public virtual IObservable<AircraftIdentifier> WhenSelectedAircraftChanges => _actualRepo.WhenSelectedAircraftChanges;

	public virtual IObservable<DataChangeNotification<AircraftIdentifier>> AircraftChanges => _actualRepo.AircraftChanges;

	public AircraftRepoWrapper(IAircraftRepoEvolved repo)
	{
		_actualRepo = repo;
	}

	public virtual System.Threading.Tasks.Task<AircraftSnapshotData> CopyAircraft(AircraftListData startingInfo, AircraftIdentifier copyIdentifier)
	{
		return _actualRepo.CopyAircraft(startingInfo, copyIdentifier);
	}

	public virtual System.Threading.Tasks.Task<AircraftSnapshotData> CreateNewAircraft(AircraftListData startingInfo)
	{
		return _actualRepo.CreateNewAircraft(startingInfo);
	}

	public virtual System.Threading.Tasks.Task DeleteAircraft(AircraftIdentifier aircraftIdentifier)
	{
		return _actualRepo.DeleteAircraft(aircraftIdentifier);
	}

	public virtual System.Threading.Tasks.Task ExportAircraft(AircraftIdentifier aircraftIdentifier, string gcaPath)
	{
		return _actualRepo.ExportAircraft(aircraftIdentifier, gcaPath);
	}

	public virtual System.Threading.Tasks.Task Flush()
	{
		return _actualRepo.Flush();
	}

	public virtual System.Threading.Tasks.Task<AircraftHeavyData> GetAircraftHeavyDataById(AircraftIdentifier aircraftIdentifier)
	{
		return _actualRepo.GetAircraftHeavyDataById(aircraftIdentifier);
	}

	public virtual System.Threading.Tasks.Task<AircraftListData> GetAircraftListDataById(AircraftIdentifier aircraftIdentifier)
	{
		return _actualRepo.GetAircraftListDataById(aircraftIdentifier);
	}

	public virtual System.Threading.Tasks.Task<AircraftSnapshotData> GetAircraftSnapshotById(AircraftIdentifier aircraftIdentifier)
	{
		return _actualRepo.GetAircraftSnapshotById(aircraftIdentifier);
	}

	public IObservable<AircraftIdentifier> GetAllAircraftIds()
	{
		return _actualRepo.GetAllAircraftIds();
	}

	public virtual IObservable<AircraftListData> GetAllAircraftListData()
	{
		return _actualRepo.GetAllAircraftListData();
	}

	public virtual System.Threading.Tasks.Task<AircraftIdentifier> GetLastSelectedAircraft()
	{
		return _actualRepo.GetLastSelectedAircraft();
	}

	public virtual System.Threading.Tasks.Task<AircraftSnapshotData> ImportAircaft(AircraftListData newListData, string gcaPath)
	{
		return _actualRepo.ImportAircaft(newListData, gcaPath);
	}

	public virtual System.Threading.Tasks.Task<AircraftSnapshotData> ParseAircraftGCA(string gcaPath)
	{
		return _actualRepo.ParseAircraftGCA(gcaPath);
	}

	public virtual System.Threading.Tasks.Task StoreLastSelectedAircraft(AircraftIdentifier aircraftIdentifier)
	{
		return _actualRepo.StoreLastSelectedAircraft(aircraftIdentifier);
	}

	public virtual System.Threading.Tasks.Task UpdateAircraft(AircraftSnapshotData aircraftSnapshot)
	{
		return _actualRepo.UpdateAircraft(aircraftSnapshot);
	}

	public virtual System.Threading.Tasks.Task UpdateAircraftHeavyData(AircraftHeavyData heavyData)
	{
		return _actualRepo.UpdateAircraftHeavyData(heavyData);
	}

	public virtual System.Threading.Tasks.Task UpdateAircraftListData(AircraftListData updatedListData)
	{
		return _actualRepo.UpdateAircraftListData(updatedListData);
	}
}
