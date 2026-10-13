using System;
using LauncherCoreLib.Models.Simulated.Airport;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IAirworthiness
{
	IObservable<bool> IsAirworthyOverTime(ReactiveAircraft aircraft);
}
