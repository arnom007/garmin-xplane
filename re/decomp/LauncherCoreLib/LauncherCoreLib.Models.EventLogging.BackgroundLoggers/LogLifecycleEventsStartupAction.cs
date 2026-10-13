using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Reactive;
using LOLA.Tasks;
using LanguageExt;
using LauncherCoreLib.Models.AppLifeCycle;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherCoreLib.Models.EventLogging.BackgroundLoggers;

public class LogLifecycleEventsStartupAction : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass9_0
	{
		public CancellationToken shutdown;

		public int powerCycles;

		public LogLifecycleEventsStartupAction _003C_003E4__this;

		public DateTimeOffset startSessionOffset;

		public Action<PowerStage> _003C_003E9__5;

		public Action<TrackedEventDataDescriptor> _003C_003E9__8;

		public Func<ISimSessionEvolved, IObservable<Unit>> _003C_003E9__2;

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
					_003C_003Ec__DisplayClass9_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass9_1
					{
						CS_0024_003C_003E8__locals1 = this,
						session = session
					};
					return CS_0024_003C_003E8__locals0.session.Stage.GetCachedThenUpdatedAndThenContinualValues().DistinctUntilChanged<PowerStage>().Publish<PowerStage, Unit>((IObservable<PowerStage> stages) => stages.TakeUntil<PowerStage>(CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.shutdown).Where<PowerStage>((Func<PowerStage, bool>)((PowerStage s) => s == PowerStage.PoweredUp)).Do<PowerStage>((Action<PowerStage>)delegate
					{
						CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.powerCycles++;
					})
						.Select<PowerStage, IObservable<Unit>>((Func<PowerStage, IObservable<Unit>>)((PowerStage _) => Observable.FromAsync<Unit>((Func<CancellationToken, System.Threading.Tasks.Task<Unit>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass9_1._003C_003COnStartup_003Eb__7_003Ed))] async (CancellationToken _) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							ITrainingSession simSessions = await CS_0024_003C_003E8__locals0.session.TrainingSession.DetachContext();
							SimSettingsInjector getDataDescriptor = CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1._003C_003E4__this._getDataDescriptor;
							IActiveSetting navigationDatabaseSetting = CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1._003C_003E4__this._reactiveHangar.UnitOfWork.Settings.Globals.GetNavigationDatabaseSetting();
							ITrainingSession session2 = simSessions;
							getDataDescriptor(navigationDatabaseSetting, session2, await CS_0024_003C_003E8__locals0.session.SessionSnapshot.DetachContext()).IfSome((Action<TrackedEventDataDescriptor>)delegate(TrackedEventDataDescriptor d)
							{
								CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(d);
							});
							if (simSessions != null)
							{
								CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(new SlotUsageDataDescriptor(simSessions.Simulators));
							}
							return Unit.Default;
						}))))
						.Concat<Unit>());
				})
				.Merge<Unit>();
		}

		internal IObservable<Unit> _003COnStartup_003Eb__2(ISimSessionEvolved session)
		{
			_003C_003Ec__DisplayClass9_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass9_1
			{
				CS_0024_003C_003E8__locals1 = this,
				session = session
			};
			return CS_0024_003C_003E8__locals0.session.Stage.GetCachedThenUpdatedAndThenContinualValues().DistinctUntilChanged<PowerStage>().Publish<PowerStage, Unit>((IObservable<PowerStage> stages) => stages.TakeUntil<PowerStage>(CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.shutdown).Where<PowerStage>((Func<PowerStage, bool>)((PowerStage s) => s == PowerStage.PoweredUp)).Do<PowerStage>((Action<PowerStage>)delegate
			{
				CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.powerCycles++;
			})
				.Select<PowerStage, IObservable<Unit>>((Func<PowerStage, IObservable<Unit>>)((PowerStage _) => Observable.FromAsync<Unit>((Func<CancellationToken, System.Threading.Tasks.Task<Unit>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass9_1._003C_003COnStartup_003Eb__7_003Ed))] async (CancellationToken _) =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					ITrainingSession simSessions = await CS_0024_003C_003E8__locals0.session.TrainingSession.DetachContext();
					SimSettingsInjector getDataDescriptor = CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1._003C_003E4__this._getDataDescriptor;
					IActiveSetting navigationDatabaseSetting = CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1._003C_003E4__this._reactiveHangar.UnitOfWork.Settings.Globals.GetNavigationDatabaseSetting();
					ITrainingSession session2 = simSessions;
					getDataDescriptor(navigationDatabaseSetting, session2, await CS_0024_003C_003E8__locals0.session.SessionSnapshot.DetachContext()).IfSome((Action<TrackedEventDataDescriptor>)delegate(TrackedEventDataDescriptor d)
					{
						CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(d);
					});
					if (simSessions != null)
					{
						CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(new SlotUsageDataDescriptor(simSessions.Simulators));
					}
					return Unit.Default;
				}))))
				.Concat<Unit>());
		}

		internal void _003COnStartup_003Eb__5(PowerStage _)
		{
			powerCycles++;
		}

		internal void _003COnStartup_003Eb__8(TrackedEventDataDescriptor d)
		{
			_003C_003E4__this._sendLog(d);
		}

		internal void _003COnStartup_003Eb__1()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			_003C_003E4__this._sendLog(new EndLauncherSessionDataDescriptor(powerCycles, startSessionOffset));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass9_1
	{
		[StructLayout(3)]
		private struct _003C_003COnStartup_003Eb__7_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Unit> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass9_1 _003C_003E4__this;

			private ITrainingSession _003CsimSessions_003E5__2;

			private ConfiguredTaskAwaiter<ITrainingSession> _003C_003Eu__1;

			private SimSettingsInjector _003C_003E7__wrap2;

			private IActiveSetting _003C_003E7__wrap3;

			private ITrainingSession _003C_003E7__wrap4;

			private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass9_1 _003C_003Ec__DisplayClass9_ = _003C_003E4__this;
				Unit @default;
				try
				{
					ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter;
					ConfiguredTaskAwaiter<ITrainingSession> awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
							num = (_003C_003E1__state = -1);
							goto IL_013d;
						}
						awaiter2 = _003C_003Ec__DisplayClass9_.session.TrainingSession.DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainingSession>, _003C_003COnStartup_003Eb__7_003Ed>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ITrainingSession>);
						num = (_003C_003E1__state = -1);
					}
					ITrainingSession result = awaiter2.GetResult();
					_003CsimSessions_003E5__2 = result;
					_003C_003E7__wrap2 = _003C_003Ec__DisplayClass9_.CS_0024_003C_003E8__locals1._003C_003E4__this._getDataDescriptor;
					_003C_003E7__wrap3 = _003C_003Ec__DisplayClass9_.CS_0024_003C_003E8__locals1._003C_003E4__this._reactiveHangar.UnitOfWork.Settings.Globals.GetNavigationDatabaseSetting();
					_003C_003E7__wrap4 = _003CsimSessions_003E5__2;
					awaiter = _003C_003Ec__DisplayClass9_.session.SessionSnapshot.DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003C_003COnStartup_003Eb__7_003Ed>(ref awaiter, ref this);
						return;
					}
					goto IL_013d;
					IL_013d:
					AircraftSnapshotData result2 = awaiter.GetResult();
					_003C_003E7__wrap2(_003C_003E7__wrap3, _003C_003E7__wrap4, result2).IfSome((Action<TrackedEventDataDescriptor>)delegate(TrackedEventDataDescriptor d)
					{
						_003C_003Ec__DisplayClass9_.CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(d);
					});
					_003C_003E7__wrap2 = null;
					_003C_003E7__wrap3 = null;
					_003C_003E7__wrap4 = null;
					if (_003CsimSessions_003E5__2 != null)
					{
						_003C_003Ec__DisplayClass9_.CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(new SlotUsageDataDescriptor(_003CsimSessions_003E5__2.Simulators));
					}
					@default = Unit.Default;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003CsimSessions_003E5__2 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003CsimSessions_003E5__2 = null;
				_003C_003Et__builder.SetResult(@default);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public ISimSessionEvolved session;

		public _003C_003Ec__DisplayClass9_0 CS_0024_003C_003E8__locals1;

		public Func<CancellationToken, System.Threading.Tasks.Task<Unit>> _003C_003E9__7;

		public Func<PowerStage, IObservable<Unit>> _003C_003E9__6;

		internal IObservable<Unit> _003COnStartup_003Eb__3(IObservable<PowerStage> stages)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return stages.TakeUntil<PowerStage>(CS_0024_003C_003E8__locals1.shutdown).Where<PowerStage>((Func<PowerStage, bool>)((PowerStage s) => s == PowerStage.PoweredUp)).Do<PowerStage>((Action<PowerStage>)delegate
			{
				CS_0024_003C_003E8__locals1.powerCycles++;
			})
				.Select<PowerStage, IObservable<Unit>>((Func<PowerStage, IObservable<Unit>>)((PowerStage _) => Observable.FromAsync<Unit>((Func<CancellationToken, System.Threading.Tasks.Task<Unit>>)([AsyncStateMachine(typeof(_003C_003COnStartup_003Eb__7_003Ed))] async (CancellationToken _) =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					ITrainingSession simSessions = await session.TrainingSession.DetachContext();
					SimSettingsInjector getDataDescriptor = CS_0024_003C_003E8__locals1._003C_003E4__this._getDataDescriptor;
					IActiveSetting navigationDatabaseSetting = CS_0024_003C_003E8__locals1._003C_003E4__this._reactiveHangar.UnitOfWork.Settings.Globals.GetNavigationDatabaseSetting();
					ITrainingSession trainingSession = simSessions;
					getDataDescriptor(navigationDatabaseSetting, trainingSession, await session.SessionSnapshot.DetachContext()).IfSome((Action<TrackedEventDataDescriptor>)delegate(TrackedEventDataDescriptor d)
					{
						CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(d);
					});
					if (simSessions != null)
					{
						CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(new SlotUsageDataDescriptor(simSessions.Simulators));
					}
					return Unit.Default;
				}))))
				.Concat<Unit>();
		}

		internal IObservable<Unit> _003COnStartup_003Eb__6(PowerStage _)
		{
			return Observable.FromAsync<Unit>((Func<CancellationToken, System.Threading.Tasks.Task<Unit>>)([AsyncStateMachine(typeof(_003C_003COnStartup_003Eb__7_003Ed))] async (CancellationToken _) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				ITrainingSession simSessions = await session.TrainingSession.DetachContext();
				SimSettingsInjector getDataDescriptor = CS_0024_003C_003E8__locals1._003C_003E4__this._getDataDescriptor;
				IActiveSetting navigationDatabaseSetting = CS_0024_003C_003E8__locals1._003C_003E4__this._reactiveHangar.UnitOfWork.Settings.Globals.GetNavigationDatabaseSetting();
				ITrainingSession trainingSession = simSessions;
				getDataDescriptor(navigationDatabaseSetting, trainingSession, await session.SessionSnapshot.DetachContext()).IfSome((Action<TrackedEventDataDescriptor>)delegate(TrackedEventDataDescriptor d)
				{
					CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(d);
				});
				if (simSessions != null)
				{
					CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(new SlotUsageDataDescriptor(simSessions.Simulators));
				}
				return Unit.Default;
			}));
		}

		[AsyncStateMachine(typeof(_003C_003COnStartup_003Eb__7_003Ed))]
		internal async System.Threading.Tasks.Task<Unit> _003COnStartup_003Eb__7(CancellationToken _)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ITrainingSession simSessions = await session.TrainingSession.DetachContext();
			SimSettingsInjector getDataDescriptor = CS_0024_003C_003E8__locals1._003C_003E4__this._getDataDescriptor;
			IActiveSetting navigationDatabaseSetting = CS_0024_003C_003E8__locals1._003C_003E4__this._reactiveHangar.UnitOfWork.Settings.Globals.GetNavigationDatabaseSetting();
			ITrainingSession trainingSession = simSessions;
			getDataDescriptor(navigationDatabaseSetting, trainingSession, await session.SessionSnapshot.DetachContext()).IfSome((Action<TrackedEventDataDescriptor>)delegate(TrackedEventDataDescriptor d)
			{
				CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(d);
			});
			if (simSessions != null)
			{
				CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(new SlotUsageDataDescriptor(simSessions.Simulators));
			}
			return Unit.Default;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnStartup_003Ed__9 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public CancellationToken shutdown;

		public LogLifecycleEventsStartupAction _003C_003E4__this;

		private ConfiguredTaskAwaiter<Unit> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Expected O, but got Unknown
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			LogLifecycleEventsStartupAction logLifecycleEventsStartupAction = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<Unit> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass9_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass9_0
					{
						shutdown = shutdown,
						_003C_003E4__this = _003C_003E4__this,
						powerCycles = 0,
						startSessionOffset = DateTimeOffset.Now
					};
					awaiter = logLifecycleEventsStartupAction._coordinator.CurrentlyRunning.GetUpdateToDateAndThenContinualValues().TakeUntil<ISimRunnerEvolved>(CS_0024_003C_003E8__locals0.shutdown).Select<ISimRunnerEvolved, IObservable<Unit>>((Func<ISimRunnerEvolved, IObservable<Unit>>)((ISimRunnerEvolved runner) => (!(runner is IAircraftSimRunner)) ? Observable.Empty<Unit>() : runner.Sessions.GetUpdateToDateAndThenContinualValues().TakeUntil<ISimSessionEvolved>(CS_0024_003C_003E8__locals0.shutdown).WhereClassNotNull<ISimSessionEvolved>()
						.Select<ISimSessionEvolved, IObservable<Unit>>((Func<ISimSessionEvolved, IObservable<Unit>>)delegate(ISimSessionEvolved session)
						{
							_003C_003Ec__DisplayClass9_1 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass9_1
							{
								CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals0,
								session = session
							};
							return CS_0024_003C_003E8__locals1.session.Stage.GetCachedThenUpdatedAndThenContinualValues().DistinctUntilChanged<PowerStage>().Publish<PowerStage, Unit>((IObservable<PowerStage> stages) => stages.TakeUntil<PowerStage>(CS_0024_003C_003E8__locals1.CS_0024_003C_003E8__locals1.shutdown).Where<PowerStage>((Func<PowerStage, bool>)((PowerStage s) => s == PowerStage.PoweredUp)).Do<PowerStage>((Action<PowerStage>)delegate
							{
								CS_0024_003C_003E8__locals1.CS_0024_003C_003E8__locals1.powerCycles++;
							})
								.Select<PowerStage, IObservable<Unit>>((Func<PowerStage, IObservable<Unit>>)((PowerStage _) => Observable.FromAsync<Unit>((Func<CancellationToken, System.Threading.Tasks.Task<Unit>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass9_1._003C_003COnStartup_003Eb__7_003Ed))] async (CancellationToken _) =>
								{
									//IL_0002: Unknown result type (might be due to invalid IL or missing references)
									//IL_0007: Unknown result type (might be due to invalid IL or missing references)
									ITrainingSession simSessions = await CS_0024_003C_003E8__locals1.session.TrainingSession.DetachContext();
									SimSettingsInjector getDataDescriptor = CS_0024_003C_003E8__locals1.CS_0024_003C_003E8__locals1._003C_003E4__this._getDataDescriptor;
									IActiveSetting navigationDatabaseSetting = CS_0024_003C_003E8__locals1.CS_0024_003C_003E8__locals1._003C_003E4__this._reactiveHangar.UnitOfWork.Settings.Globals.GetNavigationDatabaseSetting();
									ITrainingSession session2 = simSessions;
									getDataDescriptor(navigationDatabaseSetting, session2, await CS_0024_003C_003E8__locals1.session.SessionSnapshot.DetachContext()).IfSome((Action<TrackedEventDataDescriptor>)delegate(TrackedEventDataDescriptor d)
									{
										CS_0024_003C_003E8__locals1.CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(d);
									});
									if (simSessions != null)
									{
										CS_0024_003C_003E8__locals1.CS_0024_003C_003E8__locals1._003C_003E4__this._sendLog(new SlotUsageDataDescriptor(simSessions.Simulators));
									}
									return Unit.Default;
								}))))
								.Concat<Unit>());
						})
						.Merge<Unit>()))
						.Switch<Unit>()
						.OnCompleted<Unit>((Action)delegate
						{
							//IL_0012: Unknown result type (might be due to invalid IL or missing references)
							CS_0024_003C_003E8__locals0._003C_003E4__this._sendLog(new EndLauncherSessionDataDescriptor(CS_0024_003C_003E8__locals0.powerCycles, CS_0024_003C_003E8__locals0.startSessionOffset));
						})
						.LastOrDefaultAsync<Unit>()
						.ToTask<Unit>()
						.DetachContext()
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<Unit>, _003COnStartup_003Ed__9>(ref awaiter, ref this);
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

	private readonly IReactiveHangar _reactiveHangar;

	private readonly TrackedEvent.LogTrackedEvent _sendLog;

	private readonly SimSettingsInjector _getDataDescriptor;

	[field: CompilerGenerated]
	public bool IsPilot
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = true;


	public LogLifecycleEventsStartupAction(ISimSessionCoordinator coordinator, IReactiveHangar reactiveHangar, TrackedEvent.LogTrackedEvent sendLog, SimSettingsInjector getDataDescriptor)
	{
		_coordinator = coordinator;
		_reactiveHangar = reactiveHangar;
		_sendLog = sendLog;
		_getDataDescriptor = getDataDescriptor;
	}

	[AsyncStateMachine(typeof(_003COnStartup_003Ed__9))]
	public System.Threading.Tasks.Task OnStartup(CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003COnStartup_003Ed__9 _003COnStartup_003Ed__ = default(_003COnStartup_003Ed__9);
		_003COnStartup_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnStartup_003Ed__._003C_003E4__this = this;
		_003COnStartup_003Ed__.shutdown = shutdown;
		_003COnStartup_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Start<_003COnStartup_003Ed__9>(ref _003COnStartup_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Task;
	}
}
