using System;
using System.Collections;
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
using LOLA.Tasks;
using LauncherCoreLib.Models.AppLifeCycle;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;
using LauncherCoreLib.Models.Simulated.Flight;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.EventLogging.BackgroundLoggers;

public class LogTrainerToAppCenterStartupAction : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		public CancellationToken shutdown;

		public LogTrainerToAppCenterStartupAction _003C_003E4__this;

		public Func<ISimSessionEvolved, IObservable<Unit>> _003C_003E9__1;

		internal IObservable<Unit> _003COnStartup_003Eb__0(ISimRunnerEvolved runner)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (!(runner is IAircraftSimRunner))
			{
				return Observable.Empty<Unit>();
			}
			return runner.Sessions.GetUpdateToDateAndThenContinualValues().TakeUntil<ISimSessionEvolved>(shutdown).WhereClassNotNull<ISimSessionEvolved>()
				.Select<ISimSessionEvolved, IObservable<Unit>>((Func<ISimSessionEvolved, IObservable<Unit>>)delegate(ISimSessionEvolved session)
				{
					_003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_1
					{
						CS_0024_003C_003E8__locals1 = this,
						session = session
					};
					return CS_0024_003C_003E8__locals0.session.Stage.GetCachedThenUpdatedAndThenContinualValues().DistinctUntilChanged<PowerStage>().Publish<PowerStage, Unit>(delegate(IObservable<PowerStage> stages)
					{
						//IL_0020: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec__DisplayClass3_2 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass3_2
						{
							CS_0024_003C_003E8__locals2 = CS_0024_003C_003E8__locals0,
							stages = stages
						};
						return CS_0024_003C_003E8__locals1.stages.TakeUntil<PowerStage>(CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.shutdown).Where<PowerStage>((Func<PowerStage, bool>)((PowerStage s) => s == PowerStage.PoweredUp)).Select<PowerStage, IObservable<Unit>>((Func<PowerStage, IObservable<Unit>>)((PowerStage _) => Observable.FromAsync((Func<CancellationToken, System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass3_3
						{
							CS_0024_003C_003E8__locals3 = CS_0024_003C_003E8__locals1,
							_ = _
						}._003COnStartup_003Eb__5)))
							.Concat<Unit>();
					});
				})
				.Merge<Unit>();
		}

		internal IObservable<Unit> _003COnStartup_003Eb__1(ISimSessionEvolved session)
		{
			_003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_1
			{
				CS_0024_003C_003E8__locals1 = this,
				session = session
			};
			return CS_0024_003C_003E8__locals0.session.Stage.GetCachedThenUpdatedAndThenContinualValues().DistinctUntilChanged<PowerStage>().Publish<PowerStage, Unit>(delegate(IObservable<PowerStage> stages)
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				_003C_003Ec__DisplayClass3_2 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass3_2
				{
					CS_0024_003C_003E8__locals2 = CS_0024_003C_003E8__locals0,
					stages = stages
				};
				return CS_0024_003C_003E8__locals1.stages.TakeUntil<PowerStage>(CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.shutdown).Where<PowerStage>((Func<PowerStage, bool>)((PowerStage s) => s == PowerStage.PoweredUp)).Select<PowerStage, IObservable<Unit>>((Func<PowerStage, IObservable<Unit>>)((PowerStage _) => Observable.FromAsync((Func<CancellationToken, System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass3_3
				{
					CS_0024_003C_003E8__locals3 = CS_0024_003C_003E8__locals1,
					_ = _
				}._003COnStartup_003Eb__5)))
					.Concat<Unit>();
			});
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_1
	{
		public ISimSessionEvolved session;

		public _003C_003Ec__DisplayClass3_0 CS_0024_003C_003E8__locals1;

		internal IObservable<Unit> _003COnStartup_003Eb__2(IObservable<PowerStage> stages)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass3_2 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_2
			{
				CS_0024_003C_003E8__locals2 = this,
				stages = stages
			};
			return CS_0024_003C_003E8__locals0.stages.TakeUntil<PowerStage>(CS_0024_003C_003E8__locals1.shutdown).Where<PowerStage>((Func<PowerStage, bool>)((PowerStage s) => s == PowerStage.PoweredUp)).Select<PowerStage, IObservable<Unit>>((Func<PowerStage, IObservable<Unit>>)((PowerStage _) => Observable.FromAsync((Func<CancellationToken, System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass3_3
			{
				CS_0024_003C_003E8__locals3 = CS_0024_003C_003E8__locals0,
				_ = _
			}._003COnStartup_003Eb__5)))
				.Concat<Unit>();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_2
	{
		public IObservable<PowerStage> stages;

		public _003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals2;

		internal IObservable<Unit> _003COnStartup_003Eb__4(PowerStage _)
		{
			return Observable.FromAsync((Func<CancellationToken, System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass3_3
			{
				CS_0024_003C_003E8__locals3 = this,
				_ = _
			}._003COnStartup_003Eb__5);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_3
	{
		[StructLayout(3)]
		private struct _003C_003COnStartup_003Eb__5_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass3_3 _003C_003E4__this;

			public CancellationToken t;

			private DateTimeOffset _003CstartSessionTime_003E5__2;

			private ConfiguredTaskAwaiter<PowerStage> _003C_003Eu__1;

			private ConfiguredTaskAwaiter<ITrainingSession> _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0134: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0181: Unknown result type (might be due to invalid IL or missing references)
				//IL_0186: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass3_3 _003C_003Ec__DisplayClass3_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<ITrainingSession> awaiter;
					ConfiguredTaskAwaiter<PowerStage> awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ITrainingSession>);
							num = (_003C_003E1__state = -1);
							goto IL_0143;
						}
						_003CstartSessionTime_003E5__2 = DateTimeOffset.Now;
						awaiter2 = _003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals3.stages.Where<PowerStage>((Func<PowerStage, bool>)((PowerStage s) => s != PowerStage.PoweredUp)).TakeUntil<PowerStage>(t).FirstOrDefaultAsync<PowerStage>()
							.ToTask<PowerStage>()
							.ConfigureAwait(false)
							.GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PowerStage>, _003C_003COnStartup_003Eb__5_003Ed>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PowerStage>);
						num = (_003C_003E1__state = -1);
					}
					PowerStage result = awaiter2.GetResult();
					_003C_003Ec__DisplayClass3_._ = result;
					awaiter = _003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals3.CS_0024_003C_003E8__locals2.session.TrainingSession.DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainingSession>, _003C_003COnStartup_003Eb__5_003Ed>(ref awaiter, ref this);
						return;
					}
					goto IL_0143;
					IL_0143:
					ITrainingSession result2 = awaiter.GetResult();
					System.Collections.Generic.IEnumerable<ITrainerPackage> enumerable = ((result2 != null) ? Enumerable.Select<SimProcessSession, ITrainerPackage>((System.Collections.Generic.IEnumerable<SimProcessSession>)result2.Simulators, (Func<SimProcessSession, ITrainerPackage>)((SimProcessSession session) => session.Simulator.PackageInfo.Package)) : null);
					if (enumerable != null)
					{
						DateTimeOffset now = DateTimeOffset.Now;
						System.Collections.Generic.IEnumerator<ITrainerPackage> enumerator = enumerable.GetEnumerator();
						try
						{
							while (((System.Collections.IEnumerator)enumerator).MoveNext())
							{
								ITrainerPackage current = enumerator.Current;
								_003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(new SimSessionDataDescriptor(current, _003CstartSessionTime_003E5__2, now));
								_003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(new EndTrainerSessionDataDescriptor(_003CstartSessionTime_003E5__2, now, current));
							}
						}
						finally
						{
							if (num < 0)
							{
								((System.IDisposable)enumerator)?.Dispose();
							}
						}
					}
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

		public PowerStage _;

		public _003C_003Ec__DisplayClass3_2 CS_0024_003C_003E8__locals3;

		[AsyncStateMachine(typeof(_003C_003COnStartup_003Eb__5_003Ed))]
		internal System.Threading.Tasks.Task _003COnStartup_003Eb__5(CancellationToken t)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			_003C_003COnStartup_003Eb__5_003Ed _003C_003COnStartup_003Eb__5_003Ed = default(_003C_003COnStartup_003Eb__5_003Ed);
			_003C_003COnStartup_003Eb__5_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003COnStartup_003Eb__5_003Ed._003C_003E4__this = this;
			_003C_003COnStartup_003Eb__5_003Ed.t = t;
			_003C_003COnStartup_003Eb__5_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003COnStartup_003Eb__5_003Ed._003C_003Et__builder)).Start<_003C_003COnStartup_003Eb__5_003Ed>(ref _003C_003COnStartup_003Eb__5_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003COnStartup_003Eb__5_003Ed._003C_003Et__builder)).Task;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnStartup_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public CancellationToken shutdown;

		public LogTrainerToAppCenterStartupAction _003C_003E4__this;

		private ConfiguredTaskAwaiter<Unit> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			LogTrainerToAppCenterStartupAction logTrainerToAppCenterStartupAction = _003C_003E4__this;
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
					awaiter = logTrainerToAppCenterStartupAction._coordinator.CurrentlyRunning.GetUpdateToDateAndThenContinualValues().TakeUntil<ISimRunnerEvolved>(CS_0024_003C_003E8__locals0.shutdown).Select<ISimRunnerEvolved, IObservable<Unit>>((Func<ISimRunnerEvolved, IObservable<Unit>>)((ISimRunnerEvolved runner) => (!(runner is IAircraftSimRunner)) ? Observable.Empty<Unit>() : runner.Sessions.GetUpdateToDateAndThenContinualValues().TakeUntil<ISimSessionEvolved>(CS_0024_003C_003E8__locals0.shutdown).WhereClassNotNull<ISimSessionEvolved>()
						.Select<ISimSessionEvolved, IObservable<Unit>>((Func<ISimSessionEvolved, IObservable<Unit>>)delegate(ISimSessionEvolved session)
						{
							_003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass3_1
							{
								CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals0,
								session = session
							};
							return CS_0024_003C_003E8__locals1.session.Stage.GetCachedThenUpdatedAndThenContinualValues().DistinctUntilChanged<PowerStage>().Publish<PowerStage, Unit>(delegate(IObservable<PowerStage> stages)
							{
								//IL_0020: Unknown result type (might be due to invalid IL or missing references)
								_003C_003Ec__DisplayClass3_2 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass3_2
								{
									CS_0024_003C_003E8__locals2 = CS_0024_003C_003E8__locals1,
									stages = stages
								};
								return CS_0024_003C_003E8__locals2.stages.TakeUntil<PowerStage>(CS_0024_003C_003E8__locals1.CS_0024_003C_003E8__locals1.shutdown).Where<PowerStage>((Func<PowerStage, bool>)((PowerStage s) => s == PowerStage.PoweredUp)).Select<PowerStage, IObservable<Unit>>((Func<PowerStage, IObservable<Unit>>)((PowerStage _) => Observable.FromAsync((Func<CancellationToken, System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass3_3
								{
									CS_0024_003C_003E8__locals3 = CS_0024_003C_003E8__locals2,
									_ = _
								}._003COnStartup_003Eb__5)))
									.Concat<Unit>();
							});
						})
						.Merge<Unit>()))
						.Switch<Unit>()
						.LastOrDefaultAsync<Unit>()
						.ToTask<Unit>()
						.DetachContext()
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

	private readonly TrackedEvent.LogTrackedEvent _sendLog;

	public LogTrainerToAppCenterStartupAction(ISimSessionCoordinator coordinator, TrackedEvent.LogTrackedEvent sendLog)
	{
		_coordinator = coordinator;
		_sendLog = sendLog;
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
