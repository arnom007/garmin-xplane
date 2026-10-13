using System;
using System.Collections.Generic;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public interface IAircraftPackageInstallationProvider
{
	IObservable<System.Collections.Generic.IReadOnlyCollection<ITrainerPackage>> GetRelevantPackages(ReactiveAircraft aircraft);
}
