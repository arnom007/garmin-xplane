using System;
using System.Collections.Generic;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public interface IAircraftLinkedInstallations
{
	IObservable<ISet<ITrainerPackage>> CurrentPackageInstallsLinkedToAircraft(AircraftIdentifier aircraftIdentifier);
}
