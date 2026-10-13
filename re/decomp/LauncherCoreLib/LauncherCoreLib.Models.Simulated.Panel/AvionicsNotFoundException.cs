using System;
using LauncherCoreLib.Models.Simulated.Airport;

namespace LauncherCoreLib.Models.Simulated.Panel;

public class AvionicsNotFoundException : RepoItemNotFoundException
{
	public AvionicsNotFoundException(string message)
		: base(message)
	{
	}

	public AvionicsNotFoundException(string message, System.Exception innerException)
		: base(message, innerException)
	{
	}

	public AvionicsNotFoundException()
		: this("No matching avionics were found.")
	{
	}
}
