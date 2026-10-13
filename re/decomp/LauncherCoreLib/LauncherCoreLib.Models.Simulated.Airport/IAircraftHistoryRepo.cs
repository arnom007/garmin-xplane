using System;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Airport;

public interface IAircraftHistoryRepo
{
	IObservable<AircraftSnapshotInfo> ListHistoricalSnapshots(AircraftIdentifier aircraftIdentifier);

	System.Threading.Tasks.Task<AircraftSnapshotInfo> CreateSnapshot(AircraftIdentifier aircraftIdentifier, AircraftSnapshotMetaData metadata);

	System.Threading.Tasks.Task<TemporaryHangarDirectory> MountHistoricalSnapshot(AircraftIdentifier aircraftIdentifier, SnapshotIdentifier snapshotIdentifier);
}
