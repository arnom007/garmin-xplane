namespace LauncherCoreLib.Models.Simulated.Flight;

public static class TrainingSessionServiceProviderExtensions
{
	public static T GetSessionService<T>(this ITrainingSessionServiceProvider provider, ITrainingSession trainingSession)
	{
		return (T)provider.GetSessionService(typeof(T), trainingSession);
	}
}
