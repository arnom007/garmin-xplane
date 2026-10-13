using System;
using System.Threading.Tasks;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Airport;

public interface IAircraftHistoryRepoV2 : IAircraftHistoryRepo
{
	IObservable<DataChangeNotification<SnapshotIdentifier>> WhenSnapshotChanges(AircraftIdentifier aircraftIdentifier);

	System.Threading.Tasks.Task DeleteSnapshot(AircraftIdentifier aircraftIdentifier, SnapshotIdentifier snapshotIdentifier);

	System.Threading.Tasks.Task RevertAircraftToSnapshot(AircraftIdentifier aircraftIdentifier, SnapshotIdentifier snapshotIdentifier);
}
