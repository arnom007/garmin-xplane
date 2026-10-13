using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.Logging;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Utility;
using Nito.AsyncEx;

namespace LauncherCoreLib.Models.Simulated.Panel;

public sealed class InMemSimulatorRepo : ISimulatorRepository, IFlushable, ICacheAircraftData, System.IDisposable
{
	private sealed class AircraftPanelCacheBehavior : System.IDisposable
	{
		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003C_003CFlush_003Eb__12_0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public AircraftPanelCacheBehavior _003C_003E4__this;

			public AircraftIdentifier k;

			private SimPanel _003Cpanel_003E5__2;

			private System.IDisposable _003C_003E7__wrap2;

			private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

			private ConfiguredTaskAwaiter _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0117: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				AircraftPanelCacheBehavior aircraftPanelCacheBehavior = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<System.IDisposable> awaiter;
					if (num == 0)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
						num = (_003C_003E1__state = -1);
						goto IL_00a1;
					}
					if (num == 1)
					{
						goto IL_00b1;
					}
					if (aircraftPanelCacheBehavior._dirtyPanels.TryRemove(k, ref _003Cpanel_003E5__2))
					{
						awaiter = aircraftPanelCacheBehavior.GetFlushLock(k).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
							.GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003C_003CFlush_003Eb__12_0_003Ed>(ref awaiter, ref this);
							return;
						}
						goto IL_00a1;
					}
					goto end_IL_000e;
					IL_00b1:
					try
					{
						ConfiguredTaskAwaiter awaiter2;
						if (num != 1)
						{
							ConfiguredTaskAwaitable val = aircraftPanelCacheBehavior._actualRepo.UpdateSimulatorPanel(k, _003Cpanel_003E5__2).DetachContext();
							awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter2;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CFlush_003Eb__12_0_003Ed>(ref awaiter2, ref this);
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
					finally
					{
						if (num < 0 && _003C_003E7__wrap2 != null)
						{
							_003C_003E7__wrap2.Dispose();
						}
					}
					_003C_003E7__wrap2 = null;
					goto end_IL_000e;
					IL_00a1:
					System.IDisposable result = awaiter.GetResult();
					_003C_003E7__wrap2 = result;
					goto IL_00b1;
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003Cpanel_003E5__2 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003Cpanel_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass11_0
		{
			public SimPanel previous;

			public SimPanel simulatorPanel;

			public AircraftIdentifier aircraftIdentifier;

			internal SimPanel _003CCreateOrUpdatePanel_003Eb__1(AircraftIdentifier _, SimPanel old)
			{
				previous = old;
				return simulatorPanel;
			}

			internal DataChangeNotification<SimAircraftKey> _003CCreateOrUpdatePanel_003Eb__0(DataChangeNotification<SimInstallKey> ik)
			{
				return DataChangeNotification.Create(new SimAircraftKey(aircraftIdentifier, ik.Value), ik.Change);
			}
		}

		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass9_0
		{
			public SimInstallKey simulatorKey;
		}

		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CCreateOrUpdatePanel_003Ed__11 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public SimPanel simulatorPanel;

			public AircraftIdentifier aircraftIdentifier;

			public AircraftPanelCacheBehavior _003C_003E4__this;

			private _003C_003Ec__DisplayClass11_0 _003C_003E8__1;

			private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				AircraftPanelCacheBehavior aircraftPanelCacheBehavior = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<System.IDisposable> awaiter;
					if (num != 0)
					{
						_003C_003E8__1 = new _003C_003Ec__DisplayClass11_0();
						_003C_003E8__1.simulatorPanel = simulatorPanel;
						_003C_003E8__1.aircraftIdentifier = aircraftIdentifier;
						_003C_003E8__1.previous = new SimPanel();
						awaiter = aircraftPanelCacheBehavior.GetFlushLock(_003C_003E8__1.aircraftIdentifier).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false)
							.GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CCreateOrUpdatePanel_003Ed__11>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
						num = (_003C_003E1__state = -1);
					}
					System.IDisposable result = awaiter.GetResult();
					try
					{
						aircraftPanelCacheBehavior._panelCache.AddOrUpdate(_003C_003E8__1.aircraftIdentifier, _003C_003E8__1.simulatorPanel, (Func<AircraftIdentifier, SimPanel, SimPanel>)delegate(AircraftIdentifier _, SimPanel old)
						{
							_003C_003E8__1.previous = old;
							return _003C_003E8__1.simulatorPanel;
						});
						aircraftPanelCacheBehavior._dirtyPanels[_003C_003E8__1.aircraftIdentifier] = _003C_003E8__1.simulatorPanel;
					}
					finally
					{
						if (num < 0)
						{
							result?.Dispose();
						}
					}
					aircraftPanelCacheBehavior._hangarEvents.PostSimulatorChanges(Enumerable.Select<DataChangeNotification<SimInstallKey>, DataChangeNotification<SimAircraftKey>>(_003C_003E8__1.previous.SimulatorChanges(_003C_003E8__1.simulatorPanel), (Func<DataChangeNotification<SimInstallKey>, DataChangeNotification<SimAircraftKey>>)((DataChangeNotification<SimInstallKey> ik) => DataChangeNotification.Create(new SimAircraftKey(_003C_003E8__1.aircraftIdentifier, ik.Value), ik.Change))));
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CCreateOrUpdateSimulator_003Ed__10 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public AircraftPanelCacheBehavior _003C_003E4__this;

			public AircraftIdentifier aircraftIdentifier;

			public SimInstallData simulator;

			private System.IDisposable _003C_003E7__wrap1;

			private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

			private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				AircraftPanelCacheBehavior aircraftPanelCacheBehavior = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<System.IDisposable> awaiter;
					if (num != 0)
					{
						if (num == 1)
						{
							goto IL_009a;
						}
						awaiter = aircraftPanelCacheBehavior.GetFlushLock(aircraftIdentifier).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false)
							.GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CCreateOrUpdateSimulator_003Ed__10>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
						num = (_003C_003E1__state = -1);
					}
					System.IDisposable result = awaiter.GetResult();
					_003C_003E7__wrap1 = result;
					goto IL_009a;
					IL_009a:
					SimPanel result2;
					try
					{
						if (num != 1)
						{
							goto IL_009e;
						}
						ConfiguredTaskAwaiter<SimPanel> awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<SimPanel>);
						num = (_003C_003E1__state = -1);
						goto IL_0104;
						IL_0104:
						SimPanel simPanel = (result2 = awaiter2.GetResult());
						if (simulator.IsActiveSimulator)
						{
							SimInstallData activeSimInSlot = result2.GetActiveSimInSlot(simulator.InstallationKey.Slot);
							if (activeSimInSlot != null)
							{
								simPanel = simPanel.WithUpdatedSimulators(activeSimInSlot.ToActive(isActive: false));
							}
						}
						simPanel = simPanel.WithUpdatedSimulators(simulator);
						if (!aircraftPanelCacheBehavior._panelCache.TryUpdate(aircraftIdentifier, simPanel, result2))
						{
							goto IL_009e;
						}
						aircraftPanelCacheBehavior._dirtyPanels[aircraftIdentifier] = simPanel;
						goto end_IL_009a;
						IL_009e:
						awaiter2 = aircraftPanelCacheBehavior.GetCachedPanelOrFillFromActual(aircraftIdentifier).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CCreateOrUpdateSimulator_003Ed__10>(ref awaiter2, ref this);
							return;
						}
						goto IL_0104;
						end_IL_009a:;
					}
					finally
					{
						if (num < 0 && _003C_003E7__wrap1 != null)
						{
							_003C_003E7__wrap1.Dispose();
						}
					}
					_003C_003E7__wrap1 = null;
					aircraftPanelCacheBehavior._hangarEvents.PostSimulatorChange(DataChangeNotification.Create(simulator.AircraftSimKey, (!result2.ContainsSimulatorKey(simulator.AircraftSimKey)) ? DataChangeType.Created : DataChangeType.Updated));
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
		[CompilerGenerated]
		private struct _003CFlush_003Ed__12 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public AircraftPanelCacheBehavior _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				AircraftPanelCacheBehavior CS_0024_003C_003E8__locals0 = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					ConfiguredTaskAwaitable val;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_00f4;
						}
						val = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<AircraftIdentifier, System.Threading.Tasks.Task>(CS_0024_003C_003E8__locals0._dirtyPanels.NonBlockingKeys<AircraftIdentifier, SimPanel>(), (Func<AircraftIdentifier, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003CFlush_003Eb__12_0_003Ed))] [CompilerGenerated] (AircraftIdentifier k) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003CFlush_003Eb__12_0_003Ed _003C_003CFlush_003Eb__12_0_003Ed = default(_003C_003CFlush_003Eb__12_0_003Ed);
							_003C_003CFlush_003Eb__12_0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CFlush_003Eb__12_0_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
							_003C_003CFlush_003Eb__12_0_003Ed.k = k;
							_003C_003CFlush_003Eb__12_0_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CFlush_003Eb__12_0_003Ed._003C_003Et__builder)).Start<_003C_003CFlush_003Eb__12_0_003Ed>(ref _003C_003CFlush_003Eb__12_0_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CFlush_003Eb__12_0_003Ed._003C_003Et__builder)).Task;
						}))).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CFlush_003Ed__12>(ref awaiter, ref this);
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
					val = CS_0024_003C_003E8__locals0._actualRepo.Flush().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CFlush_003Ed__12>(ref awaiter, ref this);
						return;
					}
					goto IL_00f4;
					IL_00f4:
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
		[CompilerGenerated]
		private struct _003CGetCachedPanelOrFillFromActual_003Ed__15 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<SimPanel> _003C_003Et__builder;

			public AircraftPanelCacheBehavior _003C_003E4__this;

			public AircraftIdentifier aircraftIdentifier;

			private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				AircraftPanelCacheBehavior aircraftPanelCacheBehavior = _003C_003E4__this;
				SimPanel result2;
				try
				{
					if (num != 0)
					{
						goto IL_009d;
					}
					ConfiguredTaskAwaiter<SimPanel> awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPanel>);
					num = (_003C_003E1__state = -1);
					goto IL_0081;
					IL_0081:
					SimPanel result = awaiter.GetResult();
					if (!aircraftPanelCacheBehavior._panelCache.TryAdd(aircraftIdentifier, result))
					{
						goto IL_009d;
					}
					goto IL_00b5;
					IL_009d:
					if (!aircraftPanelCacheBehavior._panelCache.TryGetValue(aircraftIdentifier, ref result))
					{
						awaiter = aircraftPanelCacheBehavior._actualRepo.GetSimulatorPanel(aircraftIdentifier).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CGetCachedPanelOrFillFromActual_003Ed__15>(ref awaiter, ref this);
							return;
						}
						goto IL_0081;
					}
					goto IL_00b5;
					IL_00b5:
					result2 = result;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult(result2);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CGetSimulator_003Ed__9 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<SimInstallData> _003C_003Et__builder;

			public SimInstallKey simulatorKey;

			public AircraftPanelCacheBehavior _003C_003E4__this;

			public AircraftIdentifier aircraftIdentifier;

			private _003C_003Ec__DisplayClass9_0 _003C_003E8__1;

			private SimPanel _003Cpanel_003E5__2;

			private System.IDisposable _003C_003E7__wrap2;

			private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

			private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__2;

			private ConfiguredTaskAwaiter<SimInstallData> _003C_003Eu__3;

			private void MoveNext()
			{
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0173: Unknown result type (might be due to invalid IL or missing references)
				//IL_0178: Unknown result type (might be due to invalid IL or missing references)
				//IL_017c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0181: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0196: Unknown result type (might be due to invalid IL or missing references)
				//IL_0198: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				AircraftPanelCacheBehavior aircraftPanelCacheBehavior = _003C_003E4__this;
				SimInstallData result3;
				try
				{
					ConfiguredTaskAwaiter<System.IDisposable> awaiter;
					if (num != 0)
					{
						if ((uint)(num - 1) <= 1u)
						{
							goto IL_00ab;
						}
						_003C_003E8__1.simulatorKey = simulatorKey;
						awaiter = aircraftPanelCacheBehavior.GetFlushLock(aircraftIdentifier).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false)
							.GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CGetSimulator_003Ed__9>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
						num = (_003C_003E1__state = -1);
					}
					System.IDisposable result = awaiter.GetResult();
					_003C_003E7__wrap2 = result;
					goto IL_00ab;
					IL_00ab:
					try
					{
						ConfiguredTaskAwaiter<SimInstallData> awaiter2;
						if (num != 1)
						{
							if (num != 2)
							{
								goto IL_00b6;
							}
							awaiter2 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter<SimInstallData>);
							num = (_003C_003E1__state = -1);
							goto IL_01cd;
						}
						ConfiguredTaskAwaiter<SimPanel> awaiter3 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<SimPanel>);
						num = (_003C_003E1__state = -1);
						goto IL_011c;
						IL_011c:
						SimPanel result2 = awaiter3.GetResult();
						_003Cpanel_003E5__2 = result2;
						SimAircraftKey aircraftSimKey = new SimAircraftKey(aircraftIdentifier, _003C_003E8__1.simulatorKey);
						if (!_003Cpanel_003E5__2.ContainsSimulatorKey(aircraftSimKey))
						{
							awaiter2 = aircraftPanelCacheBehavior._actualRepo.GetSimulator(aircraftIdentifier, _003C_003E8__1.simulatorKey).DetachContext().GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimInstallData>, _003CGetSimulator_003Ed__9>(ref awaiter2, ref this);
								return;
							}
							goto IL_01cd;
						}
						result3 = _003CGetSimulator_003Eg__ThrowNotFoundIfNull_007C9_0(_003Cpanel_003E5__2.GetSimulatorDataOrNull(_003C_003E8__1.simulatorKey), ref _003C_003E8__1);
						goto end_IL_00ab;
						IL_00b6:
						awaiter3 = aircraftPanelCacheBehavior.GetCachedPanelOrFillFromActual(aircraftIdentifier).DetachContext().GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter3;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CGetSimulator_003Ed__9>(ref awaiter3, ref this);
							return;
						}
						goto IL_011c;
						IL_01cd:
						SimInstallData result4 = awaiter2.GetResult();
						SimPanel simPanel = _003Cpanel_003E5__2.WithUpdatedSimulators(result4);
						if (!aircraftPanelCacheBehavior._panelCache.TryUpdate(aircraftIdentifier, simPanel, _003Cpanel_003E5__2))
						{
							goto IL_00b6;
						}
						result3 = _003CGetSimulator_003Eg__ThrowNotFoundIfNull_007C9_0(simPanel.GetSimulatorDataOrNull(_003C_003E8__1.simulatorKey), ref _003C_003E8__1);
						end_IL_00ab:;
					}
					finally
					{
						if (num < 0 && _003C_003E7__wrap2 != null)
						{
							_003C_003E7__wrap2.Dispose();
						}
					}
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = default(_003C_003Ec__DisplayClass9_0);
					_003Cpanel_003E5__2 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = default(_003C_003Ec__DisplayClass9_0);
				_003Cpanel_003E5__2 = null;
				_003C_003Et__builder.SetResult(result3);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CGetSimulatorPanel_003Ed__7 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<SimPanel> _003C_003Et__builder;

			public AircraftPanelCacheBehavior _003C_003E4__this;

			public AircraftIdentifier aircraftIdentifier;

			private System.IDisposable _003C_003E7__wrap1;

			private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

			private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				AircraftPanelCacheBehavior aircraftPanelCacheBehavior = _003C_003E4__this;
				SimPanel result2;
				try
				{
					ConfiguredTaskAwaiter<System.IDisposable> awaiter;
					if (num != 0)
					{
						if (num == 1)
						{
							goto IL_0095;
						}
						awaiter = aircraftPanelCacheBehavior.GetFlushLock(aircraftIdentifier).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false)
							.GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CGetSimulatorPanel_003Ed__7>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
						num = (_003C_003E1__state = -1);
					}
					System.IDisposable result = awaiter.GetResult();
					_003C_003E7__wrap1 = result;
					goto IL_0095;
					IL_0095:
					try
					{
						ConfiguredTaskAwaiter<SimPanel> awaiter2;
						if (num != 1)
						{
							awaiter2 = aircraftPanelCacheBehavior.GetCachedPanelOrFillFromActual(aircraftIdentifier).DetachContext().GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CGetSimulatorPanel_003Ed__7>(ref awaiter2, ref this);
								return;
							}
						}
						else
						{
							awaiter2 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<SimPanel>);
							num = (_003C_003E1__state = -1);
						}
						result2 = awaiter2.GetResult();
					}
					finally
					{
						if (num < 0 && _003C_003E7__wrap1 != null)
						{
							_003C_003E7__wrap1.Dispose();
						}
					}
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult(result2);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		private readonly ISimulatorRepository _actualRepo;

		private readonly ConcurrentDictionary<AircraftIdentifier, AsyncReaderWriterLock> _flushLocks = new ConcurrentDictionary<AircraftIdentifier, AsyncReaderWriterLock>();

		private readonly ConcurrentDictionary<AircraftIdentifier, SimPanel> _panelCache = new ConcurrentDictionary<AircraftIdentifier, SimPanel>();

		private readonly ConcurrentDictionary<AircraftIdentifier, SimPanel> _dirtyPanels = new ConcurrentDictionary<AircraftIdentifier, SimPanel>();

		private readonly HangarEvents _hangarEvents = new HangarEvents();

		public AircraftPanelCacheBehavior(ISimulatorRepository repo)
		{
			_actualRepo = repo;
		}

		public IObservable<DataChangeNotification<SimInstallKey>> WhenSimulatorIsChangedOnAircraft(AircraftIdentifier aircraftIdentifier)
		{
			return _hangarEvents.WhenSimulatorChanges.Where<DataChangeNotification<SimAircraftKey>>((Func<DataChangeNotification<SimAircraftKey>, bool>)((DataChangeNotification<SimAircraftKey> sak) => sak.Value.AircraftIdentifier == aircraftIdentifier)).Select<DataChangeNotification<SimAircraftKey>, DataChangeNotification<SimInstallKey>>((Func<DataChangeNotification<SimAircraftKey>, DataChangeNotification<SimInstallKey>>)((DataChangeNotification<SimAircraftKey> sak) => DataChangeNotification.Create(sak.Value.InstallationKey, sak.Change)));
		}

		[AsyncStateMachine(typeof(_003CGetSimulatorPanel_003Ed__7))]
		public async System.Threading.Tasks.Task<SimPanel> GetSimulatorPanel(AircraftIdentifier aircraftIdentifier)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			using (await GetFlushLock(aircraftIdentifier).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false))
			{
				return await GetCachedPanelOrFillFromActual(aircraftIdentifier).DetachContext();
			}
		}

		public SimPanel? InvalidateEntirePanelById(AircraftIdentifier aircraftIdentifier)
		{
			SimPanel simPanel = default(SimPanel);
			SimPanel result = default(SimPanel);
			if (_dirtyPanels.TryRemove(aircraftIdentifier, ref simPanel) | _panelCache.TryRemove(aircraftIdentifier, ref result))
			{
				Logger.Log(LogLevel.Debug, "Invalidated the '{0}' sim panel cache.", aircraftIdentifier);
			}
			return result;
		}

		[AsyncStateMachine(typeof(_003CGetSimulator_003Ed__9))]
		public async System.Threading.Tasks.Task<SimInstallData> GetSimulator(AircraftIdentifier aircraftIdentifier, SimInstallKey simulatorKey)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			SimInstallKey simulatorKey2 = simulatorKey;
			using (await GetFlushLock(aircraftIdentifier).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false))
			{
				SimPanel panel;
				while (true)
				{
					panel = await GetCachedPanelOrFillFromActual(aircraftIdentifier).DetachContext();
					SimAircraftKey aircraftSimKey = new SimAircraftKey(aircraftIdentifier, simulatorKey2);
					if (panel.ContainsSimulatorKey(aircraftSimKey))
					{
						break;
					}
					SimPanel simPanel = panel.WithUpdatedSimulators(await _actualRepo.GetSimulator(aircraftIdentifier, simulatorKey2).DetachContext());
					if (_panelCache.TryUpdate(aircraftIdentifier, simPanel, panel))
					{
						return ThrowNotFoundIfNull(simPanel.GetSimulatorDataOrNull(simulatorKey2));
					}
				}
				return ThrowNotFoundIfNull(panel.GetSimulatorDataOrNull(simulatorKey2));
			}
			[CompilerGenerated]
			SimInstallData ThrowNotFoundIfNull(SimInstallData? data)
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				if (data == null)
				{
					throw new KeyNotFoundException("The simulator was not found in the aircraft. " + (object)simulatorKey2);
				}
				return data;
			}
		}

		[AsyncStateMachine(typeof(_003CCreateOrUpdateSimulator_003Ed__10))]
		public System.Threading.Tasks.Task CreateOrUpdateSimulator(AircraftIdentifier aircraftIdentifier, SimInstallData simulator)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003CCreateOrUpdateSimulator_003Ed__10 _003CCreateOrUpdateSimulator_003Ed__ = default(_003CCreateOrUpdateSimulator_003Ed__10);
			_003CCreateOrUpdateSimulator_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003CCreateOrUpdateSimulator_003Ed__._003C_003E4__this = this;
			_003CCreateOrUpdateSimulator_003Ed__.aircraftIdentifier = aircraftIdentifier;
			_003CCreateOrUpdateSimulator_003Ed__.simulator = simulator;
			_003CCreateOrUpdateSimulator_003Ed__._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003CCreateOrUpdateSimulator_003Ed__._003C_003Et__builder)).Start<_003CCreateOrUpdateSimulator_003Ed__10>(ref _003CCreateOrUpdateSimulator_003Ed__);
			return ((AsyncTaskMethodBuilder)(ref _003CCreateOrUpdateSimulator_003Ed__._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003CCreateOrUpdatePanel_003Ed__11))]
		public System.Threading.Tasks.Task CreateOrUpdatePanel(AircraftIdentifier aircraftIdentifier, SimPanel simulatorPanel)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003CCreateOrUpdatePanel_003Ed__11 _003CCreateOrUpdatePanel_003Ed__ = default(_003CCreateOrUpdatePanel_003Ed__11);
			_003CCreateOrUpdatePanel_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003CCreateOrUpdatePanel_003Ed__._003C_003E4__this = this;
			_003CCreateOrUpdatePanel_003Ed__.aircraftIdentifier = aircraftIdentifier;
			_003CCreateOrUpdatePanel_003Ed__.simulatorPanel = simulatorPanel;
			_003CCreateOrUpdatePanel_003Ed__._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003CCreateOrUpdatePanel_003Ed__._003C_003Et__builder)).Start<_003CCreateOrUpdatePanel_003Ed__11>(ref _003CCreateOrUpdatePanel_003Ed__);
			return ((AsyncTaskMethodBuilder)(ref _003CCreateOrUpdatePanel_003Ed__._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003CFlush_003Ed__12))]
		public System.Threading.Tasks.Task Flush()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003CFlush_003Ed__12 _003CFlush_003Ed__ = default(_003CFlush_003Ed__12);
			_003CFlush_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003CFlush_003Ed__._003C_003E4__this = this;
			_003CFlush_003Ed__._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003CFlush_003Ed__._003C_003Et__builder)).Start<_003CFlush_003Ed__12>(ref _003CFlush_003Ed__);
			return ((AsyncTaskMethodBuilder)(ref _003CFlush_003Ed__._003C_003Et__builder)).Task;
		}

		public void Dispose()
		{
			_hangarEvents.Dispose();
		}

		private AsyncReaderWriterLock GetFlushLock(AircraftIdentifier k)
		{
			return _flushLocks.GetOrAdd(k, (Func<AircraftIdentifier, AsyncReaderWriterLock>)((AircraftIdentifier _) => new AsyncReaderWriterLock()));
		}

		[AsyncStateMachine(typeof(_003CGetCachedPanelOrFillFromActual_003Ed__15))]
		private async System.Threading.Tasks.Task<SimPanel> GetCachedPanelOrFillFromActual(AircraftIdentifier aircraftIdentifier)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			SimPanel simPanel = default(SimPanel);
			while (!_panelCache.TryGetValue(aircraftIdentifier, ref simPanel))
			{
				simPanel = await _actualRepo.GetSimulatorPanel(aircraftIdentifier).DetachContext();
				if (_panelCache.TryAdd(aircraftIdentifier, simPanel))
				{
					break;
				}
			}
			return simPanel;
		}

		[CompilerGenerated]
		internal static SimInstallData _003CGetSimulator_003Eg__ThrowNotFoundIfNull_007C9_0(SimInstallData? data, ref _003C_003Ec__DisplayClass9_0 P_1)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			if (data == null)
			{
				throw new KeyNotFoundException("The simulator was not found in the aircraft. " + (object)P_1.simulatorKey);
			}
			return data;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCreateOrUpdateSimulator_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public InMemSimulatorRepo _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public SimInstallData simulator;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			InMemSimulatorRepo inMemSimulatorRepo = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = inMemSimulatorRepo._panel.CreateOrUpdateSimulator(aircraftIdentifier, simulator).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCreateOrUpdateSimulator_003Ed__7>(ref awaiter, ref this);
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
	[CompilerGenerated]
	private struct _003CDeleteAircraftWinfsArchives_003Ed__11 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public InMemSimulatorRepo _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public System.Collections.Generic.IEnumerable<SimInstallKey> simsToClear;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			InMemSimulatorRepo inMemSimulatorRepo = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaitable val;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_00e5;
					}
					val = inMemSimulatorRepo.Flush().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CDeleteAircraftWinfsArchives_003Ed__11>(ref awaiter, ref this);
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
				val = inMemSimulatorRepo._actualRepo.DeleteAircraftWinfsArchives(aircraftIdentifier, simsToClear).DetachContext();
				awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__1 = awaiter;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CDeleteAircraftWinfsArchives_003Ed__11>(ref awaiter, ref this);
					return;
				}
				goto IL_00e5;
				IL_00e5:
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
	[CompilerGenerated]
	private struct _003CFlush_003Ed__12 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public InMemSimulatorRepo _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			InMemSimulatorRepo inMemSimulatorRepo = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = inMemSimulatorRepo._panel.Flush().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CFlush_003Ed__12>(ref awaiter, ref this);
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
	[CompilerGenerated]
	private struct _003CGetAircraftWinfsArchives_003Ed__9 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<System.Collections.Generic.IList<SimInstallKey>> _003C_003Et__builder;

		public InMemSimulatorRepo _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public Dictionary<SimInstallKey, string> simWinfsArchiveOutputMap;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			InMemSimulatorRepo inMemSimulatorRepo = _003C_003E4__this;
			System.Collections.Generic.IList<SimInstallKey> result;
			try
			{
				ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>> awaiter;
				ConfiguredTaskAwaiter awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>>);
						num = (_003C_003E1__state = -1);
						goto IL_00ea;
					}
					ConfiguredTaskAwaitable val = inMemSimulatorRepo.Flush().DetachContext();
					awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CGetAircraftWinfsArchives_003Ed__9>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
				}
				((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
				awaiter = inMemSimulatorRepo._actualRepo.GetAircraftWinfsArchives(aircraftIdentifier, simWinfsArchiveOutputMap).DetachContext().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>>, _003CGetAircraftWinfsArchives_003Ed__9>(ref awaiter, ref this);
					return;
				}
				goto IL_00ea;
				IL_00ea:
				result = awaiter.GetResult();
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
	[CompilerGenerated]
	private struct _003CUpdateAircraftWinfsArchives_003Ed__10 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public InMemSimulatorRepo _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public Dictionary<SimInstallKey, string> simWinfsArchiveInputMap;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			InMemSimulatorRepo inMemSimulatorRepo = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaitable val;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_00e5;
					}
					val = inMemSimulatorRepo.Flush().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateAircraftWinfsArchives_003Ed__10>(ref awaiter, ref this);
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
				val = inMemSimulatorRepo._actualRepo.UpdateAircraftWinfsArchives(aircraftIdentifier, simWinfsArchiveInputMap).DetachContext();
				awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__1 = awaiter;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateAircraftWinfsArchives_003Ed__10>(ref awaiter, ref this);
					return;
				}
				goto IL_00e5;
				IL_00e5:
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
	[CompilerGenerated]
	private struct _003CUpdateSimulatorPanel_003Ed__8 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public InMemSimulatorRepo _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public SimPanel simulatorPanel;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			InMemSimulatorRepo inMemSimulatorRepo = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = inMemSimulatorRepo._panel.CreateOrUpdatePanel(aircraftIdentifier, simulatorPanel).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateSimulatorPanel_003Ed__8>(ref awaiter, ref this);
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

	private readonly ISimulatorRepository _actualRepo;

	private readonly AircraftPanelCacheBehavior _panel;

	private readonly ConcurrentDictionary<AircraftIdentifier, IObservable<DataChangeNotification<SimInstallKey>>> _sharedChangeStreams = new ConcurrentDictionary<AircraftIdentifier, IObservable<DataChangeNotification<SimInstallKey>>>();

	public InMemSimulatorRepo(ISimulatorRepository repo)
	{
		_actualRepo = repo;
		_panel = new AircraftPanelCacheBehavior(repo);
	}

	public IObservable<DataChangeNotification<SimInstallKey>> WhenSimulatorIsChangedOnAircraft(AircraftIdentifier aircraftIdentifier)
	{
		return _sharedChangeStreams.GetOrAdd(aircraftIdentifier, (Func<AircraftIdentifier, IObservable<DataChangeNotification<SimInstallKey>>>)([CompilerGenerated] (AircraftIdentifier ai) => RestartableStream.FromFactory<DataChangeNotification<SimInstallKey>>(() => _actualRepo.WhenSimulatorIsChangedOnAircraft(ai).Do<DataChangeNotification<SimInstallKey>>((Action<DataChangeNotification<SimInstallKey>>)delegate
		{
			_panel.InvalidateEntirePanelById(ai);
		}).Publish<DataChangeNotification<SimInstallKey>>()).RefCount())).Merge<DataChangeNotification<SimInstallKey>>(_panel.WhenSimulatorIsChangedOnAircraft(aircraftIdentifier));
	}

	public System.Threading.Tasks.Task<SimInstallData> GetSimulator(AircraftIdentifier aircraftIdentifier, SimInstallKey simulatorKey)
	{
		return _panel.GetSimulator(aircraftIdentifier, simulatorKey);
	}

	public System.Threading.Tasks.Task<SimPanel> GetSimulatorPanel(AircraftIdentifier aircraftIdentifier)
	{
		return _panel.GetSimulatorPanel(aircraftIdentifier);
	}

	[AsyncStateMachine(typeof(_003CCreateOrUpdateSimulator_003Ed__7))]
	public System.Threading.Tasks.Task CreateOrUpdateSimulator(AircraftIdentifier aircraftIdentifier, SimInstallData simulator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CCreateOrUpdateSimulator_003Ed__7 _003CCreateOrUpdateSimulator_003Ed__ = default(_003CCreateOrUpdateSimulator_003Ed__7);
		_003CCreateOrUpdateSimulator_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CCreateOrUpdateSimulator_003Ed__._003C_003E4__this = this;
		_003CCreateOrUpdateSimulator_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CCreateOrUpdateSimulator_003Ed__.simulator = simulator;
		_003CCreateOrUpdateSimulator_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CCreateOrUpdateSimulator_003Ed__._003C_003Et__builder)).Start<_003CCreateOrUpdateSimulator_003Ed__7>(ref _003CCreateOrUpdateSimulator_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CCreateOrUpdateSimulator_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUpdateSimulatorPanel_003Ed__8))]
	public System.Threading.Tasks.Task UpdateSimulatorPanel(AircraftIdentifier aircraftIdentifier, SimPanel simulatorPanel)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateSimulatorPanel_003Ed__8 _003CUpdateSimulatorPanel_003Ed__ = default(_003CUpdateSimulatorPanel_003Ed__8);
		_003CUpdateSimulatorPanel_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateSimulatorPanel_003Ed__._003C_003E4__this = this;
		_003CUpdateSimulatorPanel_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CUpdateSimulatorPanel_003Ed__.simulatorPanel = simulatorPanel;
		_003CUpdateSimulatorPanel_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateSimulatorPanel_003Ed__._003C_003Et__builder)).Start<_003CUpdateSimulatorPanel_003Ed__8>(ref _003CUpdateSimulatorPanel_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateSimulatorPanel_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CGetAircraftWinfsArchives_003Ed__9))]
	public async System.Threading.Tasks.Task<System.Collections.Generic.IList<SimInstallKey>> GetAircraftWinfsArchives(AircraftIdentifier aircraftIdentifier, Dictionary<SimInstallKey, string> simWinfsArchiveOutputMap)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		await Flush().DetachContext();
		return await _actualRepo.GetAircraftWinfsArchives(aircraftIdentifier, simWinfsArchiveOutputMap).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CUpdateAircraftWinfsArchives_003Ed__10))]
	public System.Threading.Tasks.Task UpdateAircraftWinfsArchives(AircraftIdentifier aircraftIdentifier, Dictionary<SimInstallKey, string> simWinfsArchiveInputMap)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateAircraftWinfsArchives_003Ed__10 _003CUpdateAircraftWinfsArchives_003Ed__ = default(_003CUpdateAircraftWinfsArchives_003Ed__10);
		_003CUpdateAircraftWinfsArchives_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateAircraftWinfsArchives_003Ed__._003C_003E4__this = this;
		_003CUpdateAircraftWinfsArchives_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CUpdateAircraftWinfsArchives_003Ed__.simWinfsArchiveInputMap = simWinfsArchiveInputMap;
		_003CUpdateAircraftWinfsArchives_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateAircraftWinfsArchives_003Ed__._003C_003Et__builder)).Start<_003CUpdateAircraftWinfsArchives_003Ed__10>(ref _003CUpdateAircraftWinfsArchives_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateAircraftWinfsArchives_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CDeleteAircraftWinfsArchives_003Ed__11))]
	public System.Threading.Tasks.Task DeleteAircraftWinfsArchives(AircraftIdentifier aircraftIdentifier, System.Collections.Generic.IEnumerable<SimInstallKey> simsToClear)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CDeleteAircraftWinfsArchives_003Ed__11 _003CDeleteAircraftWinfsArchives_003Ed__ = default(_003CDeleteAircraftWinfsArchives_003Ed__11);
		_003CDeleteAircraftWinfsArchives_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CDeleteAircraftWinfsArchives_003Ed__._003C_003E4__this = this;
		_003CDeleteAircraftWinfsArchives_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CDeleteAircraftWinfsArchives_003Ed__.simsToClear = simsToClear;
		_003CDeleteAircraftWinfsArchives_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CDeleteAircraftWinfsArchives_003Ed__._003C_003Et__builder)).Start<_003CDeleteAircraftWinfsArchives_003Ed__11>(ref _003CDeleteAircraftWinfsArchives_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CDeleteAircraftWinfsArchives_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CFlush_003Ed__12))]
	public System.Threading.Tasks.Task Flush()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CFlush_003Ed__12 _003CFlush_003Ed__ = default(_003CFlush_003Ed__12);
		_003CFlush_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CFlush_003Ed__._003C_003E4__this = this;
		_003CFlush_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CFlush_003Ed__._003C_003Et__builder)).Start<_003CFlush_003Ed__12>(ref _003CFlush_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CFlush_003Ed__._003C_003Et__builder)).Task;
	}

	public void InvalidateAircraftData(AircraftIdentifier aircraftIdentifier)
	{
		_panel.InvalidateEntirePanelById(aircraftIdentifier);
	}

	public void Dispose()
	{
		_panel.Dispose();
	}
}
