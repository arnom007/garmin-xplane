using System.Runtime.CompilerServices;
using LOLA.Processes;

namespace LauncherCoreLib.Models.Simulated.Flight;

public sealed class SimProcessSession
{
	[field: CompilerGenerated]
	public TrainerSessionData Simulator
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IProcessSession Process
	{
		[CompilerGenerated]
		get;
	}

	public SimProcessSession(TrainerSessionData sim, IProcessSession process)
	{
		Simulator = sim;
		Process = process;
	}
}
