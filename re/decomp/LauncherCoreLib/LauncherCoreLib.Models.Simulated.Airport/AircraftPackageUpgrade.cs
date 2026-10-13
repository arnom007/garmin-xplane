using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Tasks;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;
using LauncherCoreLib.Utility;
using Newtonsoft.Json.Linq;
using RecordShims;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public static class AircraftPackageUpgrade
{
	private sealed class PackageUpgradeSnapshotJsonMeta
	{
		public class Entry
		{
			[field: CompilerGenerated]
			public string EmbeddedReleaseId
			{
				[CompilerGenerated]
				get;
				[CompilerGenerated]
				set;
			}

			[field: CompilerGenerated]
			public Version LocalVersion
			{
				[CompilerGenerated]
				get;
				[CompilerGenerated]
				set;
			}

			[field: CompilerGenerated]
			public Version UpgradeVersion
			{
				[CompilerGenerated]
				get;
			}

			public Entry(PackageUpgradeVersions upgradeVersions)
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				if (upgradeVersions == null)
				{
					throw new ArgumentNullException("upgradeVersions");
				}
				EmbeddedReleaseId = ((object)upgradeVersions.EmbeddedReleaseId).ToString();
				LocalVersion = upgradeVersions.LocalVersion;
				UpgradeVersion = upgradeVersions.UpgradeVersion;
			}
		}

		[field: CompilerGenerated]
		public List<Entry> UpgradesInvolved
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = new List<Entry>();

	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass1_0
	{
		public System.Collections.Generic.IList<UpgradeDecisionLog> priorDecisionLogs;

		internal UpgradeDecisionLog _003CAskAndApplyUpgrade_003Eb__4(PackageReferenceUpgradeInfo up)
		{
			_003C_003Ec__DisplayClass1_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass1_1
			{
				up = up
			};
			return Enumerable.First<UpgradeDecisionLog>((System.Collections.Generic.IEnumerable<UpgradeDecisionLog>)priorDecisionLogs, (Func<UpgradeDecisionLog, bool>)((UpgradeDecisionLog l) => l.EmbeddedReleaseId == CS_0024_003C_003E8__locals0.up.CurrentId.EmbeddedReleaseCompositeId));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass1_1
	{
		public PackageReferenceUpgradeInfo up;

		internal bool _003CAskAndApplyUpgrade_003Eb__14(UpgradeDecisionLog l)
		{
			return l.EmbeddedReleaseId == up.CurrentId.EmbeddedReleaseCompositeId;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		public List<PackageReferenceUpgradeInfo> approved;

		public Func<KeyValuePair<SettingIdentifier, string>, KeyValuePair<SettingIdentifier, string>> _003C_003E9__7;

		internal bool _003CApplyStandardPackageUpgrades_003Eb__0(SimInstallData sim)
		{
			_003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_1
			{
				sim = sim
			};
			return Enumerable.Any<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)approved, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.ToUpgradeVersions().CanUpgradePackage(CS_0024_003C_003E8__locals0.sim.FullPackageIdOrThrow())));
		}

		internal SimInstallData _003CApplyStandardPackageUpgrades_003Eb__1(SimInstallData sim)
		{
			_003C_003Ec__DisplayClass3_2 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_2
			{
				CS_0024_003C_003E8__locals1 = this,
				sim = sim
			};
			return CS_0024_003C_003E8__locals0.sim.With(delegate(IPropertyChangeSet<SimInstallData> _)
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Expected O, but got Unknown
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "s");
				ISubsequentPropertyChangeSetApi<SimInstallData> subsequentPropertyChangeSetApi = _.Mutate(Expression.Lambda<Func<SimInstallData, TrainerPackageIdentifier>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), Enumerable.First<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.approved, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.ToUpgradeVersions().CanUpgradePackage(CS_0024_003C_003E8__locals0.sim.FullPackageIdOrThrow()))).UpgradeId);
				val = Expression.Parameter(typeof(SimInstallData), "s");
				subsequentPropertyChangeSetApi.AndMutate<ImmutableDictionary<SettingIdentifier, string>>(Expression.Lambda<Func<SimInstallData, ImmutableDictionary<SettingIdentifier, string>>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), ImmutableDictionary.ToImmutableDictionary<SettingIdentifier, string>(Enumerable.Select<KeyValuePair<SettingIdentifier, string>, KeyValuePair<SettingIdentifier, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)CS_0024_003C_003E8__locals0.sim.Settings, (Func<KeyValuePair<SettingIdentifier, string>, KeyValuePair<SettingIdentifier, string>>)((KeyValuePair<SettingIdentifier, string> setting) => setting.ToUpdatedPackageReference((System.Collections.Generic.IReadOnlyList<PackageReferenceUpgradeInfo>)CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.approved)))));
			});
		}

		internal KeyValuePair<SettingIdentifier, string> _003CApplyStandardPackageUpgrades_003Eb__7(KeyValuePair<SettingIdentifier, string> setting)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return setting.ToUpdatedPackageReference((System.Collections.Generic.IReadOnlyList<PackageReferenceUpgradeInfo>)approved);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_1
	{
		public SimInstallData sim;

		internal bool _003CApplyStandardPackageUpgrades_003Eb__2(PackageReferenceUpgradeInfo up)
		{
			return up.ToUpgradeVersions().CanUpgradePackage(sim.FullPackageIdOrThrow());
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_2
	{
		public SimInstallData sim;

		public _003C_003Ec__DisplayClass3_0 CS_0024_003C_003E8__locals1;

		public Func<PackageReferenceUpgradeInfo, bool> _003C_003E9__5;

		internal void _003CApplyStandardPackageUpgrades_003Eb__3(IPropertyChangeSet<SimInstallData> _)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Expected O, but got Unknown
			ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "s");
			ISubsequentPropertyChangeSetApi<SimInstallData> subsequentPropertyChangeSetApi = _.Mutate(Expression.Lambda<Func<SimInstallData, TrainerPackageIdentifier>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), Enumerable.First<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)CS_0024_003C_003E8__locals1.approved, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.ToUpgradeVersions().CanUpgradePackage(sim.FullPackageIdOrThrow()))).UpgradeId);
			val = Expression.Parameter(typeof(SimInstallData), "s");
			subsequentPropertyChangeSetApi.AndMutate<ImmutableDictionary<SettingIdentifier, string>>(Expression.Lambda<Func<SimInstallData, ImmutableDictionary<SettingIdentifier, string>>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), ImmutableDictionary.ToImmutableDictionary<SettingIdentifier, string>(Enumerable.Select<KeyValuePair<SettingIdentifier, string>, KeyValuePair<SettingIdentifier, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)sim.Settings, (Func<KeyValuePair<SettingIdentifier, string>, KeyValuePair<SettingIdentifier, string>>)((KeyValuePair<SettingIdentifier, string> setting) => setting.ToUpdatedPackageReference((System.Collections.Generic.IReadOnlyList<PackageReferenceUpgradeInfo>)CS_0024_003C_003E8__locals1.approved)))));
		}

		internal bool _003CApplyStandardPackageUpgrades_003Eb__5(PackageReferenceUpgradeInfo up)
		{
			return up.ToUpgradeVersions().CanUpgradePackage(sim.FullPackageIdOrThrow());
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CApplyStandardPackageUpgrades_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<System.Collections.Generic.IList<SimInstallData>> _003C_003Et__builder;

		public System.Collections.Generic.IList<PackageReferenceUpgradeInfo> upgrades;

		public ReactiveAircraft aircraft;

		private _003C_003Ec__DisplayClass3_0 _003C_003E8__1;

		private List<SimInstallData> _003CsimUpdates_003E5__2;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			System.Collections.Generic.IList<SimInstallData> result;
			try
			{
				ConfiguredTaskAwaiter<SimPanel> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPanel>);
					num = (_003C_003E1__state = -1);
					goto IL_00e2;
				}
				ConfiguredTaskAwaiter awaiter2;
				if (num == 1)
				{
					awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_019c;
				}
				_003C_003E8__1 = new _003C_003Ec__DisplayClass3_0();
				_003C_003E8__1.approved = Enumerable.ToList<PackageReferenceUpgradeInfo>(FilterUpgradesToApply((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)upgrades));
				if (_003C_003E8__1.approved.Count != ((System.Collections.Generic.ICollection<PackageReferenceUpgradeInfo>)upgrades).Count)
				{
					throw new ArgumentException("Only specify valid upgrades.");
				}
				if (_003C_003E8__1.approved.Count > 0)
				{
					awaiter = aircraft.Panel.GetUpdatedValue().DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CApplyStandardPackageUpgrades_003Ed__3>(ref awaiter, ref this);
						return;
					}
					goto IL_00e2;
				}
				result = (System.Collections.Generic.IList<SimInstallData>)new List<SimInstallData>();
				goto end_IL_0007;
				IL_00e2:
				SimPanel result2 = awaiter.GetResult();
				_003CsimUpdates_003E5__2 = Enumerable.ToList<SimInstallData>(Enumerable.Select<SimInstallData, SimInstallData>(Enumerable.Where<SimInstallData>(result2.AllSimulators.Values, (Func<SimInstallData, bool>)delegate(SimInstallData sim)
				{
					_003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass3_1
					{
						sim = sim
					};
					return Enumerable.Any<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)_003C_003E8__1.approved, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.ToUpgradeVersions().CanUpgradePackage(CS_0024_003C_003E8__locals1.sim.FullPackageIdOrThrow())));
				}), (Func<SimInstallData, SimInstallData>)delegate(SimInstallData sim)
				{
					_003C_003Ec__DisplayClass3_2 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_2
					{
						CS_0024_003C_003E8__locals1 = _003C_003E8__1,
						sim = sim
					};
					return CS_0024_003C_003E8__locals0.sim.With(delegate(IPropertyChangeSet<SimInstallData> _)
					{
						//IL_0021: Unknown result type (might be due to invalid IL or missing references)
						//IL_002b: Expected O, but got Unknown
						//IL_0093: Unknown result type (might be due to invalid IL or missing references)
						//IL_009d: Expected O, but got Unknown
						ParameterExpression val2 = Expression.Parameter(typeof(SimInstallData), "s");
						ISubsequentPropertyChangeSetApi<SimInstallData> subsequentPropertyChangeSetApi = _.Mutate(Expression.Lambda<Func<SimInstallData, TrainerPackageIdentifier>>((Expression)(object)Expression.Property((Expression)(object)val2, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val2 }), Enumerable.First<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.approved, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.ToUpgradeVersions().CanUpgradePackage(CS_0024_003C_003E8__locals0.sim.FullPackageIdOrThrow()))).UpgradeId);
						val2 = Expression.Parameter(typeof(SimInstallData), "s");
						subsequentPropertyChangeSetApi.AndMutate<ImmutableDictionary<SettingIdentifier, string>>(Expression.Lambda<Func<SimInstallData, ImmutableDictionary<SettingIdentifier, string>>>((Expression)(object)Expression.Property((Expression)(object)val2, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val2 }), ImmutableDictionary.ToImmutableDictionary<SettingIdentifier, string>(Enumerable.Select<KeyValuePair<SettingIdentifier, string>, KeyValuePair<SettingIdentifier, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)CS_0024_003C_003E8__locals0.sim.Settings, (Func<KeyValuePair<SettingIdentifier, string>, KeyValuePair<SettingIdentifier, string>>)((KeyValuePair<SettingIdentifier, string> setting) => setting.ToUpdatedPackageReference((System.Collections.Generic.IReadOnlyList<PackageReferenceUpgradeInfo>)CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.approved)))));
					});
				}));
				SimPanel updatedPanel = result2.WithUpdatedSimulators((System.Collections.Generic.IEnumerable<SimInstallData>)_003CsimUpdates_003E5__2);
				ConfiguredTaskAwaitable val = aircraft.UpdatePanel(updatedPanel).DetachContext();
				awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter2;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CApplyStandardPackageUpgrades_003Ed__3>(ref awaiter2, ref this);
					return;
				}
				goto IL_019c;
				IL_019c:
				((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
				result = (System.Collections.Generic.IList<SimInstallData>)_003CsimUpdates_003E5__2;
				end_IL_0007:;
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CAskAndApplyUpgrade_003Ed__1 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public Func<ReactiveAircraft, System.Collections.Generic.IList<PackageReferenceUpgradeInfo>, CancellationToken, System.Threading.Tasks.Task<UpgradeUserDecision>> askUser;

		public System.Collections.Generic.IList<PackageReferenceUpgradeInfo> allupgrades;

		public ReactiveAircraft aircraft;

		private _003C_003Ec__DisplayClass1_0 _003C_003E8__1;

		public CancellationToken token;

		public Func<ReactiveAircraft, System.Collections.Generic.IList<PackageReferenceUpgradeInfo>, System.Threading.Tasks.Task> applyUpgrades;

		private List<PackageReferenceUpgradeInfo> _003CneedsUserAttention_003E5__2;

		private List<PackageReferenceUpgradeInfo> _003ChopelessUpgrades_003E5__3;

		private List<PackageReferenceUpgradeInfo> _003CactionableUpgrades_003E5__4;

		private UpgradeUserDecision _003CuserSelection_003E5__5;

		private bool _003CuserWasAsked_003E5__6;

		private ConfiguredTaskAwaiter<System.Collections.Generic.IList<UpgradeDecisionLog>> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<UpgradeUserDecision> _003C_003Eu__2;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter<System.Collections.Generic.IList<UpgradeDecisionLog>> awaiter3;
				ConfiguredTaskAwaiter<UpgradeUserDecision> awaiter2;
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaitable val;
				System.Collections.Generic.IList<UpgradeDecisionLog> result2;
				Dictionary<PackageReferenceUpgradeInfo, UpgradeUserDecision> val2;
				UpgradeUserDecision result;
				switch (num)
				{
				default:
					askUser = DecorateAskWithLogging(askUser);
					_003CneedsUserAttention_003E5__2 = Enumerable.ToList<PackageReferenceUpgradeInfo>(Enumerable.Where<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)allupgrades, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.RequiresUserAttention)));
					if (_003CneedsUserAttention_003E5__2.Count > 0)
					{
						_003C_003E8__1 = new _003C_003Ec__DisplayClass1_0();
						_003ChopelessUpgrades_003E5__3 = Enumerable.ToList<PackageReferenceUpgradeInfo>(Enumerable.Where<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)_003CneedsUserAttention_003E5__2, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.IsHopeless)));
						_003CactionableUpgrades_003E5__4 = Enumerable.ToList<PackageReferenceUpgradeInfo>(Enumerable.Where<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)_003CneedsUserAttention_003E5__2, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.IsActionable)));
						awaiter3 = aircraft.UnitOfWork.PackageRepos.UpgradeDecisions.GetPriorUserUpgradeDecisions(Enumerable.Select<PackageReferenceUpgradeInfo, TrainerEmbeddedReleaseId>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)_003CactionableUpgrades_003E5__4, (Func<PackageReferenceUpgradeInfo, TrainerEmbeddedReleaseId>)((PackageReferenceUpgradeInfo up) => up.CurrentId.EmbeddedReleaseCompositeId))).DetachContext().GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter3;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IList<UpgradeDecisionLog>>, _003CAskAndApplyUpgrade_003Ed__1>(ref awaiter3, ref this);
							return;
						}
						goto IL_0188;
					}
					goto end_IL_0007;
				case 0:
					awaiter3 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IList<UpgradeDecisionLog>>);
					num = (_003C_003E1__state = -1);
					goto IL_0188;
				case 1:
					awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<UpgradeUserDecision>);
					num = (_003C_003E1__state = -1);
					goto IL_02fd;
				case 2:
					awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<UpgradeUserDecision>);
					num = (_003C_003E1__state = -1);
					goto IL_03ea;
				case 3:
					awaiter = _003C_003Eu__3;
					_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_04a4;
				case 4:
					{
						awaiter = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_05bd;
					}
					IL_03ea:
					result = awaiter2.GetResult();
					_003CuserSelection_003E5__5 = result;
					_003CuserWasAsked_003E5__6 = true;
					goto IL_042a;
					IL_04a4:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					goto IL_0511;
					IL_042a:
					if (_003CuserSelection_003E5__5 == UpgradeUserDecision.ApprovedUpgrade)
					{
						val = applyUpgrades.Invoke(aircraft, (System.Collections.Generic.IList<PackageReferenceUpgradeInfo>)_003CactionableUpgrades_003E5__4).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 3);
							_003C_003Eu__3 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CAskAndApplyUpgrade_003Ed__1>(ref awaiter, ref this);
							return;
						}
						goto IL_04a4;
					}
					if (_003CuserSelection_003E5__5 != 0)
					{
						goto IL_0511;
					}
					if (Enumerable.Any<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)_003CneedsUserAttention_003E5__2, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.IsRequired)))
					{
						if (!((CancellationToken)(ref token)).CanBeCanceled || !((CancellationToken)(ref token)).IsCancellationRequested)
						{
							throw new InvalidOperationException("The only reason an undecided result should occur is because the user switched to a different view or aircraft and canceled the token.");
						}
						throw new OperationCanceledException();
					}
					goto end_IL_0007;
					IL_0511:
					if (!_003CuserWasAsked_003E5__6)
					{
						break;
					}
					val = aircraft.UnitOfWork.PackageRepos.UpgradeDecisions.StoreUserUpgradeDecision(Enumerable.Select<PackageReferenceUpgradeInfo, PackageUpgradeVersions>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)_003CactionableUpgrades_003E5__4, (Func<PackageReferenceUpgradeInfo, PackageUpgradeVersions>)((PackageReferenceUpgradeInfo up) => up.ToUpgradeVersions())), _003CuserSelection_003E5__5).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 4);
						_003C_003Eu__3 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CAskAndApplyUpgrade_003Ed__1>(ref awaiter, ref this);
						return;
					}
					goto IL_05bd;
					IL_0188:
					result2 = awaiter3.GetResult();
					_003C_003E8__1.priorDecisionLogs = result2;
					val2 = Enumerable.ToDictionary<KeyValuePair<PackageReferenceUpgradeInfo, UpgradeDecisionLog>, PackageReferenceUpgradeInfo, UpgradeUserDecision>((System.Collections.Generic.IEnumerable<KeyValuePair<PackageReferenceUpgradeInfo, UpgradeDecisionLog>>)Enumerable.ToDictionary<PackageReferenceUpgradeInfo, PackageReferenceUpgradeInfo, UpgradeDecisionLog>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)_003CactionableUpgrades_003E5__4, (Func<PackageReferenceUpgradeInfo, PackageReferenceUpgradeInfo>)((PackageReferenceUpgradeInfo up) => up), (Func<PackageReferenceUpgradeInfo, UpgradeDecisionLog>)delegate(PackageReferenceUpgradeInfo up)
					{
						_003C_003Ec__DisplayClass1_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass1_1
						{
							up = up
						};
						return Enumerable.First<UpgradeDecisionLog>((System.Collections.Generic.IEnumerable<UpgradeDecisionLog>)_003C_003E8__1.priorDecisionLogs, (Func<UpgradeDecisionLog, bool>)((UpgradeDecisionLog l) => l.EmbeddedReleaseId == CS_0024_003C_003E8__locals0.up.CurrentId.EmbeddedReleaseCompositeId));
					}), (Func<KeyValuePair<PackageReferenceUpgradeInfo, UpgradeDecisionLog>, PackageReferenceUpgradeInfo>)((KeyValuePair<PackageReferenceUpgradeInfo, UpgradeDecisionLog> kv) => kv.Key), (Func<KeyValuePair<PackageReferenceUpgradeInfo, UpgradeDecisionLog>, UpgradeUserDecision>)((KeyValuePair<PackageReferenceUpgradeInfo, UpgradeDecisionLog> kv) => kv.Value.GetDecision(kv.Key.ToUpgradeVersions())));
					_003CuserWasAsked_003E5__6 = false;
					if (Enumerable.Any<UpgradeUserDecision>((System.Collections.Generic.IEnumerable<UpgradeUserDecision>)val2.Values, (Func<UpgradeUserDecision, bool>)((UpgradeUserDecision d) => d == UpgradeUserDecision.Undecided)) || _003ChopelessUpgrades_003E5__3.Count > 0 || Enumerable.Any<KeyValuePair<PackageReferenceUpgradeInfo, UpgradeUserDecision>>((System.Collections.Generic.IEnumerable<KeyValuePair<PackageReferenceUpgradeInfo, UpgradeUserDecision>>)val2, (Func<KeyValuePair<PackageReferenceUpgradeInfo, UpgradeUserDecision>, bool>)((KeyValuePair<PackageReferenceUpgradeInfo, UpgradeUserDecision> kv) => kv.Key.HasBreakingChanges && kv.Value == UpgradeUserDecision.ApprovedUpgrade)))
					{
						awaiter2 = askUser.Invoke(aircraft, (System.Collections.Generic.IList<PackageReferenceUpgradeInfo>)_003CneedsUserAttention_003E5__2, token).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<UpgradeUserDecision>, _003CAskAndApplyUpgrade_003Ed__1>(ref awaiter2, ref this);
							return;
						}
						goto IL_02fd;
					}
					if (Enumerable.Any<UpgradeUserDecision>((System.Collections.Generic.IEnumerable<UpgradeUserDecision>)val2.Values, (Func<UpgradeUserDecision, bool>)((UpgradeUserDecision d) => d == UpgradeUserDecision.SkippedUpgrade)))
					{
						if (Enumerable.Any<KeyValuePair<PackageReferenceUpgradeInfo, UpgradeUserDecision>>((System.Collections.Generic.IEnumerable<KeyValuePair<PackageReferenceUpgradeInfo, UpgradeUserDecision>>)val2, (Func<KeyValuePair<PackageReferenceUpgradeInfo, UpgradeUserDecision>, bool>)((KeyValuePair<PackageReferenceUpgradeInfo, UpgradeUserDecision> kv) => kv.Value == UpgradeUserDecision.SkippedUpgrade && kv.Key.IsRequired)))
						{
							awaiter2 = askUser.Invoke(aircraft, (System.Collections.Generic.IList<PackageReferenceUpgradeInfo>)_003CneedsUserAttention_003E5__2, token).DetachContext().GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__2 = awaiter2;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<UpgradeUserDecision>, _003CAskAndApplyUpgrade_003Ed__1>(ref awaiter2, ref this);
								return;
							}
							goto IL_03ea;
						}
						Logger.Log(LogLevel.Info, "At least one upgrade has specified skip, so we must skip them all.");
						_003CuserSelection_003E5__5 = UpgradeUserDecision.SkippedUpgrade;
					}
					else
					{
						Logger.Log(LogLevel.Info, "All upgrades have prior approval.");
						_003CuserSelection_003E5__5 = UpgradeUserDecision.ApprovedUpgrade;
					}
					goto IL_042a;
					IL_02fd:
					result = awaiter2.GetResult();
					_003CuserSelection_003E5__5 = result;
					_003CuserWasAsked_003E5__6 = true;
					goto IL_042a;
					IL_05bd:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					break;
				}
				_003C_003E8__1 = null;
				_003ChopelessUpgrades_003E5__3 = null;
				_003CactionableUpgrades_003E5__4 = null;
				end_IL_0007:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CneedsUserAttention_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CneedsUserAttention_003E5__2 = null;
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
	private struct _003CGetPackageUpgradeInfo_003Ed__0 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<PackageReferenceUpgradeInfo> _003C_003Et__builder;

		public ReactiveAircraft aircraftChange;

		public TrainerPackageIdentifier trainerPackageIdentifier;

		private ConfiguredTaskAwaiter<PackageInstallationInfo> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PackageReferenceUpgradeInfo result2;
			try
			{
				ConfiguredTaskAwaiter<PackageInstallationInfo> awaiter;
				if (num != 0)
				{
					awaiter = aircraftChange.UnitOfWork.Packages.GetPackageByReleaseId(trainerPackageIdentifier.EmbeddedReleaseCompositeId).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PackageInstallationInfo>, _003CGetPackageUpgradeInfo_003Ed__0>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PackageInstallationInfo>);
					num = (_003C_003E1__state = -1);
				}
				PackageInstallationInfo result = awaiter.GetResult();
				result2 = new PackageReferenceUpgradeInfo(trainerPackageIdentifier, result);
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
	private struct _003CTakeUpgradeSnapshot_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftSnapshotInfo> _003C_003Et__builder;

		public System.Collections.Generic.IList<PackageReferenceUpgradeInfo> upgradesInvolved;

		public ReactiveAircraft aircraft;

		private ConfiguredTaskAwaiter<AircraftSnapshotInfo> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			AircraftSnapshotInfo result;
			try
			{
				ConfiguredTaskAwaiter<AircraftSnapshotInfo> awaiter;
				if (num != 0)
				{
					if (((System.Collections.Generic.ICollection<PackageReferenceUpgradeInfo>)upgradesInvolved).Count == 0)
					{
						throw new ArgumentException("No reason to take a snapshot without any upgrades.");
					}
					List<PackageUpgradeSnapshotJsonMeta.Entry> val = Enumerable.ToList<PackageUpgradeSnapshotJsonMeta.Entry>(Enumerable.Select<PackageReferenceUpgradeInfo, PackageUpgradeSnapshotJsonMeta.Entry>(FilterUpgradesToApply((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)upgradesInvolved), (Func<PackageReferenceUpgradeInfo, PackageUpgradeSnapshotJsonMeta.Entry>)((PackageReferenceUpgradeInfo up) => new PackageUpgradeSnapshotJsonMeta.Entry(up.ToUpgradeVersions()))));
					if (val.Count != ((System.Collections.Generic.ICollection<PackageReferenceUpgradeInfo>)upgradesInvolved).Count)
					{
						throw new ArgumentException("Only specify valid upgrades.");
					}
					SynchronizedJsonDataSource synchronizedJsonDataSource = new SynchronizedJsonDataSource();
					PackageUpgradeSnapshotJsonMeta o = new PackageUpgradeSnapshotJsonMeta
					{
						UpgradesInvolved = val
					};
					synchronizedJsonDataSource.StoreValueSync("PackageUpgrade", JObject.FromObject(o, SerializationUtil.JSON_SERIALIZER.Value));
					awaiter = aircraft.TakeSnapshot(new AircraftSnapshotMetaData("Package Upgrade", synchronizedJsonDataSource.ToReadonlySourceSync())).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotInfo>, _003CTakeUpgradeSnapshot_003Ed__2>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotInfo>);
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

	[AsyncStateMachine(typeof(_003CGetPackageUpgradeInfo_003Ed__0))]
	public static async System.Threading.Tasks.Task<PackageReferenceUpgradeInfo> GetPackageUpgradeInfo(ReactiveAircraft aircraftChange, TrainerPackageIdentifier trainerPackageIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new PackageReferenceUpgradeInfo(trainerPackageIdentifier, await aircraftChange.UnitOfWork.Packages.GetPackageByReleaseId(trainerPackageIdentifier.EmbeddedReleaseCompositeId).DetachContext());
	}

	[AsyncStateMachine(typeof(_003CAskAndApplyUpgrade_003Ed__1))]
	public static System.Threading.Tasks.Task AskAndApplyUpgrade(ReactiveAircraft aircraft, System.Collections.Generic.IList<PackageReferenceUpgradeInfo> allupgrades, Func<ReactiveAircraft, System.Collections.Generic.IList<PackageReferenceUpgradeInfo>, CancellationToken, System.Threading.Tasks.Task<UpgradeUserDecision>> askUser, Func<ReactiveAircraft, System.Collections.Generic.IList<PackageReferenceUpgradeInfo>, System.Threading.Tasks.Task> applyUpgrades, CancellationToken token = default(CancellationToken))
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		_003CAskAndApplyUpgrade_003Ed__1 _003CAskAndApplyUpgrade_003Ed__ = default(_003CAskAndApplyUpgrade_003Ed__1);
		_003CAskAndApplyUpgrade_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CAskAndApplyUpgrade_003Ed__.aircraft = aircraft;
		_003CAskAndApplyUpgrade_003Ed__.allupgrades = allupgrades;
		_003CAskAndApplyUpgrade_003Ed__.askUser = askUser;
		_003CAskAndApplyUpgrade_003Ed__.applyUpgrades = applyUpgrades;
		_003CAskAndApplyUpgrade_003Ed__.token = token;
		_003CAskAndApplyUpgrade_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CAskAndApplyUpgrade_003Ed__._003C_003Et__builder)).Start<_003CAskAndApplyUpgrade_003Ed__1>(ref _003CAskAndApplyUpgrade_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CAskAndApplyUpgrade_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CTakeUpgradeSnapshot_003Ed__2))]
	public static async System.Threading.Tasks.Task<AircraftSnapshotInfo> TakeUpgradeSnapshot(ReactiveAircraft aircraft, System.Collections.Generic.IList<PackageReferenceUpgradeInfo> upgradesInvolved)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (((System.Collections.Generic.ICollection<PackageReferenceUpgradeInfo>)upgradesInvolved).Count == 0)
		{
			throw new ArgumentException("No reason to take a snapshot without any upgrades.");
		}
		List<PackageUpgradeSnapshotJsonMeta.Entry> val = Enumerable.ToList<PackageUpgradeSnapshotJsonMeta.Entry>(Enumerable.Select<PackageReferenceUpgradeInfo, PackageUpgradeSnapshotJsonMeta.Entry>(FilterUpgradesToApply((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)upgradesInvolved), (Func<PackageReferenceUpgradeInfo, PackageUpgradeSnapshotJsonMeta.Entry>)((PackageReferenceUpgradeInfo up) => new PackageUpgradeSnapshotJsonMeta.Entry(up.ToUpgradeVersions()))));
		if (val.Count != ((System.Collections.Generic.ICollection<PackageReferenceUpgradeInfo>)upgradesInvolved).Count)
		{
			throw new ArgumentException("Only specify valid upgrades.");
		}
		SynchronizedJsonDataSource synchronizedJsonDataSource = new SynchronizedJsonDataSource();
		PackageUpgradeSnapshotJsonMeta o = new PackageUpgradeSnapshotJsonMeta
		{
			UpgradesInvolved = val
		};
		synchronizedJsonDataSource.StoreValueSync("PackageUpgrade", JObject.FromObject(o, SerializationUtil.JSON_SERIALIZER.Value));
		return await aircraft.TakeSnapshot(new AircraftSnapshotMetaData("Package Upgrade", synchronizedJsonDataSource.ToReadonlySourceSync())).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CApplyStandardPackageUpgrades_003Ed__3))]
	public static async System.Threading.Tasks.Task<System.Collections.Generic.IList<SimInstallData>> ApplyStandardPackageUpgrades(ReactiveAircraft aircraft, System.Collections.Generic.IList<PackageReferenceUpgradeInfo> upgrades)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<PackageReferenceUpgradeInfo> approved = Enumerable.ToList<PackageReferenceUpgradeInfo>(FilterUpgradesToApply((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)upgrades));
		if (approved.Count != ((System.Collections.Generic.ICollection<PackageReferenceUpgradeInfo>)upgrades).Count)
		{
			throw new ArgumentException("Only specify valid upgrades.");
		}
		if (approved.Count > 0)
		{
			SimPanel simPanel = await aircraft.Panel.GetUpdatedValue().DetachContext();
			List<SimInstallData> simUpdates = Enumerable.ToList<SimInstallData>(Enumerable.Select<SimInstallData, SimInstallData>(Enumerable.Where<SimInstallData>(simPanel.AllSimulators.Values, (Func<SimInstallData, bool>)((SimInstallData sim) => Enumerable.Any<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)approved, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.ToUpgradeVersions().CanUpgradePackage(sim.FullPackageIdOrThrow()))))), (Func<SimInstallData, SimInstallData>)((SimInstallData sim) => sim.With(delegate(IPropertyChangeSet<SimInstallData> _)
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Expected O, but got Unknown
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "s");
				ISubsequentPropertyChangeSetApi<SimInstallData> subsequentPropertyChangeSetApi = _.Mutate(Expression.Lambda<Func<SimInstallData, TrainerPackageIdentifier>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), Enumerable.First<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)approved, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.ToUpgradeVersions().CanUpgradePackage(sim.FullPackageIdOrThrow()))).UpgradeId);
				val = Expression.Parameter(typeof(SimInstallData), "s");
				subsequentPropertyChangeSetApi.AndMutate<ImmutableDictionary<SettingIdentifier, string>>(Expression.Lambda<Func<SimInstallData, ImmutableDictionary<SettingIdentifier, string>>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), ImmutableDictionary.ToImmutableDictionary<SettingIdentifier, string>(Enumerable.Select<KeyValuePair<SettingIdentifier, string>, KeyValuePair<SettingIdentifier, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)sim.Settings, (Func<KeyValuePair<SettingIdentifier, string>, KeyValuePair<SettingIdentifier, string>>)((KeyValuePair<SettingIdentifier, string> setting) => setting.ToUpdatedPackageReference((System.Collections.Generic.IReadOnlyList<PackageReferenceUpgradeInfo>)approved)))));
			}))));
			SimPanel updatedPanel = simPanel.WithUpdatedSimulators((System.Collections.Generic.IEnumerable<SimInstallData>)simUpdates);
			await aircraft.UpdatePanel(updatedPanel).DetachContext();
			return (System.Collections.Generic.IList<SimInstallData>)simUpdates;
		}
		return (System.Collections.Generic.IList<SimInstallData>)new List<SimInstallData>();
	}

	public static System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo> FilterUpgradesToApply(System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo> upgrades)
	{
		return Enumerable.Where<PackageReferenceUpgradeInfo>(upgrades, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.IsActionable));
	}

	private static Func<ReactiveAircraft, System.Collections.Generic.IList<PackageReferenceUpgradeInfo>, CancellationToken, System.Threading.Tasks.Task<UpgradeUserDecision>> DecorateAskWithLogging(Func<ReactiveAircraft, System.Collections.Generic.IList<PackageReferenceUpgradeInfo>, CancellationToken, System.Threading.Tasks.Task<UpgradeUserDecision>> askUser)
	{
		Func<ReactiveAircraft, System.Collections.Generic.IList<PackageReferenceUpgradeInfo>, CancellationToken, System.Threading.Tasks.Task<UpgradeUserDecision>> originalAskUser = askUser;
		askUser = [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass5_0._003C_003CDecorateAskWithLogging_003Eb__0_003Ed))] async (ReactiveAircraft a, System.Collections.Generic.IList<PackageReferenceUpgradeInfo> ups, CancellationToken t) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (Enumerable.Any<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)ups, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => (object)up.UpgradeId == null)))
			{
				throw new ArgumentException("All package install infos must have an upgrade available.");
			}
			string text = string.Join("\n\t", Enumerable.Select<PackageReferenceUpgradeInfo, string>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)ups, (Func<PackageReferenceUpgradeInfo, string>)delegate(PackageReferenceUpgradeInfo up)
			{
				string text2 = (up.IsDiscontinuedAndNotInstalled ? "Discontinued" : ((object)up.UpgradeId.BinaryVersion).ToString());
				return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "[{0,20}] {1} {2} -> {3}", new object[4]
				{
					GetSimpleUpgradeLabel(up),
					up.CurrentId.EmbeddedReleaseCompositeId,
					up.CurrentId.BinaryVersion,
					text2
				});
			}));
			Logger.Log(LogLevel.Info, "[Package Upgrade Decision][{0} - {1}]\n\t{2}", a.AircraftIdentifier, a.ListData.CachedValue.DisplayName ?? "<not cached>", text);
			UpgradeUserDecision upgradeUserDecision = await originalAskUser.Invoke(a, ups, t).DetachContext();
			Logger.Log(LogLevel.Info, "[Package Upgrade Answer][{0} - {1}] {2}", a.AircraftIdentifier, a.ListData.CachedValue.DisplayName ?? "<not cached>", upgradeUserDecision);
			return upgradeUserDecision;
		};
		return askUser;
	}

	private static string GetSimpleUpgradeLabel(PackageReferenceUpgradeInfo up)
	{
		if (up.IsDowngrade)
		{
			return "Downgrade";
		}
		if (up.VersionsAreEqual)
		{
			return "None";
		}
		if (up.IsDiscontinuedAndNotInstalled)
		{
			return "Discontinued";
		}
		if (up.IsUpgrade)
		{
			if (up.IsRequired)
			{
				return "Required Upgrade";
			}
			return "Upgrade";
		}
		return "Unknown";
	}
}
