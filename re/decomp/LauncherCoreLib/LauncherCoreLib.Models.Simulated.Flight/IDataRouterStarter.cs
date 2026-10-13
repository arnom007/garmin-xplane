using System.Threading.Tasks;
using DataRouterMatrix;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IDataRouterStarter
{
	System.Threading.Tasks.Task<IDataRouterSession> StartRouter(DataRouterAssemblyInfo router, DataRouterAssemblyInfo client);
}
