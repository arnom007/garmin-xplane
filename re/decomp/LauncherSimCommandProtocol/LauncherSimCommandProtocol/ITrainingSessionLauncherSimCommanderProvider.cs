using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherSimCommandProtocol;

public interface ITrainingSessionLauncherSimCommanderProvider
{
	bool DoesSimSupportLscp(SimInstallKey installKey);

	ILauncherSimCommander GetOrCreateSimCommander(SimInstallKey installKey);
}
