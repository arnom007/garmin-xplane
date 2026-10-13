using System.Threading;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

public abstract class PreflightOnlyWorkflow : ISessionWorkflow
{
	public System.Threading.Tasks.Task<ISessionEnvironment> PostFlight(ISessionEnvironment sessionEnvironment)
	{
		return System.Threading.Tasks.Task.FromResult<ISessionEnvironment>(sessionEnvironment);
	}

	public abstract System.Threading.Tasks.Task<ISessionEnvironment> PreFlight(ISessionEnvironment sessionEnvironment, CancellationToken token);
}
