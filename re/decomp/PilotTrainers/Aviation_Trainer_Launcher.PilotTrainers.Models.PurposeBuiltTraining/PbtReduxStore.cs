using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.ExceptionServices;
using Domain.PurposeBuiltTraining;
using LOLA.Redux;
using LanguageExt;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public class PbtReduxStore
{
	private readonly IStore<PbtPageState> _store;

	public PbtPageState State => _store.State;

	public IObservable<PbtPageState> StateOverTime => _store.StateOverTime().DistinctUntilChanged();

	public PbtReduxStore(IStore<PbtPageState> store)
	{
		_store = store;
	}

	public static PbtReduxStore CreatePackageStore(Store.MiddleWareFactory<PbtPageState> middleWare)
	{
		return new PbtReduxStore(Store.CreateStoreWithMiddleWare(PbtPageState.Empty, PackageReducer, middleWare));
	}

	public static PbtPageState PackageReducer(PbtPageState pageState, IAction action)
	{
		PbtPageState pageState2 = pageState;
		if (!(action is InitAction))
		{
			PbtReduxActions.SelectTrainingItemAction selectTrainingItemAction = action as PbtReduxActions.SelectTrainingItemAction;
			if ((object)selectTrainingItemAction == null)
			{
				if (!(action is PbtReduxActions.UpdateAircraftStateAction updateAircraftStateAction))
				{
					if (!(action is PbtReduxActions.UpdateRunningAircraftAction updateRunningAircraftAction))
					{
						if (action is UncaughtRxMiddlewareStreamErrorAction uncaught)
						{
							return TakeDownApplication(pageState2, uncaught);
						}
						return pageState2;
					}
					return pageState2 with
					{
						RunningAircraft = updateRunningAircraftAction.Running.ToOption()
					};
				}
				return UpdateState(updateAircraftStateAction.UpdatedState);
			}
			return Prelude.Some(pageState2).Filter((PbtPageState ps) => ps.SelectedItem != selectTrainingItemAction.Key && ps.ListState.Menu.ListItems.Find((TrainingItemListInfo m) => m.Key == selectTrainingItemAction.Key).IsSome).Map((PbtPageState ps) => ps with
			{
				SelectedItem = selectTrainingItemAction.Key
			})
				.IfNone(pageState2);
		}
		return pageState2;
		PbtPageState UpdateState(PbtAircraftState s)
		{
			PbtAircraftListState pbtAircraftListState = pageState2.ListState.UpdateState(s);
			return pageState2 with
			{
				ListState = pbtAircraftListState,
				SelectedItem = Prelude.Seq(pageState2.SelectedItem, pbtAircraftListState.Menu.ListItems.Map((TrainingItemListInfo l) => l.Key).HeadOrNone()).Somes().HeadOrNone()
			};
		}
	}

	public IDispatchResult Dispatch(IAction action)
	{
		return _store.Dispatch(action);
	}

	private static PbtPageState TakeDownApplication(PbtPageState packageInfo, UncaughtRxMiddlewareStreamErrorAction uncaught)
	{
		ExceptionDispatchInfo exceptionInfo = ExceptionDispatchInfo.Capture(uncaught.StreamError);
		((IScheduler)Scheduler.Default).Schedule((Action)delegate
		{
			exceptionInfo.Throw();
		});
		return packageInfo;
	}
}
