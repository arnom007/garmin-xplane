using System.Threading;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ISessionWorkflow
{
	System.Threading.Tasks.Task<ISessionEnvironment> PreFlight(ISessionEnvironment sessionEnvironment, CancellationToken token);

	System.Threading.Tasks.Task<ISessionEnvironment> PostFlight(ISessionEnvironment sessionEnvironment);
}
