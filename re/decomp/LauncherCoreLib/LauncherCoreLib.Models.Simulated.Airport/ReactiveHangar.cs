using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.DAL;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class ReactiveHangar : IReactiveHangar
{
	private readonly ConcurrentDictionary<AircraftIdentifier, ReactiveAircraft> _reactiveAircraftCache = new ConcurrentDictionary<AircraftIdentifier, ReactiveAircraft>();

	[field: CompilerGenerated]
	public IUnitOfWork UnitOfWork
	{
		[CompilerGenerated]
		get;
	}

	public ReactiveHangar(IUnitOfWork unitOfWork)
	{
		UnitOfWork = unitOfWork;
	}

	public ReactiveAircraft GetOrCreateAircraftContainer(AircraftListData aircraftListdata)
	{
		AircraftListData aircraftListdata2 = aircraftListdata;
		return _reactiveAircraftCache.GetOrAdd(aircraftListdata2.AircraftIdentifier, (Func<AircraftIdentifier, ReactiveAircraft>)((AircraftIdentifier ai) => new ReactiveAircraft(UnitOfWork, AircraftSnapshotData.FromListData(aircraftListdata2))));
	}

	public IObservable<AircraftListData> ListAircraft()
	{
		return UnitOfWork.Aircraft.GetAllAircraftListData();
	}
}
