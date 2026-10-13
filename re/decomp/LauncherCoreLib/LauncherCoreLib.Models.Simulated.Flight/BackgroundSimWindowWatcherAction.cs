using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Reactive;
using LauncherCoreLib.Models.AppLifeCycle;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class BackgroundSimWindowWatcherAction : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		public CancellationToken shutdown;

		public BackgroundSimWindowWatcherAction _003C_003E4__this;

		internal IObservable<Unit> _003COnStartup_003Eb__0(ISimRunnerEvolved runner)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_1
			{
				CS_0024_003C_003E8__locals1 = this,
				asr = (runner as IAircraftSimRunner)
			};
			if (CS_0024_003C_003E8__locals0.asr == null)
			{
				return Observable.Empty<Unit>();
			}
			return runner.Sessions.GetCachedThenUpdatedAndThenContinualValues().TakeUntil<ISimSessionEvolved>(shutdown).WhereClassNotNull<ISimSessionEvolved>()
				.Select<ISimSessionEvolved, IObservable<IObservable<Unit>>>((Func<ISimSessionEvolved, IObservable<IObservable<Unit>>>)((ISimSessionEvolved session) => Observable.FromAsync<IObservable<Unit>>((Func<System.Threading.Tasks.Task<IObservable<Unit>>>)new _003C_003Ec__DisplayClass3_2
				{
					CS_0024_003C_003E8__locals2 = CS_0024_003C_003E8__locals0,
					session = session
				}._003COnStartup_003Eb__2)))
				.Switch<IObservable<Unit>>()
				.Switch<Unit>();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_1
	{
		public IAircraftSimRunner asr;

		public _003C_003Ec__DisplayClass3_0 CS_0024_003C_003E8__locals1;

		public Func<SimProcessSession, IObservable<Unit>> _003C_003E9__3;

		internal IObservable<IObservable<Unit>> _003COnStartup_003Eb__1(ISimSessionEvolved session)
		{
			return Observable.FromAsync<IObservable<Unit>>((Func<System.Threading.Tasks.Task<IObservable<Unit>>>)new _003C_003Ec__DisplayClass3_2
			{
				CS_0024_003C_003E8__locals2 = this,
				session = session
			}._003COnStartup_003Eb__2);
		}

		internal IObservable<Unit> _003COnStartup_003Eb__3(SimProcessSession runningSim)
		{
			_003C_003Ec__DisplayClass3_3 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_3();
			ReactiveSimulator sim = asr.Aircraft.CreateSimWrapper(runningSim.Simulator.InstallData.InstallationKey);
			CS_0024_003C_003E8__locals0.updater = TrainerWindowScreenCoordinatesStorage.CreateScreenCoordsSaver(sim);
			return CS_0024_003C_003E8__locals1._003C_003E4__this._windowWatcher(runningSim).DistinctUntilChanged<TrainerWindowScreenCoordinates>().Select<TrainerWindowScreenCoordinates, IObservable<Unit>>((Func<TrainerWindowScreenCoordinates, IObservable<Unit>>)((TrainerWindowScreenCoordinates screenCoords) => Observable.FromAsync<Unit>((Func<CancellationToken, System.Threading.Tasks.Task<Unit>>)new _003C_003Ec__DisplayClass3_4
			{
				CS_0024_003C_003E8__locals3 = CS_0024_003C_003E8__locals0,
				screenCoords = screenCoords
			}._003COnStartup_003Eb__5)))
				.Concat<Unit>()
				.ToSideEffectStream<Unit>();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_2
	{
		[StructLayout(3)]
		private struct _003C_003COnStartup_003Eb__2_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<IObservable<Unit>> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass3_2 _003C_003E4__this;

			private ConfiguredTaskAwaiter<ITrainingSession?> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass3_2 _003C_003Ec__DisplayClass3_ = _003C_003E4__this;
				IObservable<Unit> result;
				try
				{
					ITrainingSession trainingSession;
					try
					{
						ConfiguredTaskAwaiter<ITrainingSession> awaiter;
						if (num != 0)
						{
							awaiter = _003C_003Ec__DisplayClass3_.session.TrainingSession.ConfigureAwait(false).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainingSession>, _003C_003COnStartup_003Eb__2_003Ed>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ITrainingSession>);
							num = (_003C_003E1__state = -1);
						}
						trainingSession = awaiter.GetResult();
					}
					catch (SimPowerException)
					{
						trainingSession = null;
					}
					result = ((trainingSession != null) ? Enumerable.Select<SimProcessSession, IObservable<Unit>>((System.Collections.Generic.IEnumerable<SimProcessSession>)trainingSession.Simulators, (Func<SimProcessSession, IObservable<Unit>>)delegate(SimProcessSession runningSim)
					{
						_003C_003Ec__DisplayClass3_3 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_3();
						ReactiveSimulator sim = _003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals2.asr.Aircraft.CreateSimWrapper(runningSim.Simulator.InstallData.InstallationKey);
						CS_0024_003C_003E8__locals0.updater = TrainerWindowScreenCoordinatesStorage.CreateScreenCoordsSaver(sim);
						return _003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._windowWatcher(runningSim).DistinctUntilChanged<TrainerWindowScreenCoordinates>().Select<TrainerWindowScreenCoordinates, IObservable<Unit>>((Func<TrainerWindowScreenCoordinates, IObservable<Unit>>)((TrainerWindowScreenCoordinates screenCoords) => Observable.FromAsync<Unit>((Func<CancellationToken, System.Threading.Tasks.Task<Unit>>)new _003C_003Ec__DisplayClass3_4
						{
							CS_0024_003C_003E8__locals3 = CS_0024_003C_003E8__locals0,
							screenCoords = screenCoords
						}._003COnStartup_003Eb__5)))
							.Concat<Unit>()
							.ToSideEffectStream<Unit>();
					}).Merge() : Observable.Empty<Unit>());
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult(result);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public ISimSessionEvolved session;

		public _003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals2;

		[AsyncStateMachine(typeof(_003C_003COnStartup_003Eb__2_003Ed))]
		internal async System.Threading.Tasks.Task<IObservable<Unit>> _003COnStartup_003Eb__2()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ITrainingSession trainingSession;
			try
			{
				trainingSession = await session.TrainingSession.ConfigureAwait(false);
			}
			catch (SimPowerException)
			{
				trainingSession = null;
			}
			if (trainingSession == null)
			{
				return Observable.Empty<Unit>();
			}
			return Enumerable.Select<SimProcessSession, IObservable<Unit>>((System.Collections.Generic.IEnumerable<SimProcessSession>)trainingSession.Simulators, (Func<SimProcessSession, IObservable<Unit>>)delegate(SimProcessSession runningSim)
			{
				_003C_003Ec__DisplayClass3_3 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_3();
				ReactiveSimulator sim = CS_0024_003C_003E8__locals2.asr.Aircraft.CreateSimWrapper(runningSim.Simulator.InstallData.InstallationKey);
				CS_0024_003C_003E8__locals0.updater = TrainerWindowScreenCoordinatesStorage.CreateScreenCoordsSaver(sim);
				return CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._windowWatcher(runningSim).DistinctUntilChanged<TrainerWindowScreenCoordinates>().Select<TrainerWindowScreenCoordinates, IObservable<Unit>>((Func<TrainerWindowScreenCoordinates, IObservable<Unit>>)((TrainerWindowScreenCoordinates screenCoords) => Observable.FromAsync<Unit>((Func<CancellationToken, System.Threading.Tasks.Task<Unit>>)new _003C_003Ec__DisplayClass3_4
				{
					CS_0024_003C_003E8__locals3 = CS_0024_003C_003E8__locals0,
					screenCoords = screenCoords
				}._003COnStartup_003Eb__5)))
					.Concat<Unit>()
					.ToSideEffectStream<Unit>();
			}).Merge();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_3
	{
		public StoreSimulatorScreenCoords updater;

		internal IObservable<Unit> _003COnStartup_003Eb__4(TrainerWindowScreenCoordinates screenCoords)
		{
			return Observable.FromAsync<Unit>((Func<CancellationToken, System.Threading.Tasks.Task<Unit>>)new _003C_003Ec__DisplayClass3_4
			{
				CS_0024_003C_003E8__locals3 = this,
				screenCoords = screenCoords
			}._003COnStartup_003Eb__5);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_4
	{
		[StructLayout(3)]
		private struct _003C_003COnStartup_003Eb__5_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Unit> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass3_4 _003C_003E4__this;

			public CancellationToken c;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass3_4 _003C_003Ec__DisplayClass3_ = _003C_003E4__this;
				Unit @default;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals3.updater(_003C_003Ec__DisplayClass3_.screenCoords, c).ConfigureAwait(false);
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003COnStartup_003Eb__5_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					@default = Unit.Default;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult(@default);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public TrainerWindowScreenCoordinates screenCoords;

		public _003C_003Ec__DisplayClass3_3 CS_0024_003C_003E8__locals3;

		[AsyncStateMachine(typeof(_003C_003COnStartup_003Eb__5_003Ed))]
		internal async System.Threading.Tasks.Task<Unit> _003COnStartup_003Eb__5(CancellationToken c)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			await CS_0024_003C_003E8__locals3.updater(screenCoords, c).ConfigureAwait(false);
			return Unit.Default;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnStartup_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public CancellationToken shutdown;

		public BackgroundSimWindowWatcherAction _003C_003E4__this;

		private ConfiguredTaskAwaiter<Unit> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			BackgroundSimWindowWatcherAction backgroundSimWindowWatcherAction = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<Unit> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass3_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_0
					{
						shutdown = shutdown,
						_003C_003E4__this = _003C_003E4__this
					};
					awaiter = backgroundSimWindowWatcherAction._coordinator.CurrentlyRunning.GetCachedThenUpdatedAndThenContinualValues().TakeUntil<ISimRunnerEvolved>(CS_0024_003C_003E8__locals0.shutdown).Select<ISimRunnerEvolved, IObservable<Unit>>((Func<ISimRunnerEvolved, IObservable<Unit>>)delegate(ISimRunnerEvolved runner)
					{
						//IL_0033: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass3_1
						{
							CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals0,
							asr = (runner as IAircraftSimRunner)
						};
						return (CS_0024_003C_003E8__locals1.asr == null) ? Observable.Empty<Unit>() : runner.Sessions.GetCachedThenUpdatedAndThenContinualValues().TakeUntil<ISimSessionEvolved>(CS_0024_003C_003E8__locals0.shutdown).WhereClassNotNull<ISimSessionEvolved>()
							.Select<ISimSessionEvolved, IObservable<IObservable<Unit>>>((Func<ISimSessionEvolved, IObservable<IObservable<Unit>>>)((ISimSessionEvolved session) => Observable.FromAsync<IObservable<Unit>>((Func<System.Threading.Tasks.Task<IObservable<Unit>>>)new _003C_003Ec__DisplayClass3_2
							{
								CS_0024_003C_003E8__locals2 = CS_0024_003C_003E8__locals1,
								session = session
							}._003COnStartup_003Eb__2)))
							.Switch<IObservable<Unit>>()
							.Switch<Unit>();
					})
						.Switch<Unit>()
						.LastOrDefaultAsync<Unit>()
						.ToTask<Unit>()
						.ConfigureAwait(false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<Unit>, _003COnStartup_003Ed__3>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<Unit>);
					num = (_003C_003E1__state = -1);
				}
				awaiter.GetResult();
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	private readonly ISimSessionCoordinator _coordinator;

	private readonly WatchTrainerWindowRectangle _windowWatcher;

	public BackgroundSimWindowWatcherAction(ISimSessionCoordinator coordinator, WatchTrainerWindowRectangle windowWatcher)
	{
		_coordinator = coordinator;
		_windowWatcher = windowWatcher;
	}

	[AsyncStateMachine(typeof(_003COnStartup_003Ed__3))]
	public System.Threading.Tasks.Task OnStartup(CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003COnStartup_003Ed__3 _003COnStartup_003Ed__ = default(_003COnStartup_003Ed__3);
		_003COnStartup_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnStartup_003Ed__._003C_003E4__this = this;
		_003COnStartup_003Ed__.shutdown = shutdown;
		_003COnStartup_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Start<_003COnStartup_003Ed__3>(ref _003COnStartup_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Task;
	}
}
