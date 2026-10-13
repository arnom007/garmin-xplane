using System;
using System.Reactive.Linq;

namespace LauncherCoreLib.Models.Simulated.Airport;

public static class ReactiveHangarExtensions
{
	public static IObservable<ReactiveAircraft?> ConvertListDataToAircraft(this IObservable<AircraftListData?> listData, IReactiveHangar hangar)
	{
		IReactiveHangar hangar2 = hangar;
		return listData.Select<AircraftListData, ReactiveAircraft>((Func<AircraftListData, ReactiveAircraft>)((AircraftListData ld) => (ld == null) ? null : hangar2.GetOrCreateAircraftContainer(ld)));
	}
}
