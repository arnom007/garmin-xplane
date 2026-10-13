using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Reactive;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Panel;

public class SessionWinfsSimResetter : IResetSimulators
{
	private readonly IRunSimulators _simRunner;

	private readonly IUserIdProvider _userIdProvider;

	private readonly ISubject<SimAircraftKey> _deleted = new Subject<SimAircraftKey>();

	public SessionWinfsSimResetter(IRunSimulators simRunner, IUserIdProvider userIdProvider)
	{
		_simRunner = simRunner;
		_userIdProvider = userIdProvider;
	}

	public IObservable<bool> CanBeReset(ReactiveSimulator simulator)
	{
		ReactiveSimulator simulator2 = simulator;
		string simSessionWinfsDirPath = GetSimSessionWinFsDirPath(simulator2);
		return TimingEvent.StartStreamAndConcat<TimingEvent>(_simRunner.SafeWhenIsRunningChanges().WhereEqual(operand: false).ToTimingEventStream<bool>()
			.Merge<TimingEvent>(((IObservable<SimAircraftKey>)_deleted).Where<SimAircraftKey>((Func<SimAircraftKey, bool>)((SimAircraftKey d) => d == simulator2.AircraftAndSim)).ToTimingEventStream<SimAircraftKey>())).Select<TimingEvent, bool>((Func<TimingEvent, bool>)((TimingEvent _) => !_simRunner.IsRunning && simulator2.UnitOfWork.LowLevel.FileSystem.PathExists(simSessionWinfsDirPath)));
	}

	public System.Threading.Tasks.Task ResetSimulator(ReactiveSimulator simulator)
	{
		string simSessionWinFsDirPath = GetSimSessionWinFsDirPath(simulator);
		if (!_simRunner.IsRunning)
		{
			simulator.UnitOfWork.LowLevel.FileSystem.DeleteAnythingNotLocked(simSessionWinFsDirPath);
			lock (_deleted)
			{
				((IObserver<SimAircraftKey>)_deleted).OnNext(simulator.AircraftAndSim);
			}
			try
			{
				simulator.UnitOfWork.LowLevel.FileSystem.DeleteIfExistsAndParentsThatAreEmpty(simSessionWinFsDirPath, simulator.UnitOfWork.LowLevel.Deployment.AsciiSafeDirectory());
			}
			catch (object obj) when (((Func<bool>)delegate
			{
				// Could not convert BlockContainer to single expression
				object obj2 = ((obj is IOException) ? obj : null);
				return obj2 != null && ((IOException)obj2).IsAccessDenied();
			}).Invoke())
			{
			}
		}
		return System.Threading.Tasks.Task.CompletedTask;
	}

	private string GetSimSessionWinFsDirPath(ReactiveSimulator simulator)
	{
		return SessionWinfsDirectory.GetSimDirPath(simulator.UnitOfWork.LowLevel.Deployment, _userIdProvider, simulator.AircraftAndSim);
	}
}
