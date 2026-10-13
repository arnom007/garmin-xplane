using System;
using LauncherCoreLib.Models.DAL;

namespace LauncherCoreLib.Models.Simulated.Airport;

public interface IReactiveHangar
{
	IUnitOfWork UnitOfWork { get; }

	IObservable<AircraftListData> ListAircraft();

	ReactiveAircraft GetOrCreateAircraftContainer(AircraftListData aircraftListdata);
}
