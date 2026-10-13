using System;
using System.Diagnostics;
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
using LauncherCoreLib.Models.Simulated.Airport;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class BackgroundPowerUpStatsBehavior : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass2_0
	{
		public ISimRunnerEvolved runner;

		public BackgroundPowerUpStatsBehavior _003C_003E4__this;

		internal IObservable<Unit> _003COnStartup_003Eb__1(ISimSessionEvolved session)
		{
			_003C_003Ec__DisplayClass2_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass2_1
			{
				CS_0024_003C_003E8__locals1 = this,
				session = session
			};
			return CS_0024_003C_003E8__locals0.session.Stage.GetCachedThenUpdatedAndThenContinualValues().DistinctUntilChanged<PowerStage>().Where<PowerStage>((Func<PowerStage, bool>)((PowerStage stage) => stage == PowerStage.PoweredUp))
				.Take<PowerStage>(1)
				.SelectMany<PowerStage, Unit>((Func<PowerStage, System.Threading.Tasks.Task<Unit>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass2_1._003C_003COnStartup_003Eb__3_003Ed))] async (PowerStage _) =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_ = 1;
					try
					{
						IAircraftSimRunner aircraftSimRunner = (IAircraftSimRunner)CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.runner;
						ReactiveAircraft aircraft = aircraftSimRunner.Aircraft;
						AircraftSnapshotData aircraftSnapshotData = await CS_0024_003C_003E8__locals0.session.SessionSnapshot.DetachContext();
						await aircraft.UnitOfWork.PowerStats.IncrementPowerUpsForPanel(aircraftSnapshotData.Panel).DetachContext();
					}
					catch (System.Exception e)
					{
						CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1._003C_003E4__this.Log(e, "Updating power up stats.");
					}
					return Unit.Default;
				}));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass2_1
	{
		[StructLayout(3)]
		private struct _003C_003COnStartup_003Eb__3_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Unit> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass2_1 _003C_003E4__this;

			private ReactiveAircraft _003Caircraft_003E5__2;

			private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__1;

			private ConfiguredTaskAwaiter _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass2_1 _003C_003Ec__DisplayClass2_ = _003C_003E4__this;
				Unit @default;
				try
				{
					_ = 1;
					try
					{
						ConfiguredTaskAwaiter awaiter;
						ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter2;
						if (num != 0)
						{
							if (num == 1)
							{
								awaiter = _003C_003Eu__2;
								_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
								num = (_003C_003E1__state = -1);
								goto IL_011e;
							}
							IAircraftSimRunner aircraftSimRunner = (IAircraftSimRunner)_003C_003Ec__DisplayClass2_.CS_0024_003C_003E8__locals1.runner;
							_003Caircraft_003E5__2 = aircraftSimRunner.Aircraft;
							awaiter2 = _003C_003Ec__DisplayClass2_.session.SessionSnapshot.DetachContext().GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003C_003COnStartup_003Eb__3_003Ed>(ref awaiter2, ref this);
								return;
							}
						}
						else
						{
							awaiter2 = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
							num = (_003C_003E1__state = -1);
						}
						AircraftSnapshotData result = awaiter2.GetResult();
						ConfiguredTaskAwaitable val = _003Caircraft_003E5__2.UnitOfWork.PowerStats.IncrementPowerUpsForPanel(result.Panel).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003COnStartup_003Eb__3_003Ed>(ref awaiter, ref this);
							return;
						}
						goto IL_011e;
						IL_011e:
						((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
						_003Caircraft_003E5__2 = null;
					}
					catch (System.Exception e)
					{
						_003C_003Ec__DisplayClass2_.CS_0024_003C_003E8__locals1._003C_003E4__this.Log(e, "Updating power up stats.");
					}
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

		public ISimSessionEvolved session;

		public _003C_003Ec__DisplayClass2_0 CS_0024_003C_003E8__locals1;

		[AsyncStateMachine(typeof(_003C_003COnStartup_003Eb__3_003Ed))]
		internal async System.Threading.Tasks.Task<Unit> _003COnStartup_003Eb__3(PowerStage _)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			int num = 1;
			try
			{
				IAircraftSimRunner aircraftSimRunner = (IAircraftSimRunner)CS_0024_003C_003E8__locals1.runner;
				ReactiveAircraft aircraft = aircraftSimRunner.Aircraft;
				AircraftSnapshotData aircraftSnapshotData = await session.SessionSnapshot.DetachContext();
				await aircraft.UnitOfWork.PowerStats.IncrementPowerUpsForPanel(aircraftSnapshotData.Panel).DetachContext();
			}
			catch (System.Exception e)
			{
				CS_0024_003C_003E8__locals1._003C_003E4__this.Log(e, "Updating power up stats.");
			}
			return Unit.Default;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnStartup_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public BackgroundPowerUpStatsBehavior _003C_003E4__this;

		public CancellationToken shutdown;

		private ConfiguredTaskAwaiter<Unit> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			BackgroundPowerUpStatsBehavior CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<Unit> awaiter;
				if (num != 0)
				{
					awaiter = CS_0024_003C_003E8__locals0._coordinator.CurrentlyRunning.GetUpdateToDateAndThenContinualValues().Select<ISimRunnerEvolved, IObservable<Unit>>((Func<ISimRunnerEvolved, IObservable<Unit>>)([CompilerGenerated] (ISimRunnerEvolved runner) =>
					{
						_003C_003Ec__DisplayClass2_0 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass2_0
						{
							_003C_003E4__this = CS_0024_003C_003E8__locals0,
							runner = runner
						};
						return (!(CS_0024_003C_003E8__locals1.runner is IAircraftSimRunner)) ? Observable.Empty<Unit>() : CS_0024_003C_003E8__locals1.runner.Sessions.GetUpdateToDateAndThenContinualValues().WhereClassNotNull<ISimSessionEvolved>().Select<ISimSessionEvolved, IObservable<Unit>>((Func<ISimSessionEvolved, IObservable<Unit>>)delegate(ISimSessionEvolved session)
						{
							_003C_003Ec__DisplayClass2_1 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass2_1
							{
								CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals1,
								session = session
							};
							return CS_0024_003C_003E8__locals2.session.Stage.GetCachedThenUpdatedAndThenContinualValues().DistinctUntilChanged<PowerStage>().Where<PowerStage>((Func<PowerStage, bool>)((PowerStage stage) => stage == PowerStage.PoweredUp))
								.Take<PowerStage>(1)
								.SelectMany<PowerStage, Unit>((Func<PowerStage, System.Threading.Tasks.Task<Unit>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass2_1._003C_003COnStartup_003Eb__3_003Ed))] async (PowerStage _) =>
								{
									//IL_0002: Unknown result type (might be due to invalid IL or missing references)
									//IL_0007: Unknown result type (might be due to invalid IL or missing references)
									_ = 1;
									try
									{
										IAircraftSimRunner aircraftSimRunner = (IAircraftSimRunner)CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.runner;
										ReactiveAircraft aircraft = aircraftSimRunner.Aircraft;
										AircraftSnapshotData aircraftSnapshotData = await CS_0024_003C_003E8__locals2.session.SessionSnapshot.DetachContext();
										await aircraft.UnitOfWork.PowerStats.IncrementPowerUpsForPanel(aircraftSnapshotData.Panel).DetachContext();
									}
									catch (System.Exception e)
									{
										CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this.Log(e, "Updating power up stats.");
									}
									return Unit.Default;
								}));
						})
							.Switch<Unit>();
					})).Switch<Unit>()
						.TakeUntil<Unit>(shutdown)
						.LastOrDefaultAsync<Unit>()
						.ToTask<Unit>()
						.DetachContext()
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<Unit>, _003COnStartup_003Ed__2>(ref awaiter, ref this);
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

	public BackgroundPowerUpStatsBehavior(ISimSessionCoordinator coordinator)
	{
		_coordinator = coordinator;
	}

	[AsyncStateMachine(typeof(_003COnStartup_003Ed__2))]
	public System.Threading.Tasks.Task OnStartup(CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003COnStartup_003Ed__2 _003COnStartup_003Ed__ = default(_003COnStartup_003Ed__2);
		_003COnStartup_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnStartup_003Ed__._003C_003E4__this = this;
		_003COnStartup_003Ed__.shutdown = shutdown;
		_003COnStartup_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Start<_003COnStartup_003Ed__2>(ref _003COnStartup_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Task;
	}
}
