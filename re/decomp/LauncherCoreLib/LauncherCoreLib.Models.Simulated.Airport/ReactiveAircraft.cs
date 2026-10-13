using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.Logging;
using LOLA.Logging.Reactive;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using LauncherCoreLib.Utility;
using RecordShims;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class ReactiveAircraft : DalContainer, IHaveAircraftId
{
	private sealed class HeavyDataContainer : ReactiveEntityBase<AircraftHeavyData>
	{
		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CGetLatestValue_003Ed__4 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<AircraftHeavyData> _003C_003Et__builder;

			public HeavyDataContainer _003C_003E4__this;

			private ConfiguredTaskAwaiter<AircraftHeavyData> _003C_003Eu__1;

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
				HeavyDataContainer heavyDataContainer = _003C_003E4__this;
				AircraftHeavyData result;
				try
				{
					ConfiguredTaskAwaiter<AircraftHeavyData> awaiter;
					if (num != 0)
					{
						awaiter = heavyDataContainer._unitOfWork.Aircraft.GetAircraftHeavyDataById(heavyDataContainer._aircraftIdentifier).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftHeavyData>, _003CGetLatestValue_003Ed__4>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftHeavyData>);
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

		private readonly IUnitOfWork _unitOfWork;

		private readonly AircraftIdentifier _aircraftIdentifier;

		public HeavyDataContainer(IUnitOfWork unitOfWork, AircraftHeavyData heavyData)
		{
			_unitOfWork = unitOfWork;
			_aircraftIdentifier = heavyData.AircraftIdentifier;
			base.CachedValue = heavyData;
		}

		protected override IObservable<TimingEvent> GetHotUpdateEvents()
		{
			return _unitOfWork.Aircraft.AircraftChanges.ForAircraft(_aircraftIdentifier).OnCreateOrUpdate<AircraftIdentifier>().ToTimingEventStream<AircraftIdentifier>();
		}

		[AsyncStateMachine(typeof(_003CGetLatestValue_003Ed__4))]
		protected override async System.Threading.Tasks.Task<AircraftHeavyData> GetLatestValue(CancellationToken token = default(CancellationToken))
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await _unitOfWork.Aircraft.GetAircraftHeavyDataById(_aircraftIdentifier).DetachContext();
		}
	}

	private sealed class FamilyContainer : ReactiveEntityBase<FamilyDataLookup>
	{
		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CGetLatestValue_003Ed__4 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<FamilyDataLookup> _003C_003Et__builder;

			public FamilyContainer _003C_003E4__this;

			private ConfiguredTaskAwaiter<FamilyDataLookup> _003C_003Eu__1;

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
				FamilyContainer familyContainer = _003C_003E4__this;
				FamilyDataLookup result;
				try
				{
					ConfiguredTaskAwaiter<FamilyDataLookup> awaiter;
					if (num != 0)
					{
						awaiter = familyContainer._unitOfWork.FamilyData.GetAllFamilyDataForAircraft(familyContainer._aircraftIdentifier).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<FamilyDataLookup>, _003CGetLatestValue_003Ed__4>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<FamilyDataLookup>);
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

		private readonly IUnitOfWork _unitOfWork;

		private readonly AircraftIdentifier _aircraftIdentifier;

		public FamilyContainer(IUnitOfWork unitOfWork, AircraftIdentifier aircraftIdentifier, FamilyDataLookup lookup)
		{
			_unitOfWork = unitOfWork;
			_aircraftIdentifier = aircraftIdentifier;
			base.CachedValue = lookup;
		}

		protected override IObservable<TimingEvent> GetHotUpdateEvents()
		{
			return _unitOfWork.FamilyData.WhenAircraftFamilyDataChanges.StripChangeType<AircraftPackageFamily>().Where<AircraftPackageFamily>((Func<AircraftPackageFamily, bool>)([CompilerGenerated] (AircraftPackageFamily apf) => apf.AircraftIdentifier == _aircraftIdentifier)).ToTimingEventStream<AircraftPackageFamily>();
		}

		[AsyncStateMachine(typeof(_003CGetLatestValue_003Ed__4))]
		protected override async System.Threading.Tasks.Task<FamilyDataLookup> GetLatestValue(CancellationToken token = default(CancellationToken))
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await _unitOfWork.FamilyData.GetAllFamilyDataForAircraft(_aircraftIdentifier).DetachContext();
		}
	}

	private sealed class PanelContainer : ReactiveEntityBase<SimPanel>
	{
		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CGetLatestValue_003Ed__4 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<SimPanel> _003C_003Et__builder;

			public PanelContainer _003C_003E4__this;

			private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__1;

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
				PanelContainer panelContainer = _003C_003E4__this;
				SimPanel result;
				try
				{
					ConfiguredTaskAwaiter<SimPanel> awaiter;
					if (num != 0)
					{
						awaiter = panelContainer._unitOfWork.Simulators.GetSimulatorPanel(panelContainer._aircraftIdentifier).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CGetLatestValue_003Ed__4>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPanel>);
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

		private readonly IUnitOfWork _unitOfWork;

		private readonly AircraftIdentifier _aircraftIdentifier;

		public PanelContainer(IUnitOfWork unitOfWork, AircraftIdentifier aircraftIdentifier, SimPanel panel)
		{
			_unitOfWork = unitOfWork;
			_aircraftIdentifier = aircraftIdentifier;
			base.CachedValue = panel;
		}

		protected override IObservable<TimingEvent> GetHotUpdateEvents()
		{
			return _unitOfWork.Simulators.WhenSimulatorIsChangedOnAircraft(_aircraftIdentifier).StripChangeType<SimInstallKey>().ToTimingEventStream<SimInstallKey>();
		}

		[AsyncStateMachine(typeof(_003CGetLatestValue_003Ed__4))]
		protected override async System.Threading.Tasks.Task<SimPanel> GetLatestValue(CancellationToken token = default(CancellationToken))
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await _unitOfWork.Simulators.GetSimulatorPanel(_aircraftIdentifier).DetachContext();
		}
	}

	private sealed class ListDataContainer : ReactiveEntityBase<AircraftListData>
	{
		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CGetLatestValue_003Ed__4 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<AircraftListData> _003C_003Et__builder;

			public ListDataContainer _003C_003E4__this;

			private ConfiguredTaskAwaiter<AircraftListData> _003C_003Eu__1;

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
				ListDataContainer listDataContainer = _003C_003E4__this;
				AircraftListData result;
				try
				{
					ConfiguredTaskAwaiter<AircraftListData> awaiter;
					if (num != 0)
					{
						awaiter = listDataContainer._unitOfWork.Aircraft.GetAircraftListDataById(listDataContainer._aircraftIdentifier).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftListData>, _003CGetLatestValue_003Ed__4>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftListData>);
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

		private readonly IUnitOfWork _unitOfWork;

		private readonly AircraftIdentifier _aircraftIdentifier;

		public ListDataContainer(IUnitOfWork unitOfWork, AircraftListData listData)
		{
			_unitOfWork = unitOfWork;
			_aircraftIdentifier = listData.AircraftIdentifier;
			base.CachedValue = listData;
		}

		protected override IObservable<TimingEvent> GetHotUpdateEvents()
		{
			return _unitOfWork.Aircraft.AircraftChanges.ForAircraft(_aircraftIdentifier).OnCreateOrUpdate<AircraftIdentifier>().ToTimingEventStream<AircraftIdentifier>();
		}

		[AsyncStateMachine(typeof(_003CGetLatestValue_003Ed__4))]
		protected override async System.Threading.Tasks.Task<AircraftListData> GetLatestValue(CancellationToken token = default(CancellationToken))
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await _unitOfWork.Aircraft.GetAircraftListDataById(_aircraftIdentifier).DetachContext();
		}
	}

	private sealed class CompositeSnapshotContainer : IReactiveEntity<AircraftSnapshotData>
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass11_0
		{
			public System.Threading.Tasks.Task<AircraftListData> listTask;

			public System.Threading.Tasks.Task<AircraftHeavyData> heavyTask;

			public System.Threading.Tasks.Task<SimPanel> panelTask;

			public System.Threading.Tasks.Task<FamilyDataLookup> familyTask;

			internal AircraftSnapshotData _003CGetUpdatedValue_003Eb__0(AircraftSnapshotData _)
			{
				return new AircraftSnapshotData(listTask.Result, heavyTask.Result, panelTask.Result, familyTask.Result);
			}
		}

		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CGetUpdatedValue_003Ed__11 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<AircraftSnapshotData> _003C_003Et__builder;

			public CompositeSnapshotContainer _003C_003E4__this;

			private _003C_003Ec__DisplayClass11_0 _003C_003E8__1;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				CompositeSnapshotContainer compositeSnapshotContainer = _003C_003E4__this;
				AircraftSnapshotData result;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						_003C_003E8__1 = new _003C_003Ec__DisplayClass11_0();
						_003C_003E8__1.listTask = compositeSnapshotContainer._listData.GetUpdatedValue();
						_003C_003E8__1.heavyTask = compositeSnapshotContainer._heavyData.GetUpdatedValue();
						_003C_003E8__1.panelTask = compositeSnapshotContainer._panel.GetUpdatedValue();
						_003C_003E8__1.familyTask = compositeSnapshotContainer._familyLookup.GetUpdatedValue();
						ConfiguredTaskAwaitable val = System.Threading.Tasks.Task.WhenAll(new System.Threading.Tasks.Task[4] { _003C_003E8__1.listTask, _003C_003E8__1.heavyTask, _003C_003E8__1.panelTask, _003C_003E8__1.familyTask }).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CGetUpdatedValue_003Ed__11>(ref awaiter, ref this);
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
					result = compositeSnapshotContainer.UpdateWithLock((AircraftSnapshotData _) => new AircraftSnapshotData(_003C_003E8__1.listTask.Result, _003C_003E8__1.heavyTask.Result, _003C_003E8__1.panelTask.Result, _003C_003E8__1.familyTask.Result));
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
				_003C_003Et__builder.SetResult(result);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		private readonly IReactiveEntity<AircraftListData> _listData;

		private readonly IReactiveEntity<AircraftHeavyData> _heavyData;

		private readonly IReactiveEntity<SimPanel> _panel;

		private readonly IReactiveEntity<FamilyDataLookup> _familyLookup;

		private readonly object _cacheLock = new object();

		[field: CompilerGenerated]
		public AircraftSnapshotData CachedValue
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			private set;
		}

		public CompositeSnapshotContainer(IReactiveEntity<AircraftListData> listData, IReactiveEntity<AircraftHeavyData> heavyData, IReactiveEntity<SimPanel> panel, IReactiveEntity<FamilyDataLookup> familyLookup)
		{
			_listData = listData;
			_heavyData = heavyData;
			_panel = panel;
			_familyLookup = familyLookup;
			CachedValue = new AircraftSnapshotData(listData.CachedValue, heavyData.CachedValue, panel.CachedValue, familyLookup.CachedValue);
		}

		public IObservable<AircraftSnapshotData> GetHotContinualValues()
		{
			return _listData.GetHotContinualValues().Select<AircraftListData, AircraftSnapshotData>((Func<AircraftListData, AircraftSnapshotData>)([CompilerGenerated] (AircraftListData ld) => UpdateWithLock((AircraftSnapshotData c) => c.Relist(ld)))).Merge<AircraftSnapshotData>(_heavyData.GetHotContinualValues().Select<AircraftHeavyData, AircraftSnapshotData>((Func<AircraftHeavyData, AircraftSnapshotData>)([CompilerGenerated] (AircraftHeavyData hd) => UpdateWithLock((AircraftSnapshotData c) => c.ReplaceHeavyData(hd)))))
				.Merge<AircraftSnapshotData>(_panel.GetHotContinualValues().Select<SimPanel, AircraftSnapshotData>((Func<SimPanel, AircraftSnapshotData>)([CompilerGenerated] (SimPanel p) => UpdateWithLock((AircraftSnapshotData c) => c.ReplacePanel(p)))))
				.Merge<AircraftSnapshotData>(_familyLookup.GetHotContinualValues().Select<FamilyDataLookup, AircraftSnapshotData>((Func<FamilyDataLookup, AircraftSnapshotData>)([CompilerGenerated] (FamilyDataLookup fl) => UpdateWithLock((AircraftSnapshotData c) => c.ReplaceFamilyLookup(fl)))));
		}

		[AsyncStateMachine(typeof(_003CGetUpdatedValue_003Ed__11))]
		public async System.Threading.Tasks.Task<AircraftSnapshotData> GetUpdatedValue()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			System.Threading.Tasks.Task<AircraftListData> listTask = _listData.GetUpdatedValue();
			System.Threading.Tasks.Task<AircraftHeavyData> heavyTask = _heavyData.GetUpdatedValue();
			System.Threading.Tasks.Task<SimPanel> panelTask = _panel.GetUpdatedValue();
			System.Threading.Tasks.Task<FamilyDataLookup> familyTask = _familyLookup.GetUpdatedValue();
			await System.Threading.Tasks.Task.WhenAll(new System.Threading.Tasks.Task[4] { listTask, heavyTask, panelTask, familyTask }).DetachContext();
			return UpdateWithLock((AircraftSnapshotData _) => new AircraftSnapshotData(listTask.Result, heavyTask.Result, panelTask.Result, familyTask.Result));
		}

		private AircraftSnapshotData UpdateWithLock(Func<AircraftSnapshotData, AircraftSnapshotData> update)
		{
			lock (_cacheLock)
			{
				return CachedValue = update.Invoke(CachedValue);
			}
		}
	}

	private sealed class SameSimKeyEquality : EqualityComparer<ReactiveSimulator>
	{
		public override bool Equals(ReactiveSimulator? x, ReactiveSimulator? y)
		{
			return x?.AircraftAndSim == y?.AircraftAndSim;
		}

		public override int GetHashCode(ReactiveSimulator obj)
		{
			return ((object)obj.AircraftAndSim).GetHashCode();
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003CLoadDefaultSims_003Eb__54_2_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ReactiveSimulator> _003C_003Et__builder;

		public AvionicsModel firstInCategory;

		public ReactiveAircraft _003C_003E4__this;

		private ConfiguredTaskAwaiter<ReactiveSimulator> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			ReactiveSimulator result;
			try
			{
				InstallationSlot installSlot = default(InstallationSlot);
				if (num != 0)
				{
					installSlot = new InstallationSlot(1, firstInCategory.AvionicsType);
				}
				try
				{
					ConfiguredTaskAwaiter<ReactiveSimulator> awaiter;
					if (num != 0)
					{
						awaiter = reactiveAircraft.Install(firstInCategory, installSlot).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ReactiveSimulator>, _003C_003CLoadDefaultSims_003Eb__54_2_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ReactiveSimulator>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter.GetResult();
				}
				catch (System.Exception e)
				{
					Logger.Log(e, $"[Warning] Could not install default simulator {firstInCategory}.");
					throw;
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
	[CompilerGenerated]
	private struct _003C_003CSupportedModels_003Eb__26_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftListData> _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		private ConfiguredTaskAwaiter<AircraftListData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			AircraftListData result;
			try
			{
				ConfiguredTaskAwaiter<AircraftListData> awaiter;
				if (num != 0)
				{
					awaiter = reactiveAircraft.ListData.GetUpdatedValue().DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftListData>, _003C_003CSupportedModels_003Eb__26_0_003Ed>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftListData>);
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
	private struct _003C_003CUpToDateActiveAvionics_003Eb__28_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IObservable<ReactiveSimulator>> _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			IObservable<ReactiveSimulator> result;
			try
			{
				ConfiguredTaskAwaiter<SimPanel> awaiter;
				if (num != 0)
				{
					awaiter = CS_0024_003C_003E8__locals0.Panel.GetUpdatedValue().DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003C_003CUpToDateActiveAvionics_003Eb__28_0_003Ed>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPanel>);
					num = (_003C_003E1__state = -1);
				}
				result = Enumerable.Select<SimInstallData, ReactiveSimulator>(awaiter.GetResult().ActiveSims, (Func<SimInstallData, ReactiveSimulator>)([CompilerGenerated] (SimInstallData s) => new ReactiveSimulator(CS_0024_003C_003E8__locals0.UnitOfWork, s))).ToObservable();
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

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass42_0
	{
		[StructLayout(3)]
		private struct _003C_003CUseMountedSnapshot_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<int> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass42_0 _003C_003E4__this;

			public ReactiveAircraft aircraft;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass42_0 _003C_003Ec__DisplayClass42_ = _003C_003E4__this;
				int result;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass42_.useSandboxedAircraft.Invoke(aircraft).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CUseMountedSnapshot_003Eb__0_003Ed>(ref awaiter, ref this);
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
					result = 0;
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

		public Func<ReactiveAircraft, System.Threading.Tasks.Task> useSandboxedAircraft;

		[AsyncStateMachine(typeof(_003C_003CUseMountedSnapshot_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<int> _003CUseMountedSnapshot_003Eb__0(ReactiveAircraft aircraft)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			await useSandboxedAircraft.Invoke(aircraft).DetachContext();
			return 0;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass51_0
	{
		public AvionicsModel model;

		internal bool _003CInstall_003Eb__1(AvionicsModel m)
		{
			return m == model;
		}

		internal void _003CInstall_003Eb__0(IPropertyChangeSet<SimInstallData> copy)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "s");
			ISubsequentPropertyChangeSetApi<SimInstallData> subsequentPropertyChangeSetApi = copy.Mutate(Expression.Lambda<Func<SimInstallData, TrainerPackageIdentifier>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), model.Package.TrainerPackageId);
			val = Expression.Parameter(typeof(SimInstallData), "s");
			subsequentPropertyChangeSetApi.AndMutate<ImmutableDictionary<SettingIdentifier, string>>(Expression.Lambda<Func<SimInstallData, ImmutableDictionary<SettingIdentifier, string>>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), model.Package.GetSlotSettingDefinitions().ToDefaultPersistenceDictonary());
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCopyWithNewListData_003Ed__44 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ReactiveAircraft> _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public AircraftListData newStartingData;

		private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			ReactiveAircraft result2;
			try
			{
				ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter;
				if (num != 0)
				{
					awaiter = reactiveAircraft.UnitOfWork.Aircraft.CopyAircraft(newStartingData, reactiveAircraft.AircraftIdentifier).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CCopyWithNewListData_003Ed__44>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
					num = (_003C_003E1__state = -1);
				}
				AircraftSnapshotData result = awaiter.GetResult();
				result2 = new ReactiveAircraft(reactiveAircraft.UnitOfWork, result);
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
	private struct _003CDelete_003Ed__45 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

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
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = reactiveAircraft.UnitOfWork.Aircraft.DeleteAircraft(reactiveAircraft.AircraftIdentifier).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CDelete_003Ed__45>(ref awaiter, ref this);
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
	private struct _003CDeleteSnapshot_003Ed__41 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public SnapshotIdentifier snapshotIdentifier;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = reactiveAircraft.UnitOfWork.AircraftHistory.DeleteSnapshot(reactiveAircraft.AircraftIdentifier, snapshotIdentifier).ConfigureAwait(false);
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CDeleteSnapshot_003Ed__41>(ref awaiter, ref this);
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
	private struct _003CGetSimulatorOrNullByKey_003Ed__65 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<SimInstallData> _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public SimInstallKey simInstallKey;

		private ConfiguredTaskAwaiter<SimInstallData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			SimInstallData result;
			try
			{
				try
				{
					ConfiguredTaskAwaiter<SimInstallData> awaiter;
					if (num != 0)
					{
						awaiter = reactiveAircraft.SimRepo.GetSimulator(reactiveAircraft.AircraftIdentifier, simInstallKey).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimInstallData>, _003CGetSimulatorOrNullByKey_003Ed__65>(ref awaiter, ref this);
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
				catch (SimulatorNotFoundException)
				{
					result = null;
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
	[CompilerGenerated]
	private struct _003CInstall_003Ed__48 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ReactiveSimulator> _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public AvionicsType category;

		public TrainerModelKey completeModelKey;

		public int installId;

		private object _003C_003Eu__1;

		private ConfiguredTaskAwaiter<ReactiveSimulator> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			ReactiveSimulator result2;
			try
			{
				ConfiguredTaskAwaiter<ReactiveSimulator> awaiter;
				AsyncSubject<AvionicsModel> asyncSubject;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ReactiveSimulator>);
						num = (_003C_003E1__state = -1);
						goto IL_0125;
					}
					asyncSubject = GetNewestModelOrNull(reactiveAircraft.SupportedModels().FilterByAvionicsType(category), completeModelKey).GetAwaiter<AvionicsModel>();
					if (!asyncSubject.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = asyncSubject;
						_003C_003Et__builder.AwaitOnCompleted<AsyncSubject<AvionicsModel>, _003CInstall_003Ed__48>(ref asyncSubject, ref this);
						return;
					}
				}
				else
				{
					asyncSubject = (AsyncSubject<AvionicsModel>)_003C_003Eu__1;
					_003C_003Eu__1 = null;
					num = (_003C_003E1__state = -1);
				}
				AvionicsModel result = asyncSubject.GetResult();
				if ((object)result == null)
				{
					throw new InvalidOperationException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0} is not a supported {1}.", (object)completeModelKey, (object)category));
				}
				awaiter = reactiveAircraft.Install(result, new InstallationSlot(installId, category)).DetachContext().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ReactiveSimulator>, _003CInstall_003Ed__48>(ref awaiter, ref this);
					return;
				}
				goto IL_0125;
				IL_0125:
				result2 = awaiter.GetResult();
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
	private struct _003CInstall_003Ed__49 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ReactiveSimulator> _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public InstallationSlot slot;

		public TrainerModelKey completeModelKey;

		private ConfiguredTaskAwaiter<ReactiveSimulator> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			ReactiveSimulator result;
			try
			{
				ConfiguredTaskAwaiter<ReactiveSimulator> awaiter;
				if (num != 0)
				{
					awaiter = reactiveAircraft.Install(slot.Category, completeModelKey, slot.InstanceId).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ReactiveSimulator>, _003CInstall_003Ed__49>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ReactiveSimulator>);
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
	private struct _003CInstall_003Ed__50 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ReactiveSimulator> _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public AvionicsModel model;

		public int instanceId;

		private ConfiguredTaskAwaiter<ReactiveSimulator> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			ReactiveSimulator result;
			try
			{
				ConfiguredTaskAwaiter<ReactiveSimulator> awaiter;
				if (num != 0)
				{
					awaiter = reactiveAircraft.Install(model, new InstallationSlot(instanceId, model.AvionicsType)).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ReactiveSimulator>, _003CInstall_003Ed__50>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ReactiveSimulator>);
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
	private struct _003CInstall_003Ed__51 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ReactiveSimulator> _003C_003Et__builder;

		public AvionicsModel model;

		public InstallationSlot installSlot;

		public ReactiveAircraft _003C_003E4__this;

		private _003C_003Ec__DisplayClass51_0 _003C_003E8__1;

		private SimInstallData _003CtoInstall_003E5__2;

		private bool _003CisNewSim_003E5__3;

		private ReactiveSimulator _003CreactiveSim_003E5__4;

		private object _003C_003Eu__1;

		private ConfiguredTaskAwaiter<SimInstallData> _003C_003Eu__2;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private ISettingsBinder<ReactiveSimulator> _003CfamilyBinder_003E5__5;

		private ISettingsBinder<ReactiveSimulator> _003CslotBinder_003E5__6;

		private ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>> _003C_003Eu__4;

		private void MoveNext()
		{
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0520: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			ReactiveSimulator result4;
			try
			{
				AsyncSubject<AvionicsModel> asyncSubject;
				ConfiguredTaskAwaiter<SimInstallData> awaiter3;
				ConfiguredTaskAwaiter awaiter2;
				ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>> awaiter;
				ConfiguredTaskAwaitable val2;
				ISettingsBinder<ReactiveSimulator> result;
				AvionicsModel result2;
				SimInstallKey simKey;
				SimInstallData result3;
				switch (num)
				{
				default:
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass51_0();
					_003C_003E8__1.model = model;
					if ((object)_003C_003E8__1.model == null)
					{
						throw new ArgumentNullException("model");
					}
					if (installSlot.Category != _003C_003E8__1.model.AvionicsType)
					{
						throw new ArgumentException("Slot and model category must match.");
					}
					if (installSlot.InstanceId > _003C_003E8__1.model.MaxInstances)
					{
						throw new InvalidOperationException("Cannot install more than " + _003C_003E8__1.model.MaxInstances + " instances.");
					}
					IObservable<AvionicsModel> val = reactiveAircraft.SupportedModels().Replay<AvionicsModel>().RefCount();
					asyncSubject = val.Where<AvionicsModel>((Func<AvionicsModel, bool>)((AvionicsModel m) => m == _003C_003E8__1.model)).Take<AvionicsModel>(1).Merge<AvionicsModel>(GetNewestModelOrNull(val, _003C_003E8__1.model.Key))
						.WhereClassNotNull<AvionicsModel>()
						.FirstOrDefaultAsync<AvionicsModel>()
						.GetAwaiter<AvionicsModel>();
					if (!asyncSubject.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = asyncSubject;
						_003C_003Et__builder.AwaitOnCompleted<AsyncSubject<AvionicsModel>, _003CInstall_003Ed__51>(ref asyncSubject, ref this);
						return;
					}
					goto IL_017f;
				}
				case 0:
					asyncSubject = (AsyncSubject<AvionicsModel>)_003C_003Eu__1;
					_003C_003Eu__1 = null;
					num = (_003C_003E1__state = -1);
					goto IL_017f;
				case 1:
					awaiter3 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<SimInstallData>);
					num = (_003C_003E1__state = -1);
					goto IL_0293;
				case 2:
					awaiter2 = _003C_003Eu__3;
					_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_037c;
				case 3:
					awaiter = _003C_003Eu__4;
					_003C_003Eu__4 = default(ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>>);
					num = (_003C_003E1__state = -1);
					goto IL_040a;
				case 4:
				case 5:
				case 6:
				case 7:
					{
						try
						{
							if (num != 4)
							{
								if ((uint)(num - 5) <= 2u)
								{
									goto IL_049d;
								}
								awaiter = _003CreactiveSim_003E5__4.BindSettings().DetachContext().GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_003C_003E1__state = 4);
									_003C_003Eu__4 = awaiter;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>>, _003CInstall_003Ed__51>(ref awaiter, ref this);
									return;
								}
							}
							else
							{
								awaiter = _003C_003Eu__4;
								_003C_003Eu__4 = default(ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>>);
								num = (_003C_003E1__state = -1);
							}
							result = awaiter.GetResult();
							_003CslotBinder_003E5__6 = result;
							goto IL_049d;
							IL_049d:
							try
							{
								switch (num)
								{
								default:
									val2 = System.Threading.Tasks.Task.WhenAll(new System.Threading.Tasks.Task[2]
									{
										_003CfamilyBinder_003E5__5.Table.LoadSettingConstraints(),
										_003CslotBinder_003E5__6.Table.LoadSettingConstraints()
									}).DetachContext();
									awaiter2 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
									if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
									{
										num = (_003C_003E1__state = 5);
										_003C_003Eu__3 = awaiter2;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CInstall_003Ed__51>(ref awaiter2, ref this);
										return;
									}
									goto IL_053c;
								case 5:
									awaiter2 = _003C_003Eu__3;
									_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
									num = (_003C_003E1__state = -1);
									goto IL_053c;
								case 6:
									awaiter2 = _003C_003Eu__3;
									_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
									num = (_003C_003E1__state = -1);
									goto IL_05b8;
								case 7:
									{
										awaiter2 = _003C_003Eu__3;
										_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
										num = (_003C_003E1__state = -1);
										break;
									}
									IL_05b8:
									((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
									val2 = _003CreactiveSim_003E5__4.UpdateFamilySettings((IDictionary<SettingIdentifier, string>)(object)_003CfamilyBinder_003E5__5.Table.ToPersistenceDictionaryV2()).DetachContext();
									awaiter2 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
									if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
									{
										num = (_003C_003E1__state = 7);
										_003C_003Eu__3 = awaiter2;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CInstall_003Ed__51>(ref awaiter2, ref this);
										return;
									}
									break;
									IL_053c:
									((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
									val2 = _003CreactiveSim_003E5__4.UpdateSettings((IDictionary<SettingIdentifier, string>)(object)_003CslotBinder_003E5__6.Table.ToPersistenceDictionaryV2()).DetachContext();
									awaiter2 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
									if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
									{
										num = (_003C_003E1__state = 6);
										_003C_003Eu__3 = awaiter2;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CInstall_003Ed__51>(ref awaiter2, ref this);
										return;
									}
									goto IL_05b8;
								}
								((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
							}
							finally
							{
								if (num < 0 && _003CslotBinder_003E5__6 != null)
								{
									((System.IDisposable)_003CslotBinder_003E5__6).Dispose();
								}
							}
						}
						finally
						{
							if (num < 0 && _003CfamilyBinder_003E5__5 != null)
							{
								((System.IDisposable)_003CfamilyBinder_003E5__5).Dispose();
							}
						}
						_003CfamilyBinder_003E5__5 = null;
						_003CslotBinder_003E5__6 = null;
						break;
					}
					IL_040a:
					result = awaiter.GetResult();
					_003CfamilyBinder_003E5__5 = result;
					goto case 4;
					IL_037c:
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					_003CreactiveSim_003E5__4 = new ReactiveSimulator(reactiveAircraft.UnitOfWork, _003CtoInstall_003E5__2);
					if (!_003CisNewSim_003E5__3)
					{
						break;
					}
					awaiter = _003CreactiveSim_003E5__4.BindFamilySettings().DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 3);
						_003C_003Eu__4 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>>, _003CInstall_003Ed__51>(ref awaiter, ref this);
						return;
					}
					goto IL_040a;
					IL_017f:
					result2 = asyncSubject.GetResult();
					if ((object)result2 == null)
					{
						throw new InvalidOperationException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "'{0}' cannot be installed because it is not a supported model.", (object)_003C_003E8__1.model.DisplayName));
					}
					if (result2 != _003C_003E8__1.model)
					{
						throw new InvalidOperationException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "'{0}' cannot be installed because it is out of date.", (object)_003C_003E8__1.model.DisplayName));
					}
					simKey = new SimInstallKey(installSlot, _003C_003E8__1.model.Key.TrainerPackageId.EmbeddedReleaseCompositeId, _003C_003E8__1.model.Key.ModelId);
					awaiter3 = reactiveAircraft.SimRepo.GetSimulatorOrDefault(reactiveAircraft.AircraftIdentifier, simKey).DetachContext().GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter3;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimInstallData>, _003CInstall_003Ed__51>(ref awaiter3, ref this);
						return;
					}
					goto IL_0293;
					IL_0293:
					result3 = awaiter3.GetResult();
					_003CtoInstall_003E5__2 = result3;
					_003CisNewSim_003E5__3 = _003CtoInstall_003E5__2.FullPackageId != _003C_003E8__1.model.Package.TrainerPackageId;
					if (_003CisNewSim_003E5__3)
					{
						_003CtoInstall_003E5__2 = _003CtoInstall_003E5__2.With(delegate(IPropertyChangeSet<SimInstallData> copy)
						{
							//IL_0021: Unknown result type (might be due to invalid IL or missing references)
							//IL_002b: Expected O, but got Unknown
							//IL_006f: Unknown result type (might be due to invalid IL or missing references)
							//IL_0079: Expected O, but got Unknown
							ParameterExpression val3 = Expression.Parameter(typeof(SimInstallData), "s");
							ISubsequentPropertyChangeSetApi<SimInstallData> subsequentPropertyChangeSetApi = copy.Mutate(Expression.Lambda<Func<SimInstallData, TrainerPackageIdentifier>>((Expression)(object)Expression.Property((Expression)(object)val3, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val3 }), _003C_003E8__1.model.Package.TrainerPackageId);
							val3 = Expression.Parameter(typeof(SimInstallData), "s");
							subsequentPropertyChangeSetApi.AndMutate<ImmutableDictionary<SettingIdentifier, string>>(Expression.Lambda<Func<SimInstallData, ImmutableDictionary<SettingIdentifier, string>>>((Expression)(object)Expression.Property((Expression)(object)val3, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val3 }), _003C_003E8__1.model.Package.GetSlotSettingDefinitions().ToDefaultPersistenceDictonary());
						});
					}
					_003CtoInstall_003E5__2 = _003CtoInstall_003E5__2.ToActive(isActive: true);
					val2 = reactiveAircraft.SimRepo.CreateOrUpdateSimulator(reactiveAircraft.AircraftIdentifier, _003CtoInstall_003E5__2).DetachContext();
					awaiter2 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__3 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CInstall_003Ed__51>(ref awaiter2, ref this);
						return;
					}
					goto IL_037c;
				}
				result4 = _003CreactiveSim_003E5__4;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CtoInstall_003E5__2 = null;
				_003CreactiveSim_003E5__4 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CtoInstall_003E5__2 = null;
			_003CreactiveSim_003E5__4 = null;
			_003C_003Et__builder.SetResult(result4);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CRelist_003Ed__56 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftListData updatedListData;

		public ReactiveAircraft _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_0099;
				}
				if (updatedListData != reactiveAircraft.ListData.CachedValue)
				{
					reactiveAircraft.ThrowIfDifferentId(updatedListData);
					ConfiguredTaskAwaitable val = reactiveAircraft.UnitOfWork.Aircraft.UpdateAircraftListData(updatedListData).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CRelist_003Ed__56>(ref awaiter, ref this);
						return;
					}
					goto IL_0099;
				}
				goto end_IL_000e;
				IL_0099:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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
	[CompilerGenerated]
	private struct _003CRevertToSnapshot_003Ed__34 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public SnapshotIdentifier snapshotIdentifier;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = reactiveAircraft.UnitOfWork.AircraftHistory.RevertAircraftToSnapshot(reactiveAircraft.AircraftIdentifier, snapshotIdentifier).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CRevertToSnapshot_003Ed__34>(ref awaiter, ref this);
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
	private struct _003CSelectAircraft_003Ed__47 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = reactiveAircraft.AircraftRepo.StoreLastSelectedAircraft(reactiveAircraft.AircraftIdentifier).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CSelectAircraft_003Ed__47>(ref awaiter, ref this);
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
	private struct _003CTakeSnapshot_003Ed__33 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftSnapshotInfo> _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public AircraftSnapshotMetaData aircraftSnapshotMetaData;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private ConfiguredTaskAwaiter<AircraftSnapshotInfo> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			AircraftSnapshotInfo result;
			try
			{
				ConfiguredTaskAwaiter<AircraftSnapshotInfo> awaiter;
				ConfiguredTaskAwaiter awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<AircraftSnapshotInfo>);
						num = (_003C_003E1__state = -1);
						goto IL_00f4;
					}
					ConfiguredTaskAwaitable val = reactiveAircraft.UnitOfWork.FlushEverything().DetachContext();
					awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CTakeSnapshot_003Ed__33>(ref awaiter2, ref this);
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
				awaiter = reactiveAircraft.UnitOfWork.AircraftHistory.CreateSnapshot(reactiveAircraft.AircraftIdentifier, aircraftSnapshotMetaData).DetachContext().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotInfo>, _003CTakeSnapshot_003Ed__33>(ref awaiter, ref this);
					return;
				}
				goto IL_00f4;
				IL_00f4:
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
	private struct _003CUninstall_003Ed__52 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public InstallationSlot installSlot;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<SimPanel> awaiter2;
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaitable val;
				SimInstallData activeSimInSlot;
				switch (num)
				{
				default:
					awaiter2 = reactiveAircraft.Panel.GetUpdatedValue().DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CUninstall_003Ed__52>(ref awaiter2, ref this);
						return;
					}
					goto IL_0082;
				case 0:
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPanel>);
					num = (_003C_003E1__state = -1);
					goto IL_0082;
				case 1:
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPanel>);
					num = (_003C_003E1__state = -1);
					goto IL_0103;
				case 2:
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						break;
					}
					IL_011e:
					val = reactiveAircraft.UninstallSimByKeyWithLock(activeSimInSlot.InstallationKey).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUninstall_003Ed__52>(ref awaiter, ref this);
						return;
					}
					break;
					IL_0082:
					activeSimInSlot = awaiter2.GetResult().GetActiveSimInSlot(installSlot);
					if (activeSimInSlot == null)
					{
						awaiter2 = reactiveAircraft.SimRepo.GetSimulatorPanel(reactiveAircraft.AircraftIdentifier).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CUninstall_003Ed__52>(ref awaiter2, ref this);
							return;
						}
						goto IL_0103;
					}
					goto IL_011e;
					IL_0103:
					activeSimInSlot = awaiter2.GetResult().GetActiveSimInSlot(installSlot);
					if (activeSimInSlot != null)
					{
						goto IL_011e;
					}
					goto end_IL_000e;
				}
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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
	[CompilerGenerated]
	private struct _003CUninstall_003Ed__53 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public SimInstallKey simInstallKey;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = reactiveAircraft.UninstallSimByKeyWithLock(simInstallKey).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUninstall_003Ed__53>(ref awaiter, ref this);
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
	private struct _003CUninstallSimByKeyWithLock_003Ed__64 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public SimInstallKey simInstallKey;

		private ConfiguredTaskAwaiter<SimInstallData> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
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
						goto IL_00ff;
					}
					awaiter2 = reactiveAircraft.GetSimulatorOrNullByKey(simInstallKey).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimInstallData>, _003CUninstallSimByKeyWithLock_003Ed__64>(ref awaiter2, ref this);
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
				if (result != null)
				{
					SimInstallData simulator = result.ToActive(isActive: false);
					ConfiguredTaskAwaitable val = reactiveAircraft.SimRepo.CreateOrUpdateSimulator(reactiveAircraft.AircraftIdentifier, simulator).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUninstallSimByKeyWithLock_003Ed__64>(ref awaiter, ref this);
						return;
					}
					goto IL_00ff;
				}
				goto end_IL_000e;
				IL_00ff:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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
	[CompilerGenerated]
	private struct _003CUpdateEntireAircraft_003Ed__58 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public AircraftSnapshotData updatedSnapshot;

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
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = reactiveAircraft.UnitOfWork.Aircraft.UpdateAircraft(updatedSnapshot).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateEntireAircraft_003Ed__58>(ref awaiter, ref this);
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
	private struct _003CUpdateHeavy_003Ed__57 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftHeavyData updatedHeavy;

		public ReactiveAircraft _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_0099;
				}
				if (updatedHeavy != reactiveAircraft.HeavyData.CachedValue)
				{
					reactiveAircraft.ThrowIfDifferentId(updatedHeavy);
					ConfiguredTaskAwaitable val = reactiveAircraft.UnitOfWork.Aircraft.UpdateAircraftHeavyData(updatedHeavy).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateHeavy_003Ed__57>(ref awaiter, ref this);
						return;
					}
					goto IL_0099;
				}
				goto end_IL_000e;
				IL_0099:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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
	[CompilerGenerated]
	private struct _003CUpdatePanel_003Ed__46 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public SimPanel updatedPanel;

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
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = reactiveAircraft.SimRepo.UpdateSimulatorPanel(reactiveAircraft.AircraftIdentifier, updatedPanel).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdatePanel_003Ed__46>(ref awaiter, ref this);
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
	private struct _003CUseMountedSnapshot_003Ed__42 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public Func<ReactiveAircraft, System.Threading.Tasks.Task> useSandboxedAircraft;

		public ReactiveAircraft _003C_003E4__this;

		public AircraftSnapshotInfo protoTypeSnapshot;

		private ConfiguredTaskAwaiter<int> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<int> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass42_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass42_0
					{
						useSandboxedAircraft = useSandboxedAircraft
					};
					awaiter = reactiveAircraft.UseMountedSnapshot<int>(protoTypeSnapshot, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass42_0._003C_003CUseMountedSnapshot_003Eb__0_003Ed))] async (ReactiveAircraft aircraft) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						await CS_0024_003C_003E8__locals0.useSandboxedAircraft.Invoke(aircraft).DetachContext();
						return 0;
					}).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<int>, _003CUseMountedSnapshot_003Ed__42>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<int>);
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
	private struct _003CUseMountedSnapshot_003Ed__43<TResult> : IAsyncStateMachine where TResult : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TResult> _003C_003Et__builder;

		public ReactiveAircraft _003C_003E4__this;

		public AircraftSnapshotInfo protoTypeSnapshot;

		public Func<ReactiveAircraft, System.Threading.Tasks.Task<TResult>> useSandboxedAircraft;

		private TemporaryHangarDirectory _003CmountedHangar_003E5__2;

		private ConfiguredTaskAwaiter<TemporaryHangarDirectory> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<TResult> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ReactiveAircraft reactiveAircraft = _003C_003E4__this;
			TResult result2;
			try
			{
				ConfiguredTaskAwaiter<TemporaryHangarDirectory> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_00a5;
					}
					awaiter = reactiveAircraft.UnitOfWork.AircraftHistory.MountHistoricalSnapshot(reactiveAircraft.AircraftIdentifier, protoTypeSnapshot.SnapshotIdentifier).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TemporaryHangarDirectory>, _003CUseMountedSnapshot_003Ed__43<TResult>>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TemporaryHangarDirectory>);
					num = (_003C_003E1__state = -1);
				}
				TemporaryHangarDirectory result = awaiter.GetResult();
				_003CmountedHangar_003E5__2 = result;
				goto IL_00a5;
				IL_00a5:
				try
				{
					ConfiguredTaskAwaiter<TResult> awaiter2;
					if (num != 1)
					{
						UnitOfWork unitOfWork = new UnitOfWork
						{
							Aircraft = _003CmountedHangar_003E5__2,
							Simulators = _003CmountedHangar_003E5__2,
							AircraftHistory = _003CmountedHangar_003E5__2,
							FamilyData = _003CmountedHangar_003E5__2,
							LowLevel = reactiveAircraft.UnitOfWork.LowLevel,
							Avionics = reactiveAircraft.UnitOfWork.Avionics,
							Packages = reactiveAircraft.UnitOfWork.Packages,
							Settings = reactiveAircraft.UnitOfWork.Settings
						};
						awaiter2 = ((Func<ReactiveAircraft, System.Threading.Tasks.Task<ReactiveAircraft>>)(object)useSandboxedAircraft).Invoke(new ReactiveAircraft(unitOfWork, protoTypeSnapshot.AircraftIdentifierInSnapShot)).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TResult>, _003CUseMountedSnapshot_003Ed__43<TResult>>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<TResult>);
						num = (_003C_003E1__state = -1);
					}
					result2 = awaiter2.GetResult();
				}
				finally
				{
					if (num < 0 && _003CmountedHangar_003E5__2 != null)
					{
						((System.IDisposable)_003CmountedHangar_003E5__2).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CmountedHangar_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CmountedHangar_003E5__2 = null;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[field: CompilerGenerated]
	public AircraftIdentifier AircraftIdentifier
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IReactiveEntity<AircraftListData> ListData
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IReactiveEntity<AircraftHeavyData> HeavyData
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IReactiveEntity<SimPanel> Panel
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IReactiveEntity<FamilyDataLookup> FamilyLookup
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IReactiveEntity<AircraftSnapshotData> CompositeSnapshots
	{
		[CompilerGenerated]
		get;
	}

	private IAircraftRepoEvolved AircraftRepo => base.UnitOfWork.Aircraft;

	private ISimulatorRepository SimRepo => base.UnitOfWork.Simulators;

	public ReactiveAircraft(IUnitOfWork unitOfWork, AircraftSnapshotData startingSnapshot)
		: base(unitOfWork)
	{
		AircraftIdentifier = startingSnapshot.AircraftIdentifier.ThrowIfDefault();
		ListData = new ListDataContainer(base.UnitOfWork, startingSnapshot.ListData);
		HeavyData = new HeavyDataContainer(unitOfWork, startingSnapshot.HeavyData);
		Panel = new PanelContainer(unitOfWork, AircraftIdentifier, startingSnapshot.Panel);
		FamilyLookup = new FamilyContainer(unitOfWork, AircraftIdentifier, startingSnapshot.FamilyLookup);
		CompositeSnapshots = new CompositeSnapshotContainer(ListData, HeavyData, Panel, FamilyLookup);
	}

	public ReactiveAircraft(IUnitOfWork unitOfWork, AircraftIdentifier aircraftIdentifier)
		: this(unitOfWork, AircraftSnapshotData.CreateAircraftStartingData(aircraftIdentifier))
	{
	}

	public ReactiveAircraft(IUnitOfWork unitOfWork, AircraftListData startingListData)
		: this(unitOfWork, AircraftSnapshotData.FromListData(startingListData))
	{
	}

	public IObservable<DataChangeType> WhenAircraftIsUpdatedInRepo()
	{
		return AircraftRepo.AircraftChanges.ForAircraft(AircraftIdentifier).Select<DataChangeNotification<AircraftIdentifier>, DataChangeType>((Func<DataChangeNotification<AircraftIdentifier>, DataChangeType>)((DataChangeNotification<AircraftIdentifier> change) => change.Change));
	}

	public IObservable<AvionicsModel> SupportedModels()
	{
		return Observable.FromAsync<AircraftListData>((Func<System.Threading.Tasks.Task<AircraftListData>>)([AsyncStateMachine(typeof(_003C_003CSupportedModels_003Eb__26_0_003Ed))] [CompilerGenerated] async () => await ListData.GetUpdatedValue().DetachContext())).SelectMany<AircraftListData, AvionicsModel>((Func<AircraftListData, IObservable<AvionicsModel>>)([CompilerGenerated] (AircraftListData listData) => base.UnitOfWork.Avionics.AllModels().Where<AvionicsModel>((Func<AvionicsModel, bool>)((AvionicsModel model) => model.ModelInfo.SupportedAirframes.Contains(listData.AirframeCCT)))));
	}

	public IObservable<AvionicsType> SupportedAvionicsCategories()
	{
		return SupportedModels().Select<AvionicsModel, AvionicsType>((Func<AvionicsModel, AvionicsType>)((AvionicsModel sa) => sa.AvionicsType)).Distinct<AvionicsType>();
	}

	public IObservable<ReactiveSimulator> UpToDateActiveAvionics()
	{
		return Observable.DeferAsync<ReactiveSimulator>([AsyncStateMachine(typeof(_003C_003CUpToDateActiveAvionics_003Eb__28_0_003Ed))] [CompilerGenerated] async (CancellationToken t) => Enumerable.Select<SimInstallData, ReactiveSimulator>((await Panel.GetUpdatedValue().DetachContext()).ActiveSims, (Func<SimInstallData, ReactiveSimulator>)([CompilerGenerated] (SimInstallData s) => new ReactiveSimulator(base.UnitOfWork, s))).ToObservable());
	}

	public IObservable<TimingEvent> WhenAircraftIsDeleted()
	{
		return base.UnitOfWork.Aircraft.AircraftChanges.FilterByChangeType<AircraftIdentifier>(new DataChangeType[1] { DataChangeType.Deleted }).StripChangeType<AircraftIdentifier>().Where<AircraftIdentifier>((Func<AircraftIdentifier, bool>)([CompilerGenerated] (AircraftIdentifier ai) => ai == AircraftIdentifier))
			.ToTimingEventStream<AircraftIdentifier>();
	}

	public ReactiveSimulator CreateSimWrapper(SimInstallKey installationKey)
	{
		return new ReactiveSimulator(base.UnitOfWork, new SimAircraftKey(AircraftIdentifier, installationKey));
	}

	public IObservable<DataChangeNotification<ReactiveSimulator>> SimulatorCreationAndUpdates()
	{
		return (from change in SimRepo.WhenSimulatorIsChangedOnAircraft(AircraftIdentifier).FilterByChangeType<SimInstallKey>(new DataChangeType[2]
			{
				DataChangeType.Created,
				DataChangeType.Updated
			})
			group change by change.Value).Select<IGroupedObservable<SimInstallKey, DataChangeNotification<SimInstallKey>>, IObservable<DataChangeNotification<ReactiveSimulator>>>((Func<IGroupedObservable<SimInstallKey, DataChangeNotification<SimInstallKey>>, IObservable<DataChangeNotification<ReactiveSimulator>>>)([CompilerGenerated] (IGroupedObservable<SimInstallKey, DataChangeNotification<SimInstallKey>> simIdChanges) => ((IObservable<DataChangeNotification<SimInstallKey>>)simIdChanges).Select<DataChangeNotification<SimInstallKey>, DataChangeNotification<ReactiveSimulator>>((Func<DataChangeNotification<SimInstallKey>, DataChangeNotification<ReactiveSimulator>>)([CompilerGenerated] (DataChangeNotification<SimInstallKey> change) =>
		{
			DataChangeType change2 = change.Change;
			return DataChangeNotification.Create(new ReactiveSimulator(base.UnitOfWork, new SimAircraftKey(AircraftIdentifier, change.Value)), change2);
		})))).Merge<DataChangeNotification<ReactiveSimulator>>();
	}

	public IObservable<ISet<ReactiveSimulator>> ActiveSimulatorsOverTime()
	{
		return (IObservable<ISet<ReactiveSimulator>>)(object)Panel.GetCachedThenUpdatedAndThenContinualValues().Select<SimPanel, HashSet<ReactiveSimulator>>((Func<SimPanel, HashSet<ReactiveSimulator>>)([CompilerGenerated] (SimPanel panel) => Enumerable.ToHashSet<ReactiveSimulator>(Enumerable.Select<SimInstallData, ReactiveSimulator>(panel.ActiveSims, (Func<SimInstallData, ReactiveSimulator>)([CompilerGenerated] (SimInstallData s) => new ReactiveSimulator(base.UnitOfWork, s)))))).DistinctUntilChanged<HashSet<ReactiveSimulator>>((IEqualityComparer<HashSet<ReactiveSimulator>>)(object)new SetEquality<ReactiveSimulator>((IEqualityComparer<ReactiveSimulator>?)(object)new SameSimKeyEquality()));
	}

	[AsyncStateMachine(typeof(_003CTakeSnapshot_003Ed__33))]
	public async System.Threading.Tasks.Task<AircraftSnapshotInfo> TakeSnapshot(AircraftSnapshotMetaData aircraftSnapshotMetaData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		await base.UnitOfWork.FlushEverything().DetachContext();
		return await base.UnitOfWork.AircraftHistory.CreateSnapshot(AircraftIdentifier, aircraftSnapshotMetaData).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CRevertToSnapshot_003Ed__34))]
	public System.Threading.Tasks.Task RevertToSnapshot(SnapshotIdentifier snapshotIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CRevertToSnapshot_003Ed__34 _003CRevertToSnapshot_003Ed__ = default(_003CRevertToSnapshot_003Ed__34);
		_003CRevertToSnapshot_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CRevertToSnapshot_003Ed__._003C_003E4__this = this;
		_003CRevertToSnapshot_003Ed__.snapshotIdentifier = snapshotIdentifier;
		_003CRevertToSnapshot_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CRevertToSnapshot_003Ed__._003C_003Et__builder)).Start<_003CRevertToSnapshot_003Ed__34>(ref _003CRevertToSnapshot_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CRevertToSnapshot_003Ed__._003C_003Et__builder)).Task;
	}

	public IObservable<System.Collections.Generic.IList<ITrainerPackage>> ActivePackagesInAircraftOverTime()
	{
		return (IObservable<System.Collections.Generic.IList<ITrainerPackage>>)(object)Panel.GetCachedThenUpdatedAndThenContinualValues().Select<SimPanel, IObservable<List<ITrainerPackage>>>((Func<SimPanel, IObservable<List<ITrainerPackage>>>)([CompilerGenerated] (SimPanel panel) => Enumerable.Select<TrainerPackageIdentifier, IObservable<ITrainerPackage>>(panel.AllActivePackageReferences, (Func<TrainerPackageIdentifier, IObservable<ITrainerPackage>>)([CompilerGenerated] (TrainerPackageIdentifier packageId) => base.UnitOfWork.Packages.AfterLoading<ITrainerPackage>((Func<ITrainerPackageRepoEvolved, IObservable<ITrainerPackage>>)((ITrainerPackageRepoEvolved r) => r.GetExactPackageOverTime(packageId))))).CombineLatestOrEmpty().Select<System.Collections.Generic.IList<ITrainerPackage>, List<ITrainerPackage>>((Func<System.Collections.Generic.IList<ITrainerPackage>, List<ITrainerPackage>>)((System.Collections.Generic.IList<ITrainerPackage> allPacks) => Enumerable.ToList<ITrainerPackage>(((System.Collections.Generic.IEnumerable<ITrainerPackage?>)allPacks).WhereClassNotNull()))))).Switch<List<ITrainerPackage>>();
	}

	public IObservable<HashSet<ITrainerPackage>> UniqueActivePackagesInAircraftOverTime(IEqualityComparer<ITrainerPackage>? comparer = null)
	{
		return ActivePackagesInAircraftOverTime().Select<System.Collections.Generic.IList<ITrainerPackage>, HashSet<ITrainerPackage>>((Func<System.Collections.Generic.IList<ITrainerPackage>, HashSet<ITrainerPackage>>)((System.Collections.Generic.IList<ITrainerPackage> packages) => Enumerable.ToHashSet<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)packages))).DistinctUntilChanged<HashSet<ITrainerPackage>>((IEqualityComparer<HashSet<ITrainerPackage>>)(object)new SetEquality<ITrainerPackage>((IEqualityComparer<ITrainerPackage>?)(((object)comparer) ?? ((object)PackageDeepEquality.Instance))));
	}

	public IObservable<ISet<TrainerPackageIdentifier>> AllPackageReferencesInAircraftOverTime()
	{
		return ((IObservable<ISet<TrainerPackageIdentifier>>)(object)Panel.GetUpdateToDateAndThenContinualValues().Select<SimPanel, HashSet<TrainerPackageIdentifier>>((Func<SimPanel, HashSet<TrainerPackageIdentifier>>)((SimPanel panel) => Enumerable.ToHashSet<TrainerPackageIdentifier>(panel.AllPackageReferences)))).DistinctUntilChanged<ISet<TrainerPackageIdentifier>>((IEqualityComparer<ISet<TrainerPackageIdentifier>>)(object)new SetEquality<TrainerPackageIdentifier>());
	}

	public IObservable<DataChangeNotification<ITrainerPackage>> WhenReferencedPackageChanges()
	{
		return Observable.Select(Observable.Where(Observable.CombineLatest(base.UnitOfWork.Packages.WhenPackageIsChanged, AllPackageReferencesInAircraftOverTime(), (DataChangeNotification<ITrainerPackage> change, ISet<TrainerPackageIdentifier> allpacks) => new { change, allpacks }), combined => Enumerable.Any<TrainerPackageIdentifier>((System.Collections.Generic.IEnumerable<TrainerPackageIdentifier>)combined.allpacks, (Func<TrainerPackageIdentifier, bool>)((TrainerPackageIdentifier p) => p.IsSameEmbeddedRelease(combined.change.Value.TrainerPackageId)))), combined => combined.change);
	}

	public IObservable<IReadOnlyDictionary<TrainerPackageIdentifier, ITrainerPackage?>> ReferencedPackagesOverTime()
	{
		return (IObservable<IReadOnlyDictionary<TrainerPackageIdentifier, ITrainerPackage?>>)(object)base.UnitOfWork.Packages.AfterLoading<ISet<TrainerPackageIdentifier>>((Func<ITrainerPackageRepoEvolved, IObservable<ISet<TrainerPackageIdentifier>>>)([CompilerGenerated] (ITrainerPackageRepoEvolved _) => AllPackageReferencesInAircraftOverTime())).Select<ISet<TrainerPackageIdentifier>, IObservable<System.Collections.Generic.IList<KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>>>>((Func<ISet<TrainerPackageIdentifier>, IObservable<System.Collections.Generic.IList<KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>>>>)([CompilerGenerated] (ISet<TrainerPackageIdentifier> allRefs) => Enumerable.Select<TrainerPackageIdentifier, IObservable<KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>>>(Enumerable.Distinct<TrainerPackageIdentifier>((System.Collections.Generic.IEnumerable<TrainerPackageIdentifier>)allRefs), (Func<TrainerPackageIdentifier, IObservable<KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>>>)([CompilerGenerated] (TrainerPackageIdentifier pref) => base.UnitOfWork.PackageRepos.Packages.GetPackageLineage(pref.EmbeddedReleaseCompositeId).Select<ITrainerPackage, KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>>((Func<ITrainerPackage, KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>>)((ITrainerPackage package) => new KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>(pref, package))))).CombineLatestOrEmpty())).Switch<System.Collections.Generic.IList<KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>>>()
			.Select<System.Collections.Generic.IList<KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>>, Dictionary<TrainerPackageIdentifier, ITrainerPackage>>((Func<System.Collections.Generic.IList<KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>>, Dictionary<TrainerPackageIdentifier, ITrainerPackage>>)((System.Collections.Generic.IList<KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>> latestPackList) => ((System.Collections.Generic.IEnumerable<KeyValuePair<TrainerPackageIdentifier, ITrainerPackage>>)latestPackList).ToDictionary()));
	}

	public IObservable<AircraftSnapshotInfo> ListSnapshots()
	{
		return base.UnitOfWork.AircraftHistory.ListHistoricalSnapshots(AircraftIdentifier);
	}

	[AsyncStateMachine(typeof(_003CDeleteSnapshot_003Ed__41))]
	public System.Threading.Tasks.Task DeleteSnapshot(SnapshotIdentifier snapshotIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CDeleteSnapshot_003Ed__41 _003CDeleteSnapshot_003Ed__ = default(_003CDeleteSnapshot_003Ed__41);
		_003CDeleteSnapshot_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CDeleteSnapshot_003Ed__._003C_003E4__this = this;
		_003CDeleteSnapshot_003Ed__.snapshotIdentifier = snapshotIdentifier;
		_003CDeleteSnapshot_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CDeleteSnapshot_003Ed__._003C_003Et__builder)).Start<_003CDeleteSnapshot_003Ed__41>(ref _003CDeleteSnapshot_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CDeleteSnapshot_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUseMountedSnapshot_003Ed__42))]
	public System.Threading.Tasks.Task UseMountedSnapshot(AircraftSnapshotInfo protoTypeSnapshot, Func<ReactiveAircraft, System.Threading.Tasks.Task> useSandboxedAircraft)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUseMountedSnapshot_003Ed__42 _003CUseMountedSnapshot_003Ed__ = default(_003CUseMountedSnapshot_003Ed__42);
		_003CUseMountedSnapshot_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUseMountedSnapshot_003Ed__._003C_003E4__this = this;
		_003CUseMountedSnapshot_003Ed__.protoTypeSnapshot = protoTypeSnapshot;
		_003CUseMountedSnapshot_003Ed__.useSandboxedAircraft = useSandboxedAircraft;
		_003CUseMountedSnapshot_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUseMountedSnapshot_003Ed__._003C_003Et__builder)).Start<_003CUseMountedSnapshot_003Ed__42>(ref _003CUseMountedSnapshot_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUseMountedSnapshot_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUseMountedSnapshot_003Ed__43<>))]
	public async System.Threading.Tasks.Task<TResult> UseMountedSnapshot<TResult>(AircraftSnapshotInfo protoTypeSnapshot, Func<ReactiveAircraft, System.Threading.Tasks.Task<TResult>> useSandboxedAircraft)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		using TemporaryHangarDirectory mountedHangar = await base.UnitOfWork.AircraftHistory.MountHistoricalSnapshot(AircraftIdentifier, protoTypeSnapshot.SnapshotIdentifier).DetachContext();
		UnitOfWork unitOfWork = new UnitOfWork
		{
			Aircraft = mountedHangar,
			Simulators = mountedHangar,
			AircraftHistory = mountedHangar,
			FamilyData = mountedHangar,
			LowLevel = base.UnitOfWork.LowLevel,
			Avionics = base.UnitOfWork.Avionics,
			Packages = base.UnitOfWork.Packages,
			Settings = base.UnitOfWork.Settings
		};
		return await useSandboxedAircraft.Invoke(new ReactiveAircraft(unitOfWork, protoTypeSnapshot.AircraftIdentifierInSnapShot)).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CCopyWithNewListData_003Ed__44))]
	public async System.Threading.Tasks.Task<ReactiveAircraft> CopyWithNewListData(AircraftListData newStartingData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AircraftSnapshotData startingSnapshot = await base.UnitOfWork.Aircraft.CopyAircraft(newStartingData, AircraftIdentifier).DetachContext();
		return new ReactiveAircraft(base.UnitOfWork, startingSnapshot);
	}

	[AsyncStateMachine(typeof(_003CDelete_003Ed__45))]
	public System.Threading.Tasks.Task Delete()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CDelete_003Ed__45 _003CDelete_003Ed__ = default(_003CDelete_003Ed__45);
		_003CDelete_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CDelete_003Ed__._003C_003E4__this = this;
		_003CDelete_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CDelete_003Ed__._003C_003Et__builder)).Start<_003CDelete_003Ed__45>(ref _003CDelete_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CDelete_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUpdatePanel_003Ed__46))]
	public System.Threading.Tasks.Task UpdatePanel(SimPanel updatedPanel)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdatePanel_003Ed__46 _003CUpdatePanel_003Ed__ = default(_003CUpdatePanel_003Ed__46);
		_003CUpdatePanel_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdatePanel_003Ed__._003C_003E4__this = this;
		_003CUpdatePanel_003Ed__.updatedPanel = updatedPanel;
		_003CUpdatePanel_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdatePanel_003Ed__._003C_003Et__builder)).Start<_003CUpdatePanel_003Ed__46>(ref _003CUpdatePanel_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdatePanel_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CSelectAircraft_003Ed__47))]
	public System.Threading.Tasks.Task SelectAircraft()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CSelectAircraft_003Ed__47 _003CSelectAircraft_003Ed__ = default(_003CSelectAircraft_003Ed__47);
		_003CSelectAircraft_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CSelectAircraft_003Ed__._003C_003E4__this = this;
		_003CSelectAircraft_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CSelectAircraft_003Ed__._003C_003Et__builder)).Start<_003CSelectAircraft_003Ed__47>(ref _003CSelectAircraft_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CSelectAircraft_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CInstall_003Ed__48))]
	public async System.Threading.Tasks.Task<ReactiveSimulator> Install(AvionicsType category, TrainerModelKey completeModelKey, int installId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AvionicsModel avionicsModel = await GetNewestModelOrNull(SupportedModels().FilterByAvionicsType(category), completeModelKey);
		if ((object)avionicsModel == null)
		{
			throw new InvalidOperationException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0} is not a supported {1}.", (object)completeModelKey, (object)category));
		}
		return await Install(avionicsModel, new InstallationSlot(installId, category)).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CInstall_003Ed__49))]
	public async System.Threading.Tasks.Task<ReactiveSimulator> Install(TrainerModelKey completeModelKey, InstallationSlot slot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await Install(slot.Category, completeModelKey, slot.InstanceId).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CInstall_003Ed__50))]
	public async System.Threading.Tasks.Task<ReactiveSimulator> Install(AvionicsModel model, int instanceId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await Install(model, new InstallationSlot(instanceId, model.AvionicsType)).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CInstall_003Ed__51))]
	public async System.Threading.Tasks.Task<ReactiveSimulator> Install(AvionicsModel model, InstallationSlot installSlot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AvionicsModel model2 = model;
		if ((object)model2 == null)
		{
			throw new ArgumentNullException("model");
		}
		if (installSlot.Category != model2.AvionicsType)
		{
			throw new ArgumentException("Slot and model category must match.");
		}
		if (installSlot.InstanceId > model2.MaxInstances)
		{
			throw new InvalidOperationException("Cannot install more than " + model2.MaxInstances + " instances.");
		}
		IObservable<AvionicsModel> val = SupportedModels().Replay<AvionicsModel>().RefCount();
		AvionicsModel avionicsModel = await val.Where<AvionicsModel>((Func<AvionicsModel, bool>)((AvionicsModel m) => m == model2)).Take<AvionicsModel>(1).Merge<AvionicsModel>(GetNewestModelOrNull(val, model2.Key))
			.WhereClassNotNull<AvionicsModel>()
			.FirstOrDefaultAsync<AvionicsModel>();
		if ((object)avionicsModel == null)
		{
			throw new InvalidOperationException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "'{0}' cannot be installed because it is not a supported model.", (object)model2.DisplayName));
		}
		if (avionicsModel != model2)
		{
			throw new InvalidOperationException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "'{0}' cannot be installed because it is out of date.", (object)model2.DisplayName));
		}
		SimInstallKey simKey = new SimInstallKey(installSlot, model2.Key.TrainerPackageId.EmbeddedReleaseCompositeId, model2.Key.ModelId);
		SimInstallData toInstall = await SimRepo.GetSimulatorOrDefault(AircraftIdentifier, simKey).DetachContext();
		bool isNewSim = toInstall.FullPackageId != model2.Package.TrainerPackageId;
		if (isNewSim)
		{
			toInstall = toInstall.With(delegate(IPropertyChangeSet<SimInstallData> copy)
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Expected O, but got Unknown
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Expected O, but got Unknown
				ParameterExpression val2 = Expression.Parameter(typeof(SimInstallData), "s");
				ISubsequentPropertyChangeSetApi<SimInstallData> subsequentPropertyChangeSetApi = copy.Mutate(Expression.Lambda<Func<SimInstallData, TrainerPackageIdentifier>>((Expression)(object)Expression.Property((Expression)(object)val2, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val2 }), model2.Package.TrainerPackageId);
				val2 = Expression.Parameter(typeof(SimInstallData), "s");
				subsequentPropertyChangeSetApi.AndMutate<ImmutableDictionary<SettingIdentifier, string>>(Expression.Lambda<Func<SimInstallData, ImmutableDictionary<SettingIdentifier, string>>>((Expression)(object)Expression.Property((Expression)(object)val2, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val2 }), model2.Package.GetSlotSettingDefinitions().ToDefaultPersistenceDictonary());
			});
		}
		toInstall = toInstall.ToActive(isActive: true);
		await SimRepo.CreateOrUpdateSimulator(AircraftIdentifier, toInstall).DetachContext();
		ReactiveSimulator reactiveSim = new ReactiveSimulator(base.UnitOfWork, toInstall);
		if (isNewSim)
		{
			using ISettingsBinder<ReactiveSimulator> familyBinder = await reactiveSim.BindFamilySettings().DetachContext();
			using ISettingsBinder<ReactiveSimulator> slotBinder = await reactiveSim.BindSettings().DetachContext();
			await System.Threading.Tasks.Task.WhenAll(new System.Threading.Tasks.Task[2]
			{
				familyBinder.Table.LoadSettingConstraints(),
				slotBinder.Table.LoadSettingConstraints()
			}).DetachContext();
			await reactiveSim.UpdateSettings((IDictionary<SettingIdentifier, string>)(object)slotBinder.Table.ToPersistenceDictionaryV2()).DetachContext();
			await reactiveSim.UpdateFamilySettings((IDictionary<SettingIdentifier, string>)(object)familyBinder.Table.ToPersistenceDictionaryV2()).DetachContext();
		}
		return reactiveSim;
	}

	[AsyncStateMachine(typeof(_003CUninstall_003Ed__52))]
	public System.Threading.Tasks.Task Uninstall(InstallationSlot installSlot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUninstall_003Ed__52 _003CUninstall_003Ed__ = default(_003CUninstall_003Ed__52);
		_003CUninstall_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUninstall_003Ed__._003C_003E4__this = this;
		_003CUninstall_003Ed__.installSlot = installSlot;
		_003CUninstall_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUninstall_003Ed__._003C_003Et__builder)).Start<_003CUninstall_003Ed__52>(ref _003CUninstall_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUninstall_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUninstall_003Ed__53))]
	public System.Threading.Tasks.Task Uninstall(SimInstallKey simInstallKey)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUninstall_003Ed__53 _003CUninstall_003Ed__ = default(_003CUninstall_003Ed__53);
		_003CUninstall_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUninstall_003Ed__._003C_003E4__this = this;
		_003CUninstall_003Ed__.simInstallKey = simInstallKey;
		_003CUninstall_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUninstall_003Ed__._003C_003Et__builder)).Start<_003CUninstall_003Ed__53>(ref _003CUninstall_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUninstall_003Ed__._003C_003Et__builder)).Task;
	}

	public IObservable<ReactiveSimulator> LoadDefaultSims()
	{
		return (from am in SupportedModels().LogError<AvionicsModel>("Loading Default simulators.").CatchAndComplete<AvionicsModel>()
			group am by am.AvionicsType).SelectMany<IGroupedObservable<AvionicsType, AvionicsModel>, AvionicsModel>((Func<IGroupedObservable<AvionicsType, AvionicsModel>, IObservable<AvionicsModel>>)((IGroupedObservable<AvionicsType, AvionicsModel> amg) => ((IObservable<AvionicsModel>)amg).MaxBy<AvionicsModel, int>((AvionicsModel am) => am.DisplayOrderIndex).Select<System.Collections.Generic.IList<AvionicsModel>, AvionicsModel>((Func<System.Collections.Generic.IList<AvionicsModel>, AvionicsModel>)((System.Collections.Generic.IList<AvionicsModel> modelsWithMaxDisplayOrder) => Enumerable.LastOrDefault<AvionicsModel>((System.Collections.Generic.IEnumerable<AvionicsModel>)modelsWithMaxDisplayOrder))).SingleOrDefaultAsync<AvionicsModel>())).WhereClassNotNull<AvionicsModel>().SelectMany<AvionicsModel, ReactiveSimulator>((Func<AvionicsModel, System.Threading.Tasks.Task<ReactiveSimulator>>)([AsyncStateMachine(typeof(_003C_003CLoadDefaultSims_003Eb__54_2_003Ed))] [CompilerGenerated] async (AvionicsModel firstInCategory) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			InstallationSlot installSlot = new InstallationSlot(1, firstInCategory.AvionicsType);
			try
			{
				return await Install(firstInCategory, installSlot).DetachContext();
			}
			catch (System.Exception e)
			{
				Logger.Log(e, $"[Warning] Could not install default simulator {firstInCategory}.");
				throw;
			}
		}));
	}

	public virtual string ToString()
	{
		return AircraftIdentifier.IdentStringChecked;
	}

	[AsyncStateMachine(typeof(_003CRelist_003Ed__56))]
	public System.Threading.Tasks.Task Relist(AircraftListData updatedListData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CRelist_003Ed__56 _003CRelist_003Ed__ = default(_003CRelist_003Ed__56);
		_003CRelist_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CRelist_003Ed__._003C_003E4__this = this;
		_003CRelist_003Ed__.updatedListData = updatedListData;
		_003CRelist_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CRelist_003Ed__._003C_003Et__builder)).Start<_003CRelist_003Ed__56>(ref _003CRelist_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CRelist_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUpdateHeavy_003Ed__57))]
	public System.Threading.Tasks.Task UpdateHeavy(AircraftHeavyData updatedHeavy)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateHeavy_003Ed__57 _003CUpdateHeavy_003Ed__ = default(_003CUpdateHeavy_003Ed__57);
		_003CUpdateHeavy_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateHeavy_003Ed__._003C_003E4__this = this;
		_003CUpdateHeavy_003Ed__.updatedHeavy = updatedHeavy;
		_003CUpdateHeavy_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateHeavy_003Ed__._003C_003Et__builder)).Start<_003CUpdateHeavy_003Ed__57>(ref _003CUpdateHeavy_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateHeavy_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUpdateEntireAircraft_003Ed__58))]
	public System.Threading.Tasks.Task UpdateEntireAircraft(AircraftSnapshotData updatedSnapshot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateEntireAircraft_003Ed__58 _003CUpdateEntireAircraft_003Ed__ = default(_003CUpdateEntireAircraft_003Ed__58);
		_003CUpdateEntireAircraft_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateEntireAircraft_003Ed__._003C_003E4__this = this;
		_003CUpdateEntireAircraft_003Ed__.updatedSnapshot = updatedSnapshot;
		_003CUpdateEntireAircraft_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateEntireAircraft_003Ed__._003C_003Et__builder)).Start<_003CUpdateEntireAircraft_003Ed__58>(ref _003CUpdateEntireAircraft_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateEntireAircraft_003Ed__._003C_003Et__builder)).Task;
	}

	private static bool IsTheSameModel(TrainerModelKey completeModelKey, AvionicsModel m)
	{
		return m.Key.ModelId == completeModelKey.ModelId;
	}

	private static bool IsTheSameOrNewerPackage(TrainerModelKey completeModelKey, AvionicsModel m)
	{
		if (!(m.Key.TrainerPackageId == completeModelKey.TrainerPackageId))
		{
			return m.Key.TrainerPackageId.IsSuccessorOf(completeModelKey.TrainerPackageId);
		}
		return true;
	}

	private static bool NoBreakingChanges(TrainerModelKey completeModelKey, AvionicsModel m)
	{
		return !m.Package.ContainsBreakingChangesSince(completeModelKey.TrainerPackageId);
	}

	private static IObservable<AvionicsModel?> GetNewestModelOrNull(IObservable<AvionicsModel> models, TrainerModelKey completeModelKey)
	{
		TrainerModelKey completeModelKey2 = completeModelKey;
		return models.Where<AvionicsModel>((Func<AvionicsModel, bool>)((AvionicsModel m) => IsTheSameModel(completeModelKey2, m) && IsTheSameOrNewerPackage(completeModelKey2, m) && NoBreakingChanges(completeModelKey2, m))).MaxBy<AvionicsModel, Version>((AvionicsModel m) => m.Key.TrainerPackageId.BinaryVersion).Select<System.Collections.Generic.IList<AvionicsModel>, AvionicsModel>((Func<System.Collections.Generic.IList<AvionicsModel>, AvionicsModel>)((System.Collections.Generic.IList<AvionicsModel> matchingModelsWithNewestVersion) => Enumerable.LastOrDefault<AvionicsModel>((System.Collections.Generic.IEnumerable<AvionicsModel>)matchingModelsWithNewestVersion)))
			.SingleOrDefaultAsync<AvionicsModel>();
	}

	private void ThrowIfDifferentId(IHaveAircraftId idProvider)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (idProvider.AircraftIdentifier != AircraftIdentifier)
		{
			throw new ArgumentException("Aircraft identifiers must match.");
		}
	}

	[AsyncStateMachine(typeof(_003CUninstallSimByKeyWithLock_003Ed__64))]
	private System.Threading.Tasks.Task UninstallSimByKeyWithLock(SimInstallKey simInstallKey)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUninstallSimByKeyWithLock_003Ed__64 _003CUninstallSimByKeyWithLock_003Ed__ = default(_003CUninstallSimByKeyWithLock_003Ed__64);
		_003CUninstallSimByKeyWithLock_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUninstallSimByKeyWithLock_003Ed__._003C_003E4__this = this;
		_003CUninstallSimByKeyWithLock_003Ed__.simInstallKey = simInstallKey;
		_003CUninstallSimByKeyWithLock_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUninstallSimByKeyWithLock_003Ed__._003C_003Et__builder)).Start<_003CUninstallSimByKeyWithLock_003Ed__64>(ref _003CUninstallSimByKeyWithLock_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUninstallSimByKeyWithLock_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CGetSimulatorOrNullByKey_003Ed__65))]
	private async System.Threading.Tasks.Task<SimInstallData?> GetSimulatorOrNullByKey(SimInstallKey simInstallKey)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			return await SimRepo.GetSimulator(AircraftIdentifier, simInstallKey).DetachContext();
		}
		catch (SimulatorNotFoundException)
		{
			return null;
		}
	}
}
