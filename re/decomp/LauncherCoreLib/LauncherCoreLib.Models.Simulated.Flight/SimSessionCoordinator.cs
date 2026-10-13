using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.Logging;
using LOLA.Processes;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Schedulers;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using LauncherCoreLib.Utility;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class SimSessionCoordinator : ISimSessionCoordinator
{
	private interface IAircraftBasedSimCoordinatorStrategy
	{
		IObservable<PowerStage> GetPowerStageStream(ReactiveAircraft aircraft);

		IObservable<bool> GetCanStartStream(ReactiveAircraft aircraft);

		IObservable<ISimSessionEvolved> GetSessionStream(ReactiveAircraft aircraft);

		ISimSessionEvolved StartAircraftSession(ReactiveAircraft aircraft);
	}

	private sealed class AircraftInternal : IAircraftBasedSimCoordinatorStrategy
	{
		private sealed class CurrentSession : ISimSessionEvolved
		{
			public interface ISimRunnerHarness
			{
				System.Threading.Tasks.Task<IFlightSession> Preflight(AircraftSnapshotData sessionSnapshot, CancellationToken shutdown);

				System.Threading.Tasks.Task<SimSession> TakeOff(IFlightSession flightSession, CancellationToken shutdown);

				System.Threading.Tasks.Task FlightDuration(SimSession simSession, CancellationToken shutdown);

				System.Threading.Tasks.Task Land(SimSession simSession);

				System.Threading.Tasks.Task PostFlight(IFlightSession flightSession);

				System.Threading.Tasks.Task Cleanup();
			}

			public class FunctionalSimRunnerHarness : ISimRunnerHarness
			{
				private readonly Func<AircraftSnapshotData, CancellationToken, System.Threading.Tasks.Task<IFlightSession>> _powerUp;

				private readonly Func<IFlightSession, CancellationToken, System.Threading.Tasks.Task<SimSession>> _takeOff;

				private readonly Func<SimSession, CancellationToken, System.Threading.Tasks.Task> _flight;

				private readonly Func<SimSession, System.Threading.Tasks.Task> _land;

				private readonly Func<IFlightSession, System.Threading.Tasks.Task> _powerDown;

				private readonly Func<System.Threading.Tasks.Task> _cleanup;

				public FunctionalSimRunnerHarness(Func<AircraftSnapshotData, CancellationToken, System.Threading.Tasks.Task<IFlightSession>> powerUp, Func<IFlightSession, CancellationToken, System.Threading.Tasks.Task<SimSession>> takeOff, Func<SimSession, CancellationToken, System.Threading.Tasks.Task> flight, Func<SimSession, System.Threading.Tasks.Task> land, Func<IFlightSession, System.Threading.Tasks.Task> powerDown, Func<System.Threading.Tasks.Task> cleanup)
				{
					_powerUp = powerUp;
					_takeOff = takeOff;
					_flight = flight;
					_land = land;
					_powerDown = powerDown;
					_cleanup = cleanup;
				}

				public System.Threading.Tasks.Task Cleanup()
				{
					return _cleanup.Invoke();
				}

				public System.Threading.Tasks.Task FlightDuration(SimSession simSession, CancellationToken shutdown)
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					return _flight.Invoke(simSession, shutdown);
				}

				public System.Threading.Tasks.Task Land(SimSession simSession)
				{
					return _land.Invoke(simSession);
				}

				public System.Threading.Tasks.Task PostFlight(IFlightSession flightSession)
				{
					return _powerDown.Invoke(flightSession);
				}

				public System.Threading.Tasks.Task<IFlightSession> Preflight(AircraftSnapshotData sessionSnapshot, CancellationToken shutdown)
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					return _powerUp.Invoke(sessionSnapshot, shutdown);
				}

				public System.Threading.Tasks.Task<SimSession> TakeOff(IFlightSession flightSession, CancellationToken shutdown)
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					return _takeOff.Invoke(flightSession, shutdown);
				}
			}

			[CompilerGenerated]
			private sealed class _003C_003Ec__DisplayClass32_0
			{
				[StructLayout(3)]
				private struct _003C_003C_002Dctor_003Eb__0_003Ed : IAsyncStateMachine
				{
					public int _003C_003E1__state;

					public AsyncTaskMethodBuilder<IFlightSession> _003C_003Et__builder;

					public _003C_003Ec__DisplayClass32_0 _003C_003E4__this;

					private ISimRunnerHarness _003C_003E7__wrap1;

					private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__1;

					private ConfiguredTaskAwaiter<IFlightSession> _003C_003Eu__2;

					private void MoveNext()
					{
						//IL_0072: Unknown result type (might be due to invalid IL or missing references)
						//IL_0077: Unknown result type (might be due to invalid IL or missing references)
						//IL_007f: Unknown result type (might be due to invalid IL or missing references)
						//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
						//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
						//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
						//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
						//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
						//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
						//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
						//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
						//IL_0034: Unknown result type (might be due to invalid IL or missing references)
						//IL_0039: Unknown result type (might be due to invalid IL or missing references)
						//IL_003d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0042: Unknown result type (might be due to invalid IL or missing references)
						//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
						//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
						//IL_0057: Unknown result type (might be due to invalid IL or missing references)
						//IL_0059: Unknown result type (might be due to invalid IL or missing references)
						int num = _003C_003E1__state;
						_003C_003Ec__DisplayClass32_0 _003C_003Ec__DisplayClass32_ = _003C_003E4__this;
						IFlightSession result3;
						try
						{
							_ = 1;
							try
							{
								ConfiguredTaskAwaiter<IFlightSession> awaiter;
								ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter2;
								if (num != 0)
								{
									if (num == 1)
									{
										awaiter = _003C_003Eu__2;
										_003C_003Eu__2 = default(ConfiguredTaskAwaiter<IFlightSession>);
										num = (_003C_003E1__state = -1);
										goto IL_0109;
									}
									_003C_003E7__wrap1 = _003C_003Ec__DisplayClass32_.harness;
									awaiter2 = _003C_003Ec__DisplayClass32_._003C_003E4__this.SessionSnapshot.DetachContext().GetAwaiter();
									if (!awaiter2.IsCompleted)
									{
										num = (_003C_003E1__state = 0);
										_003C_003Eu__1 = awaiter2;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003C_003C_002Dctor_003Eb__0_003Ed>(ref awaiter2, ref this);
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
								awaiter = _003C_003E7__wrap1.Preflight(result, _003C_003Ec__DisplayClass32_._003C_003E4__this._shutdown.Token).DetachContext().GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_003C_003E1__state = 1);
									_003C_003Eu__2 = awaiter;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IFlightSession>, _003C_003C_002Dctor_003Eb__0_003Ed>(ref awaiter, ref this);
									return;
								}
								goto IL_0109;
								IL_0109:
								IFlightSession result2 = awaiter.GetResult();
								_003C_003E7__wrap1 = null;
								result3 = result2;
							}
							catch (System.Exception e)
							{
								throw _003C_003Ec__DisplayClass32_._003C_003E4__this.CreatePowerException(e);
							}
						}
						catch (System.Exception exception)
						{
							_003C_003E1__state = -2;
							_003C_003Et__builder.SetException(exception);
							return;
						}
						_003C_003E1__state = -2;
						_003C_003Et__builder.SetResult(result3);
					}

					[DebuggerHidden]
					private void SetStateMachine(IAsyncStateMachine stateMachine)
					{
						_003C_003Et__builder.SetStateMachine(stateMachine);
					}
				}

				[StructLayout(3)]
				private struct _003C_003C_002Dctor_003Eb__1_003Ed : IAsyncStateMachine
				{
					public int _003C_003E1__state;

					public AsyncTaskMethodBuilder<SimSession> _003C_003Et__builder;

					public _003C_003Ec__DisplayClass32_0 _003C_003E4__this;

					public IFlightSession session;

					private ConfiguredTaskAwaiter<SimSession> _003C_003Eu__1;

					private void MoveNext()
					{
						//IL_0071: Unknown result type (might be due to invalid IL or missing references)
						//IL_0076: Unknown result type (might be due to invalid IL or missing references)
						//IL_007d: Unknown result type (might be due to invalid IL or missing references)
						//IL_002b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0035: Unknown result type (might be due to invalid IL or missing references)
						//IL_003a: Unknown result type (might be due to invalid IL or missing references)
						//IL_003e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0043: Unknown result type (might be due to invalid IL or missing references)
						//IL_0057: Unknown result type (might be due to invalid IL or missing references)
						//IL_0058: Unknown result type (might be due to invalid IL or missing references)
						int num = _003C_003E1__state;
						_003C_003Ec__DisplayClass32_0 _003C_003Ec__DisplayClass32_ = _003C_003E4__this;
						SimSession result;
						try
						{
							try
							{
								ConfiguredTaskAwaiter<SimSession> awaiter;
								if (num != 0)
								{
									awaiter = _003C_003Ec__DisplayClass32_.harness.TakeOff(session, _003C_003Ec__DisplayClass32_._003C_003E4__this._shutdown.Token).DetachContext().GetAwaiter();
									if (!awaiter.IsCompleted)
									{
										num = (_003C_003E1__state = 0);
										_003C_003Eu__1 = awaiter;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimSession>, _003C_003C_002Dctor_003Eb__1_003Ed>(ref awaiter, ref this);
										return;
									}
								}
								else
								{
									awaiter = _003C_003Eu__1;
									_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimSession>);
									num = (_003C_003E1__state = -1);
								}
								result = awaiter.GetResult() ?? throw new SimPowerException("Power up error: ", new GenericErrorException("Factory did not create a simulator session."))
								{
									Stage = PowerStage.PoweringUp
								};
							}
							catch (System.Exception e)
							{
								throw _003C_003Ec__DisplayClass32_._003C_003E4__this.CreatePowerException(e);
							}
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

				[StructLayout(3)]
				private struct _003C_003C_002Dctor_003Eb__3_003Ed : IAsyncStateMachine
				{
					public int _003C_003E1__state;

					public AsyncTaskMethodBuilder<SimSession> _003C_003Et__builder;

					public _003C_003Ec__DisplayClass32_0 _003C_003E4__this;

					public SimSession simSession;

					private ConfiguredTaskAwaiter _003C_003Eu__1;

					private void MoveNext()
					{
						//IL_0080: Unknown result type (might be due to invalid IL or missing references)
						//IL_0085: Unknown result type (might be due to invalid IL or missing references)
						//IL_008c: Unknown result type (might be due to invalid IL or missing references)
						//IL_003d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0047: Unknown result type (might be due to invalid IL or missing references)
						//IL_004c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0050: Unknown result type (might be due to invalid IL or missing references)
						//IL_0055: Unknown result type (might be due to invalid IL or missing references)
						//IL_0069: Unknown result type (might be due to invalid IL or missing references)
						//IL_006a: Unknown result type (might be due to invalid IL or missing references)
						int num = _003C_003E1__state;
						_003C_003Ec__DisplayClass32_0 _003C_003Ec__DisplayClass32_ = _003C_003E4__this;
						SimSession result;
						try
						{
							if (num != 0)
							{
								_003C_003Ec__DisplayClass32_._003C_003E4__this._powerStage.OnNext(PowerStage.PoweredUp);
							}
							try
							{
								ConfiguredTaskAwaiter awaiter;
								if (num != 0)
								{
									ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass32_.harness.FlightDuration(simSession, _003C_003Ec__DisplayClass32_._003C_003E4__this._shutdown.Token).DetachContext();
									awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
									if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
									{
										num = (_003C_003E1__state = 0);
										_003C_003Eu__1 = awaiter;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003C_002Dctor_003Eb__3_003Ed>(ref awaiter, ref this);
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
								result = simSession;
							}
							catch (System.Exception e)
							{
								throw _003C_003Ec__DisplayClass32_._003C_003E4__this.CreatePowerException(e);
							}
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

				[StructLayout(3)]
				private struct _003C_003C_002Dctor_003Eb__4_003Ed : IAsyncStateMachine
				{
					public int _003C_003E1__state;

					public AsyncTaskMethodBuilder _003C_003Et__builder;

					public _003C_003Ec__DisplayClass32_0 _003C_003E4__this;

					private SimSession _003CsimSession_003E5__2;

					private ConfiguredTaskAwaiter<SimSession> _003C_003Eu__1;

					private ConfiguredTaskAwaiter _003C_003Eu__2;

					private void MoveNext()
					{
						//IL_0085: Unknown result type (might be due to invalid IL or missing references)
						//IL_008a: Unknown result type (might be due to invalid IL or missing references)
						//IL_0091: Unknown result type (might be due to invalid IL or missing references)
						//IL_0049: Unknown result type (might be due to invalid IL or missing references)
						//IL_004e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0052: Unknown result type (might be due to invalid IL or missing references)
						//IL_0057: Unknown result type (might be due to invalid IL or missing references)
						//IL_0100: Unknown result type (might be due to invalid IL or missing references)
						//IL_0105: Unknown result type (might be due to invalid IL or missing references)
						//IL_010d: Unknown result type (might be due to invalid IL or missing references)
						//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
						//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
						//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
						//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
						//IL_006b: Unknown result type (might be due to invalid IL or missing references)
						//IL_006c: Unknown result type (might be due to invalid IL or missing references)
						//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
						//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
						int num = _003C_003E1__state;
						_003C_003Ec__DisplayClass32_0 _003C_003Ec__DisplayClass32_ = _003C_003E4__this;
						try
						{
							ConfiguredTaskAwaiter<SimSession> awaiter;
							if (num == 0)
							{
								awaiter = _003C_003Eu__1;
								_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimSession>);
								num = (_003C_003E1__state = -1);
								goto IL_00a0;
							}
							if (num == 1)
							{
								goto IL_00b0;
							}
							_003C_003Ec__DisplayClass32_._003C_003E4__this._powerStage.OnNext(PowerStage.PoweringDown);
							if (!((System.Threading.Tasks.Task)_003C_003Ec__DisplayClass32_._003C_003E4__this.TakeOff).IsFaulted)
							{
								awaiter = _003C_003Ec__DisplayClass32_._003C_003E4__this.TakeOff.DetachContext().GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_003C_003E1__state = 0);
									_003C_003Eu__1 = awaiter;
									((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimSession>, _003C_003C_002Dctor_003Eb__4_003Ed>(ref awaiter, ref this);
									return;
								}
								goto IL_00a0;
							}
							goto end_IL_000e;
							IL_00b0:
							try
							{
								ConfiguredTaskAwaiter awaiter2;
								if (num != 1)
								{
									ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass32_.harness.Land(_003CsimSession_003E5__2).DetachContext();
									awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
									if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
									{
										num = (_003C_003E1__state = 1);
										_003C_003Eu__2 = awaiter2;
										((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003C_002Dctor_003Eb__4_003Ed>(ref awaiter2, ref this);
										return;
									}
								}
								else
								{
									awaiter2 = _003C_003Eu__2;
									_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
									num = (_003C_003E1__state = -1);
								}
								((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
							}
							catch (System.Exception e)
							{
								throw _003C_003Ec__DisplayClass32_._003C_003E4__this.CreatePowerException(e);
							}
							finally
							{
								if (num < 0)
								{
									_003CsimSession_003E5__2.Dispose();
								}
							}
							_003CsimSession_003E5__2 = null;
							goto end_IL_000e;
							IL_00a0:
							SimSession result = awaiter.GetResult();
							_003CsimSession_003E5__2 = result;
							goto IL_00b0;
							end_IL_000e:;
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

				[StructLayout(3)]
				private struct _003C_003C_002Dctor_003Eb__5_003Ed : IAsyncStateMachine
				{
					public int _003C_003E1__state;

					public AsyncTaskMethodBuilder _003C_003Et__builder;

					public _003C_003Ec__DisplayClass32_0 _003C_003E4__this;

					private ISimRunnerHarness _003C_003E7__wrap1;

					private ConfiguredTaskAwaiter<IFlightSession> _003C_003Eu__1;

					private ConfiguredTaskAwaiter _003C_003Eu__2;

					private void MoveNext()
					{
						//IL_0085: Unknown result type (might be due to invalid IL or missing references)
						//IL_008a: Unknown result type (might be due to invalid IL or missing references)
						//IL_0091: Unknown result type (might be due to invalid IL or missing references)
						//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
						//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
						//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
						//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
						//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
						//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
						//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
						//IL_0049: Unknown result type (might be due to invalid IL or missing references)
						//IL_004e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0052: Unknown result type (might be due to invalid IL or missing references)
						//IL_0057: Unknown result type (might be due to invalid IL or missing references)
						//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
						//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
						//IL_006b: Unknown result type (might be due to invalid IL or missing references)
						//IL_006c: Unknown result type (might be due to invalid IL or missing references)
						int num = _003C_003E1__state;
						_003C_003Ec__DisplayClass32_0 _003C_003Ec__DisplayClass32_ = _003C_003E4__this;
						try
						{
							if ((uint)num <= 1u || !((System.Threading.Tasks.Task)_003C_003Ec__DisplayClass32_._003C_003E4__this.PowerUp).IsFaulted)
							{
								try
								{
									ConfiguredTaskAwaiter awaiter;
									ConfiguredTaskAwaiter<IFlightSession> awaiter2;
									if (num != 0)
									{
										if (num == 1)
										{
											awaiter = _003C_003Eu__2;
											_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
											num = (_003C_003E1__state = -1);
											goto IL_010b;
										}
										_003C_003E7__wrap1 = _003C_003Ec__DisplayClass32_.harness;
										awaiter2 = _003C_003Ec__DisplayClass32_._003C_003E4__this.PowerUp.DetachContext().GetAwaiter();
										if (!awaiter2.IsCompleted)
										{
											num = (_003C_003E1__state = 0);
											_003C_003Eu__1 = awaiter2;
											((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IFlightSession>, _003C_003C_002Dctor_003Eb__5_003Ed>(ref awaiter2, ref this);
											return;
										}
									}
									else
									{
										awaiter2 = _003C_003Eu__1;
										_003C_003Eu__1 = default(ConfiguredTaskAwaiter<IFlightSession>);
										num = (_003C_003E1__state = -1);
									}
									IFlightSession result = awaiter2.GetResult();
									ConfiguredTaskAwaitable val = _003C_003E7__wrap1.PostFlight(result).DetachContext();
									awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
									if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
									{
										num = (_003C_003E1__state = 1);
										_003C_003Eu__2 = awaiter;
										((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003C_002Dctor_003Eb__5_003Ed>(ref awaiter, ref this);
										return;
									}
									goto IL_010b;
									IL_010b:
									((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
									_003C_003E7__wrap1 = null;
								}
								catch (System.Exception e)
								{
									throw _003C_003Ec__DisplayClass32_._003C_003E4__this.CreatePowerException(e);
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

				[StructLayout(3)]
				private struct _003C_003C_002Dctor_003Eb__6_003Ed : IAsyncStateMachine
				{
					public int _003C_003E1__state;

					public AsyncTaskMethodBuilder _003C_003Et__builder;

					public _003C_003Ec__DisplayClass32_0 _003C_003E4__this;

					private ConfiguredTaskAwaiter _003C_003Eu__1;

					private void MoveNext()
					{
						//IL_005a: Unknown result type (might be due to invalid IL or missing references)
						//IL_005f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0066: Unknown result type (might be due to invalid IL or missing references)
						//IL_001f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0024: Unknown result type (might be due to invalid IL or missing references)
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						//IL_002c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0040: Unknown result type (might be due to invalid IL or missing references)
						//IL_0041: Unknown result type (might be due to invalid IL or missing references)
						int num = _003C_003E1__state;
						_003C_003Ec__DisplayClass32_0 _003C_003Ec__DisplayClass32_ = _003C_003E4__this;
						try
						{
							try
							{
								ConfiguredTaskAwaiter awaiter;
								if (num != 0)
								{
									ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass32_.harness.Cleanup().DetachContext();
									awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
									if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
									{
										num = (_003C_003E1__state = 0);
										_003C_003Eu__1 = awaiter;
										((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003C_002Dctor_003Eb__6_003Ed>(ref awaiter, ref this);
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
							}
							catch (System.Exception e)
							{
								throw _003C_003Ec__DisplayClass32_._003C_003E4__this.CreatePowerException(e);
							}
							finally
							{
								if (num < 0)
								{
									((System.IDisposable)_003C_003Ec__DisplayClass32_._003C_003E4__this._shutdown).Dispose();
									_003C_003Ec__DisplayClass32_._003C_003E4__this._powerStage.OnNext(PowerStage.PoweredDown);
									_003C_003Ec__DisplayClass32_._003C_003E4__this._powerStage.Dispose();
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

				public ISimRunnerHarness harness;

				public CurrentSession _003C_003E4__this;

				[AsyncStateMachine(typeof(_003C_003C_002Dctor_003Eb__0_003Ed))]
				internal async System.Threading.Tasks.Task<IFlightSession> _003C_002Ector_003Eb__0()
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_ = 1;
					try
					{
						ISimRunnerHarness simRunnerHarness = harness;
						return await simRunnerHarness.Preflight(await _003C_003E4__this.SessionSnapshot.DetachContext(), _003C_003E4__this._shutdown.Token).DetachContext();
					}
					catch (System.Exception e)
					{
						throw _003C_003E4__this.CreatePowerException(e);
					}
				}

				[AsyncStateMachine(typeof(_003C_003C_002Dctor_003Eb__1_003Ed))]
				internal async System.Threading.Tasks.Task<SimSession> _003C_002Ector_003Eb__1(IFlightSession session)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					try
					{
						return (await harness.TakeOff(session, _003C_003E4__this._shutdown.Token).DetachContext()) ?? throw new SimPowerException("Power up error: ", new GenericErrorException("Factory did not create a simulator session."))
						{
							Stage = PowerStage.PoweringUp
						};
					}
					catch (System.Exception e)
					{
						throw _003C_003E4__this.CreatePowerException(e);
					}
				}

				[AsyncStateMachine(typeof(_003C_003C_002Dctor_003Eb__3_003Ed))]
				internal async System.Threading.Tasks.Task<SimSession> _003C_002Ector_003Eb__3(SimSession simSession)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003E4__this._powerStage.OnNext(PowerStage.PoweredUp);
					try
					{
						await harness.FlightDuration(simSession, _003C_003E4__this._shutdown.Token).DetachContext();
						return simSession;
					}
					catch (System.Exception e)
					{
						throw _003C_003E4__this.CreatePowerException(e);
					}
				}

				[AsyncStateMachine(typeof(_003C_003C_002Dctor_003Eb__4_003Ed))]
				internal System.Threading.Tasks.Task _003C_002Ector_003Eb__4()
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003C_002Dctor_003Eb__4_003Ed _003C_003C_002Dctor_003Eb__4_003Ed = default(_003C_003C_002Dctor_003Eb__4_003Ed);
					_003C_003C_002Dctor_003Eb__4_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003C_002Dctor_003Eb__4_003Ed._003C_003E4__this = this;
					_003C_003C_002Dctor_003Eb__4_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eb__4_003Ed._003C_003Et__builder)).Start<_003C_003C_002Dctor_003Eb__4_003Ed>(ref _003C_003C_002Dctor_003Eb__4_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eb__4_003Ed._003C_003Et__builder)).Task;
				}

				[AsyncStateMachine(typeof(_003C_003C_002Dctor_003Eb__5_003Ed))]
				internal System.Threading.Tasks.Task _003C_002Ector_003Eb__5()
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003C_002Dctor_003Eb__5_003Ed _003C_003C_002Dctor_003Eb__5_003Ed = default(_003C_003C_002Dctor_003Eb__5_003Ed);
					_003C_003C_002Dctor_003Eb__5_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003C_002Dctor_003Eb__5_003Ed._003C_003E4__this = this;
					_003C_003C_002Dctor_003Eb__5_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eb__5_003Ed._003C_003Et__builder)).Start<_003C_003C_002Dctor_003Eb__5_003Ed>(ref _003C_003C_002Dctor_003Eb__5_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eb__5_003Ed._003C_003Et__builder)).Task;
				}

				[AsyncStateMachine(typeof(_003C_003C_002Dctor_003Eb__6_003Ed))]
				internal System.Threading.Tasks.Task _003C_002Ector_003Eb__6()
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003C_002Dctor_003Eb__6_003Ed _003C_003C_002Dctor_003Eb__6_003Ed = default(_003C_003C_002Dctor_003Eb__6_003Ed);
					_003C_003C_002Dctor_003Eb__6_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003C_002Dctor_003Eb__6_003Ed._003C_003E4__this = this;
					_003C_003C_002Dctor_003Eb__6_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eb__6_003Ed._003C_003Et__builder)).Start<_003C_003C_002Dctor_003Eb__6_003Ed>(ref _003C_003C_002Dctor_003Eb__6_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eb__6_003Ed._003C_003Et__builder)).Task;
				}
			}

			private readonly ICancellation _shutdown;

			private readonly BehaviorSubject<PowerStage> _powerStage = new BehaviorSubject<PowerStage>(PowerStage.PoweredDown);

			private readonly TaskCompletionSource<object?> _waitForStartToBeCalled = new TaskCompletionSource<object>();

			[field: CompilerGenerated]
			public ReactiveAircraft Aircraft
			{
				[CompilerGenerated]
				get;
			}

			[field: CompilerGenerated]
			public System.Threading.Tasks.Task<AircraftSnapshotData> SessionSnapshot
			{
				[CompilerGenerated]
				get;
			}

			[field: CompilerGenerated]
			public IReactiveEntity<PowerStage> Stage
			{
				[CompilerGenerated]
				get;
			}

			[field: CompilerGenerated]
			public System.Threading.Tasks.Task<ITrainingSession?> TrainingSession
			{
				[CompilerGenerated]
				get;
			}

			public System.Threading.Tasks.Task Completion => PowerDown;

			[field: CompilerGenerated]
			private System.Threading.Tasks.Task<IFlightSession> PowerUp
			{
				[CompilerGenerated]
				get;
			}

			[field: CompilerGenerated]
			private System.Threading.Tasks.Task<SimSession> TakeOff
			{
				[CompilerGenerated]
				get;
			}

			[field: CompilerGenerated]
			private System.Threading.Tasks.Task<SimSession> Flight
			{
				[CompilerGenerated]
				get;
			}

			[field: CompilerGenerated]
			private System.Threading.Tasks.Task Land
			{
				[CompilerGenerated]
				get;
			}

			[field: CompilerGenerated]
			private System.Threading.Tasks.Task PowerDown
			{
				[CompilerGenerated]
				get;
			}

			public CurrentSession(ReactiveAircraft aircraft, ISimRunnerHarness harness)
			{
				_003C_003Ec__DisplayClass32_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass32_0();
				CS_0024_003C_003E8__locals0.harness = harness;
				base._002Ector();
				CS_0024_003C_003E8__locals0._003C_003E4__this = this;
				Aircraft = aircraft;
				_shutdown = CancellationFactory.Create();
				SessionSnapshot = Aircraft.CompositeSnapshots.GetUpdatedValue();
				Stage = ReactiveEntity.WrapStreamWithCachedValues<PowerStage>((IObservable<PowerStage>)(object)_powerStage);
				PowerUp = _waitForStartToBeCalled.Task.Then<IFlightSession>([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__0_003Ed))] async () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_ = 1;
					try
					{
						ISimRunnerHarness harness2 = CS_0024_003C_003E8__locals0.harness;
						return await harness2.Preflight(await CS_0024_003C_003E8__locals0._003C_003E4__this.SessionSnapshot.DetachContext(), CS_0024_003C_003E8__locals0._003C_003E4__this._shutdown.Token).DetachContext();
					}
					catch (System.Exception e3)
					{
						throw CS_0024_003C_003E8__locals0._003C_003E4__this.CreatePowerException(e3);
					}
				});
				TakeOff = PowerUp.Then<IFlightSession, SimSession>((Func<IFlightSession, System.Threading.Tasks.Task<SimSession>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__1_003Ed))] async (IFlightSession session) =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					try
					{
						return (await CS_0024_003C_003E8__locals0.harness.TakeOff(session, CS_0024_003C_003E8__locals0._003C_003E4__this._shutdown.Token).DetachContext()) ?? throw new SimPowerException("Power up error: ", new GenericErrorException("Factory did not create a simulator session."))
						{
							Stage = PowerStage.PoweringUp
						};
					}
					catch (System.Exception e2)
					{
						throw CS_0024_003C_003E8__locals0._003C_003E4__this.CreatePowerException(e2);
					}
				}));
				TrainingSession = TakeOff.Then<SimSession, ITrainingSession>((Func<SimSession, ITrainingSession>)((SimSession s) => s));
				Flight = TakeOff.Then<SimSession, SimSession>((Func<SimSession, System.Threading.Tasks.Task<SimSession>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__3_003Ed))] async (SimSession simSession) =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					CS_0024_003C_003E8__locals0._003C_003E4__this._powerStage.OnNext(PowerStage.PoweredUp);
					try
					{
						await CS_0024_003C_003E8__locals0.harness.FlightDuration(simSession, CS_0024_003C_003E8__locals0._003C_003E4__this._shutdown.Token).DetachContext();
						return simSession;
					}
					catch (System.Exception e)
					{
						throw CS_0024_003C_003E8__locals0._003C_003E4__this.CreatePowerException(e);
					}
				}));
				Land = Flight.Finally((Func<System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__4_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__4_003Ed _003C_003C_002Dctor_003Eb__4_003Ed = default(_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__4_003Ed);
					_003C_003C_002Dctor_003Eb__4_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003C_002Dctor_003Eb__4_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
					_003C_003C_002Dctor_003Eb__4_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eb__4_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__4_003Ed>(ref _003C_003C_002Dctor_003Eb__4_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eb__4_003Ed._003C_003Et__builder)).Task;
				}));
				PowerDown = Land.Finally((Func<System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__5_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__5_003Ed _003C_003C_002Dctor_003Eb__5_003Ed = default(_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__5_003Ed);
					_003C_003C_002Dctor_003Eb__5_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003C_002Dctor_003Eb__5_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
					_003C_003C_002Dctor_003Eb__5_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eb__5_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__5_003Ed>(ref _003C_003C_002Dctor_003Eb__5_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eb__5_003Ed._003C_003Et__builder)).Task;
				})).Finally((Func<System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__6_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__6_003Ed _003C_003C_002Dctor_003Eb__6_003Ed = default(_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__6_003Ed);
					_003C_003C_002Dctor_003Eb__6_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003C_002Dctor_003Eb__6_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
					_003C_003C_002Dctor_003Eb__6_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eb__6_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass32_0._003C_003C_002Dctor_003Eb__6_003Ed>(ref _003C_003C_002Dctor_003Eb__6_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eb__6_003Ed._003C_003Et__builder)).Task;
				}));
			}

			public void Start()
			{
				if (!((System.Threading.Tasks.Task)_waitForStartToBeCalled.Task).IsCompleted)
				{
					_powerStage.OnNext(PowerStage.PoweringUp);
					_waitForStartToBeCalled.SetResult((object)null);
				}
			}

			public void InitiateSessionShutdown()
			{
				_shutdown.Cancel();
			}

			private static System.Exception CreatePowerException(PowerStage currentStage, System.Exception e)
			{
				return (System.Exception)(object)new SimPowerException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Power {0} error:", (object)((currentStage < PowerStage.PoweredUp) ? "up" : "down")), e)
				{
					Stage = currentStage
				};
			}

			private System.Exception CreatePowerException(System.Exception e)
			{
				return CreatePowerException(_powerStage.Value, e);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass14_0
		{
			[StructLayout(3)]
			private struct _003C_003CStartAircraftSession_003Eb__0_003Ed : IAsyncStateMachine
			{
				public int _003C_003E1__state;

				public AsyncTaskMethodBuilder<IFlightSession> _003C_003Et__builder;

				public AircraftSnapshotData snapshot;

				public _003C_003Ec__DisplayClass14_0 _003C_003E4__this;

				public CancellationToken shutdown;

				private System.Collections.Generic.IEnumerable<TrainerPackageIdentifier> _003CuniquePackageids_003E5__2;

				private ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage> _003Cpackages_003E5__3;

				private IFlightSession _003CflightSession_003E5__4;

				private ConfiguredTaskAwaiter<ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>> _003C_003Eu__1;

				private ConfiguredTaskAwaiter _003C_003Eu__2;

				private ConfiguredTaskAwaiter<IFlightSession> _003C_003Eu__3;

				private void MoveNext()
				{
					//IL_0105: Unknown result type (might be due to invalid IL or missing references)
					//IL_010a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0112: Unknown result type (might be due to invalid IL or missing references)
					//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
					//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
					//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
					//IL_026e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0273: Unknown result type (might be due to invalid IL or missing references)
					//IL_027b: Unknown result type (might be due to invalid IL or missing references)
					//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
					//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
					//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
					//IL_0226: Unknown result type (might be due to invalid IL or missing references)
					//IL_0230: Unknown result type (might be due to invalid IL or missing references)
					//IL_0235: Unknown result type (might be due to invalid IL or missing references)
					//IL_0239: Unknown result type (might be due to invalid IL or missing references)
					//IL_023e: Unknown result type (might be due to invalid IL or missing references)
					//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
					//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
					//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
					//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
					//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
					//IL_0154: Unknown result type (might be due to invalid IL or missing references)
					//IL_0253: Unknown result type (might be due to invalid IL or missing references)
					//IL_0255: Unknown result type (might be due to invalid IL or missing references)
					//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
					//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
					//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
					//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
					//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
					//IL_01af: Unknown result type (might be due to invalid IL or missing references)
					//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
					//IL_0190: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
					//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
					//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
					int num = _003C_003E1__state;
					_003C_003Ec__DisplayClass14_0 CS_0024_003C_003E8__locals0 = _003C_003E4__this;
					IFlightSession result3;
					try
					{
						ConfiguredTaskAwaiter<ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>> awaiter3;
						ConfiguredTaskAwaiter awaiter;
						ConfiguredTaskAwaiter<IFlightSession> awaiter2;
						FlightSessionRequestInfo sessionRequest;
						ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage> result;
						ConfiguredTaskAwaitable val;
						IFlightSession result2;
						switch (num)
						{
						default:
							_003CuniquePackageids_003E5__2 = (System.Collections.Generic.IEnumerable<TrainerPackageIdentifier>)Enumerable.ToHashSet<TrainerPackageIdentifier>(Enumerable.Select<SimInstallData, TrainerPackageIdentifier>(snapshot.Panel.ActiveSims, (Func<SimInstallData, TrainerPackageIdentifier>)((SimInstallData activeSim) => activeSim.FullPackageId)).WhereNotNull());
							awaiter3 = Enumerable.Select<TrainerPackageIdentifier, IObservable<ITrainerPackage>>(_003CuniquePackageids_003E5__2, (Func<TrainerPackageIdentifier, IObservable<ITrainerPackage>>)((TrainerPackageIdentifier pid) => CS_0024_003C_003E8__locals0.aircraft.UnitOfWork.Packages.GetPackageLineage(pid.EmbeddedReleaseCompositeId))).CombineLatestOrEmpty().Select<System.Collections.Generic.IList<ITrainerPackage>, ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>((Func<System.Collections.Generic.IList<ITrainerPackage>, ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>)((System.Collections.Generic.IList<ITrainerPackage> fullPackages) => ImmutableDictionary.ToImmutableDictionary<ITrainerPackage, TrainerPackageIdentifier>(((System.Collections.Generic.IEnumerable<ITrainerPackage?>)fullPackages).WhereClassNotNull(), (Func<ITrainerPackage, TrainerPackageIdentifier>)((ITrainerPackage p) => p.TrainerPackageId))))
								.FirstAsync<ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>()
								.ToTask<ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>()
								.DetachContext()
								.GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter3;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>, _003C_003CStartAircraftSession_003Eb__0_003Ed>(ref awaiter3, ref this);
								return;
							}
							goto IL_0121;
						case 0:
							awaiter3 = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>);
							num = (_003C_003E1__state = -1);
							goto IL_0121;
						case 1:
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_0200;
						case 2:
							awaiter2 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter<IFlightSession>);
							num = (_003C_003E1__state = -1);
							goto IL_028a;
						case 3:
							{
								awaiter = _003C_003Eu__2;
								_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
								num = (_003C_003E1__state = -1);
								break;
							}
							IL_0200:
							((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
							sessionRequest = new FlightSessionRequestInfo(snapshot, (IReadOnlyDictionary<TrainerPackageIdentifier, ITrainerPackage>)(object)_003Cpackages_003E5__3);
							awaiter2 = CS_0024_003C_003E8__locals0._003C_003E4__this._flightSessionFactory.CreateSession(sessionRequest, shutdown).DetachContext().GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IFlightSession>, _003C_003CStartAircraftSession_003Eb__0_003Ed>(ref awaiter2, ref this);
								return;
							}
							goto IL_028a;
							IL_0121:
							result = awaiter3.GetResult();
							_003Cpackages_003E5__3 = result;
							if (Enumerable.Any<TrainerPackageIdentifier>((System.Collections.Generic.IEnumerable<TrainerPackageIdentifier>)_003Cpackages_003E5__3.Keys.SymmetricDifference(_003CuniquePackageids_003E5__2)))
							{
								throw new InvalidOperationException("Some aircraft package references could not be found.");
							}
							if (Enumerable.Any<ITrainerPackage>(_003Cpackages_003E5__3.Values, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => !p.BaseInfo.PackageType.IsCompleteOnDisk())))
							{
								throw new InvalidOperationException("Some packages referenced in the aircraft have not been downloaded yet.");
							}
							val = CS_0024_003C_003E8__locals0.aircraft.UnitOfWork.FlushEverything().DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CStartAircraftSession_003Eb__0_003Ed>(ref awaiter, ref this);
								return;
							}
							goto IL_0200;
							IL_028a:
							result2 = awaiter2.GetResult();
							_003CflightSession_003E5__4 = result2;
							val = _003CflightSession_003E5__4.PreFlight(shutdown).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 3);
								_003C_003Eu__2 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CStartAircraftSession_003Eb__0_003Ed>(ref awaiter, ref this);
								return;
							}
							break;
						}
						((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
						result3 = _003CflightSession_003E5__4;
					}
					catch (System.Exception exception)
					{
						_003C_003E1__state = -2;
						_003CuniquePackageids_003E5__2 = null;
						_003Cpackages_003E5__3 = null;
						_003CflightSession_003E5__4 = null;
						_003C_003Et__builder.SetException(exception);
						return;
					}
					_003C_003E1__state = -2;
					_003CuniquePackageids_003E5__2 = null;
					_003Cpackages_003E5__3 = null;
					_003CflightSession_003E5__4 = null;
					_003C_003Et__builder.SetResult(result3);
				}

				[DebuggerHidden]
				private void SetStateMachine(IAsyncStateMachine stateMachine)
				{
					_003C_003Et__builder.SetStateMachine(stateMachine);
				}
			}

			[StructLayout(3)]
			private struct _003C_003CStartAircraftSession_003Eb__2_003Ed : IAsyncStateMachine
			{
				public int _003C_003E1__state;

				public AsyncTaskMethodBuilder _003C_003Et__builder;

				public _003C_003Ec__DisplayClass14_0 _003C_003E4__this;

				public SimSession session;

				public CancellationToken shutdown;

				private ConfiguredTaskAwaiter<System.Threading.Tasks.Task> _003C_003Eu__1;

				private ConfiguredTaskAwaiter _003C_003Eu__2;

				private void MoveNext()
				{
					//IL_0079: Unknown result type (might be due to invalid IL or missing references)
					//IL_007e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0085: Unknown result type (might be due to invalid IL or missing references)
					//IL_009b: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
					//IL_00db: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
					//IL_002f: Unknown result type (might be due to invalid IL or missing references)
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0043: Unknown result type (might be due to invalid IL or missing references)
					//IL_0046: Unknown result type (might be due to invalid IL or missing references)
					//IL_004b: Unknown result type (might be due to invalid IL or missing references)
					//IL_00be: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
					//IL_005f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0060: Unknown result type (might be due to invalid IL or missing references)
					int num = _003C_003E1__state;
					_003C_003Ec__DisplayClass14_0 _003C_003Ec__DisplayClass14_ = _003C_003E4__this;
					try
					{
						ConfiguredTaskAwaiter awaiter;
						ConfiguredTaskAwaiter<System.Threading.Tasks.Task> awaiter2;
						if (num != 0)
						{
							if (num == 1)
							{
								awaiter = _003C_003Eu__2;
								_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
								num = (_003C_003E1__state = -1);
								goto IL_00f2;
							}
							awaiter2 = System.Threading.Tasks.Task.WhenAny(_003C_003Ec__DisplayClass14_._003C_003E4__this._automaticShutdownStrategy.AutomaticShutdownEvent(session), shutdown.UntilCancelled()).DetachContext().GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter2;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Threading.Tasks.Task>, _003C_003CStartAircraftSession_003Eb__2_003Ed>(ref awaiter2, ref this);
								return;
							}
						}
						else
						{
							awaiter2 = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Threading.Tasks.Task>);
							num = (_003C_003E1__state = -1);
						}
						ConfiguredTaskAwaitable val = awaiter2.GetResult().DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CStartAircraftSession_003Eb__2_003Ed>(ref awaiter, ref this);
							return;
						}
						goto IL_00f2;
						IL_00f2:
						((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

			[StructLayout(3)]
			private struct _003C_003CStartAircraftSession_003Eb__3_003Ed : IAsyncStateMachine
			{
				public int _003C_003E1__state;

				public AsyncTaskMethodBuilder _003C_003Et__builder;

				public SimSession session;

				public _003C_003Ec__DisplayClass14_0 _003C_003E4__this;

				private System.Threading.Tasks.Task _003CsimsExited_003E5__2;

				private DateTimeOffset _003CsimCancelTime_003E5__3;

				private System.Threading.Tasks.Task<ProcessResult> _003CrouterExited_003E5__4;

				private DateTimeOffset _003CrouterCancelTime_003E5__5;

				private ConfiguredTaskAwaiter<System.Threading.Tasks.Task> _003C_003Eu__1;

				private void MoveNext()
				{
					//IL_0103: Unknown result type (might be due to invalid IL or missing references)
					//IL_0108: Unknown result type (might be due to invalid IL or missing references)
					//IL_010f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0263: Unknown result type (might be due to invalid IL or missing references)
					//IL_0268: Unknown result type (might be due to invalid IL or missing references)
					//IL_026f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0038: Unknown result type (might be due to invalid IL or missing references)
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0146: Unknown result type (might be due to invalid IL or missing references)
					//IL_014c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0151: Unknown result type (might be due to invalid IL or missing references)
					//IL_0052: Unknown result type (might be due to invalid IL or missing references)
					//IL_0058: Unknown result type (might be due to invalid IL or missing references)
					//IL_005d: Unknown result type (might be due to invalid IL or missing references)
					//IL_006b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0198: Unknown result type (might be due to invalid IL or missing references)
					//IL_019d: Unknown result type (might be due to invalid IL or missing references)
					//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
					//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
					//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
					//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
					//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
					//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
					//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
					//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
					//IL_0209: Unknown result type (might be due to invalid IL or missing references)
					//IL_0228: Unknown result type (might be due to invalid IL or missing references)
					//IL_022d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0230: Unknown result type (might be due to invalid IL or missing references)
					//IL_0235: Unknown result type (might be due to invalid IL or missing references)
					//IL_0249: Unknown result type (might be due to invalid IL or missing references)
					//IL_024a: Unknown result type (might be due to invalid IL or missing references)
					int num = _003C_003E1__state;
					_003C_003Ec__DisplayClass14_0 _003C_003Ec__DisplayClass14_ = _003C_003E4__this;
					try
					{
						ConfiguredTaskAwaiter<System.Threading.Tasks.Task> awaiter;
						if (num != 0)
						{
							if (num != 1)
							{
								_003CsimsExited_003E5__2 = session.AllSimsExited;
								_003CsimCancelTime_003E5__3 = _003C_003Ec__DisplayClass14_._003C_003E4__this._rxScheduler.Now;
								goto IL_0161;
							}
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Threading.Tasks.Task>);
							num = (_003C_003E1__state = -1);
							goto IL_027e;
						}
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Threading.Tasks.Task>);
						num = (_003C_003E1__state = -1);
						goto IL_011e;
						IL_027e:
						if (awaiter.GetResult() != _003CsimsExited_003E5__2)
						{
							Logger.Log(LogLevel.Info, "The router is still running after {0}. Attempting to close or kill again.", _003C_003Ec__DisplayClass14_._003C_003E4__this._rxScheduler.Now - _003CrouterCancelTime_003E5__5);
						}
						goto IL_02c1;
						IL_0161:
						if (!_003CsimsExited_003E5__2.IsCompleted)
						{
							if (_003C_003Ec__DisplayClass14_._003C_003E4__this._rxScheduler.Now - _003CsimCancelTime_003E5__3 >= TimeSpan.FromSeconds(5.0))
							{
								Logger.Log(LogLevel.Warning, "Simulators are taking too long to close. Attempting to force kill them.");
								session.ForceCloseAll();
							}
							else
							{
								session.CloseAllSims();
							}
							awaiter = System.Threading.Tasks.Task.WhenAny(_003CsimsExited_003E5__2, (System.Threading.Tasks.Task)Observable.Timer(TimeSpan.FromMilliseconds(500.0), _003C_003Ec__DisplayClass14_._003C_003E4__this._rxScheduler).ToTask<long>()).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Threading.Tasks.Task>, _003C_003CStartAircraftSession_003Eb__3_003Ed>(ref awaiter, ref this);
								return;
							}
							goto IL_011e;
						}
						_003CrouterExited_003E5__4 = session.Router.RouterProcess.Exited;
						_003CrouterCancelTime_003E5__5 = _003C_003Ec__DisplayClass14_._003C_003E4__this._rxScheduler.Now;
						goto IL_02c1;
						IL_011e:
						if (awaiter.GetResult() != _003CsimsExited_003E5__2)
						{
							Logger.Log(LogLevel.Info, "Simulators are still running after {0}. Attempting to close or kill again.", _003C_003Ec__DisplayClass14_._003C_003E4__this._rxScheduler.Now - _003CsimCancelTime_003E5__3);
						}
						goto IL_0161;
						IL_02c1:
						if (!((System.Threading.Tasks.Task)_003CrouterExited_003E5__4).IsCompleted)
						{
							if (_003C_003Ec__DisplayClass14_._003C_003E4__this._rxScheduler.Now - _003CrouterCancelTime_003E5__5 >= TimeSpan.FromSeconds(3.0))
							{
								Logger.Log(LogLevel.Warning, "The router is taking too long to close. Attempting to force kill it.");
								session.ForceCloseAll();
							}
							else
							{
								session.CloseRouter();
							}
							awaiter = System.Threading.Tasks.Task.WhenAny((System.Threading.Tasks.Task)_003CrouterExited_003E5__4, (System.Threading.Tasks.Task)Observable.Timer(TimeSpan.FromMilliseconds(500.0), _003C_003Ec__DisplayClass14_._003C_003E4__this._rxScheduler).ToTask<long>()).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Threading.Tasks.Task>, _003C_003CStartAircraftSession_003Eb__3_003Ed>(ref awaiter, ref this);
								return;
							}
							goto IL_027e;
						}
					}
					catch (System.Exception exception)
					{
						_003C_003E1__state = -2;
						_003CsimsExited_003E5__2 = null;
						_003CrouterExited_003E5__4 = null;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
						return;
					}
					_003C_003E1__state = -2;
					_003CsimsExited_003E5__2 = null;
					_003CrouterExited_003E5__4 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
				}

				[DebuggerHidden]
				private void SetStateMachine(IAsyncStateMachine stateMachine)
				{
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
				}
			}

			public ReactiveAircraft aircraft;

			public AircraftInternal _003C_003E4__this;

			public CurrentSession cs;

			public Func<TrainerPackageIdentifier, IObservable<ITrainerPackage>> _003C_003E9__8;

			[AsyncStateMachine(typeof(_003C_003CStartAircraftSession_003Eb__0_003Ed))]
			internal async System.Threading.Tasks.Task<IFlightSession> _003CStartAircraftSession_003Eb__0(AircraftSnapshotData snapshot, CancellationToken shutdown)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				System.Collections.Generic.IEnumerable<TrainerPackageIdentifier> uniquePackageids = (System.Collections.Generic.IEnumerable<TrainerPackageIdentifier>)Enumerable.ToHashSet<TrainerPackageIdentifier>(Enumerable.Select<SimInstallData, TrainerPackageIdentifier>(snapshot.Panel.ActiveSims, (Func<SimInstallData, TrainerPackageIdentifier>)((SimInstallData activeSim) => activeSim.FullPackageId)).WhereNotNull());
				ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage> packages = await Enumerable.Select<TrainerPackageIdentifier, IObservable<ITrainerPackage>>(uniquePackageids, (Func<TrainerPackageIdentifier, IObservable<ITrainerPackage>>)((TrainerPackageIdentifier pid) => aircraft.UnitOfWork.Packages.GetPackageLineage(pid.EmbeddedReleaseCompositeId))).CombineLatestOrEmpty().Select<System.Collections.Generic.IList<ITrainerPackage>, ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>((Func<System.Collections.Generic.IList<ITrainerPackage>, ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>)((System.Collections.Generic.IList<ITrainerPackage> fullPackages) => ImmutableDictionary.ToImmutableDictionary<ITrainerPackage, TrainerPackageIdentifier>(((System.Collections.Generic.IEnumerable<ITrainerPackage?>)fullPackages).WhereClassNotNull(), (Func<ITrainerPackage, TrainerPackageIdentifier>)((ITrainerPackage p) => p.TrainerPackageId))))
					.FirstAsync<ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>()
					.ToTask<ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>()
					.DetachContext();
				if (Enumerable.Any<TrainerPackageIdentifier>((System.Collections.Generic.IEnumerable<TrainerPackageIdentifier>)packages.Keys.SymmetricDifference(uniquePackageids)))
				{
					throw new InvalidOperationException("Some aircraft package references could not be found.");
				}
				if (Enumerable.Any<ITrainerPackage>(packages.Values, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => !p.BaseInfo.PackageType.IsCompleteOnDisk())))
				{
					throw new InvalidOperationException("Some packages referenced in the aircraft have not been downloaded yet.");
				}
				await aircraft.UnitOfWork.FlushEverything().DetachContext();
				FlightSessionRequestInfo sessionRequest = new FlightSessionRequestInfo(snapshot, (IReadOnlyDictionary<TrainerPackageIdentifier, ITrainerPackage>)(object)packages);
				IFlightSession flightSession = await _003C_003E4__this._flightSessionFactory.CreateSession(sessionRequest, shutdown).DetachContext();
				await flightSession.PreFlight(shutdown).DetachContext();
				return flightSession;
			}

			internal IObservable<ITrainerPackage> _003CStartAircraftSession_003Eb__8(TrainerPackageIdentifier pid)
			{
				return aircraft.UnitOfWork.Packages.GetPackageLineage(pid.EmbeddedReleaseCompositeId);
			}

			[AsyncStateMachine(typeof(_003C_003CStartAircraftSession_003Eb__2_003Ed))]
			internal System.Threading.Tasks.Task _003CStartAircraftSession_003Eb__2(SimSession session, CancellationToken shutdown)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				_003C_003CStartAircraftSession_003Eb__2_003Ed _003C_003CStartAircraftSession_003Eb__2_003Ed = default(_003C_003CStartAircraftSession_003Eb__2_003Ed);
				_003C_003CStartAircraftSession_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003CStartAircraftSession_003Eb__2_003Ed._003C_003E4__this = this;
				_003C_003CStartAircraftSession_003Eb__2_003Ed.session = session;
				_003C_003CStartAircraftSession_003Eb__2_003Ed.shutdown = shutdown;
				_003C_003CStartAircraftSession_003Eb__2_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003CStartAircraftSession_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003CStartAircraftSession_003Eb__2_003Ed>(ref _003C_003CStartAircraftSession_003Eb__2_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003CStartAircraftSession_003Eb__2_003Ed._003C_003Et__builder)).Task;
			}

			[AsyncStateMachine(typeof(_003C_003CStartAircraftSession_003Eb__3_003Ed))]
			internal System.Threading.Tasks.Task _003CStartAircraftSession_003Eb__3(SimSession session)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				_003C_003CStartAircraftSession_003Eb__3_003Ed _003C_003CStartAircraftSession_003Eb__3_003Ed = default(_003C_003CStartAircraftSession_003Eb__3_003Ed);
				_003C_003CStartAircraftSession_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003CStartAircraftSession_003Eb__3_003Ed._003C_003E4__this = this;
				_003C_003CStartAircraftSession_003Eb__3_003Ed.session = session;
				_003C_003CStartAircraftSession_003Eb__3_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003CStartAircraftSession_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003CStartAircraftSession_003Eb__3_003Ed>(ref _003C_003CStartAircraftSession_003Eb__3_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003CStartAircraftSession_003Eb__3_003Ed._003C_003Et__builder)).Task;
			}

			internal System.Threading.Tasks.Task _003CStartAircraftSession_003Eb__5()
			{
				lock (_003C_003E4__this._lock)
				{
					if (_003C_003E4__this._currentlyRunning.Value == cs)
					{
						_003C_003E4__this._currentlyRunning.OnNext(null);
					}
				}
				return System.Threading.Tasks.Task.CompletedTask;
			}
		}

		private readonly IAirworthiness _airworthiness;

		private readonly IFlightSessionFactory _flightSessionFactory;

		private readonly IAutomaticShutdownStrategy _automaticShutdownStrategy;

		private readonly IScheduler _rxScheduler;

		private readonly object _lock = new object();

		private readonly BehaviorSubject<CurrentSession?> _currentlyRunning;

		public IObservable<ReactiveAircraft?> CurrentlyRunning => ((IObservable<CurrentSession>)(object)_currentlyRunning).Select<CurrentSession, ReactiveAircraft>((Func<CurrentSession, ReactiveAircraft>)((CurrentSession cr) => cr?.Aircraft));

		private ReactiveAircraft? GetCurrentAircraftWithLock => _currentlyRunning.Value?.Aircraft;

		public AircraftInternal(IAirworthiness canRunProvider, IFlightSessionFactory flightSessionFactory, IAutomaticShutdownStrategy automaticShutdownStrategy, IScheduler rxScheduler)
		{
			_airworthiness = canRunProvider;
			_flightSessionFactory = flightSessionFactory;
			_automaticShutdownStrategy = automaticShutdownStrategy;
			_rxScheduler = rxScheduler;
			_currentlyRunning = new BehaviorSubject<CurrentSession>(null);
		}

		public IObservable<bool> GetCanStartStream(ReactiveAircraft aircraft)
		{
			return _airworthiness.IsAirworthyOverTime(aircraft).DistinctUntilChanged<bool>();
		}

		public IObservable<PowerStage> GetPowerStageStream(ReactiveAircraft aircraft)
		{
			ReactiveAircraft aircraft2 = aircraft;
			return ((IObservable<CurrentSession>)(object)_currentlyRunning).Select<CurrentSession, IObservable<PowerStage>>((Func<CurrentSession, IObservable<PowerStage>>)((CurrentSession cr) => (cr == null || !(cr.Aircraft.AircraftIdentifier == aircraft2.AircraftIdentifier)) ? Observable.Return(PowerStage.PoweredDown).Concat<PowerStage>(Observable.Never<PowerStage>()) : cr.Stage.GetCacheThenContinualValues())).Switch<PowerStage>().DistinctUntilChanged<PowerStage>();
		}

		public IObservable<ISimSessionEvolved> GetSessionStream(ReactiveAircraft aircraft)
		{
			ReactiveAircraft aircraft2 = aircraft;
			return (IObservable<ISimSessionEvolved>)(object)((IObservable<CurrentSession>)(object)_currentlyRunning).Where<CurrentSession>((Func<CurrentSession, bool>)((CurrentSession cr) => cr != null && cr.Aircraft.AircraftIdentifier == aircraft2.AircraftIdentifier));
		}

		public ISimSessionEvolved StartAircraftSession(ReactiveAircraft aircraft)
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass14_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass14_0();
			CS_0024_003C_003E8__locals0.aircraft = aircraft;
			CS_0024_003C_003E8__locals0._003C_003E4__this = this;
			CS_0024_003C_003E8__locals0.cs = null;
			lock (_lock)
			{
				ReactiveAircraft getCurrentAircraftWithLock = GetCurrentAircraftWithLock;
				if (getCurrentAircraftWithLock != null)
				{
					if (!(getCurrentAircraftWithLock.AircraftIdentifier == CS_0024_003C_003E8__locals0.aircraft.AircraftIdentifier))
					{
						throw new InvalidOperationException("Another aircraft is powered on.");
					}
					CS_0024_003C_003E8__locals0.cs = _currentlyRunning.Value;
				}
				if (CS_0024_003C_003E8__locals0.cs == null)
				{
					CS_0024_003C_003E8__locals0.cs = new CurrentSession(CS_0024_003C_003E8__locals0.aircraft, new CurrentSession.FunctionalSimRunnerHarness([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass14_0._003C_003CStartAircraftSession_003Eb__0_003Ed))] async (AircraftSnapshotData snapshot, CancellationToken shutdown) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_001e: Unknown result type (might be due to invalid IL or missing references)
						//IL_001f: Unknown result type (might be due to invalid IL or missing references)
						System.Collections.Generic.IEnumerable<TrainerPackageIdentifier> uniquePackageids = (System.Collections.Generic.IEnumerable<TrainerPackageIdentifier>)Enumerable.ToHashSet<TrainerPackageIdentifier>(Enumerable.Select<SimInstallData, TrainerPackageIdentifier>(snapshot.Panel.ActiveSims, (Func<SimInstallData, TrainerPackageIdentifier>)((SimInstallData activeSim) => activeSim.FullPackageId)).WhereNotNull());
						ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage> packages = await Enumerable.Select<TrainerPackageIdentifier, IObservable<ITrainerPackage>>(uniquePackageids, (Func<TrainerPackageIdentifier, IObservable<ITrainerPackage>>)((TrainerPackageIdentifier pid) => CS_0024_003C_003E8__locals0.aircraft.UnitOfWork.Packages.GetPackageLineage(pid.EmbeddedReleaseCompositeId))).CombineLatestOrEmpty().Select<System.Collections.Generic.IList<ITrainerPackage>, ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>((Func<System.Collections.Generic.IList<ITrainerPackage>, ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>)((System.Collections.Generic.IList<ITrainerPackage> fullPackages) => ImmutableDictionary.ToImmutableDictionary<ITrainerPackage, TrainerPackageIdentifier>(((System.Collections.Generic.IEnumerable<ITrainerPackage?>)fullPackages).WhereClassNotNull(), (Func<ITrainerPackage, TrainerPackageIdentifier>)((ITrainerPackage p) => p.TrainerPackageId))))
							.FirstAsync<ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>()
							.ToTask<ImmutableDictionary<TrainerPackageIdentifier, ITrainerPackage>>()
							.DetachContext();
						if (Enumerable.Any<TrainerPackageIdentifier>((System.Collections.Generic.IEnumerable<TrainerPackageIdentifier>)packages.Keys.SymmetricDifference(uniquePackageids)))
						{
							throw new InvalidOperationException("Some aircraft package references could not be found.");
						}
						if (Enumerable.Any<ITrainerPackage>(packages.Values, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => !p.BaseInfo.PackageType.IsCompleteOnDisk())))
						{
							throw new InvalidOperationException("Some packages referenced in the aircraft have not been downloaded yet.");
						}
						await CS_0024_003C_003E8__locals0.aircraft.UnitOfWork.FlushEverything().DetachContext();
						FlightSessionRequestInfo sessionRequest = new FlightSessionRequestInfo(snapshot, (IReadOnlyDictionary<TrainerPackageIdentifier, ITrainerPackage>)(object)packages);
						IFlightSession flightSession2 = await CS_0024_003C_003E8__locals0._003C_003E4__this._flightSessionFactory.CreateSession(sessionRequest, shutdown).DetachContext();
						await flightSession2.PreFlight(shutdown).DetachContext();
						return flightSession2;
					}, [AsyncStateMachine(typeof(_003C_003Ec._003C_003CStartAircraftSession_003Eb__14_1_003Ed))] async (IFlightSession flight, CancellationToken shutdown) => await flight.TakeOff().DetachContext(), [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass14_0._003C_003CStartAircraftSession_003Eb__2_003Ed))] (SimSession session, CancellationToken shutdown) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_001e: Unknown result type (might be due to invalid IL or missing references)
						//IL_001f: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec__DisplayClass14_0._003C_003CStartAircraftSession_003Eb__2_003Ed _003C_003CStartAircraftSession_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass14_0._003C_003CStartAircraftSession_003Eb__2_003Ed);
						_003C_003CStartAircraftSession_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
						_003C_003CStartAircraftSession_003Eb__2_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
						_003C_003CStartAircraftSession_003Eb__2_003Ed.session = session;
						_003C_003CStartAircraftSession_003Eb__2_003Ed.shutdown = shutdown;
						_003C_003CStartAircraftSession_003Eb__2_003Ed._003C_003E1__state = -1;
						((AsyncTaskMethodBuilder)(ref _003C_003CStartAircraftSession_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass14_0._003C_003CStartAircraftSession_003Eb__2_003Ed>(ref _003C_003CStartAircraftSession_003Eb__2_003Ed);
						return ((AsyncTaskMethodBuilder)(ref _003C_003CStartAircraftSession_003Eb__2_003Ed._003C_003Et__builder)).Task;
					}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass14_0._003C_003CStartAircraftSession_003Eb__3_003Ed))] (SimSession session) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec__DisplayClass14_0._003C_003CStartAircraftSession_003Eb__3_003Ed _003C_003CStartAircraftSession_003Eb__3_003Ed = default(_003C_003Ec__DisplayClass14_0._003C_003CStartAircraftSession_003Eb__3_003Ed);
						_003C_003CStartAircraftSession_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
						_003C_003CStartAircraftSession_003Eb__3_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
						_003C_003CStartAircraftSession_003Eb__3_003Ed.session = session;
						_003C_003CStartAircraftSession_003Eb__3_003Ed._003C_003E1__state = -1;
						((AsyncTaskMethodBuilder)(ref _003C_003CStartAircraftSession_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass14_0._003C_003CStartAircraftSession_003Eb__3_003Ed>(ref _003C_003CStartAircraftSession_003Eb__3_003Ed);
						return ((AsyncTaskMethodBuilder)(ref _003C_003CStartAircraftSession_003Eb__3_003Ed._003C_003Et__builder)).Task;
					}, [AsyncStateMachine(typeof(_003C_003Ec._003C_003CStartAircraftSession_003Eb__14_4_003Ed))] (IFlightSession flightSession) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec._003C_003CStartAircraftSession_003Eb__14_4_003Ed _003C_003CStartAircraftSession_003Eb__14_4_003Ed = default(_003C_003Ec._003C_003CStartAircraftSession_003Eb__14_4_003Ed);
						_003C_003CStartAircraftSession_003Eb__14_4_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
						_003C_003CStartAircraftSession_003Eb__14_4_003Ed.flightSession = flightSession;
						_003C_003CStartAircraftSession_003Eb__14_4_003Ed._003C_003E1__state = -1;
						((AsyncTaskMethodBuilder)(ref _003C_003CStartAircraftSession_003Eb__14_4_003Ed._003C_003Et__builder)).Start<_003C_003Ec._003C_003CStartAircraftSession_003Eb__14_4_003Ed>(ref _003C_003CStartAircraftSession_003Eb__14_4_003Ed);
						return ((AsyncTaskMethodBuilder)(ref _003C_003CStartAircraftSession_003Eb__14_4_003Ed._003C_003Et__builder)).Task;
					}, delegate
					{
						lock (CS_0024_003C_003E8__locals0._003C_003E4__this._lock)
						{
							if (CS_0024_003C_003E8__locals0._003C_003E4__this._currentlyRunning.Value == CS_0024_003C_003E8__locals0.cs)
							{
								CS_0024_003C_003E8__locals0._003C_003E4__this._currentlyRunning.OnNext(null);
							}
						}
						return System.Threading.Tasks.Task.CompletedTask;
					}));
					_currentlyRunning.OnNext(CS_0024_003C_003E8__locals0.cs);
					CS_0024_003C_003E8__locals0.cs.Start();
				}
			}
			return CS_0024_003C_003E8__locals0.cs;
		}
	}

	private sealed class AircraftSimRunner : IAircraftSimRunner, ISimRunnerEvolved
	{
		private readonly IAircraftBasedSimCoordinatorStrategy _coordinatorStrategy;

		[field: CompilerGenerated]
		public ReactiveAircraft Aircraft
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		public ISimSessionCoordinator Coordinator
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		public IReactiveEntity<PowerStage> Stage
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		public IReactiveEntity<bool> CanPowerOn
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		public IReactiveEntity<ISimSessionEvolved?> Sessions
		{
			[CompilerGenerated]
			get;
		}

		public AircraftSimRunner(ReactiveAircraft aircraft, ISimSessionCoordinator coordinator, IAircraftBasedSimCoordinatorStrategy simCoordinatorStrategy)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			Aircraft = aircraft;
			Coordinator = coordinator;
			_coordinatorStrategy = simCoordinatorStrategy;
			Stage = ReactiveEntity.WrapStream(simCoordinatorStrategy.GetPowerStageStream(aircraft), PowerStage.PoweredDown, TimeSpan.FromMilliseconds(10.0));
			CanPowerOn = ReactiveEntity.WrapStream(Observable.CombineLatest<bool, PowerStage, bool>(simCoordinatorStrategy.GetCanStartStream(aircraft), coordinator.CurrentlyRunning.GetCachedThenUpdatedAndThenContinualValues().SelectMany<ISimRunnerEvolved, PowerStage>((Func<ISimRunnerEvolved, IObservable<PowerStage>>)((ISimRunnerEvolved r) => r.Stage.GetCachedThenUpdatedAndThenContinualValues())).DistinctUntilChanged<PowerStage>(), (bool aircraftCanRun, PowerStage globalPowerStage) => aircraftCanRun && globalPowerStage == PowerStage.PoweredDown), firstCachedValue: false, TimeSpan.FromMilliseconds(10.0));
			Sessions = ReactiveEntity.WrapStream<ISimSessionEvolved>(simCoordinatorStrategy.GetSessionStream(aircraft), null, TimeSpan.FromMilliseconds(10.0));
		}

		public ISimSessionEvolved StartSession()
		{
			return _coordinatorStrategy.StartAircraftSession(Aircraft);
		}
	}

	private readonly AircraftInternal _internal;

	[field: CompilerGenerated]
	public IReactiveEntity<ISimRunnerEvolved> CurrentlyRunning
	{
		[CompilerGenerated]
		get;
	}

	public SimSessionCoordinator(IAirworthiness canRunProvider, IFlightSessionFactory flightSessionFactory, IAutomaticShutdownStrategy automaticShutdownStrategy, IRxSchedulerProvider rxScheduler)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		_internal = new AircraftInternal(canRunProvider, flightSessionFactory, automaticShutdownStrategy, rxScheduler.IOThreads);
		IObservable<ISimRunnerEvolved> stream = _internal.CurrentlyRunning.Select<ReactiveAircraft, ISimRunnerEvolved>((Func<ReactiveAircraft, ISimRunnerEvolved>)([CompilerGenerated] (ReactiveAircraft a) => (a == null) ? ((ISimRunnerEvolved)new NeverRunner(this)) : ((ISimRunnerEvolved)new AircraftSimRunner(a, this, _internal))));
		CurrentlyRunning = ReactiveEntity.WrapStream(stream, (ISimRunnerEvolved)new NeverRunner(this), TimeSpan.FromMilliseconds(10.0));
	}

	public IAircraftSimRunner GetRunnerForAircraft(ReactiveAircraft aircraft)
	{
		return new AircraftSimRunner(aircraft, this, _internal);
	}
}
