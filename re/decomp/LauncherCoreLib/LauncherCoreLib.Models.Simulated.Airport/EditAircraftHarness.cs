using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Simulated.Panel;
using RecordShims;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class EditAircraftHarness : TemporaryUnitHarness<ReactiveAircraft>
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass5_0
	{
		public SimPanel panelToReplace;

		internal bool _003CDoConfirm_003Eb__0(SimInstallData s)
		{
			return panelToReplace.ContainsSimulatorKey(s.AircraftSimKey);
		}

		internal SimInstallData _003CDoConfirm_003Eb__1(SimInstallData s)
		{
			_003C_003Ec__DisplayClass5_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass5_1
			{
				CS_0024_003C_003E8__locals1 = this,
				s = s
			};
			return CS_0024_003C_003E8__locals0.s.With(delegate(IPropertyChangeSet<SimInstallData> _)
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "sd");
				_.Mutate(Expression.Lambda<Func<SimInstallData, bool>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.panelToReplace.AllSimulators[CS_0024_003C_003E8__locals0.s.InstallationKey].HasArchivedWinfsMount);
			});
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass5_1
	{
		public SimInstallData s;

		public _003C_003Ec__DisplayClass5_0 CS_0024_003C_003E8__locals1;

		internal void _003CDoConfirm_003Eb__2(IPropertyChangeSet<SimInstallData> _)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "sd");
			_.Mutate(Expression.Lambda<Func<SimInstallData, bool>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), CS_0024_003C_003E8__locals1.panelToReplace.AllSimulators[s.InstallationKey].HasArchivedWinfsMount);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CDoConfirm_003Ed__5 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ReactiveAircraft> _003C_003Et__builder;

		public EditAircraftHarness _003C_003E4__this;

		private _003C_003Ec__DisplayClass5_0 _003C_003E8__1;

		private System.Threading.Tasks.Task<SimPanel> _003CpanelToReplaceTask_003E5__2;

		private AircraftSnapshotData _003CedittedSnapshot_003E5__3;

		private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__2;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			EditAircraftHarness editAircraftHarness = _003C_003E4__this;
			ReactiveAircraft result3;
			try
			{
				ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter3;
				ConfiguredTaskAwaiter<SimPanel> awaiter2;
				ConfiguredTaskAwaiter awaiter;
				SimPanel result;
				System.Collections.Generic.IEnumerable<SimInstallData> simUpdates;
				ConfiguredTaskAwaitable val;
				AircraftSnapshotData result2;
				switch (num)
				{
				default:
					_003C_003E8__1 = new _003C_003Ec__DisplayClass5_0();
					_003CpanelToReplaceTask_003E5__2 = editAircraftHarness.ActualUnit.Simulators.GetSimulatorPanel(editAircraftHarness.TemporaryAircraft.AircraftIdentifier);
					awaiter3 = editAircraftHarness.TemporaryAircraft.CompositeSnapshots.GetUpdatedValue().DetachContext().GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter3;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CDoConfirm_003Ed__5>(ref awaiter3, ref this);
						return;
					}
					goto IL_00b6;
				case 0:
					awaiter3 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
					num = (_003C_003E1__state = -1);
					goto IL_00b6;
				case 1:
					awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<SimPanel>);
					num = (_003C_003E1__state = -1);
					goto IL_0127;
				case 2:
					{
						awaiter = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						break;
					}
					IL_0127:
					result = awaiter2.GetResult();
					_003C_003E8__1.panelToReplace = result;
					simUpdates = Enumerable.Select<SimInstallData, SimInstallData>(Enumerable.Where<SimInstallData>(_003CedittedSnapshot_003E5__3.Panel.AllSimulators.Values, (Func<SimInstallData, bool>)((SimInstallData s) => _003C_003E8__1.panelToReplace.ContainsSimulatorKey(s.AircraftSimKey))), (Func<SimInstallData, SimInstallData>)delegate(SimInstallData s)
					{
						_003C_003Ec__DisplayClass5_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass5_1
						{
							CS_0024_003C_003E8__locals1 = _003C_003E8__1,
							s = s
						};
						return CS_0024_003C_003E8__locals0.s.With(delegate(IPropertyChangeSet<SimInstallData> _)
						{
							//IL_0021: Unknown result type (might be due to invalid IL or missing references)
							//IL_002b: Expected O, but got Unknown
							ParameterExpression val2 = Expression.Parameter(typeof(SimInstallData), "sd");
							_.Mutate(Expression.Lambda<Func<SimInstallData, bool>>((Expression)(object)Expression.Property((Expression)(object)val2, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val2 }), CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.panelToReplace.AllSimulators[CS_0024_003C_003E8__locals0.s.InstallationKey].HasArchivedWinfsMount);
						});
					});
					_003CedittedSnapshot_003E5__3 = _003CedittedSnapshot_003E5__3.ReplacePanel(_003CedittedSnapshot_003E5__3.Panel.WithUpdatedSimulators(simUpdates));
					val = editAircraftHarness.ActualUnit.Aircraft.UpdateAircraft(_003CedittedSnapshot_003E5__3).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__3 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CDoConfirm_003Ed__5>(ref awaiter, ref this);
						return;
					}
					break;
					IL_00b6:
					result2 = awaiter3.GetResult();
					_003CedittedSnapshot_003E5__3 = result2;
					awaiter2 = _003CpanelToReplaceTask_003E5__2.DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CDoConfirm_003Ed__5>(ref awaiter2, ref this);
						return;
					}
					goto IL_0127;
				}
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				result3 = new ReactiveAircraft(editAircraftHarness.ActualUnit, _003CedittedSnapshot_003E5__3);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CpanelToReplaceTask_003E5__2 = null;
				_003CedittedSnapshot_003E5__3 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CpanelToReplaceTask_003E5__2 = null;
			_003CedittedSnapshot_003E5__3 = null;
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
	private struct _003CStart_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<EditAircraftHarness> _003C_003Et__builder;

		public IUnitOfWork unitOfWork;

		public AircraftIdentifier editIdent;

		private TemporaryUnitOfWork _003CtempUnit_003E5__2;

		private AircraftSnapshotData _003CaircraftSnapshot_003E5__3;

		private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			EditAircraftHarness result2;
			try
			{
				if ((uint)num > 2u)
				{
					_003CtempUnit_003E5__2 = new TemporaryUnitOfWork(unitOfWork);
				}
				try
				{
					ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter2;
					ConfiguredTaskAwaiter awaiter;
					ConfiguredTaskAwaitable val;
					AircraftSnapshotData result;
					switch (num)
					{
					default:
						awaiter2 = unitOfWork.Aircraft.GetAircraftSnapshotById(editIdent).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CStart_003Ed__4>(ref awaiter2, ref this);
							return;
						}
						goto IL_009c;
					case 0:
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
						num = (_003C_003E1__state = -1);
						goto IL_009c;
					case 1:
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
						num = (_003C_003E1__state = -1);
						goto IL_011d;
					case 2:
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							break;
						}
						IL_011d:
						if (awaiter2.GetResult().AircraftIdentifier != _003CaircraftSnapshot_003E5__3.AircraftIdentifier)
						{
							throw new InvalidOperationException("The temporary aircraft was not created correctly.");
						}
						val = _003CtempUnit_003E5__2.Aircraft.UpdateAircraft(_003CaircraftSnapshot_003E5__3).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__2 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CStart_003Ed__4>(ref awaiter, ref this);
							return;
						}
						break;
						IL_009c:
						result = awaiter2.GetResult();
						_003CaircraftSnapshot_003E5__3 = result;
						awaiter2 = _003CtempUnit_003E5__2.Aircraft.CreateNewAircraft(_003CaircraftSnapshot_003E5__3.ListData).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CStart_003Ed__4>(ref awaiter2, ref this);
							return;
						}
						goto IL_011d;
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					result2 = new EditAircraftHarness(_003CtempUnit_003E5__2, new ReactiveAircraft(_003CtempUnit_003E5__2, _003CaircraftSnapshot_003E5__3));
				}
				catch (System.Exception)
				{
					_003CtempUnit_003E5__2.Dispose();
					throw;
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CtempUnit_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CtempUnit_003E5__2 = null;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[field: CompilerGenerated]
	public ReactiveAircraft TemporaryAircraft
	{
		[CompilerGenerated]
		get;
	}

	private EditAircraftHarness(TemporaryUnitOfWork tempUnitOfWork, ReactiveAircraft aircraft)
		: base(tempUnitOfWork)
	{
		TemporaryAircraft = aircraft;
	}

	[AsyncStateMachine(typeof(_003CStart_003Ed__4))]
	public static async System.Threading.Tasks.Task<EditAircraftHarness> Start(IUnitOfWork unitOfWork, AircraftIdentifier editIdent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		TemporaryUnitOfWork tempUnit = new TemporaryUnitOfWork(unitOfWork);
		try
		{
			AircraftSnapshotData aircraftSnapshot = await unitOfWork.Aircraft.GetAircraftSnapshotById(editIdent).DetachContext();
			if ((await tempUnit.Aircraft.CreateNewAircraft(aircraftSnapshot.ListData).DetachContext()).AircraftIdentifier != aircraftSnapshot.AircraftIdentifier)
			{
				throw new InvalidOperationException("The temporary aircraft was not created correctly.");
			}
			await tempUnit.Aircraft.UpdateAircraft(aircraftSnapshot).DetachContext();
			return new EditAircraftHarness(tempUnit, new ReactiveAircraft(tempUnit, aircraftSnapshot));
		}
		catch (System.Exception)
		{
			tempUnit.Dispose();
			throw;
		}
	}

	[AsyncStateMachine(typeof(_003CDoConfirm_003Ed__5))]
	protected override async System.Threading.Tasks.Task<ReactiveAircraft> DoConfirm()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		System.Threading.Tasks.Task<SimPanel> panelToReplaceTask = base.ActualUnit.Simulators.GetSimulatorPanel(TemporaryAircraft.AircraftIdentifier);
		AircraftSnapshotData edittedSnapshot2 = await TemporaryAircraft.CompositeSnapshots.GetUpdatedValue().DetachContext();
		SimPanel panelToReplace = await panelToReplaceTask.DetachContext();
		System.Collections.Generic.IEnumerable<SimInstallData> simUpdates = Enumerable.Select<SimInstallData, SimInstallData>(Enumerable.Where<SimInstallData>(edittedSnapshot2.Panel.AllSimulators.Values, (Func<SimInstallData, bool>)((SimInstallData s) => panelToReplace.ContainsSimulatorKey(s.AircraftSimKey))), (Func<SimInstallData, SimInstallData>)((SimInstallData s) => s.With(delegate(IPropertyChangeSet<SimInstallData> _)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "sd");
			_.Mutate(Expression.Lambda<Func<SimInstallData, bool>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), panelToReplace.AllSimulators[s.InstallationKey].HasArchivedWinfsMount);
		})));
		edittedSnapshot2 = edittedSnapshot2.ReplacePanel(edittedSnapshot2.Panel.WithUpdatedSimulators(simUpdates));
		await base.ActualUnit.Aircraft.UpdateAircraft(edittedSnapshot2).DetachContext();
		return new ReactiveAircraft(base.ActualUnit, edittedSnapshot2);
	}
}
