using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherSimCommandProtocol;

public class TrainingSessionLscpProvider : ITrainingSessionLauncherSimMasterProvider
{
	private readonly ILauncherSimMasterCommander _masterCommander;

	public TrainingSessionLscpProvider(ITrainingSession session, ITrainingSessionLauncherSimCommanderProvider commanderProvider)
	{
		_masterCommander = new LauncherSimMasterCommander(session, commanderProvider);
	}

	public ILauncherSimMasterCommander GetCommander(ITrainingSession trainingSession)
	{
		return _masterCommander;
	}
}
