using System;
using System.Threading.Tasks;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Panel;

public interface IFamilyDataRepo
{
	IObservable<DataChangeNotification<AircraftPackageFamily>> WhenAircraftFamilyDataChanges { get; }

	System.Threading.Tasks.Task<FamilyData> GetFamilyDataForReleaseOrDefault(AircraftPackageFamily aircraftPackageFamily);

	System.Threading.Tasks.Task<FamilyDataLookup> GetAllFamilyDataForAircraft(AircraftIdentifier aircraft);

	System.Threading.Tasks.Task UpdateFamilyDataForRelease(AircraftPackageFamily aircraftPackageFamily, FamilyData data);

	System.Threading.Tasks.Task DeleteFamilyDataForRelease(AircraftPackageFamily aircraftPackageFamily);
}
