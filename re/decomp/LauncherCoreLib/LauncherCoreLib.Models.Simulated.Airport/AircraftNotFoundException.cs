using System;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class AircraftNotFoundException : RepoItemNotFoundException
{
	public AircraftNotFoundException()
		: this("No matching aircraft found.")
	{
	}

	public AircraftNotFoundException(string message)
		: base(message)
	{
	}

	public AircraftNotFoundException(string message, System.Exception innerException)
		: base(message, innerException)
	{
	}
}
