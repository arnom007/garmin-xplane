using LauncherCoreLib.Models.Simulated.Panel;
using ReliableDirectMessageProtocol;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ITrainingSessionRdmpProvider
{
	bool DoesSimSupportRdmp(SimInstallKey installKey);

	IReliableDirectMessageCom GetOrCreateRdmpClient(SimInstallKey installKey);
}
