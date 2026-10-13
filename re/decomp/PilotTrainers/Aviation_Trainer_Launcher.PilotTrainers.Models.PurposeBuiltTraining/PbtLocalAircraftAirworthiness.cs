using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Aviation_Trainer_Launcher.LauncherAdvisories;
using Domain.PurposeBuiltTraining;
using Domain.UtilTypes;
using LOLA.Logging;
using LOLA.Reactive;
using LanguageExt;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public class PbtLocalAircraftAirworthiness : IAirworthiness
{
	private readonly AdvisoryUnitOfWork<PbtAdvisoryContext> _aircraftAdvisorySourceProvider;

	private readonly PbtLocalAircraftRecognizer _localAircraftRecognizer;

	private readonly PbtReduxStore _reduxStore;

	private readonly LocalAircraftIdentifierLookup _localLookup;

	public PbtLocalAircraftAirworthiness(AdvisoryUnitOfWork<PbtAdvisoryContext> aircraftAdvisorySourceProvider, PbtLocalAircraftRecognizer localAircraftRecognizer, PbtReduxStore reduxStore, LocalAircraftIdentifierLookup localLookup)
	{
		_aircraftAdvisorySourceProvider = aircraftAdvisorySourceProvider;
		_localAircraftRecognizer = localAircraftRecognizer;
		_reduxStore = reduxStore;
		_localLookup = localLookup;
	}

	public IObservable<bool> IsAirworthyOverTime(ReactiveAircraft aircraft)
	{
		ReactiveAircraft aircraft2 = aircraft;
		if (aircraft2 == null)
		{
			return Observable.Return(value: false);
		}
		IObservable<PbtAdvisoryContext> source = (from o in aircraft2.CompositeSnapshots.GetCachedThenUpdatedAndThenContinualValues().CombineLatest(_reduxStore.StateOverTime, (AircraftSnapshotData snap, PbtPageState state) => _localAircraftRecognizer(snap).Bind(delegate(PbtDataStoredInLocalAircraft d)
			{
				PbtDataStoredInLocalAircraft d2 = d;
				return state.ListState.AircraftState.Lookup.AsEnumerable().Find<(TrainingItemKey, PbtAircraftState)>(((TrainingItemKey Key, PbtAircraftState Value) kv) => _localLookup(kv.Value.AircraftDescriptor) == d2.LocalId);
			})).DistinctUntilChanged()
			select o.Map(((TrainingItemKey Key, PbtAircraftState Value) t) => new PbtAdvisoryContext(t.Key, t.Value, aircraft2))).Somes();
		return Observable.Return(value: true).Concat(source.Select((PbtAdvisoryContext c) => from mostRecentAdvisories in _aircraftAdvisorySourceProvider.Advisories.GetForContext(c).DetectedAdvisoriesOverTime().Scan(ImmutableDictionary<string, IAdvisoryInstance>.Empty, (ImmutableDictionary<string, IAdvisoryInstance> allAdvisories, IAdvisoryInstance newAdvisory) => allAdvisories.SetItem(newAdvisory.InstanceId.Id, newAdvisory))
			select !mostRecentAdvisories.Values.Any((IAdvisoryInstance i) => i.CheckState != SelfCheckState.WithinParameters && i.Severity == AdvisorySeverity.Error)).Switch()).CatchAndComplete(delegate(Exception e)
		{
			Logger.LogFormattedException(e, "PBT Airworthiness checks have been disabled due to an advisory error.");
		})
			.Append(value: false)
			.DistinctUntilChanged();
	}
}
