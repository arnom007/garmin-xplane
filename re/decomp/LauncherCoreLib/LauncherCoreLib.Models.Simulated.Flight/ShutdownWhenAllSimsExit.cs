using System;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class ShutdownWhenAllSimsExit : IAutomaticShutdownStrategy
{
	public System.Threading.Tasks.Task AutomaticShutdownEvent(ITrainingSession session)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (session == null)
		{
			throw new ArgumentNullException("session");
		}
		return session.AllSimsExited;
	}
}
