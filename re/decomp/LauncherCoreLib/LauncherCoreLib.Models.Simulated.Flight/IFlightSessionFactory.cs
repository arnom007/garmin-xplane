using System.Threading;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IFlightSessionFactory
{
	System.Threading.Tasks.Task<IFlightSession> CreateSession(FlightSessionRequestInfo sessionRequest, CancellationToken token = default(CancellationToken));
}
