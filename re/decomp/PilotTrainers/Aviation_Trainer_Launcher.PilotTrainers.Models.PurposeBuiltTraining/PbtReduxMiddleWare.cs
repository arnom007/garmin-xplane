using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.JsonParsing;
using Domain.PurposeBuiltTraining;
using Domain.UtilTypes;
using IO.AircraftDescriptor;
using LOLA.Extensions;
using LOLA.FileSystem;
using LOLA.Reactive;
using LOLA.Redux;
using LanguageExt;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Utility;
using TrainerPackageLib;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public static class PbtReduxMiddleWare
{
	public static RxMiddleware.ActionStream<PbtPageState> InitialLoad(IAircraftProvider aircraftProvider)
	{
		IAircraftProvider aircraftProvider2 = aircraftProvider;
		return (IObservable<IAction> action, Store.GetState<PbtPageState> getState) => from e in aircraftProvider2.ParseAllAircraftFromAssembly().AsPbtDescriptor()
			select e.AssertValid("These are embedded in the assembly and should never fail or be invalid.") into d
			select new PbtReduxActions.UpdateAircraftStateAction(new PbtAircraftState(d, PbtLocalRepresentation.Missing));
	}

	public static RxMiddleware.ActionStream<PbtPageState> LocalAircraftSubscription(IUnitOfWork unitOfWork, LocalAircraftIdentifierLookup localLookup, PbtLocalAircraftLocator locator)
	{
		LocalAircraftIdentifierLookup localLookup2 = localLookup;
		IUnitOfWork unitOfWork2 = unitOfWork;
		PbtLocalAircraftLocator locator2 = locator;
		return delegate(IObservable<IAction> actions, Store.GetState<PbtPageState> getState)
		{
			Store.GetState<PbtPageState> getState2 = getState;
			return actions.Select((IAction _) => getState2().ListState.AircraftState.Lookup.Values.Select((PbtAircraftState a) => a.AircraftDescriptor).ToHashSet()).DistinctUntilChanged(SetEquality<PbtAircraftDescriptor>.Default).Select(delegate(ISet<PbtAircraftDescriptor> descriptors)
			{
				Dictionary<PbtLocalAircraftIdentifier, PbtAircraftDescriptor> byLocal = descriptors.ToDictionary((PbtAircraftDescriptor d) => localLookup2(d));
				return from s in (from change in unitOfWork2.Aircraft.AircraftChanges.FilterByChangeType(DataChangeType.Created, DataChangeType.Updated, DataChangeType.Deleted)
						select Observable.FromAsync(async delegate(CancellationToken cancel)
						{
							PbtLocalAircraftIdentifier localId = change.Value.ToLocalAircraftId();
							return await byLocal.TryGetValueRo(localId).MapAsync(async delegate(PbtAircraftDescriptor d)
							{
								PbtLocalRepresentation localRepresentation = ((change.Change != DataChangeType.Deleted) ? (await locator2(byLocal[localId], cancel).ConfigureAwait(continueOnCapturedContext: false)) : PbtLocalRepresentation.Missing);
								return new PbtAircraftState(d, localRepresentation);
							}).ToOption()
								.ConfigureAwait(continueOnCapturedContext: false);
						})).Switch().Somes()
					select new PbtReduxActions.UpdateAircraftStateAction(s);
			})
				.Switch();
		};
	}

	public static RxMiddleware.ActionStream<PbtPageState> LoadOnSelection(PbtLocalAircraftLocator locator, PurposeBuiltAircraftCreator creator)
	{
		PbtLocalAircraftLocator locator2 = locator;
		PurposeBuiltAircraftCreator creator2 = creator;
		return delegate(IObservable<IAction> actions, Store.GetState<PbtPageState> getState)
		{
			Store.GetState<PbtPageState> getState2 = getState;
			return from a in (from s in actions.OfType<PbtReduxActions.SelectTrainingItemAction>()
					select Observable.FromAsync(async (CancellationToken t) => await getState2().ListState.AircraftState.Lookup.Find(s.Key).BindAsync(async delegate(PbtAircraftState aircraftState)
					{
						if (aircraftState.LocalRepresentation.IsMissing)
						{
							PbtLocalRepresentation pbtLocalRepresentation = await locator2(aircraftState.AircraftDescriptor, t).ConfigureAwait(continueOnCapturedContext: false);
							if (pbtLocalRepresentation.IsMissing)
							{
								pbtLocalRepresentation = await creator2(aircraftState.AircraftDescriptor).ConfigureAwait(continueOnCapturedContext: false);
							}
							return Prelude.Some(new PbtAircraftState(aircraftState.AircraftDescriptor, pbtLocalRepresentation)).ToAsync();
						}
						return Prelude.None;
					}).ToOption()
						.ConfigureAwait(continueOnCapturedContext: false))).Merge().Somes()
				select new PbtReduxActions.UpdateAircraftStateAction(a);
		};
	}

	public static RxMiddleware.ActionStream<PbtPageState> AutoMigrateMissingBugFixPackageReferences(IUnitOfWork unitOfWork, LocalAircraftIdentifierLookup localLookup, PbtAircraftBugFixUpdateChecker bugFixChecker, PbtLocalAircraftLocator locator)
	{
		LocalAircraftIdentifierLookup localLookup2 = localLookup;
		IUnitOfWork unitOfWork2 = unitOfWork;
		PbtAircraftBugFixUpdateChecker bugFixChecker2 = bugFixChecker;
		PbtLocalAircraftLocator locator2 = locator;
		return delegate(IObservable<IAction> actions, Store.GetState<PbtPageState> getState)
		{
			Store.GetState<PbtPageState> getState2 = getState;
			return (from o in (from _ in TimingEvent.StartStreamAndConcat(actions)
					select getState2() into state
					select state.SelectedItem.Bind((TrainingItemKey k) => state.ListState.AircraftState.Lookup.Find(k).Map((PbtAircraftState a) => (k: k, a: a)))).DistinctUntilChanged()
				select o.Bind(delegate((TrainingItemKey k, PbtAircraftState a) t)
				{
					PbtLocalAircraftIdentifier localId = localLookup2(t.a.AircraftDescriptor);
					if (!t.a.LocalRepresentation.IsMissing)
					{
						ReactiveAircraft localAircraft = new ReactiveAircraft(unitOfWork2, localId.ToStorageAircraftId());
						return (from _ in localAircraft.ReferencedPackagesOverTime().CatchAndComplete()
							select Observable.FromAsync(async (CancellationToken cancel) => await bugFixChecker2(t.a.AircraftDescriptor, localAircraft, cancel).Map((PbtAircraftError error) => PbtLocalRepresentation.Broken(localId, error)).IfNoneAsync(async () => await locator2(t.a.AircraftDescriptor, cancel).ConfigureAwait(continueOnCapturedContext: false)).ConfigureAwait(continueOnCapturedContext: false))).Switch().Select((Func<PbtLocalRepresentation, IAction>)((PbtLocalRepresentation l) => new PbtReduxActions.UpdateAircraftStateAction(new PbtAircraftState(t.a.AircraftDescriptor, l)))).Apply((IObservable<IAction> o) => Prelude.Some(o));
					}
					return Prelude.None;
				}).IfNone(Observable.Empty<IAction>())).Switch();
		};
	}

	public static RxMiddleware.ActionStream<PbtPageState> UpdatePbtRunningItem(Func<ISimSessionCoordinator> simCoordinator, PbtLocalAircraftRecognizer recognizer)
	{
		Func<ISimSessionCoordinator> simCoordinator2 = simCoordinator;
		PbtLocalAircraftRecognizer recognizer2 = recognizer;
		return (IObservable<IAction> actions, Store.GetState<PbtPageState> getState) => actions.Take(1).SelectMany((IAction _) => from tk in simCoordinator2().CurrentlyRunning.GetCachedThenUpdatedAndThenContinualValues().Select(delegate(ISimRunnerEvolved sr)
			{
				IAircraftSimRunner ar = sr as IAircraftSimRunner;
				return (ar == null) ? Observable.Return<PbtRunningAircraftInfo?>(null) : (from s in ar.Aircraft.CompositeSnapshots.GetCachedThenUpdatedAndThenContinualValues().CatchAndComplete<AircraftSnapshotData, RepoItemNotFoundException>(delegate
					{
					})
					select recognizer2(s) into o
					select new PbtRunningAircraftInfo(new PbtLocalAircraftIdentifier(StringBasedId.Parse(ar.Aircraft.AircraftIdentifier.IdentStringChecked).AssertValid()), o.Map((PbtDataStoredInLocalAircraft pbt) => TrainingItemKey.Parse(pbt.DescriptorKey))));
			}).Switch()
				.DistinctUntilChanged()
			select new PbtReduxActions.UpdateRunningAircraftAction(tk));
	}

	public static RxMiddleware.ActionStream<PbtPageState> StoreAndLoadLastSelectedItem(ILowLevelDal dal)
	{
		ILowLevelDal dal2 = dal;
		string selectedFilePath = Path.Combine(dal2.Deployment.UserAppDataDirPath, "selected_pbt.txt");
		return delegate(IObservable<IAction> actions, Store.GetState<PbtPageState> getState)
		{
			Store.GetState<PbtPageState> getState2 = getState;
			return (from t in (from _ in actions.OfType<InitAction>().Take(1)
					select Observable.FromAsync(async () => await Prelude.Some(selectedFilePath).Filter((string s) => dal2.FileSystem.PathExists(s)).ToAsync()
						.Bind((string s) => Prelude.TryAsync(async () => await dal2.FileSystem.ReadAllText(s).ConfigureAwait(continueOnCapturedContext: false)).ToEither().ToOption())
						.ToOption()
						.ToAsync()
						.Bind((string selected) => StringBasedId.Parse(selected).ToOption().ToAsync())
						.Map((StringBasedId sid) => new PbtAircraftDescriptorKey(sid))
						.Map((PbtAircraftDescriptorKey k) => TrainingItemKey.Parse(k))
						.ToOption()
						.ConfigureAwait(continueOnCapturedContext: false))).Switch().Somes().CombineLatest((from _ in TimingEvent.StartStreamAndConcat(actions.OfType<PbtReduxActions.UpdateAircraftStateAction>())
					select getState2().ListState.Menu).DistinctUntilChanged(), (TrainingItemKey k, TrainingMenu m) => (k: k, m: m))
				where t.m.ListItems.Find((TrainingItemListInfo li) => li.Key == t.k).IsSome
				select new PbtReduxActions.SelectTrainingItemAction(t.k)).Take(TimeSpan.FromSeconds(3.0)).Concat((from s in actions.OfType<PbtReduxActions.SelectTrainingItemAction>().DistinctUntilChanged().Throttle(TimeSpan.FromSeconds(1.0))
				select Observable.FromAsync((Func<Task<System.Reactive.Unit>>)async delegate
				{
					await dal2.FileSystem.WriteAllText(selectedFilePath, s.Key.UniqueId.RawString).ConfigureAwait(continueOnCapturedContext: false);
					return System.Reactive.Unit.Default;
				})).Concat().SelectDefault<System.Reactive.Unit, IAction>(null));
		};
	}

	public static PbtActionFactories.ResetActionBuilder CreateResetTrainingItemAction(PurposeBuiltAircraftResetter resetter)
	{
		PurposeBuiltAircraftResetter resetter2 = resetter;
		return (TrainingItemKey key) => Actions.Thunk(delegate(Store.Dispatch d, Store.GetState<PbtPageState> gs)
		{
			Store.Dispatch d2 = d;
			PbtPageState s = gs();
			return DispatchResult.FromAsync(() => s.ListState.AircraftState.Lookup.Find(key).IfSomeAsync(async delegate(PbtAircraftState aircraftState)
			{
				PbtLocalRepresentation localRepresentation = await resetter2(aircraftState.AircraftDescriptor).ConfigureAwait(continueOnCapturedContext: false);
				d2(new PbtReduxActions.UpdateAircraftStateAction(aircraftState with
				{
					LocalRepresentation = localRepresentation
				}));
			}).Map((LanguageExt.Unit _) => DispatchResult.None));
		});
	}
}
