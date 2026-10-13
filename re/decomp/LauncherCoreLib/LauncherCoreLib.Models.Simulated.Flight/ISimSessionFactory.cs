using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal interface ISimSessionFactory
{
	System.Threading.Tasks.Task<SimSession> CreateRouterAndSimulators(ISessionEnvironment finalEnvironment);
}
