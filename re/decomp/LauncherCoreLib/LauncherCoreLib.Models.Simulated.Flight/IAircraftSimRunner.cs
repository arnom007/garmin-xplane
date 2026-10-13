using LauncherCoreLib.Models.Simulated.Airport;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IAircraftSimRunner : ISimRunnerEvolved
{
	ReactiveAircraft Aircraft { get; }
}
