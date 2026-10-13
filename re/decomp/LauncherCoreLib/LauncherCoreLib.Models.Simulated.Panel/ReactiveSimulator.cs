using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Utility;
using RecordShims;

namespace LauncherCoreLib.Models.Simulated.Panel;

public class ReactiveSimulator : DalContainer
{
	private sealed class SimDataContainer : ReactiveEntityBase<SimInstallData>
	{
		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CGetLatestValue_003Ed__4 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<SimInstallData> _003C_003Et__builder;

			public SimDataContainer _003C_003E4__this;

			private ConfiguredTaskAwaiter<SimInstallData> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				SimDataContainer simDataContainer = _003C_003E4__this;
				SimInstallData result;
				try
				{
					ConfiguredTaskAwaiter<SimInstallData> awaiter;
					if (num != 0)
					{
						awaiter = simDataContainer._unitofWork.Simulators.GetSimulator(simDataContainer._simAircraftKey.AircraftIdentifier, simDataContainer._simAircraftKey.InstallationKey).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimInstallData>, _003CGetLatestValue_003Ed__4>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimInstallData>);
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

		private readonly IUnitOfWork _unitofWork;

		private readonly SimAircraftKey _simAircraftKey;

		public SimDataContainer(IUnitOfWork unitOfWork, SimInstallData simInstallData)
		{
			_unitofWork = unitOfWork;
			_simAircraftKey = simInstallData.AircraftSimKey;
			base.CachedValue = simInstallData;
		}

		protected override IObservable<TimingEvent> GetHotUpdateEvents()
		{
			return _unitofWork.Simulators.WhenSimulatorIsChangedOnAircraft(_simAircraftKey.AircraftIdentifier).OnCreateOrUpdate<SimInstallKey>().ToTimingEventStream<SimInstallKey>();
		}

		[AsyncStateMachine(typeof(_003CGetLatestValue_003Ed__4))]
		protected override async System.Threading.Tasks.Task<SimInstallData> GetLatestValue(CancellationToken token = default(CancellationToken))
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await _unitofWork.Simulators.GetSimulator(_simAircraftKey.AircraftIdentifier, _simAircraftKey.InstallationKey).DetachContext();
		}
	}

	private sealed class FullSimContainer : ReactiveEntityBase<SimPackModel>
	{
		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CGetLatestValue_003Ed__4 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<SimPackModel> _003C_003Et__builder;

			public FullSimContainer _003C_003E4__this;

			private SimInstallData _003CsimData_003E5__2;

			private ConfiguredTaskAwaiter<SimInstallData> _003C_003Eu__1;

			private ConfiguredTaskAwaiter<AvionicsModel> _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				FullSimContainer fullSimContainer = _003C_003E4__this;
				SimPackModel result3;
				try
				{
					ConfiguredTaskAwaiter<AvionicsModel> awaiter;
					ConfiguredTaskAwaiter<SimInstallData> awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<AvionicsModel>);
							num = (_003C_003E1__state = -1);
							goto IL_0100;
						}
						awaiter2 = fullSimContainer._simData.GetUpdatedValue().DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimInstallData>, _003CGetLatestValue_003Ed__4>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimInstallData>);
						num = (_003C_003E1__state = -1);
					}
					SimInstallData result = awaiter2.GetResult();
					_003CsimData_003E5__2 = result;
					awaiter = fullSimContainer._unitOfWork.Avionics.GetByModelKey(_003CsimData_003E5__2.ToFullModelId()).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AvionicsModel>, _003CGetLatestValue_003Ed__4>(ref awaiter, ref this);
						return;
					}
					goto IL_0100;
					IL_0100:
					AvionicsModel result2 = awaiter.GetResult();
					result3 = new SimPackModel(_003CsimData_003E5__2, result2);
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003CsimData_003E5__2 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003CsimData_003E5__2 = null;
				_003C_003Et__builder.SetResult(result3);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		private readonly IUnitOfWork _unitOfWork;

		private readonly IReactiveEntity<SimInstallData> _simData;

		public FullSimContainer(IUnitOfWork unitOfWork, IReactiveEntity<SimInstallData> simData)
		{
			_unitOfWork = unitOfWork;
			_simData = simData;
		}

		protected override IObservable<TimingEvent> GetHotUpdateEvents()
		{
			return _simData.GetHotContinualValues().ToTimingEventStream<SimInstallData>();
		}

		[AsyncStateMachine(typeof(_003CGetLatestValue_003Ed__4))]
		protected override async System.Threading.Tasks.Task<SimPackModel> GetLatestValue(CancellationToken token = default(CancellationToken))
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			SimInstallData simData = await _simData.GetUpdatedValue().DetachContext();
			return new SimPackModel(simData, await _unitOfWork.Avionics.GetByModelKey(simData.ToFullModelId()).DetachContext());
		}
	}

	private sealed class Rebinder<T> : ISettingsBinder<ReactiveSimulator>, System.IDisposable
	{
		private readonly ISettingsBinder<T> _actualBinder;

		public ReactiveSimulator Entity
		{
			[CompilerGenerated]
			get;
		}

		public ISettingSymbolTable Table => _actualBinder.Table;

		public Rebinder(ISettingsBinder<T> binder, ReactiveSimulator sim)
		{
			_actualBinder = binder;
			Entity = sim;
		}

		public void Dispose()
		{
			((System.IDisposable)_actualBinder).Dispose();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass22_0
	{
		public IDictionary<SettingIdentifier, string> updatedSettings;

		internal void _003CUpdateSettings_003Eb__0(IPropertyChangeSet<SimInstallData> _)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "s");
			_.Mutate<ImmutableDictionary<SettingIdentifier, string>>(Expression.Lambda<Func<SimInstallData, ImmutableDictionary<SettingIdentifier, string>>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), ImmutableDictionary.ToImmutableDictionary<SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)updatedSettings));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass23_0
	{
		public IDictionary<SettingIdentifier, string> updatedSettings;

		internal void _003CUpdateFamilySettings_003Eb__0(IPropertyChangeSet<FamilyData> _)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			ParameterExpression val = Expression.Parameter(typeof(FamilyData), "s");
			_.Mutate<ImmutableDictionary<SettingIdentifier, string>>(Expression.Lambda<Func<FamilyData, ImmutableDictionary<SettingIdentifier, string>>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), ImmutableDictionary.ToImmutableDictionary<SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)updatedSettings));
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CBindFamilySettings_003Ed__21 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISettingsBinder<ReactiveSimulator>> _003C_003Et__builder;

		public ReactiveSimulator _003C_003E4__this;

		private ConfiguredTaskAwaiter<ISettingsBinder<AircraftPackageFamily>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveSimulator reactiveSimulator = _003C_003E4__this;
			ISettingsBinder<ReactiveSimulator> result;
			try
			{
				ConfiguredTaskAwaiter<ISettingsBinder<AircraftPackageFamily>> awaiter;
				if (num != 0)
				{
					awaiter = reactiveSimulator.UnitOfWork.Settings.BindFamilySettings(reactiveSimulator.AircraftPackageFamily).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISettingsBinder<AircraftPackageFamily>>, _003CBindFamilySettings_003Ed__21>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISettingsBinder<AircraftPackageFamily>>);
					num = (_003C_003E1__state = -1);
				}
				result = new Rebinder<AircraftPackageFamily>(awaiter.GetResult(), reactiveSimulator);
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
	private struct _003CBindSettings_003Ed__20 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISettingsBinder<ReactiveSimulator>> _003C_003Et__builder;

		public ReactiveSimulator _003C_003E4__this;

		private ConfiguredTaskAwaiter<ISettingsBinder<SimAircraftKey>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveSimulator reactiveSimulator = _003C_003E4__this;
			ISettingsBinder<ReactiveSimulator> result;
			try
			{
				ConfiguredTaskAwaiter<ISettingsBinder<SimAircraftKey>> awaiter;
				if (num != 0)
				{
					awaiter = reactiveSimulator.UnitOfWork.Settings.BindSimulator(reactiveSimulator.AircraftAndSim).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISettingsBinder<SimAircraftKey>>, _003CBindSettings_003Ed__20>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISettingsBinder<SimAircraftKey>>);
					num = (_003C_003E1__state = -1);
				}
				result = new Rebinder<SimAircraftKey>(awaiter.GetResult(), reactiveSimulator);
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
	private struct _003CDeleteWinfsArchive_003Ed__19 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveSimulator _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveSimulator reactiveSimulator = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = reactiveSimulator.UnitOfWork.Simulators.DeleteAircraftWinfsArchives(reactiveSimulator.AircraftAndSim.AircraftIdentifier, reactiveSimulator.AircraftAndSim.InstallationKey.Yield()).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CDeleteWinfsArchive_003Ed__19>(ref awaiter, ref this);
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
	private struct _003CGetWinfsArchive_003Ed__17 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveSimulator _003C_003E4__this;

		public string tempArchivePath;

		private ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveSimulator reactiveSimulator = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>> awaiter;
				if (num != 0)
				{
					awaiter = reactiveSimulator.UnitOfWork.Simulators.GetAircraftWinfsArchives(reactiveSimulator.AircraftAndSim.AircraftIdentifier, reactiveSimulator.CreateWinfsDictionaryForSinglePath(tempArchivePath)).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>>, _003CGetWinfsArchive_003Ed__17>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>>);
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CResetPowerUpStats_003Ed__25 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveSimulator _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveSimulator reactiveSimulator = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = reactiveSimulator.UnitOfWork.PowerStats.ResetPowerUps(reactiveSimulator.AircraftAndSim).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CResetPowerUpStats_003Ed__25>(ref awaiter, ref this);
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
	private struct _003CUpdate_003Ed__16 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveSimulator _003C_003E4__this;

		public SimInstallData updateToMake;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveSimulator reactiveSimulator = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = reactiveSimulator.UnitOfWork.Simulators.CreateOrUpdateSimulator(reactiveSimulator.AircraftAndSim.AircraftIdentifier, updateToMake).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdate_003Ed__16>(ref awaiter, ref this);
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
	private struct _003CUpdateFamilySettings_003Ed__23 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public IDictionary<SettingIdentifier, string> updatedSettings;

		public ReactiveSimulator _003C_003E4__this;

		private _003C_003Ec__DisplayClass23_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<FamilyData> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveSimulator reactiveSimulator = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaiter<FamilyData> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_012d;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass23_0();
					_003C_003E8__1.updatedSettings = updatedSettings;
					awaiter2 = reactiveSimulator.UnitOfWork.FamilyData.GetFamilyDataForReleaseOrDefault(reactiveSimulator.AircraftPackageFamily).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<FamilyData>, _003CUpdateFamilySettings_003Ed__23>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<FamilyData>);
					num = (_003C_003E1__state = -1);
				}
				FamilyData data = awaiter2.GetResult().With(delegate(IPropertyChangeSet<FamilyData> _)
				{
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Expected O, but got Unknown
					ParameterExpression val2 = Expression.Parameter(typeof(FamilyData), "s");
					_.Mutate<ImmutableDictionary<SettingIdentifier, string>>(Expression.Lambda<Func<FamilyData, ImmutableDictionary<SettingIdentifier, string>>>((Expression)(object)Expression.Property((Expression)(object)val2, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val2 }), ImmutableDictionary.ToImmutableDictionary<SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)_003C_003E8__1.updatedSettings));
				});
				ConfiguredTaskAwaitable val = reactiveSimulator.UnitOfWork.FamilyData.UpdateFamilyDataForRelease(reactiveSimulator.AircraftPackageFamily, data).DetachContext();
				awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateFamilySettings_003Ed__23>(ref awaiter, ref this);
					return;
				}
				goto IL_012d;
				IL_012d:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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
	private struct _003CUpdateSettings_003Ed__22 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public IDictionary<SettingIdentifier, string> updatedSettings;

		public ReactiveSimulator _003C_003E4__this;

		private _003C_003Ec__DisplayClass22_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<SimInstallData> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveSimulator reactiveSimulator = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaiter<SimInstallData> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_0127;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass22_0();
					_003C_003E8__1.updatedSettings = updatedSettings;
					awaiter2 = reactiveSimulator.SimData.GetUpdatedValue().DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimInstallData>, _003CUpdateSettings_003Ed__22>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimInstallData>);
					num = (_003C_003E1__state = -1);
				}
				SimInstallData simulator = awaiter2.GetResult().With(delegate(IPropertyChangeSet<SimInstallData> _)
				{
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Expected O, but got Unknown
					ParameterExpression val2 = Expression.Parameter(typeof(SimInstallData), "s");
					_.Mutate<ImmutableDictionary<SettingIdentifier, string>>(Expression.Lambda<Func<SimInstallData, ImmutableDictionary<SettingIdentifier, string>>>((Expression)(object)Expression.Property((Expression)(object)val2, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val2 }), ImmutableDictionary.ToImmutableDictionary<SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)_003C_003E8__1.updatedSettings));
				});
				ConfiguredTaskAwaitable val = reactiveSimulator.UnitOfWork.Simulators.CreateOrUpdateSimulator(reactiveSimulator.AircraftAndSim.AircraftIdentifier, simulator).DetachContext();
				awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateSettings_003Ed__22>(ref awaiter, ref this);
					return;
				}
				goto IL_0127;
				IL_0127:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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
	private struct _003CUpdateWinfsArchive_003Ed__18 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveSimulator _003C_003E4__this;

		public string tempArchivePath;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveSimulator reactiveSimulator = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = reactiveSimulator.UnitOfWork.Simulators.UpdateAircraftWinfsArchives(reactiveSimulator.AircraftAndSim.AircraftIdentifier, reactiveSimulator.CreateWinfsDictionaryForSinglePath(tempArchivePath)).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateWinfsArchive_003Ed__18>(ref awaiter, ref this);
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

	[field: CompilerGenerated]
	public SimAircraftKey AircraftAndSim
	{
		[CompilerGenerated]
		get;
	}

	public InstallationSlot InstallSlot => AircraftAndSim.InstallationKey.Slot;

	[field: CompilerGenerated]
	public IReactiveEntity<SimInstallData> SimData
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IReactiveEntity<SimPackModel> FullSimData
	{
		[CompilerGenerated]
		get;
	}

	public AircraftPackageFamily AircraftPackageFamily => new AircraftPackageFamily(AircraftAndSim.AircraftIdentifier, AircraftAndSim.InstallationKey.ReleaseId);

	public ReactiveSimulator(IUnitOfWork unitOfWork, SimAircraftKey aircraftSimKey)
		: this(unitOfWork, new SimInstallData(aircraftSimKey))
	{
	}

	public ReactiveSimulator(IUnitOfWork unitOfWork, SimInstallData installData)
		: base(unitOfWork)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (installData == null)
		{
			throw new ArgumentNullException("installData");
		}
		AircraftAndSim = installData.AircraftSimKey;
		SimData = new SimDataContainer(unitOfWork, installData);
		FullSimData = new FullSimContainer(unitOfWork, SimData);
	}

	public ReactiveAircraft CreateAircraftWrapper()
	{
		return new ReactiveAircraft(base.UnitOfWork, AircraftAndSim.AircraftIdentifier);
	}

	[AsyncStateMachine(typeof(_003CUpdate_003Ed__16))]
	public System.Threading.Tasks.Task Update(SimInstallData updateToMake)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdate_003Ed__16 _003CUpdate_003Ed__ = default(_003CUpdate_003Ed__16);
		_003CUpdate_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdate_003Ed__._003C_003E4__this = this;
		_003CUpdate_003Ed__.updateToMake = updateToMake;
		_003CUpdate_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdate_003Ed__._003C_003Et__builder)).Start<_003CUpdate_003Ed__16>(ref _003CUpdate_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdate_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CGetWinfsArchive_003Ed__17))]
	public System.Threading.Tasks.Task GetWinfsArchive(string tempArchivePath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CGetWinfsArchive_003Ed__17 _003CGetWinfsArchive_003Ed__ = default(_003CGetWinfsArchive_003Ed__17);
		_003CGetWinfsArchive_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CGetWinfsArchive_003Ed__._003C_003E4__this = this;
		_003CGetWinfsArchive_003Ed__.tempArchivePath = tempArchivePath;
		_003CGetWinfsArchive_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CGetWinfsArchive_003Ed__._003C_003Et__builder)).Start<_003CGetWinfsArchive_003Ed__17>(ref _003CGetWinfsArchive_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CGetWinfsArchive_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUpdateWinfsArchive_003Ed__18))]
	public System.Threading.Tasks.Task UpdateWinfsArchive(string tempArchivePath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateWinfsArchive_003Ed__18 _003CUpdateWinfsArchive_003Ed__ = default(_003CUpdateWinfsArchive_003Ed__18);
		_003CUpdateWinfsArchive_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateWinfsArchive_003Ed__._003C_003E4__this = this;
		_003CUpdateWinfsArchive_003Ed__.tempArchivePath = tempArchivePath;
		_003CUpdateWinfsArchive_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateWinfsArchive_003Ed__._003C_003Et__builder)).Start<_003CUpdateWinfsArchive_003Ed__18>(ref _003CUpdateWinfsArchive_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateWinfsArchive_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CDeleteWinfsArchive_003Ed__19))]
	public System.Threading.Tasks.Task DeleteWinfsArchive()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CDeleteWinfsArchive_003Ed__19 _003CDeleteWinfsArchive_003Ed__ = default(_003CDeleteWinfsArchive_003Ed__19);
		_003CDeleteWinfsArchive_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CDeleteWinfsArchive_003Ed__._003C_003E4__this = this;
		_003CDeleteWinfsArchive_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CDeleteWinfsArchive_003Ed__._003C_003Et__builder)).Start<_003CDeleteWinfsArchive_003Ed__19>(ref _003CDeleteWinfsArchive_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CDeleteWinfsArchive_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CBindSettings_003Ed__20))]
	public async System.Threading.Tasks.Task<ISettingsBinder<ReactiveSimulator>> BindSettings()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new Rebinder<SimAircraftKey>(await base.UnitOfWork.Settings.BindSimulator(AircraftAndSim).DetachContext(), this);
	}

	[AsyncStateMachine(typeof(_003CBindFamilySettings_003Ed__21))]
	public async System.Threading.Tasks.Task<ISettingsBinder<ReactiveSimulator>> BindFamilySettings()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new Rebinder<AircraftPackageFamily>(await base.UnitOfWork.Settings.BindFamilySettings(AircraftPackageFamily).DetachContext(), this);
	}

	[AsyncStateMachine(typeof(_003CUpdateSettings_003Ed__22))]
	public System.Threading.Tasks.Task UpdateSettings(IDictionary<SettingIdentifier, string> updatedSettings)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateSettings_003Ed__22 _003CUpdateSettings_003Ed__ = default(_003CUpdateSettings_003Ed__22);
		_003CUpdateSettings_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateSettings_003Ed__._003C_003E4__this = this;
		_003CUpdateSettings_003Ed__.updatedSettings = updatedSettings;
		_003CUpdateSettings_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateSettings_003Ed__._003C_003Et__builder)).Start<_003CUpdateSettings_003Ed__22>(ref _003CUpdateSettings_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateSettings_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUpdateFamilySettings_003Ed__23))]
	public System.Threading.Tasks.Task UpdateFamilySettings(IDictionary<SettingIdentifier, string> updatedSettings)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateFamilySettings_003Ed__23 _003CUpdateFamilySettings_003Ed__ = default(_003CUpdateFamilySettings_003Ed__23);
		_003CUpdateFamilySettings_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateFamilySettings_003Ed__._003C_003E4__this = this;
		_003CUpdateFamilySettings_003Ed__.updatedSettings = updatedSettings;
		_003CUpdateFamilySettings_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateFamilySettings_003Ed__._003C_003Et__builder)).Start<_003CUpdateFamilySettings_003Ed__23>(ref _003CUpdateFamilySettings_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateFamilySettings_003Ed__._003C_003Et__builder)).Task;
	}

	public IObservable<SimPowerStats> PowerUpStatsOverTime()
	{
		return base.UnitOfWork.PowerStats.SimulatorPowerStatsOverTime(AircraftAndSim);
	}

	[AsyncStateMachine(typeof(_003CResetPowerUpStats_003Ed__25))]
	public System.Threading.Tasks.Task ResetPowerUpStats()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CResetPowerUpStats_003Ed__25 _003CResetPowerUpStats_003Ed__ = default(_003CResetPowerUpStats_003Ed__25);
		_003CResetPowerUpStats_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CResetPowerUpStats_003Ed__._003C_003E4__this = this;
		_003CResetPowerUpStats_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CResetPowerUpStats_003Ed__._003C_003Et__builder)).Start<_003CResetPowerUpStats_003Ed__25>(ref _003CResetPowerUpStats_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CResetPowerUpStats_003Ed__._003C_003Et__builder)).Task;
	}

	private Dictionary<SimInstallKey, string> CreateWinfsDictionaryForSinglePath(string tempArchivePath)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new KeyValuePair<SimInstallKey, string>(AircraftAndSim.InstallationKey, tempArchivePath).Yield<KeyValuePair<SimInstallKey, string>>().ToDictionary();
	}
}
