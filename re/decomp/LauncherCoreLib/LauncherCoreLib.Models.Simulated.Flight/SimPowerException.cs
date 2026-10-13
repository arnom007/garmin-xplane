using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class SimPowerException : InvalidOperationException
{
	[field: CompilerGenerated]
	public PowerStage Stage
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		init;
	}

	public SimPowerException(string message)
		: base(message)
	{
	}

	public SimPowerException(string message, System.Exception innerException)
		: base(message, innerException)
	{
	}
}
