using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LOLA.Processes;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class ShutdownWhenAnySimsExit : IAutomaticShutdownStrategy
{
	public System.Threading.Tasks.Task AutomaticShutdownEvent(ITrainingSession session)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (session == null)
		{
			throw new ArgumentNullException("session");
		}
		return System.Threading.Tasks.Task.WhenAny<ProcessResult>(Enumerable.Select<SimProcessSession, System.Threading.Tasks.Task<ProcessResult>>((System.Collections.Generic.IEnumerable<SimProcessSession>)session.Simulators, (Func<SimProcessSession, System.Threading.Tasks.Task<ProcessResult>>)((SimProcessSession s) => s.Process.Exited)));
	}
}
