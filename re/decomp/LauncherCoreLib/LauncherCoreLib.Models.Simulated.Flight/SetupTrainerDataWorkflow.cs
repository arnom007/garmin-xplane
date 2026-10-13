using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal class SetupTrainerDataWorkflow : PreflightOnlyWorkflow
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass5_0
	{
		public IActiveSetting s;

		internal ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> _003CLoadConstraintsAndResolve_003Eb__3(System.Collections.Generic.IReadOnlyList<ISettingValueDefinition> options)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return new ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>(s, options);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCreateSessionData_003Ed__6 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TrainerSessionData> _003C_003Et__builder;

		public SetupTrainerDataWorkflow _003C_003E4__this;

		public SimInstallData sim;

		public FamilyData simFamilyData;

		public ITrainerPackage trainerPackage;

		private AvionicsModel _003Cavionics_003E5__2;

		private ConfiguredTaskAwaiter<AvionicsModel> _003C_003Eu__1;

		private ISettingsBinder<AircraftPackageFamily> _003CfamilyBinder_003E5__3;

		private ConfiguredTaskAwaiter<ISettingsBinder<AircraftPackageFamily>> _003C_003Eu__2;

		private ISettingsBinder<SimAircraftKey> _003CslotBinder_003E5__4;

		private ConfiguredTaskAwaiter<ISettingsBinder<SimAircraftKey>> _003C_003Eu__3;

		private IImmutableDictionary<string, string> _003CfamilySettings_003E5__5;

		private ConfiguredTaskAwaiter<System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>> _003C_003Eu__4;

		private void MoveNext()
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SetupTrainerDataWorkflow setupTrainerDataWorkflow = _003C_003E4__this;
			TrainerSessionData result5;
			try
			{
				ConfiguredTaskAwaiter<AvionicsModel> awaiter2;
				ConfiguredTaskAwaiter<ISettingsBinder<AircraftPackageFamily>> awaiter;
				ISettingsBinder<AircraftPackageFamily> result;
				AvionicsModel result2;
				switch (num)
				{
				default:
					awaiter2 = setupTrainerDataWorkflow.GetAvionics(sim).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AvionicsModel>, _003CCreateSessionData_003Ed__6>(ref awaiter2, ref this);
						return;
					}
					goto IL_008e;
				case 0:
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AvionicsModel>);
					num = (_003C_003E1__state = -1);
					goto IL_008e;
				case 1:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ISettingsBinder<AircraftPackageFamily>>);
					num = (_003C_003E1__state = -1);
					goto IL_0131;
				case 2:
				case 3:
				case 4:
					break;
					IL_0131:
					result = awaiter.GetResult();
					_003CfamilyBinder_003E5__3 = result;
					break;
					IL_008e:
					result2 = awaiter2.GetResult();
					_003Cavionics_003E5__2 = result2;
					awaiter = setupTrainerDataWorkflow._unitOfWork.Settings.BindFamilySettings(new AircraftPackageFamily(sim.AircraftSimKey.AircraftIdentifier, sim.AircraftSimKey.InstallationKey.ReleaseId)).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISettingsBinder<AircraftPackageFamily>>, _003CCreateSessionData_003Ed__6>(ref awaiter, ref this);
						return;
					}
					goto IL_0131;
				}
				try
				{
					ConfiguredTaskAwaiter<ISettingsBinder<SimAircraftKey>> awaiter3;
					if (num != 2)
					{
						if ((uint)(num - 3) <= 1u)
						{
							goto IL_01d7;
						}
						awaiter3 = setupTrainerDataWorkflow._unitOfWork.Settings.BindSimulator(sim.AircraftSimKey).DetachContext().GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__3 = awaiter3;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISettingsBinder<SimAircraftKey>>, _003CCreateSessionData_003Ed__6>(ref awaiter3, ref this);
							return;
						}
					}
					else
					{
						awaiter3 = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter<ISettingsBinder<SimAircraftKey>>);
						num = (_003C_003E1__state = -1);
					}
					ISettingsBinder<SimAircraftKey> result3 = awaiter3.GetResult();
					_003CslotBinder_003E5__4 = result3;
					goto IL_01d7;
					IL_01d7:
					try
					{
						ConfiguredTaskAwaiter<System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>> awaiter4;
						if (num != 3)
						{
							if (num == 4)
							{
								awaiter4 = _003C_003Eu__4;
								_003C_003Eu__4 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>>);
								num = (_003C_003E1__state = -1);
								goto IL_03b2;
							}
							System.Collections.Generic.IList<KeyValuePair<SettingIdentifier, string>> list = _003CfamilyBinder_003E5__3.Table.LoadPersistedOptionsAndGetInvalids((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)simFamilyData.Settings);
							System.Collections.Generic.IList<KeyValuePair<SettingIdentifier, string>> list2 = _003CslotBinder_003E5__4.Table.LoadPersistedOptionsAndGetInvalids((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)sim.Settings);
							List<KeyValuePair<SettingIdentifier, string>> val = Enumerable.ToList<KeyValuePair<SettingIdentifier, string>>(Enumerable.Concat<KeyValuePair<SettingIdentifier, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)list, (System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)list2));
							if (val.Count > 0)
							{
								Logger.Log(LogLevel.Warning, $"[{sim.InstallationKey}] Invalid settings were persisted: {string.Join<SettingIdentifier>(", ", Enumerable.Select<KeyValuePair<SettingIdentifier, string>, SettingIdentifier>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)val, (Func<KeyValuePair<SettingIdentifier, string>, SettingIdentifier>)((KeyValuePair<SettingIdentifier, string> s) => s.Key)))}");
							}
							awaiter4 = LoadConstraintsAndResolve(_003CfamilyBinder_003E5__3.Table.Settings).DetachContext().GetAwaiter();
							if (!awaiter4.IsCompleted)
							{
								num = (_003C_003E1__state = 3);
								_003C_003Eu__4 = awaiter4;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>>, _003CCreateSessionData_003Ed__6>(ref awaiter4, ref this);
								return;
							}
						}
						else
						{
							awaiter4 = _003C_003Eu__4;
							_003C_003Eu__4 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>>);
							num = (_003C_003E1__state = -1);
						}
						System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>> result4 = awaiter4.GetResult();
						_003CfamilySettings_003E5__5 = ResolveSimulatorValues(result4, trainerPackage.GetScopedSettingDefinitions()[ManifestSettingScope.Family]);
						awaiter4 = LoadConstraintsAndResolve(_003CslotBinder_003E5__4.Table.Settings).DetachContext().GetAwaiter();
						if (!awaiter4.IsCompleted)
						{
							num = (_003C_003E1__state = 4);
							_003C_003Eu__4 = awaiter4;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>>, _003CCreateSessionData_003Ed__6>(ref awaiter4, ref this);
							return;
						}
						goto IL_03b2;
						IL_03b2:
						IImmutableDictionary<string, string> items = ResolveSimulatorValues(awaiter4.GetResult(), trainerPackage.GetScopedSettingDefinitions()[ManifestSettingScope.Slot]);
						IImmutableDictionary<string, string> settings = _003CfamilySettings_003E5__5.SetItems((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)items);
						result5 = new TrainerSessionDataBuilder
						{
							InstallData = sim,
							EndpointName = new TrainerIdentifier(sim.InstallationKey.ReleaseId.FamilyId.IdentString + "_" + sim.InstallationKey.Slot.InstanceId),
							PackageInfo = _003Cavionics_003E5__2,
							Settings = settings,
							WinfsMountPath = setupTrainerDataWorkflow.BuildWinFsPath(sim)
						}.ToImmutable();
					}
					finally
					{
						if (num < 0 && _003CslotBinder_003E5__4 != null)
						{
							((System.IDisposable)_003CslotBinder_003E5__4).Dispose();
						}
					}
				}
				finally
				{
					if (num < 0 && _003CfamilyBinder_003E5__3 != null)
					{
						((System.IDisposable)_003CfamilyBinder_003E5__3).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Cavionics_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Cavionics_003E5__2 = null;
			_003C_003Et__builder.SetResult(result5);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetAvionics_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AvionicsModel> _003C_003Et__builder;

		public SetupTrainerDataWorkflow _003C_003E4__this;

		public SimInstallData sim;

		private ConfiguredTaskAwaiter<AvionicsModel> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SetupTrainerDataWorkflow setupTrainerDataWorkflow = _003C_003E4__this;
			AvionicsModel result;
			try
			{
				ConfiguredTaskAwaiter<AvionicsModel> awaiter;
				if (num != 0)
				{
					awaiter = setupTrainerDataWorkflow._unitOfWork.Avionics.GetByModelKey(sim.ToFullModelId()).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AvionicsModel>, _003CGetAvionics_003Ed__7>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AvionicsModel>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CLoadConstraintsAndResolve_003Ed__5 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>> _003C_003Et__builder;

		public System.Collections.Generic.IEnumerable<IActiveSetting> settings;

		private object _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>> result;
			try
			{
				AsyncSubject<System.Collections.Generic.IList<KeyValuePair<SettingIdentifier, string>>> asyncSubject;
				if (num != 0)
				{
					asyncSubject = Enumerable.Select<IActiveSetting, IObservable<ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>>>(settings, (Func<IActiveSetting, IObservable<ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>>>)delegate(IActiveSetting s)
					{
						_003C_003Ec__DisplayClass5_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass5_0
						{
							s = s
						};
						return CS_0024_003C_003E8__locals0.s.WhenAvailableOptionsChange.Take<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>(1).Select<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>, ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>>((Func<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>, ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>>)((System.Collections.Generic.IReadOnlyList<ISettingValueDefinition> options) => new ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>(CS_0024_003C_003E8__locals0.s, options)));
					}).Merge().Where<ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>>((Func<ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>, bool>)((ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> t) => ((System.Collections.Generic.IReadOnlyCollection<ISettingValueDefinition>)t.Item2).Count > 0))
						.Select<ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>, KeyValuePair<SettingIdentifier, string>>((Func<ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>, KeyValuePair<SettingIdentifier, string>>)((ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> t) => new KeyValuePair<SettingIdentifier, string>(t.Item1.LocalId, t.Item1.CurrentValue)))
						.ToList<KeyValuePair<SettingIdentifier, string>>()
						.GetAwaiter<System.Collections.Generic.IList<KeyValuePair<SettingIdentifier, string>>>();
					if (!asyncSubject.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = asyncSubject;
						_003C_003Et__builder.AwaitOnCompleted<AsyncSubject<System.Collections.Generic.IList<KeyValuePair<SettingIdentifier, string>>>, _003CLoadConstraintsAndResolve_003Ed__5>(ref asyncSubject, ref this);
						return;
					}
				}
				else
				{
					asyncSubject = (AsyncSubject<System.Collections.Generic.IList<KeyValuePair<SettingIdentifier, string>>>)_003C_003Eu__1;
					_003C_003Eu__1 = null;
					num = (_003C_003E1__state = -1);
				}
				result = (System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)asyncSubject.GetResult();
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

	private readonly IDeploymentPathProvider _deployment;

	private readonly IUnitOfWork _unitOfWork;

	public SetupTrainerDataWorkflow(IDeploymentPathProvider deployment, IUnitOfWork unitOfWork)
	{
		_deployment = deployment;
		_unitOfWork = unitOfWork;
	}

	public override System.Threading.Tasks.Task<ISessionEnvironment> PreFlight(ISessionEnvironment sessionEnvironment, CancellationToken token)
	{
		FlightSessionRequestInfo creationRequest = sessionEnvironment.Inputs.CreationRequestOrThrow();
		System.Threading.Tasks.Task<TrainerSessionData>[] asyncUpdates = Enumerable.ToArray<System.Threading.Tasks.Task<TrainerSessionData>>(Enumerable.Select<SimInstallData, System.Threading.Tasks.Task<TrainerSessionData>>(creationRequest.AircraftSnapShot.Panel.ActiveSims, (Func<SimInstallData, System.Threading.Tasks.Task<TrainerSessionData>>)((SimInstallData sim) => CreateSessionData(sim, creationRequest.Packages[sim.FullPackageIdOrThrow()], creationRequest.AircraftSnapShot.FamilyLookup.GetFamilyDataOrEmpty(sim.FullPackageIdOrThrow().EmbeddedReleaseCompositeId)))));
		return sessionEnvironment.UpdateTrainerData(asyncUpdates);
	}

	private static IImmutableDictionary<string, string> ResolveSimulatorValues(System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>> settings, ISettingDefinitionCollection definitions)
	{
		ISettingDefinitionCollection definitions2 = definitions;
		return (IImmutableDictionary<string, string>)(object)ImmutableDictionary.ToImmutableDictionary<KeyValuePair<SettingIdentifier, string>, string, string>(settings, (Func<KeyValuePair<SettingIdentifier, string>, string>)((KeyValuePair<SettingIdentifier, string> rs) => ((object)rs.Key.LocalId).ToString()), (Func<KeyValuePair<SettingIdentifier, string>, string>)((KeyValuePair<SettingIdentifier, string> rs) => ((definitions2.GetSetting(rs.Key) ?? throw new KeyNotFoundException("The " + rs.Key + " setting not found in the package.")).GetValue(rs.Value) ?? throw new KeyNotFoundException("The " + rs.Key + " Setting value (" + rs.Value + ") was not found in the package.")).SimulatorValue));
	}

	[AsyncStateMachine(typeof(_003CLoadConstraintsAndResolve_003Ed__5))]
	private static async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>> LoadConstraintsAndResolve(System.Collections.Generic.IEnumerable<IActiveSetting> settings)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return (System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)(await Enumerable.Select<IActiveSetting, IObservable<ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>>>(settings, (Func<IActiveSetting, IObservable<ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>>>)((IActiveSetting s) => s.WhenAvailableOptionsChange.Take<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>(1).Select<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>, ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>>((Func<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>, ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>>)((System.Collections.Generic.IReadOnlyList<ISettingValueDefinition> options) => new ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>(s, options))))).Merge().Where<ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>>((Func<ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>, bool>)((ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> t) => ((System.Collections.Generic.IReadOnlyCollection<ISettingValueDefinition>)t.Item2).Count > 0))
			.Select<ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>, KeyValuePair<SettingIdentifier, string>>((Func<ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>, KeyValuePair<SettingIdentifier, string>>)((ValueTuple<IActiveSetting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> t) => new KeyValuePair<SettingIdentifier, string>(t.Item1.LocalId, t.Item1.CurrentValue)))
			.ToList<KeyValuePair<SettingIdentifier, string>>());
	}

	[AsyncStateMachine(typeof(_003CCreateSessionData_003Ed__6))]
	private async System.Threading.Tasks.Task<TrainerSessionData> CreateSessionData(SimInstallData sim, ITrainerPackage trainerPackage, FamilyData simFamilyData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AvionicsModel avionics = await GetAvionics(sim).DetachContext();
		using ISettingsBinder<AircraftPackageFamily> familyBinder = await _unitOfWork.Settings.BindFamilySettings(new AircraftPackageFamily(sim.AircraftSimKey.AircraftIdentifier, sim.AircraftSimKey.InstallationKey.ReleaseId)).DetachContext();
		using ISettingsBinder<SimAircraftKey> slotBinder = await _unitOfWork.Settings.BindSimulator(sim.AircraftSimKey).DetachContext();
		System.Collections.Generic.IList<KeyValuePair<SettingIdentifier, string>> list = familyBinder.Table.LoadPersistedOptionsAndGetInvalids((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)simFamilyData.Settings);
		System.Collections.Generic.IList<KeyValuePair<SettingIdentifier, string>> list2 = slotBinder.Table.LoadPersistedOptionsAndGetInvalids((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)sim.Settings);
		List<KeyValuePair<SettingIdentifier, string>> val = Enumerable.ToList<KeyValuePair<SettingIdentifier, string>>(Enumerable.Concat<KeyValuePair<SettingIdentifier, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)list, (System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)list2));
		if (val.Count > 0)
		{
			Logger.Log(LogLevel.Warning, $"[{sim.InstallationKey}] Invalid settings were persisted: {string.Join<SettingIdentifier>(", ", Enumerable.Select<KeyValuePair<SettingIdentifier, string>, SettingIdentifier>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)val, (Func<KeyValuePair<SettingIdentifier, string>, SettingIdentifier>)((KeyValuePair<SettingIdentifier, string> s) => s.Key)))}");
		}
		IImmutableDictionary<string, string> familySettings = ResolveSimulatorValues(await LoadConstraintsAndResolve(familyBinder.Table.Settings).DetachContext(), trainerPackage.GetScopedSettingDefinitions()[ManifestSettingScope.Family]);
		IImmutableDictionary<string, string> items = ResolveSimulatorValues(await LoadConstraintsAndResolve(slotBinder.Table.Settings).DetachContext(), trainerPackage.GetScopedSettingDefinitions()[ManifestSettingScope.Slot]);
		IImmutableDictionary<string, string> settings = familySettings.SetItems((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)items);
		return new TrainerSessionDataBuilder
		{
			InstallData = sim,
			EndpointName = new TrainerIdentifier(sim.InstallationKey.ReleaseId.FamilyId.IdentString + "_" + sim.InstallationKey.Slot.InstanceId),
			PackageInfo = avionics,
			Settings = settings,
			WinfsMountPath = BuildWinFsPath(sim)
		}.ToImmutable();
	}

	[AsyncStateMachine(typeof(_003CGetAvionics_003Ed__7))]
	private async System.Threading.Tasks.Task<AvionicsModel> GetAvionics(SimInstallData sim)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await _unitOfWork.Avionics.GetByModelKey(sim.ToFullModelId()).DetachContext();
	}

	private string BuildWinFsPath(SimInstallData sim)
	{
		return SessionWinfsDirectory.GetSimDirPath(_deployment, UniqueUserIdentifierProvider.Provider, sim.AircraftSimKey);
	}
}
