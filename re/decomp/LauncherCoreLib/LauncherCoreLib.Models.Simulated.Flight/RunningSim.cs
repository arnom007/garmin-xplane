using System;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Flight;

public sealed class RunningSim
{
	[field: CompilerGenerated]
	public InstallationSlot Slot
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public SimProcessSession SimProcess
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public OsWindow Window
	{
		[CompilerGenerated]
		get;
	}

	public RunningSim(SimProcessSession simProcess, OsWindow currentWindow)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (simProcess == null)
		{
			throw new ArgumentNullException("simProcess");
		}
		Slot = simProcess.Simulator.InstallSlot;
		SimProcess = simProcess;
		Window = currentWindow ?? throw new ArgumentNullException("currentWindow");
	}
}
