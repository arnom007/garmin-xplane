using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LOLA.Processes;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherSimCommandProtocol;

public class TrainingSessionLauncherSimCommanderProviderFactory : ITrainingSessionServiceFactory
{
	public object CreateService(ITrainingSessionServiceProvider sessionDependencyProvider, ITrainingSession trainingSession)
	{
		ITrainingSessionRdmpProvider sessionService = sessionDependencyProvider.GetSessionService<ITrainingSessionRdmpProvider>(trainingSession);
		Dictionary<SimInstallKey, System.Threading.Tasks.Task<ProcessResult>> exitTasks = Enumerable.ToDictionary<SimProcessSession, SimInstallKey, System.Threading.Tasks.Task<ProcessResult>>((System.Collections.Generic.IEnumerable<SimProcessSession>)trainingSession.Simulators, (Func<SimProcessSession, SimInstallKey>)((SimProcessSession sim) => sim.Simulator.InstallData.InstallationKey), (Func<SimProcessSession, System.Threading.Tasks.Task<ProcessResult>>)((SimProcessSession sim) => sim.Process.Exited));
		return new TrainingSessionLauncherSimCommanderProvider(sessionService, (IReadOnlyDictionary<SimInstallKey, System.Threading.Tasks.Task<ProcessResult>>)(object)exitTasks);
	}
}
