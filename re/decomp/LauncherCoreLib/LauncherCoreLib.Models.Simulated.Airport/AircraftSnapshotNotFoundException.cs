using System;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class AircraftSnapshotNotFoundException : RepoItemNotFoundException
{
	public AircraftSnapshotNotFoundException(string message)
		: base(message)
	{
	}

	public AircraftSnapshotNotFoundException(string message, System.Exception innerException)
		: base(message, innerException)
	{
	}
}
