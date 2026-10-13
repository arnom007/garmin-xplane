using System.Threading;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IFlightSession
{
	ISessionEnvironment Environment { get; }

	System.Threading.Tasks.Task PreFlight(CancellationToken token);

	System.Threading.Tasks.Task<SimSession> TakeOff();

	System.Threading.Tasks.Task PostFlight();
}
