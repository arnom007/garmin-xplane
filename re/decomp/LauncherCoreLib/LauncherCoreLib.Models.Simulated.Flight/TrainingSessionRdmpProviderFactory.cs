namespace LauncherCoreLib.Models.Simulated.Flight;

public class TrainingSessionRdmpProviderFactory : ITrainingSessionServiceFactory
{
	public object CreateService(ITrainingSessionServiceProvider sessionDependencyProvider, ITrainingSession trainingSession)
	{
		return new TrainingSessionRdmpProvider(trainingSession);
	}
}
