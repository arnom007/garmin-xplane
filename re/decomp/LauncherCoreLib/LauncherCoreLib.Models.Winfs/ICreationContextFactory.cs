using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherCoreLib.Models.Winfs;

internal interface ICreationContextFactory
{
	IWinfsCreationContext CreateContext(ISessionInfo session, TrainerSessionData trainerSessionData);
}
