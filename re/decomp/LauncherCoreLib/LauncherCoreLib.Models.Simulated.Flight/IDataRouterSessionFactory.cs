using DataRouterMatrix;
using LOLA.Processes;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IDataRouterSessionFactory
{
	IDataRouterSession CreateSession(IProcessSession routerProcess, DataRouterAssemblyInfo clientInfo);
}
