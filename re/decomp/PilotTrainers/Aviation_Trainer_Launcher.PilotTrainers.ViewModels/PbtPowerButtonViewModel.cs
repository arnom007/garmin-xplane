using System;
using System.Reactive;
using System.Reactive.Linq;
using Aviation_Trainer_Launcher.LauncherUILib.Navigation;
using LOLA.Reactive;
using LauncherCoreLib.Models.AbstractUi;
using LauncherCoreLib.Models.CurrentMainAction;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;

namespace Aviation_Trainer_Launcher.PilotTrainers.ViewModels;

public class PbtPowerButtonViewModel
{
	private readonly ISimSessionCoordinator _simSessionCoordinator;

	public PbtPowerButtonViewModel(ISimSessionCoordinator simSessionCoordinator)
	{
		_simSessionCoordinator = simSessionCoordinator;
	}

	public IObservable<Unit> DriveUi(CommonPageInterfaces commonPageInterfaces, ReactiveAircraft? aircraft)
	{
		ReactiveAircraft aircraft2 = aircraft;
		CommonPageInterfaces commonPageInterfaces2 = commonPageInterfaces;
		return Observable.Defer(delegate
		{
			if (aircraft2 == null)
			{
				return Observable.Empty<Unit>();
			}
			IAircraftSimRunner runnerForAircraft = _simSessionCoordinator.GetRunnerForAircraft(aircraft2);
			return commonPageInterfaces2.PushMainAction((from a in runnerForAircraft.SimActions()
				select new MainActionUserActionWrapper(a)).TakeUntil(aircraft2.WhenAircraftIsDeleted()).CatchAndComplete<MainActionUserActionWrapper, RepoItemNotFoundException>(delegate
			{
			})).ToObservable().Merge(SimRunnerBehaviors.SimPowerSquawks(runnerForAircraft.Stage.GetCachedThenUpdatedAndThenContinualValues(), Observable.Return(runnerForAircraft)).Do(delegate(Squawk s)
			{
				commonPageInterfaces2.Squawks.Add(s);
			}).ToSideEffectStream());
		});
	}
}
