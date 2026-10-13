using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherSimCommandProtocol;

public interface ITrainingSessionLauncherSimMasterProvider
{
	ILauncherSimMasterCommander GetCommander(ITrainingSession trainingSession);
}
