using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal interface IFlightSessionCounter
{
	System.Threading.Tasks.Task<int> GetSessionCount();

	System.Threading.Tasks.Task<int> GetAndIncrementSessionCount();
}
