using System;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class SimulatorNotFoundException : RepoItemNotFoundException
{
	public SimulatorNotFoundException()
		: this("No matching simulator found.")
	{
	}

	public SimulatorNotFoundException(string message)
		: base(message)
	{
	}

	public SimulatorNotFoundException(string message, System.Exception innerException)
		: base(message, innerException)
	{
	}
}
