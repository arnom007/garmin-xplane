using LOLA.Reactive;
using LauncherCoreLib.Models.Simulated.Airport;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ISimSessionCoordinator
{
	IReactiveEntity<ISimRunnerEvolved> CurrentlyRunning { get; }

	IAircraftSimRunner GetRunnerForAircraft(ReactiveAircraft aircraft);
}
