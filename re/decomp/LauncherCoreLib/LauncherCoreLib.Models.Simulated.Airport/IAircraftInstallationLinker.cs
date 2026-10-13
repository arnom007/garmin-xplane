using System;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public interface IAircraftInstallationLinker
{
	System.IDisposable LinkAircraftToPackageInstall(AircraftIdentifier aircraftIdentifier, ITrainerPackage trainerPackage);
}
