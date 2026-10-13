using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Airport;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class FlightSessionRequestInfo
{
	[field: CompilerGenerated]
	public AircraftSnapshotData AircraftSnapShot
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IReadOnlyDictionary<TrainerPackageIdentifier, ITrainerPackage> Packages
	{
		[CompilerGenerated]
		get;
	}

	public FlightSessionRequestInfo(AircraftSnapshotData snapshot, IReadOnlyDictionary<TrainerPackageIdentifier, ITrainerPackage> packages)
	{
		AircraftSnapShot = snapshot;
		Packages = packages;
	}
}
