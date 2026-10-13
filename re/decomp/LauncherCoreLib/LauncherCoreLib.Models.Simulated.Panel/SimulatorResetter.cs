using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Panel;

public class SimulatorResetter : IResetSimulators
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		[StructLayout(3)]
		private struct _003C_003CResetWinfsToPrototypeIfItExists_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass3_0 _003C_003E4__this;

			public ReactiveAircraft snapshotAircraft;

			private _003C_003Ec__DisplayClass3_1 _003C_003E8__1;

			private bool _003CsimulatorWasInPrototype_003E5__2;

			private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__1;

			private ConfiguredTaskAwaiter _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_017d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0182: Unknown result type (might be due to invalid IL or missing references)
				//IL_018a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0142: Unknown result type (might be due to invalid IL or missing references)
				//IL_0147: Unknown result type (might be due to invalid IL or missing references)
				//IL_014b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0150: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_0167: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass3_0 _003C_003Ec__DisplayClass3_ = _003C_003E4__this;
				bool result2;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					ConfiguredTaskAwaiter<SimPanel> awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_0199;
						}
						_003C_003E8__1 = new _003C_003Ec__DisplayClass3_1();
						_003C_003E8__1.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass3_;
						_003C_003E8__1.snapshotAircraft = snapshotAircraft;
						_003CsimulatorWasInPrototype_003E5__2 = false;
						awaiter2 = _003C_003E8__1.snapshotAircraft.Panel.GetUpdatedValue().DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003C_003CResetWinfsToPrototypeIfItExists_003Eb__1_003Ed>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPanel>);
						num = (_003C_003E1__state = -1);
					}
					SimPanel result = awaiter2.GetResult();
					SimAircraftKey simAircraftKey = new SimAircraftKey(_003C_003E8__1.snapshotAircraft.AircraftIdentifier, _003C_003Ec__DisplayClass3_.simulator.AircraftAndSim.InstallationKey);
					if (result.ContainsSimulatorKey(simAircraftKey))
					{
						_003C_003E8__1.snapshotSimData = result.AllSimulators[simAircraftKey.InstallationKey];
						ConfiguredTaskAwaitable val = TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed);
							_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003E4__this = _003C_003E8__1;
							_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed>(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder)).Task;
						}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed = default(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed);
							_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003E4__this = _003C_003E8__1;
							_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed>(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder)).Task;
						}).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CResetWinfsToPrototypeIfItExists_003Eb__1_003Ed>(ref awaiter, ref this);
							return;
						}
						goto IL_0199;
					}
					goto IL_01a7;
					IL_0199:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					_003CsimulatorWasInPrototype_003E5__2 = true;
					goto IL_01a7;
					IL_01a7:
					result2 = _003CsimulatorWasInPrototype_003E5__2;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003C_003Et__builder.SetResult(result2);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public ReactiveSimulator simulator;

		[AsyncStateMachine(typeof(_003C_003CResetWinfsToPrototypeIfItExists_003Eb__1_003Ed))]
		internal async System.Threading.Tasks.Task<bool> _003CResetWinfsToPrototypeIfItExists_003Eb__1(ReactiveAircraft snapshotAircraft)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_1
			{
				CS_0024_003C_003E8__locals1 = this,
				snapshotAircraft = snapshotAircraft
			};
			bool simulatorWasInPrototype = false;
			SimPanel simPanel = await CS_0024_003C_003E8__locals0.snapshotAircraft.Panel.GetUpdatedValue().DetachContext();
			SimAircraftKey simAircraftKey = new SimAircraftKey(CS_0024_003C_003E8__locals0.snapshotAircraft.AircraftIdentifier, simulator.AircraftAndSim.InstallationKey);
			if (simPanel.ContainsSimulatorKey(simAircraftKey))
			{
				CS_0024_003C_003E8__locals0.snapshotSimData = simPanel.AllSimulators[simAircraftKey.InstallationKey];
				await TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed);
					_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
					_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed>(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder)).Task;
				}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed = default(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed);
					_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
					_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed>(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder)).Task;
				}).DetachContext();
				simulatorWasInPrototype = true;
			}
			return simulatorWasInPrototype;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_1
	{
		[StructLayout(3)]
		private struct _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass3_1 _003C_003E4__this;

			private ReactiveSimulator _003CsnapShotSim_003E5__2;

			private string _003CtempRoot_003E5__3;

			private string _003CtempArchivePath_003E5__4;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0246: Unknown result type (might be due to invalid IL or missing references)
				//IL_024b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0252: Unknown result type (might be due to invalid IL or missing references)
				//IL_0123: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_012f: Unknown result type (might be due to invalid IL or missing references)
				//IL_020d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0212: Unknown result type (might be due to invalid IL or missing references)
				//IL_0216: Unknown result type (might be due to invalid IL or missing references)
				//IL_021b: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_015b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0160: Unknown result type (might be due to invalid IL or missing references)
				//IL_0164: Unknown result type (might be due to invalid IL or missing references)
				//IL_0169: Unknown result type (might be due to invalid IL or missing references)
				//IL_0197: Unknown result type (might be due to invalid IL or missing references)
				//IL_019c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_022f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0230: Unknown result type (might be due to invalid IL or missing references)
				//IL_017d: Unknown result type (might be due to invalid IL or missing references)
				//IL_017e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass3_1 _003C_003Ec__DisplayClass3_ = _003C_003E4__this;
				try
				{
					if ((uint)num <= 1u)
					{
						goto IL_00a6;
					}
					ConfiguredTaskAwaitable val2;
					ConfiguredTaskAwaiter awaiter;
					if (num != 2)
					{
						if (_003C_003Ec__DisplayClass3_.snapshotSimData.HasArchivedWinfsMount)
						{
							_003CsnapShotSim_003E5__2 = _003C_003Ec__DisplayClass3_.snapshotAircraft.CreateSimWrapper(_003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals1.simulator.AircraftAndSim.InstallationKey);
							_003CtempRoot_003E5__3 = _003CsnapShotSim_003E5__2.UnitOfWork.LowLevel.Deployment.PerProcessTempDirectory();
							string text = _003CtempRoot_003E5__3;
							Guid val = Guid.NewGuid();
							_003CtempArchivePath_003E5__4 = Path.Combine(text, "SimReset", ((object)(Guid)(ref val)).ToString() + ".gca");
							goto IL_00a6;
						}
						val2 = _003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals1.simulator.DeleteWinfsArchive().DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed>(ref awaiter, ref this);
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
					goto end_IL_000e;
					IL_00a6:
					try
					{
						if (num != 0)
						{
							if (num == 1)
							{
								awaiter = _003C_003Eu__1;
								_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
								num = (_003C_003E1__state = -1);
								goto IL_01b2;
							}
							_003CsnapShotSim_003E5__2.UnitOfWork.LowLevel.FileSystem.CreateDirectory(LauncherPathHelpers.NonRootPathParentDirectory(_003CtempArchivePath_003E5__4));
							val2 = _003CsnapShotSim_003E5__2.GetWinfsArchive(_003CtempArchivePath_003E5__4).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed>(ref awaiter, ref this);
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
						val2 = _003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals1.simulator.UpdateWinfsArchive(_003CtempArchivePath_003E5__4).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed>(ref awaiter, ref this);
							return;
						}
						goto IL_01b2;
						IL_01b2:
						((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					}
					finally
					{
						if (num < 0)
						{
							_003CsnapShotSim_003E5__2.UnitOfWork.LowLevel.FileSystem.DeleteIfExistsAndParentsThatAreEmpty(_003CtempArchivePath_003E5__4, _003CtempRoot_003E5__3);
						}
					}
					_003CsnapShotSim_003E5__2 = null;
					_003CtempRoot_003E5__3 = null;
					_003CtempArchivePath_003E5__4 = null;
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
		private struct _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass3_1 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass3_1 _003C_003Ec__DisplayClass3_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals1.simulator.UpdateSettings((IDictionary<SettingIdentifier, string>)(object)_003C_003Ec__DisplayClass3_.snapshotSimData.Settings).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed>(ref awaiter, ref this);
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

		public ReactiveAircraft snapshotAircraft;

		public SimInstallData snapshotSimData;

		public _003C_003Ec__DisplayClass3_0 CS_0024_003C_003E8__locals1;

		[AsyncStateMachine(typeof(_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed))]
		internal System.Threading.Tasks.Task _003CResetWinfsToPrototypeIfItExists_003Eb__2()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed = default(_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed);
			_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003E4__this = this;
			_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed>(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed))]
		internal System.Threading.Tasks.Task _003CResetWinfsToPrototypeIfItExists_003Eb__3()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed = default(_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed);
			_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003E4__this = this;
			_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed>(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder)).Task;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CResetSettingsToDefault_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveSimulator simulator;

		private ISettingsBinder<ReactiveSimulator> _003CsettingBinder_003E5__2;

		private ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_007f;
					}
					awaiter = simulator.BindSettings().DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>>, _003CResetSettingsToDefault_003Ed__2>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>>);
					num = (_003C_003E1__state = -1);
				}
				ISettingsBinder<ReactiveSimulator> result = awaiter.GetResult();
				_003CsettingBinder_003E5__2 = result;
				goto IL_007f;
				IL_007f:
				try
				{
					ConfiguredTaskAwaiter awaiter2;
					if (num != 1)
					{
						System.Collections.Generic.IEnumerator<IActiveSetting> enumerator = _003CsettingBinder_003E5__2.Table.Settings.GetEnumerator();
						try
						{
							while (((System.Collections.IEnumerator)enumerator).MoveNext())
							{
								IActiveSetting current = enumerator.Current;
								current.SelectByLocalValueId(current.Definition.DefaultValue);
							}
						}
						finally
						{
							if (num < 0)
							{
								((System.IDisposable)enumerator)?.Dispose();
							}
						}
						Dictionary<SettingIdentifier, string> updatedSettings = _003CsettingBinder_003E5__2.Table.ToPersistenceDictionaryV2();
						ConfiguredTaskAwaitable val = simulator.UpdateSettings((IDictionary<SettingIdentifier, string>)(object)updatedSettings).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CResetSettingsToDefault_003Ed__2>(ref awaiter2, ref this);
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
					if (num < 0 && _003CsettingBinder_003E5__2 != null)
					{
						((System.IDisposable)_003CsettingBinder_003E5__2).Dispose();
					}
				}
				_003CsettingBinder_003E5__2 = null;
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
	private struct _003CResetSimulator_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveSimulator simulator;

		private ConfiguredTaskAwaiter<bool> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter<bool> awaiter2;
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaitable val;
				switch (num)
				{
				default:
					awaiter2 = ResetWinfsToPrototypeIfItExists(simulator).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<bool>, _003CResetSimulator_003Ed__4>(ref awaiter2, ref this);
						return;
					}
					goto IL_0082;
				case 0:
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<bool>);
					num = (_003C_003E1__state = -1);
					goto IL_0082;
				case 1:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_00f0;
				case 2:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_0159;
				case 3:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_01c2;
				case 4:
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						break;
					}
					IL_01c2:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					val = TrainerWindowScreenCoordinatesStorage.ClearStoredScreenCoords(simulator, CancellationToken.None).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 4);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CResetSimulator_003Ed__4>(ref awaiter, ref this);
						return;
					}
					break;
					IL_0082:
					if (!awaiter2.GetResult())
					{
						val = simulator.DeleteWinfsArchive().DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CResetSimulator_003Ed__4>(ref awaiter, ref this);
							return;
						}
						goto IL_00f0;
					}
					goto IL_0160;
					IL_0159:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					goto IL_0160;
					IL_0160:
					val = simulator.ResetPowerUpStats().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 3);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CResetSimulator_003Ed__4>(ref awaiter, ref this);
						return;
					}
					goto IL_01c2;
					IL_00f0:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					val = ResetSettingsToDefault(simulator).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CResetSimulator_003Ed__4>(ref awaiter, ref this);
						return;
					}
					goto IL_0159;
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
	private struct _003CResetWinfsToPrototypeIfItExists_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

		public ReactiveSimulator simulator;

		private _003C_003Ec__DisplayClass3_0 _003C_003E8__1;

		private ReactiveAircraft _003Caircraft_003E5__2;

		private object _003C_003Eu__1;

		private ConfiguredTaskAwaiter<bool> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			bool result2;
			try
			{
				ConfiguredTaskAwaiter<bool> awaiter;
				AsyncSubject<AircraftSnapshotInfo> asyncSubject;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<bool>);
						num = (_003C_003E1__state = -1);
						goto IL_0145;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass3_0();
					_003C_003E8__1.simulator = simulator;
					_003Caircraft_003E5__2 = _003C_003E8__1.simulator.CreateAircraftWrapper();
					asyncSubject = _003Caircraft_003E5__2.ListSnapshots().Where<AircraftSnapshotInfo>((Func<AircraftSnapshotInfo, bool>)((AircraftSnapshotInfo snapShot) => snapShot.IsFromPrototypeAircraft)).LastOrDefaultAsync<AircraftSnapshotInfo>()
						.GetAwaiter<AircraftSnapshotInfo>();
					if (!asyncSubject.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = asyncSubject;
						_003C_003Et__builder.AwaitOnCompleted<AsyncSubject<AircraftSnapshotInfo>, _003CResetWinfsToPrototypeIfItExists_003Ed__3>(ref asyncSubject, ref this);
						return;
					}
				}
				else
				{
					asyncSubject = (AsyncSubject<AircraftSnapshotInfo>)_003C_003Eu__1;
					_003C_003Eu__1 = null;
					num = (_003C_003E1__state = -1);
				}
				AircraftSnapshotInfo result = asyncSubject.GetResult();
				if (result != null)
				{
					awaiter = _003Caircraft_003E5__2.UseMountedSnapshot<bool>(result, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_0._003C_003CResetWinfsToPrototypeIfItExists_003Eb__1_003Ed))] async (ReactiveAircraft snapshotAircraft) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_1
						{
							CS_0024_003C_003E8__locals1 = _003C_003E8__1,
							snapshotAircraft = snapshotAircraft
						};
						bool simulatorWasInPrototype = false;
						SimPanel simPanel = await CS_0024_003C_003E8__locals0.snapshotAircraft.Panel.GetUpdatedValue().DetachContext();
						SimAircraftKey simAircraftKey = new SimAircraftKey(CS_0024_003C_003E8__locals0.snapshotAircraft.AircraftIdentifier, _003C_003E8__1.simulator.AircraftAndSim.InstallationKey);
						if (simPanel.ContainsSimulatorKey(simAircraftKey))
						{
							CS_0024_003C_003E8__locals0.snapshotSimData = simPanel.AllSimulators[simAircraftKey.InstallationKey];
							await TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed))] () =>
							{
								//IL_0002: Unknown result type (might be due to invalid IL or missing references)
								//IL_0007: Unknown result type (might be due to invalid IL or missing references)
								_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed);
								_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
								_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
								_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003E1__state = -1;
								((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed>(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed);
								return ((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder)).Task;
							}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed))] () =>
							{
								//IL_0002: Unknown result type (might be due to invalid IL or missing references)
								//IL_0007: Unknown result type (might be due to invalid IL or missing references)
								_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed = default(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed);
								_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
								_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
								_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003E1__state = -1;
								((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed>(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed);
								return ((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder)).Task;
							}).DetachContext();
							simulatorWasInPrototype = true;
						}
						return simulatorWasInPrototype;
					}).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<bool>, _003CResetWinfsToPrototypeIfItExists_003Ed__3>(ref awaiter, ref this);
						return;
					}
					goto IL_0145;
				}
				result2 = false;
				goto end_IL_0007;
				IL_0145:
				result2 = awaiter.GetResult();
				end_IL_0007:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003Caircraft_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003Caircraft_003E5__2 = null;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly IRunSimulators _simRunner;

	public SimulatorResetter(IRunSimulators simRunner)
	{
		_simRunner = simRunner;
	}

	[AsyncStateMachine(typeof(_003CResetSettingsToDefault_003Ed__2))]
	public static System.Threading.Tasks.Task ResetSettingsToDefault(ReactiveSimulator simulator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CResetSettingsToDefault_003Ed__2 _003CResetSettingsToDefault_003Ed__ = default(_003CResetSettingsToDefault_003Ed__2);
		_003CResetSettingsToDefault_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CResetSettingsToDefault_003Ed__.simulator = simulator;
		_003CResetSettingsToDefault_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CResetSettingsToDefault_003Ed__._003C_003Et__builder)).Start<_003CResetSettingsToDefault_003Ed__2>(ref _003CResetSettingsToDefault_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CResetSettingsToDefault_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CResetWinfsToPrototypeIfItExists_003Ed__3))]
	public static async System.Threading.Tasks.Task<bool> ResetWinfsToPrototypeIfItExists(ReactiveSimulator simulator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ReactiveSimulator simulator2 = simulator;
		ReactiveAircraft aircraft = simulator2.CreateAircraftWrapper();
		AircraftSnapshotInfo aircraftSnapshotInfo = await aircraft.ListSnapshots().Where<AircraftSnapshotInfo>((Func<AircraftSnapshotInfo, bool>)((AircraftSnapshotInfo snapShot) => snapShot.IsFromPrototypeAircraft)).LastOrDefaultAsync<AircraftSnapshotInfo>();
		_003C_003Ec__DisplayClass3_0 CS_0024_003C_003E8__locals0;
		if (aircraftSnapshotInfo != null)
		{
			return await aircraft.UseMountedSnapshot<bool>(aircraftSnapshotInfo, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_0._003C_003CResetWinfsToPrototypeIfItExists_003Eb__1_003Ed))] async (ReactiveAircraft snapshotAircraft) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				_003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass3_1();
				CS_0024_003C_003E8__locals1.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals0;
				CS_0024_003C_003E8__locals1.snapshotAircraft = snapshotAircraft;
				bool simulatorWasInPrototype = false;
				SimPanel simPanel = await CS_0024_003C_003E8__locals1.snapshotAircraft.Panel.GetUpdatedValue().DetachContext();
				SimAircraftKey simAircraftKey = new SimAircraftKey(CS_0024_003C_003E8__locals1.snapshotAircraft.AircraftIdentifier, simulator2.AircraftAndSim.InstallationKey);
				if (simPanel.ContainsSimulatorKey(simAircraftKey))
				{
					CS_0024_003C_003E8__locals1.snapshotSimData = simPanel.AllSimulators[simAircraftKey.InstallationKey];
					await TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed))] () =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed);
						_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
						_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals1;
						_003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003E1__state = -1;
						((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed>(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed);
						return ((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__2_003Ed._003C_003Et__builder)).Task;
					}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed))] () =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed = default(_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed);
						_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
						_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals1;
						_003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003E1__state = -1;
						((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass3_1._003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed>(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed);
						return ((AsyncTaskMethodBuilder)(ref _003C_003CResetWinfsToPrototypeIfItExists_003Eb__3_003Ed._003C_003Et__builder)).Task;
					}).DetachContext();
					simulatorWasInPrototype = true;
				}
				return simulatorWasInPrototype;
			}).DetachContext();
		}
		return false;
	}

	[AsyncStateMachine(typeof(_003CResetSimulator_003Ed__4))]
	public System.Threading.Tasks.Task ResetSimulator(ReactiveSimulator simulator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CResetSimulator_003Ed__4 _003CResetSimulator_003Ed__ = default(_003CResetSimulator_003Ed__4);
		_003CResetSimulator_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CResetSimulator_003Ed__.simulator = simulator;
		_003CResetSimulator_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CResetSimulator_003Ed__._003C_003Et__builder)).Start<_003CResetSimulator_003Ed__4>(ref _003CResetSimulator_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CResetSimulator_003Ed__._003C_003Et__builder)).Task;
	}

	public IObservable<bool> CanBeReset(ReactiveSimulator simulator)
	{
		return Observable.CombineLatest<bool, SimPowerStats, bool, bool>(simulator.SimData.GetCachedThenUpdatedAndThenContinualValues().WhereClassNotNull<SimInstallData>().Select<SimInstallData, bool>((Func<SimInstallData, bool>)((SimInstallData simData) => simData.HasArchivedWinfsMount))
			.DistinctUntilChanged<bool>(), simulator.PowerUpStatsOverTime(), _simRunner.SafeWhenIsRunningChanges(), (bool hasArchive, SimPowerStats stats, bool isRunning) => hasArchive && stats.PowerUpsSinceReset > 0 && !isRunning).PrependCachedValue(immediate: false);
	}
}
