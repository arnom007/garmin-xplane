using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LOLA.Reactive;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class TrainingSessionSource : ITrainingSessionSource
{
	private readonly IObservable<ITrainingSession?> _sharedTrainingSessionStream;

	public TrainingSessionSource(ISimSessionCoordinator coordinator)
	{
		_sharedTrainingSessionStream = coordinator.CurrentlyRunning.GetCachedThenUpdatedAndThenContinualValues().DistinctUntilChanged<ISimRunnerEvolved>().Select<ISimRunnerEvolved, IObservable<ITrainingSession>>((Func<ISimRunnerEvolved, IObservable<ITrainingSession>>)((ISimRunnerEvolved runner) => runner.Sessions.GetCachedThenUpdatedAndThenContinualValues().DistinctUntilChanged<ISimSessionEvolved>().Select<ISimSessionEvolved, System.Threading.Tasks.Task<ITrainingSession>>((Func<ISimSessionEvolved, System.Threading.Tasks.Task<ITrainingSession>>)((ISimSessionEvolved s) => s?.TrainingSession ?? System.Threading.Tasks.Task.FromResult<ITrainingSession>((ITrainingSession)null)))
			.Switch<ITrainingSession>()
			.Retry<ITrainingSession>()))
			.Switch<ITrainingSession>()
			.Replay<ITrainingSession>(1)
			.RefCount();
	}

	public IObservable<ITrainingSession?> TrainingSessions()
	{
		return _sharedTrainingSessionStream;
	}
}
