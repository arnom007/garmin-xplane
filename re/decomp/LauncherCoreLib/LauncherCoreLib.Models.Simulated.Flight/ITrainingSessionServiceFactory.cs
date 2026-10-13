namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ITrainingSessionServiceFactory
{
	object CreateService(ITrainingSessionServiceProvider sessionDependencyProvider, ITrainingSession trainingSession);
}
