using System;
using System.Threading.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Airport;

public interface IAircraftRepoEvolved : IFlushable
{
	IObservable<AircraftIdentifier> WhenSelectedAircraftChanges { get; }

	IObservable<DataChangeNotification<AircraftIdentifier>> AircraftChanges { get; }

	IObservable<AircraftIdentifier> GetAllAircraftIds();

	IObservable<AircraftListData> GetAllAircraftListData();

	System.Threading.Tasks.Task<AircraftListData> GetAircraftListDataById(AircraftIdentifier aircraftIdentifier);

	System.Threading.Tasks.Task<AircraftHeavyData> GetAircraftHeavyDataById(AircraftIdentifier aircraftIdentifier);

	System.Threading.Tasks.Task<AircraftSnapshotData> GetAircraftSnapshotById(AircraftIdentifier aircraftIdentifier);

	System.Threading.Tasks.Task<AircraftSnapshotData> CreateNewAircraft(AircraftListData startingInfo);

	System.Threading.Tasks.Task<AircraftSnapshotData> CopyAircraft(AircraftListData startingInfo, AircraftIdentifier copyIdentifier);

	System.Threading.Tasks.Task UpdateAircraftListData(AircraftListData updatedListData);

	System.Threading.Tasks.Task UpdateAircraftHeavyData(AircraftHeavyData heavyData);

	System.Threading.Tasks.Task UpdateAircraft(AircraftSnapshotData aircraftSnapshot);

	System.Threading.Tasks.Task DeleteAircraft(AircraftIdentifier aircraftIdentifier);

	System.Threading.Tasks.Task<AircraftIdentifier> GetLastSelectedAircraft();

	System.Threading.Tasks.Task StoreLastSelectedAircraft(AircraftIdentifier aircraftIdentifier);

	System.Threading.Tasks.Task ExportAircraft(AircraftIdentifier aircraftIdentifier, string gcaPath);

	System.Threading.Tasks.Task<AircraftSnapshotData> ParseAircraftGCA(string gcaPath);

	System.Threading.Tasks.Task<AircraftSnapshotData> ImportAircaft(AircraftListData newListData, string gcaPath);
}
