using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Tasks;
using LauncherCoreLib.Models.AppLifeCycle.Migrations;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Persistence;
using LauncherCoreLib.Models.Persistence.Models;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class HangarMigration : MigrationBase
{
	private sealed class MigrationResult
	{
		[field: CompilerGenerated]
		public System.Exception? Error
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public Dictionary<V1PersistedAircraft, AircraftSnapshotData> MigratedAircraft
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		public MigrationResult(Dictionary<V1PersistedAircraft, AircraftSnapshotData> migratedAircraft, System.Exception? error)
		{
			MigratedAircraft = migratedAircraft;
			Error = error;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass32_0
	{
		public string familyId;

		internal bool _003CGetMatchingTrainerPackage_003Eb__0(ITrainerPackage p)
		{
			return p.BaseInfo.FamilyId == familyId;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CDoMigration_003Ed__15 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<System.Exception> _003C_003Et__builder;

		public HangarMigration _003C_003E4__this;

		public MigrationHarness harness;

		private System.Exception _003CmigrationError_003E5__2;

		private V1PersistedHangar _003CaircraftSettings_003E5__3;

		private ConfiguredTaskAwaiter<V1PersistedHangar> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<MigrationResult> _003C_003Eu__2;

		private System.Exception _003C_003E7__wrap3;

		private ConfiguredTaskAwaiter<System.Exception> _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			HangarMigration hangarMigration = _003C_003E4__this;
			System.Exception result4;
			try
			{
				ConfiguredTaskAwaiter<MigrationResult> awaiter2;
				ConfiguredTaskAwaiter<System.Exception> awaiter;
				System.Exception result;
				MigrationResult result3;
				switch (num)
				{
				default:
					_003CmigrationError_003E5__2 = null;
					if (!hangarMigration.HasOldHangarFile())
					{
						break;
					}
					harness.ReportCurrentStatus();
					_003CaircraftSettings_003E5__3 = null;
					goto case 0;
				case 0:
					try
					{
						ConfiguredTaskAwaiter<V1PersistedHangar> awaiter3;
						if (num != 0)
						{
							awaiter3 = hangarMigration.DeserializeHangarFile().DetachContext().GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter3;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<V1PersistedHangar>, _003CDoMigration_003Ed__15>(ref awaiter3, ref this);
								return;
							}
						}
						else
						{
							awaiter3 = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<V1PersistedHangar>);
							num = (_003C_003E1__state = -1);
						}
						V1PersistedHangar result2 = awaiter3.GetResult();
						_003CaircraftSettings_003E5__3 = result2;
					}
					catch (System.Exception next)
					{
						_003CmigrationError_003E5__2 = _003CmigrationError_003E5__2.BuildUpException(next, "Deserializing hangar file");
					}
					if (_003CaircraftSettings_003E5__3 != null)
					{
						awaiter2 = hangarMigration.MigrateAircraft(harness, _003CaircraftSettings_003E5__3).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<MigrationResult>, _003CDoMigration_003Ed__15>(ref awaiter2, ref this);
							return;
						}
						goto IL_014d;
					}
					goto IL_0217;
				case 1:
					awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<MigrationResult>);
					num = (_003C_003E1__state = -1);
					goto IL_014d;
				case 2:
					{
						awaiter = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter<System.Exception>);
						num = (_003C_003E1__state = -1);
						goto IL_01ee;
					}
					IL_01ee:
					result = awaiter.GetResult();
					_003CmigrationError_003E5__2 = _003C_003E7__wrap3.BuildUpException(result, "Migrating Selected Aircraft");
					_003C_003E7__wrap3 = null;
					goto IL_0217;
					IL_014d:
					result3 = awaiter2.GetResult();
					_003CmigrationError_003E5__2 = _003CmigrationError_003E5__2.BuildUpException(result3.Error, "Migrating Aircraft");
					_003C_003E7__wrap3 = _003CmigrationError_003E5__2;
					awaiter = hangarMigration.MigrateSelectedAircraft(harness, _003CaircraftSettings_003E5__3, result3).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__3 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Exception>, _003CDoMigration_003Ed__15>(ref awaiter, ref this);
						return;
					}
					goto IL_01ee;
					IL_0217:
					_003CaircraftSettings_003E5__3 = null;
					break;
				}
				UpdateMigrationStatus(harness, "Cleaning up old hangar files.");
				_003CmigrationError_003E5__2 = hangarMigration.MoveOrDeleteOldFile(_003CmigrationError_003E5__2);
				result4 = _003CmigrationError_003E5__2;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CmigrationError_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CmigrationError_003E5__2 = null;
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
	private struct _003CGetMatchingTrainerPackage_003Ed__32 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ITrainerPackage> _003C_003Et__builder;

		public string familyId;

		public HangarMigration _003C_003E4__this;

		private object _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			HangarMigration hangarMigration = _003C_003E4__this;
			ITrainerPackage result;
			try
			{
				AsyncSubject<ITrainerPackage> asyncSubject;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass32_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass32_0
					{
						familyId = familyId
					};
					asyncSubject = hangarMigration._packageRepo.Packages().FirstOrDefaultAsync<ITrainerPackage>((ITrainerPackage p) => p.BaseInfo.FamilyId == CS_0024_003C_003E8__locals0.familyId).GetAwaiter<ITrainerPackage>();
					if (!asyncSubject.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = asyncSubject;
						_003C_003Et__builder.AwaitOnCompleted<AsyncSubject<ITrainerPackage>, _003CGetMatchingTrainerPackage_003Ed__32>(ref asyncSubject, ref this);
						return;
					}
				}
				else
				{
					asyncSubject = (AsyncSubject<ITrainerPackage>)_003C_003Eu__1;
					_003C_003Eu__1 = null;
					num = (_003C_003E1__state = -1);
				}
				result = asyncSubject.GetResult();
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
	private struct _003CLoadAndStoreAircraft_003Ed__27 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ReactiveAircraft> _003C_003Et__builder;

		public V1PersistedAircraft oldAircraft;

		public HangarMigration _003C_003E4__this;

		private CreateAircraftHarness _003CcreateAircraftHarness_003E5__2;

		private ConfiguredTaskAwaiter<CreateAircraftHarness> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private ConfiguredTaskAwaiter<ReactiveAircraft> _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			HangarMigration hangarMigration = _003C_003E4__this;
			ReactiveAircraft result2;
			try
			{
				ConfiguredTaskAwaiter<CreateAircraftHarness> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 1u)
					{
						goto IL_009e;
					}
					AircraftListData listData = GetListData(oldAircraft);
					awaiter = CreateAircraftHarness.Start(hangarMigration._unitOfWork, listData).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<CreateAircraftHarness>, _003CLoadAndStoreAircraft_003Ed__27>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<CreateAircraftHarness>);
					num = (_003C_003E1__state = -1);
				}
				CreateAircraftHarness result = awaiter.GetResult();
				_003CcreateAircraftHarness_003E5__2 = result;
				goto IL_009e;
				IL_009e:
				try
				{
					ConfiguredTaskAwaiter<ReactiveAircraft> awaiter2;
					ConfiguredTaskAwaiter awaiter3;
					if (num != 1)
					{
						if (num == 2)
						{
							awaiter2 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter<ReactiveAircraft>);
							num = (_003C_003E1__state = -1);
							goto IL_0187;
						}
						ReactiveAircraft temporaryAircraft = _003CcreateAircraftHarness_003E5__2.TemporaryAircraft;
						ConfiguredTaskAwaitable val = hangarMigration.LoadSimulators(oldAircraft, temporaryAircraft).DetachContext();
						awaiter3 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter3)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter3;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CLoadAndStoreAircraft_003Ed__27>(ref awaiter3, ref this);
							return;
						}
					}
					else
					{
						awaiter3 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter3)).GetResult();
					awaiter2 = _003CcreateAircraftHarness_003E5__2.Confirm().DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__3 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ReactiveAircraft>, _003CLoadAndStoreAircraft_003Ed__27>(ref awaiter2, ref this);
						return;
					}
					goto IL_0187;
					IL_0187:
					result2 = awaiter2.GetResult();
				}
				finally
				{
					if (num < 0 && _003CcreateAircraftHarness_003E5__2 != null)
					{
						((System.IDisposable)_003CcreateAircraftHarness_003E5__2).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CcreateAircraftHarness_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CcreateAircraftHarness_003E5__2 = null;
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
	private struct _003CLoadOptions_003Ed__23 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveSimulator simInstance;

		public V1PersistedSimulator oldPersistedSim;

		private ISettingsBinder<ReactiveSimulator> _003Csettings_003E5__2;

		private System.Exception _003CoptionException_003E5__3;

		private ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>> _003C_003Eu__1;

		private System.Collections.Generic.IEnumerator<V1SelectedOption> _003C_003E7__wrap3;

		private V1SelectedOption _003ColdOption_003E5__5;

		private IActiveSetting _003CsimSetting_003E5__6;

		private ConfiguredTaskAwaiter<bool> _003C_003Eu__2;

		private ConfiguredTaskAwaiter<ISettingValueDefinition> _003C_003Eu__3;

		private ConfiguredTaskAwaiter _003C_003Eu__4;

		private void MoveNext()
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 2u)
					{
						goto IL_0081;
					}
					awaiter = simInstance.BindSettings().DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISettingsBinder<ReactiveSimulator>>, _003CLoadOptions_003Ed__23>(ref awaiter, ref this);
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
				_003Csettings_003E5__2 = result;
				goto IL_0081;
				IL_0081:
				try
				{
					if ((uint)(num - 1) > 1u)
					{
						if (num == 3)
						{
							goto IL_0361;
						}
						_003CoptionException_003E5__3 = null;
						_003C_003E7__wrap3 = Enumerable.Where<V1SelectedOption>((System.Collections.Generic.IEnumerable<V1SelectedOption>)oldPersistedSim.Options, (Func<V1SelectedOption, bool>)((V1SelectedOption o) => !string.IsNullOrEmpty(o.SelectedDisplayName) && o.SettingName != "InstanceID")).GetEnumerator();
					}
					try
					{
						ConfiguredTaskAwaiter<ISettingValueDefinition> awaiter2;
						if (num != 1)
						{
							if (num != 2)
							{
								goto IL_032f;
							}
							awaiter2 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter<ISettingValueDefinition>);
							num = (_003C_003E1__state = -1);
							goto IL_0228;
						}
						ConfiguredTaskAwaiter<bool> awaiter3 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<bool>);
						num = (_003C_003E1__state = -1);
						goto IL_01af;
						IL_01af:
						if (awaiter3.GetResult())
						{
							awaiter2 = _003CsimSetting_003E5__6.SelectByDisplayName(_003ColdOption_003E5__5.SelectedDisplayName).DetachContext().GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter2;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISettingValueDefinition>, _003CLoadOptions_003Ed__23>(ref awaiter2, ref this);
								return;
							}
							goto IL_0228;
						}
						string errorMessage = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Could not migrate '{0}' setting '{1}'. No matching option '{2}'.", (object)simInstance.AircraftAndSim.InstallationKey.ModelId, (object)_003ColdOption_003E5__5.SettingName, (object)_003ColdOption_003E5__5.SelectedDisplayName);
						_003CoptionException_003E5__3 = BuildUpSettingKeyNotFoundException(_003CoptionException_003E5__3, errorMessage);
						goto IL_02d6;
						IL_0228:
						awaiter2.GetResult();
						goto IL_02d6;
						IL_02d6:
						_003CsimSetting_003E5__6 = null;
						goto IL_0328;
						IL_032f:
						if (((System.Collections.IEnumerator)_003C_003E7__wrap3).MoveNext())
						{
							_003ColdOption_003E5__5 = _003C_003E7__wrap3.Current;
							string text = default(string);
							if (MigratedOptionsKey.TryGetValue(_003ColdOption_003E5__5.SettingName, ref text) && text != null)
							{
								_003CsimSetting_003E5__6 = _003Csettings_003E5__2.Table.GetByScopedIdentifier(text);
								if (_003CsimSetting_003E5__6 != null)
								{
									awaiter3 = _003CsimSetting_003E5__6.HasOptionWithDisplayName(_003ColdOption_003E5__5.SelectedDisplayName).DetachContext().GetAwaiter();
									if (!awaiter3.IsCompleted)
									{
										num = (_003C_003E1__state = 1);
										_003C_003Eu__2 = awaiter3;
										((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<bool>, _003CLoadOptions_003Ed__23>(ref awaiter3, ref this);
										return;
									}
									goto IL_01af;
								}
								string errorMessage2 = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Could not migrate '{0}' setting '{1}'. No matching setting '{2}'.", (object)simInstance.AircraftAndSim.InstallationKey.ModelId, (object)_003ColdOption_003E5__5.SettingName, (object)text);
								_003CoptionException_003E5__3 = BuildUpSettingKeyNotFoundException(_003CoptionException_003E5__3, errorMessage2);
								goto IL_02d6;
							}
							string errorMessage3 = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Unknown '{0}' setting '{1}'.", (object)simInstance.AircraftAndSim.InstallationKey.ModelId, (object)_003ColdOption_003E5__5.SettingName);
							_003CoptionException_003E5__3 = BuildUpSettingKeyNotFoundException(_003CoptionException_003E5__3, errorMessage3);
							goto IL_0328;
						}
						goto end_IL_00d0;
						IL_0328:
						_003ColdOption_003E5__5 = null;
						goto IL_032f;
						end_IL_00d0:;
					}
					finally
					{
						if (num < 0 && _003C_003E7__wrap3 != null)
						{
							((System.IDisposable)_003C_003E7__wrap3).Dispose();
						}
					}
					_003C_003E7__wrap3 = null;
					goto IL_0361;
					IL_0361:
					try
					{
						ConfiguredTaskAwaiter awaiter4;
						if (num != 3)
						{
							ConfiguredTaskAwaitable val = simInstance.UpdateSettings((IDictionary<SettingIdentifier, string>)(object)_003Csettings_003E5__2.Table.ToPersistenceDictionaryV2()).DetachContext();
							awaiter4 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter4)).IsCompleted)
							{
								num = (_003C_003E1__state = 3);
								_003C_003Eu__4 = awaiter4;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CLoadOptions_003Ed__23>(ref awaiter4, ref this);
								return;
							}
						}
						else
						{
							awaiter4 = _003C_003Eu__4;
							_003C_003Eu__4 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
						}
						((ConfiguredTaskAwaiter)(ref awaiter4)).GetResult();
					}
					catch (System.Exception next)
					{
						_003CoptionException_003E5__3 = _003CoptionException_003E5__3.BuildUpException(next);
					}
					if (_003CoptionException_003E5__3 != null)
					{
						throw _003CoptionException_003E5__3;
					}
				}
				finally
				{
					if (num < 0 && _003Csettings_003E5__2 != null)
					{
						((System.IDisposable)_003Csettings_003E5__2).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Csettings_003E5__2 = null;
				_003CoptionException_003E5__3 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Csettings_003E5__2 = null;
			_003CoptionException_003E5__3 = null;
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
	private struct _003CLoadSimulators_003Ed__31 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public V1PersistedAircraft oldAircraft;

		public HangarMigration _003C_003E4__this;

		public ReactiveAircraft newAircraft;

		private System.Exception _003CsimulatorException_003E5__2;

		private System.Collections.Generic.IEnumerator<V1PersistedSimulator> _003C_003E7__wrap2;

		private V1PersistedSimulator _003ColdSim_003E5__4;

		private string _003CfamilyId_003E5__5;

		private ConfiguredTaskAwaiter<ITrainerPackage> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<ReactiveSimulator> _003C_003Eu__2;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			HangarMigration hangarMigration = _003C_003E4__this;
			try
			{
				if ((uint)num > 2u)
				{
					_003CsimulatorException_003E5__2 = null;
					_003C_003E7__wrap2 = ((System.Collections.Generic.IEnumerable<V1PersistedSimulator>)Enumerable.OrderBy<V1PersistedSimulator, int>((System.Collections.Generic.IEnumerable<V1PersistedSimulator>)oldAircraft.Simulators, (Func<V1PersistedSimulator, int>)((V1PersistedSimulator s) => s.InstallId))).GetEnumerator();
				}
				try
				{
					if ((uint)num <= 2u)
					{
						goto IL_006f;
					}
					goto IL_02ef;
					IL_02ef:
					if (((System.Collections.IEnumerator)_003C_003E7__wrap2).MoveNext())
					{
						_003ColdSim_003E5__4 = _003C_003E7__wrap2.Current;
						goto IL_006f;
					}
					goto end_IL_0054;
					IL_006f:
					try
					{
						ConfiguredTaskAwaiter<ITrainerPackage> awaiter3;
						ITrainerPackage trainerPackage;
						ConfiguredTaskAwaiter<ReactiveSimulator> awaiter2;
						ConfiguredTaskAwaiter awaiter;
						ITrainerPackage trainerPackage2;
						ITrainerBaseInfo baseInfo;
						string modelId;
						TrainerModelKey completeModelKey;
						AvionicsType avionicsType;
						ReactiveSimulator result;
						ConfiguredTaskAwaitable val;
						switch (num)
						{
						default:
							_003CfamilyId_003E5__5 = GetFamilyIdFromModelName(_003ColdSim_003E5__4.ModelName);
							if (_003CfamilyId_003E5__5 != null)
							{
								awaiter3 = hangarMigration.GetMatchingTrainerPackage(_003CfamilyId_003E5__5).DetachContext().GetAwaiter();
								if (!awaiter3.IsCompleted)
								{
									num = (_003C_003E1__state = 0);
									_003C_003Eu__1 = awaiter3;
									((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003CLoadSimulators_003Ed__31>(ref awaiter3, ref this);
									return;
								}
								goto IL_0105;
							}
							trainerPackage = null;
							goto IL_0113;
						case 0:
							awaiter3 = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
							num = (_003C_003E1__state = -1);
							goto IL_0105;
						case 1:
							awaiter2 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ReactiveSimulator>);
							num = (_003C_003E1__state = -1);
							goto IL_0209;
						case 2:
							{
								awaiter = _003C_003Eu__3;
								_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
								num = (_003C_003E1__state = -1);
								break;
							}
							IL_0113:
							trainerPackage2 = trainerPackage;
							if (trainerPackage2 == null)
							{
								Logger.Log(LogLevel.Info, "No matching package for family id: '{0}'. May have been uninstalled during upgrade.", _003CfamilyId_003E5__5 ?? "NULL");
								goto IL_02ef;
							}
							baseInfo = trainerPackage2.BaseInfo;
							modelId = GetModelId(_003ColdSim_003E5__4.ModelName);
							if (modelId == null)
							{
								throw new FormatException("Unknown model name: " + _003ColdSim_003E5__4.ModelName);
							}
							completeModelKey = new TrainerModelKey(baseInfo.TrainerPackageId, modelId);
							avionicsType = GetAvionicsType(_003ColdSim_003E5__4.Category);
							awaiter2 = newAircraft.Install(avionicsType, completeModelKey, _003ColdSim_003E5__4.InstallId).DetachContext().GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter2;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ReactiveSimulator>, _003CLoadSimulators_003Ed__31>(ref awaiter2, ref this);
								return;
							}
							goto IL_0209;
							IL_0209:
							result = awaiter2.GetResult();
							val = LoadOptions(_003ColdSim_003E5__4, result).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CLoadSimulators_003Ed__31>(ref awaiter, ref this);
								return;
							}
							break;
							IL_0105:
							trainerPackage = awaiter3.GetResult();
							goto IL_0113;
						}
						((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
						_003CfamilyId_003E5__5 = null;
					}
					catch (System.Exception next)
					{
						string topLevelAggMessage = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Migrating {0} simulator: {1} {2}.", (object)newAircraft.AircraftIdentifier, (object)(_003ColdSim_003E5__4.ModelName ?? "No model name"), (object)_003ColdSim_003E5__4.InstallId);
						_003CsimulatorException_003E5__2 = _003CsimulatorException_003E5__2.BuildUpException(next, topLevelAggMessage);
					}
					_003ColdSim_003E5__4 = null;
					goto IL_02ef;
					end_IL_0054:;
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						((System.IDisposable)_003C_003E7__wrap2).Dispose();
					}
				}
				_003C_003E7__wrap2 = null;
				if (_003CsimulatorException_003E5__2 != null)
				{
					throw _003CsimulatorException_003E5__2;
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CsimulatorException_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CsimulatorException_003E5__2 = null;
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
	private struct _003CMigrateAircraft_003Ed__28 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<MigrationResult> _003C_003Et__builder;

		public V1PersistedHangar aircraftSettings;

		public MigrationHarness harness;

		public HangarMigration _003C_003E4__this;

		private System.Exception _003CmigrationError_003E5__2;

		private Dictionary<V1PersistedAircraft, AircraftSnapshotData> _003Cmigrated_003E5__3;

		private System.Collections.Generic.IEnumerator<V1PersistedAircraft> _003C_003E7__wrap3;

		private V1PersistedAircraft _003ColdAircraft_003E5__5;

		private ConfiguredTaskAwaiter<ReactiveAircraft> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			HangarMigration hangarMigration = _003C_003E4__this;
			MigrationResult result2;
			try
			{
				if (num != 0)
				{
					_003CmigrationError_003E5__2 = null;
					_003Cmigrated_003E5__3 = new Dictionary<V1PersistedAircraft, AircraftSnapshotData>();
					if (aircraftSettings.Aircraft == null)
					{
						goto IL_017d;
					}
					_003C_003E7__wrap3 = ((System.Collections.Generic.IEnumerable<V1PersistedAircraft>)aircraftSettings.Aircraft).GetEnumerator();
				}
				try
				{
					if (num == 0)
					{
						goto IL_0089;
					}
					goto IL_014c;
					IL_014c:
					if (((System.Collections.IEnumerator)_003C_003E7__wrap3).MoveNext())
					{
						_003ColdAircraft_003E5__5 = _003C_003E7__wrap3.Current;
						UpdateMigrationStatus(harness, "Migrating " + _003ColdAircraft_003E5__5.AircraftName + " avionics and settings.");
						goto IL_0089;
					}
					goto end_IL_004a;
					IL_0089:
					try
					{
						ConfiguredTaskAwaiter<ReactiveAircraft> awaiter;
						if (num != 0)
						{
							awaiter = hangarMigration.LoadAndStoreAircraft(_003ColdAircraft_003E5__5).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ReactiveAircraft>, _003CMigrateAircraft_003Ed__28>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ReactiveAircraft>);
							num = (_003C_003E1__state = -1);
						}
						ReactiveAircraft result = awaiter.GetResult();
						_003Cmigrated_003E5__3[_003ColdAircraft_003E5__5] = result.CompositeSnapshots.CachedValue;
					}
					catch (System.Exception next)
					{
						_003CmigrationError_003E5__2 = _003CmigrationError_003E5__2.BuildUpException(next, "Migrating " + _003ColdAircraft_003E5__5.AircraftName, wrapNextInAgg: true);
					}
					_003ColdAircraft_003E5__5 = null;
					goto IL_014c;
					end_IL_004a:;
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap3 != null)
					{
						((System.IDisposable)_003C_003E7__wrap3).Dispose();
					}
				}
				_003C_003E7__wrap3 = null;
				goto IL_017d;
				IL_017d:
				result2 = new MigrationResult(_003Cmigrated_003E5__3, _003CmigrationError_003E5__2);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CmigrationError_003E5__2 = null;
				_003Cmigrated_003E5__3 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CmigrationError_003E5__2 = null;
			_003Cmigrated_003E5__3 = null;
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
	private struct _003CMigrateSelectedAircraft_003Ed__29 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<System.Exception> _003C_003Et__builder;

		public V1PersistedHangar aircraftSettings;

		public MigrationResult migrationResult;

		public MigrationHarness harness;

		public HangarMigration _003C_003E4__this;

		private System.Exception _003CmigrationError_003E5__2;

		private AircraftIdentifier _003CselectedAircraftIdentifier_003E5__3;

		private ConfiguredTaskAwaiter<AircraftListData> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Expected O, but got Unknown
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Expected O, but got Unknown
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			HangarMigration hangarMigration = _003C_003E4__this;
			System.Exception result;
			try
			{
				if ((uint)num > 1u)
				{
					_003CmigrationError_003E5__2 = null;
					_003CselectedAircraftIdentifier_003E5__3 = GetNewIdentifierForMigratedAircraft(aircraftSettings.SelectedAircraft, migrationResult);
					if (!_003CselectedAircraftIdentifier_003E5__3.IsValid)
					{
						if (((System.Collections.Generic.ICollection<V1PersistedAircraft>)aircraftSettings.Aircraft).Count > 0)
						{
							_003CselectedAircraftIdentifier_003E5__3 = GetAircraftIdentifier(aircraftSettings.Aircraft[0].AircraftName);
							if (!_003CselectedAircraftIdentifier_003E5__3.IsValid)
							{
								_003CmigrationError_003E5__2 = _003CmigrationError_003E5__2.BuildUpException((System.Exception?)new KeyNotFoundException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Could not migrate default aircraft as selected aircraft. No aircraft matching '{0}'.", (object)aircraftSettings.Aircraft[0].AircraftName)), "Migrating selected aircraft");
							}
						}
						else
						{
							Logger.Log(LogLevel.Info, "Could not migrate selected aircraft. Nothing was selected before and there are no aircraft.");
						}
						goto IL_0240;
					}
					UpdateMigrationStatus(harness, "Migrating selected aircraft.");
				}
				try
				{
					_ = 1;
					try
					{
						ConfiguredTaskAwaiter awaiter;
						ConfiguredTaskAwaiter<AircraftListData> awaiter2;
						if (num != 0)
						{
							if (num == 1)
							{
								awaiter = _003C_003Eu__2;
								_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
								num = (_003C_003E1__state = -1);
								goto IL_014a;
							}
							awaiter2 = hangarMigration._unitOfWork.Aircraft.GetAircraftListDataById(_003CselectedAircraftIdentifier_003E5__3).DetachContext().GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftListData>, _003CMigrateSelectedAircraft_003Ed__29>(ref awaiter2, ref this);
								return;
							}
						}
						else
						{
							awaiter2 = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftListData>);
							num = (_003C_003E1__state = -1);
						}
						awaiter2.GetResult();
						ConfiguredTaskAwaitable val = hangarMigration._unitOfWork.Aircraft.StoreLastSelectedAircraft(_003CselectedAircraftIdentifier_003E5__3).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CMigrateSelectedAircraft_003Ed__29>(ref awaiter, ref this);
							return;
						}
						goto IL_014a;
						IL_014a:
						((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					}
					catch (AircraftNotFoundException)
					{
						_003CmigrationError_003E5__2 = _003CmigrationError_003E5__2.BuildUpException((System.Exception?)new KeyNotFoundException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Could not migrate selected aircraft. No aircraft matching '{0}'.", (object)_003CselectedAircraftIdentifier_003E5__3)), "Migrating selected aircraft");
					}
				}
				catch (System.Exception next)
				{
					_003CmigrationError_003E5__2 = _003CmigrationError_003E5__2.BuildUpException(next, "Migrating selected aircraft");
				}
				goto IL_0240;
				IL_0240:
				result = _003CmigrationError_003E5__2;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CmigrationError_003E5__2 = null;
				_003CselectedAircraftIdentifier_003E5__3 = default(AircraftIdentifier);
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CmigrationError_003E5__2 = null;
			_003CselectedAircraftIdentifier_003E5__3 = default(AircraftIdentifier);
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	public const string PREV_HANGAR_FILE_NAME = "hangar.json";

	public const string SINGLE_ENGINE_AID = "NGSEP1";

	public const string MULTI_ENGINE_AID = "NGMEP1";

	public static readonly IReadOnlyDictionary<string, string> AircraftIdentifierTranslation;

	public static readonly IReadOnlyDictionary<int, AvionicsType> MigratedType;

	public static readonly IReadOnlyDictionary<string, string> FamilyId;

	public static readonly IReadOnlyDictionary<string, string> MigratedOptionsKey;

	private readonly IFileSystem _fs;

	private readonly IDeploymentPathProvider _pathProvider;

	private readonly IPersistArbitraryObjects _persistence;

	private readonly ITrainerPackageRepoEvolved _packageRepo;

	private readonly IUnitOfWork _unitOfWork;

	public HangarMigration(IDeploymentPathProvider pathProvider, IFileSystem fs, IPersistArbitraryObjects persist, IUnitOfWork unitofWork)
		: base("V1 Hangar Migration")
	{
		_pathProvider = pathProvider;
		_fs = fs;
		_persistence = persist;
		_unitOfWork = unitofWork;
		_packageRepo = unitofWork.Packages;
	}

	public bool HasOldHangarFile()
	{
		string oldHangarPath = GetOldHangarPath();
		return _fs.PathExists(oldHangarPath);
	}

	public override System.Threading.Tasks.Task<bool> IsMigrationRequired()
	{
		return System.Threading.Tasks.Task.FromResult<bool>(HasOldHangarFile());
	}

	[AsyncStateMachine(typeof(_003CDoMigration_003Ed__15))]
	protected override async System.Threading.Tasks.Task<System.Exception?> DoMigration(MigrationHarness harness)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		System.Exception migrationError = null;
		if (HasOldHangarFile())
		{
			harness.ReportCurrentStatus();
			V1PersistedHangar aircraftSettings = null;
			try
			{
				aircraftSettings = await DeserializeHangarFile().DetachContext();
			}
			catch (System.Exception next)
			{
				migrationError = migrationError.BuildUpException(next, "Deserializing hangar file");
			}
			if (aircraftSettings != null)
			{
				MigrationResult migrationResult = await MigrateAircraft(harness, aircraftSettings).DetachContext();
				migrationError = migrationError.BuildUpException(migrationResult.Error, "Migrating Aircraft");
				System.Exception root = migrationError;
				migrationError = root.BuildUpException(await MigrateSelectedAircraft(harness, aircraftSettings, migrationResult).DetachContext(), "Migrating Selected Aircraft");
			}
		}
		UpdateMigrationStatus(harness, "Cleaning up old hangar files.");
		return MoveOrDeleteOldFile(migrationError);
	}

	private static System.Exception? BuildUpSettingKeyNotFoundException(System.Exception? optionException, string errorMessage)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		return optionException.BuildUpException((System.Exception?)new KeyNotFoundException(errorMessage), "Migrating Options");
	}

	private static string? GetFamilyIdFromModelName(string? modelName)
	{
		if (modelName != null)
		{
			string[] array = modelName.Split(System.Array.Empty<char>());
			string result = default(string);
			if (array.Length != 0 && FamilyId.TryGetValue(array[0], ref result))
			{
				return result;
			}
		}
		return null;
	}

	private static void UpdateMigrationStatus(MigrationHarness harness, string message)
	{
		harness.CurrentStatus = harness.CurrentStatus.ChangeMessage(message);
		harness.ReportCurrentStatus();
	}

	private static AircraftIdentifier GetAircraftIdentifier(string oldSelectedAircraft)
	{
		string text = default(string);
		if (oldSelectedAircraft != null && AircraftIdentifierTranslation.TryGetValue(oldSelectedAircraft, ref text))
		{
			return new AircraftIdentifier(text);
		}
		return AircraftIdentifier.Default;
	}

	private static AircraftIdentifier GetNewIdentifierForMigratedAircraft(string? oldSelectedAircraft, MigrationResult migrationResult)
	{
		string oldSelectedAircraft2 = oldSelectedAircraft;
		if (oldSelectedAircraft2 != null)
		{
			AircraftSnapshotData aircraftSnapshotData = Enumerable.FirstOrDefault<AircraftSnapshotData>((System.Collections.Generic.IEnumerable<AircraftSnapshotData>)migrationResult.MigratedAircraft.Values, (Func<AircraftSnapshotData, bool>)((AircraftSnapshotData a) => a.ListData.DisplayName == oldSelectedAircraft2));
			if (aircraftSnapshotData != null)
			{
				return aircraftSnapshotData.ListData.AircraftIdentifier;
			}
		}
		return AircraftIdentifier.Default;
	}

	private static string? GetModelId(string modelName)
	{
		string[] array = modelName?.Split(System.Array.Empty<char>());
		if (array == null || array.Length < 2)
		{
			return null;
		}
		return array[1];
	}

	private static AvionicsType GetAvionicsType(int categoryNum)
	{
		AvionicsType result = default(AvionicsType);
		if (!MigratedType.TryGetValue(categoryNum, ref result))
		{
			return AvionicsType.Unknown;
		}
		return result;
	}

	[AsyncStateMachine(typeof(_003CLoadOptions_003Ed__23))]
	private static System.Threading.Tasks.Task LoadOptions(V1PersistedSimulator oldPersistedSim, ReactiveSimulator simInstance)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CLoadOptions_003Ed__23 _003CLoadOptions_003Ed__ = default(_003CLoadOptions_003Ed__23);
		_003CLoadOptions_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CLoadOptions_003Ed__.oldPersistedSim = oldPersistedSim;
		_003CLoadOptions_003Ed__.simInstance = simInstance;
		_003CLoadOptions_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CLoadOptions_003Ed__._003C_003Et__builder)).Start<_003CLoadOptions_003Ed__23>(ref _003CLoadOptions_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CLoadOptions_003Ed__._003C_003Et__builder)).Task;
	}

	private static AircraftListData GetListData(V1PersistedAircraft oldAircraft)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		AircraftIdentifier aircraftIdentifier = GetAircraftIdentifier(oldAircraft.AircraftName);
		if (!aircraftIdentifier.IsValid)
		{
			throw new KeyNotFoundException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Unknown previous aircraft name: '{0}'.", (object)(oldAircraft.AircraftName ?? "NULL")));
		}
		return new AircraftListData(aircraftIdentifier, oldAircraft.AircraftName, string.Empty, ((string?)aircraftIdentifier == "NGSEP1") ? AirframeCategoryClassType.SingleEngine : AirframeCategoryClassType.MultiEngine);
	}

	private System.Exception? MoveOrDeleteOldFile(System.Exception? migrationError)
	{
		try
		{
			if (migrationError != null)
			{
				Logger.Log(LogLevel.Info, "Renaming the old hangar file so that it does not continue to be migrated and fail.");
				MoveOldHangarFile();
			}
			else
			{
				RemoveOldHangarFile();
			}
		}
		catch (System.Exception next)
		{
			migrationError = migrationError.BuildUpException(next, "Handling previous hangar file");
		}
		return migrationError;
	}

	private void MoveOldHangarFile()
	{
		string oldHangarPath = GetOldHangarPath();
		string destination = oldHangarPath + ".old";
		_fs.MoveAndOverwrite(oldHangarPath, destination);
	}

	[AsyncStateMachine(typeof(_003CLoadAndStoreAircraft_003Ed__27))]
	private async System.Threading.Tasks.Task<ReactiveAircraft> LoadAndStoreAircraft(V1PersistedAircraft oldAircraft)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AircraftListData listData = GetListData(oldAircraft);
		using CreateAircraftHarness createAircraftHarness = await CreateAircraftHarness.Start(_unitOfWork, listData).DetachContext();
		ReactiveAircraft temporaryAircraft = createAircraftHarness.TemporaryAircraft;
		await LoadSimulators(oldAircraft, temporaryAircraft).DetachContext();
		return await createAircraftHarness.Confirm().DetachContext();
	}

	[AsyncStateMachine(typeof(_003CMigrateAircraft_003Ed__28))]
	private async System.Threading.Tasks.Task<MigrationResult> MigrateAircraft(MigrationHarness harness, V1PersistedHangar aircraftSettings)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		System.Exception migrationError = null;
		Dictionary<V1PersistedAircraft, AircraftSnapshotData> migrated = new Dictionary<V1PersistedAircraft, AircraftSnapshotData>();
		if (aircraftSettings.Aircraft != null)
		{
			System.Collections.Generic.IEnumerator<V1PersistedAircraft> enumerator = ((System.Collections.Generic.IEnumerable<V1PersistedAircraft>)aircraftSettings.Aircraft).GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					V1PersistedAircraft oldAircraft = enumerator.Current;
					UpdateMigrationStatus(harness, "Migrating " + oldAircraft.AircraftName + " avionics and settings.");
					try
					{
						migrated[oldAircraft] = (await LoadAndStoreAircraft(oldAircraft).DetachContext()).CompositeSnapshots.CachedValue;
					}
					catch (System.Exception next)
					{
						migrationError = migrationError.BuildUpException(next, "Migrating " + oldAircraft.AircraftName, wrapNextInAgg: true);
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		}
		return new MigrationResult(migrated, migrationError);
	}

	[AsyncStateMachine(typeof(_003CMigrateSelectedAircraft_003Ed__29))]
	private async System.Threading.Tasks.Task<System.Exception?> MigrateSelectedAircraft(MigrationHarness harness, V1PersistedHangar aircraftSettings, MigrationResult migrationResult)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		System.Exception migrationError = null;
		AircraftIdentifier selectedAircraftIdentifier = GetNewIdentifierForMigratedAircraft(aircraftSettings.SelectedAircraft, migrationResult);
		if (selectedAircraftIdentifier.IsValid)
		{
			UpdateMigrationStatus(harness, "Migrating selected aircraft.");
			try
			{
				_ = 1;
				try
				{
					await _unitOfWork.Aircraft.GetAircraftListDataById(selectedAircraftIdentifier).DetachContext();
					await _unitOfWork.Aircraft.StoreLastSelectedAircraft(selectedAircraftIdentifier).DetachContext();
				}
				catch (AircraftNotFoundException)
				{
					migrationError = migrationError.BuildUpException((System.Exception?)new KeyNotFoundException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Could not migrate selected aircraft. No aircraft matching '{0}'.", (object)selectedAircraftIdentifier)), "Migrating selected aircraft");
				}
			}
			catch (System.Exception next)
			{
				migrationError = migrationError.BuildUpException(next, "Migrating selected aircraft");
			}
		}
		else if (((System.Collections.Generic.ICollection<V1PersistedAircraft>)aircraftSettings.Aircraft).Count > 0)
		{
			if (!GetAircraftIdentifier(aircraftSettings.Aircraft[0].AircraftName).IsValid)
			{
				migrationError = migrationError.BuildUpException((System.Exception?)new KeyNotFoundException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Could not migrate default aircraft as selected aircraft. No aircraft matching '{0}'.", (object)aircraftSettings.Aircraft[0].AircraftName)), "Migrating selected aircraft");
			}
		}
		else
		{
			Logger.Log(LogLevel.Info, "Could not migrate selected aircraft. Nothing was selected before and there are no aircraft.");
		}
		return migrationError;
	}

	private void RemoveOldHangarFile()
	{
		_fs.DeleteIfExists(GetOldHangarPath());
	}

	[AsyncStateMachine(typeof(_003CLoadSimulators_003Ed__31))]
	private System.Threading.Tasks.Task LoadSimulators(V1PersistedAircraft oldAircraft, ReactiveAircraft newAircraft)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CLoadSimulators_003Ed__31 _003CLoadSimulators_003Ed__ = default(_003CLoadSimulators_003Ed__31);
		_003CLoadSimulators_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CLoadSimulators_003Ed__._003C_003E4__this = this;
		_003CLoadSimulators_003Ed__.oldAircraft = oldAircraft;
		_003CLoadSimulators_003Ed__.newAircraft = newAircraft;
		_003CLoadSimulators_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CLoadSimulators_003Ed__._003C_003Et__builder)).Start<_003CLoadSimulators_003Ed__31>(ref _003CLoadSimulators_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CLoadSimulators_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CGetMatchingTrainerPackage_003Ed__32))]
	private async System.Threading.Tasks.Task<ITrainerPackage?> GetMatchingTrainerPackage(string familyId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string familyId2 = familyId;
		return await _packageRepo.Packages().FirstOrDefaultAsync<ITrainerPackage>((ITrainerPackage p) => p.BaseInfo.FamilyId == familyId2);
	}

	private System.Threading.Tasks.Task<V1PersistedHangar> DeserializeHangarFile()
	{
		return _persistence.Load<V1PersistedHangar>(GetOldHangarPath());
	}

	private string GetOldHangarPath()
	{
		return Path.Combine(_pathProvider.DeploymentPath, "hangar.json");
	}

	static HangarMigration()
	{
		Dictionary<string, string> obj = new Dictionary<string, string>();
		obj.Add("Single-Engine", "NGSEP1");
		obj.Add("Multi-Engine", "NGMEP1");
		AircraftIdentifierTranslation = (IReadOnlyDictionary<string, string>)(object)obj;
		Dictionary<int, AvionicsType> obj2 = new Dictionary<int, AvionicsType>();
		obj2.Add(1, AvionicsType.Navigator);
		obj2.Add(2, AvionicsType.Display);
		MigratedType = (IReadOnlyDictionary<int, AvionicsType>)(object)obj2;
		Dictionary<string, string> obj3 = new Dictionary<string, string>();
		obj3.Add("GTN", "GTN");
		obj3.Add("GDU", "TXi");
		FamilyId = (IReadOnlyDictionary<string, string>)(object)obj3;
		Dictionary<string, string> obj4 = new Dictionary<string, string>();
		obj4.Add("Layout", "Layout");
		obj4.Add("PFD Alignment", "PfdAlignment");
		obj4.Add("EIS Alignment", "EisAlignment");
		MigratedOptionsKey = (IReadOnlyDictionary<string, string>)(object)obj4;
	}
}
