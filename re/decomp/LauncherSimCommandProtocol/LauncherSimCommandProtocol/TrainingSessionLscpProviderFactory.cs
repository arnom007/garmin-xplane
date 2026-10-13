using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherSimCommandProtocol;

public class TrainingSessionLscpProviderFactory : ITrainingSessionServiceFactory
{
	public object CreateService(ITrainingSessionServiceProvider sessionDependencyProvider, ITrainingSession trainingSession)
	{
		ITrainingSessionLauncherSimCommanderProvider sessionService = sessionDependencyProvider.GetSessionService<ITrainingSessionLauncherSimCommanderProvider>(trainingSession);
		return new TrainingSessionLscpProvider(trainingSession, sessionService);
	}
}
