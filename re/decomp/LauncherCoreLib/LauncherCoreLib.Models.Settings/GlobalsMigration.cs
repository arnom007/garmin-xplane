using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Tasks;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Persistence;
using LauncherCoreLib.Models.Persistence.Models;

namespace LauncherCoreLib.Models.Settings;

internal class GlobalsMigration : IGlobalsMigration
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CMigrateGlobalsFile_003Ed__17 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public GlobalsMigration _003C_003E4__this;

		private ConfiguredTaskAwaiter<V1PersistedGlobals> _003C_003Eu__1;

		private System.Collections.Generic.IEnumerator<V1SelectedOption> _003C_003E7__wrap1;

		private ConfiguredTaskAwaiter<ISettingValueDefinition> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_009d: Expected O, but got Unknown
			//IL_00ae: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			GlobalsMigration globalsMigration = _003C_003E4__this;
			try
			{
				if (num == 0)
				{
					goto IL_0024;
				}
				if (num == 1)
				{
					goto IL_00c8;
				}
				if (globalsMigration.HasOldGlobalsFile())
				{
					goto IL_0024;
				}
				goto end_IL_000e;
				IL_0024:
				V1PersistedGlobals result;
				try
				{
					ConfiguredTaskAwaiter<V1PersistedGlobals> awaiter;
					if (num != 0)
					{
						awaiter = globalsMigration.DeserializeGlobalsFile().DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<V1PersistedGlobals>, _003CMigrateGlobalsFile_003Ed__17>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<V1PersistedGlobals>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter.GetResult();
				}
				catch (IOException val)
				{
					Logger.LogFormattedException((System.Exception)val, "Could not open the old globals file.");
					throw;
				}
				catch (SerializationException val2)
				{
					Logger.LogFormattedException((System.Exception)val2, "Could not deserialize the old globals file. Removing it because it is corrupt.");
					globalsMigration.RemoveOldGlobalsFile();
					throw;
				}
				_003C_003E7__wrap1 = ((System.Collections.Generic.IEnumerable<V1SelectedOption>)result.GlobalSettings).GetEnumerator();
				goto IL_00c8;
				IL_00c8:
				try
				{
					if (num != 1)
					{
						goto IL_020a;
					}
					ConfiguredTaskAwaiter<ISettingValueDefinition> awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ISettingValueDefinition>);
					num = (_003C_003E1__state = -1);
					goto IL_0202;
					IL_0202:
					awaiter2.GetResult();
					goto IL_020a;
					IL_020a:
					while (((System.Collections.IEnumerator)_003C_003E7__wrap1).MoveNext())
					{
						V1SelectedOption current = _003C_003E7__wrap1.Current;
						if (current.SettingName == null)
						{
							Logger.Log(LogLevel.Error, "A setting has no 'SettingName' property. Cannot migrate it.");
							continue;
						}
						string migratedKey = GetMigratedKey(current.SettingName);
						if (migratedKey == null)
						{
							Logger.Log(LogLevel.Error, "No known migration for setting: '{0}'.", current.SettingName);
							continue;
						}
						string migratedSettingValue = GetMigratedSettingValue(current.SettingName, current.SelectedDisplayName);
						if (migratedSettingValue == null)
						{
							Logger.Log(LogLevel.Error, "No known migration for setting: '{0}', value: '{1}'.", current.SettingName, current.SelectedDisplayName);
							continue;
						}
						IActiveSetting setting = globalsMigration._modifiableGlobals.GetSetting(migratedKey);
						if (setting == null)
						{
							Logger.Log(LogLevel.Error, "Could not find the runtime setting for '{0}'.", migratedKey);
							continue;
						}
						awaiter2 = setting.SelectBySimulatorValue(migratedSettingValue).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISettingValueDefinition>, _003CMigrateGlobalsFile_003Ed__17>(ref awaiter2, ref this);
							return;
						}
						goto IL_0202;
					}
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap1 != null)
					{
						((System.IDisposable)_003C_003E7__wrap1).Dispose();
					}
				}
				_003C_003E7__wrap1 = null;
				globalsMigration.SaveSettings();
				globalsMigration.RemoveOldGlobalsFile();
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

	public const string CRASH_REPORT_PREF_NAME = "When a crash occurs";

	public const string NEW_CRASH_REPORT_PREF_NAME = "CrashSendUserPref";

	public const string NAV_DB_PREF_NAME = "Navigation Database";

	public const string NEW_NAV_DB_PREF_NAME = "NavigationDatabase";

	public const string GLOBAL_SETTINGS_PERSISTENCE_FILE_NAME = "globals.json";

	public const string LAUNCHER_DIR_NAME = "Launcher";

	public static readonly Dictionary<string, string> CrashReportPref;

	public static readonly Dictionary<string, string> NavDBPref;

	private readonly IModifiableGlobalSettingsTable _modifiableGlobals;

	private readonly IGlobalSettingsPersistence _globalPersistence;

	private readonly IDeploymentPathProvider _pathProvider;

	private readonly IFileSystem _fileSystem;

	private readonly IPersistArbitraryObjects _persistence;

	public GlobalsMigration(IModifiableGlobalSettingsTable globals, IGlobalSettingsPersistence globalSettingsPersistence, IDeploymentPathProvider pathProvider, IFileSystem fs, IPersistArbitraryObjects persist)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		_modifiableGlobals = globals ?? throw new ArgumentNullException("globals");
		_globalPersistence = globalSettingsPersistence ?? throw new ArgumentNullException("globalSettingsPersistence");
		_pathProvider = pathProvider ?? throw new ArgumentNullException("pathProvider");
		_fileSystem = fs ?? throw new ArgumentNullException("fs");
		_persistence = persist ?? throw new ArgumentNullException("persist");
	}

	public static string? GetMigratedKey(string keyName)
	{
		if (keyName == "When a crash occurs")
		{
			return "CrashSendUserPref";
		}
		if (keyName == "Navigation Database")
		{
			return "NavigationDatabase";
		}
		return null;
	}

	public static string? GetMigratedSettingValue(string selectedKey, string selectedValue)
	{
		if (selectedKey == "When a crash occurs")
		{
			return GetMigratedCrashRptValue(selectedValue);
		}
		if (selectedKey == "Navigation Database")
		{
			return GetMigratedNavDb(selectedValue);
		}
		return null;
	}

	public bool HasOldGlobalsFile()
	{
		string path = GlobalSettingsPath();
		return _fileSystem.PathExists(path);
	}

	[AsyncStateMachine(typeof(_003CMigrateGlobalsFile_003Ed__17))]
	public System.Threading.Tasks.Task MigrateGlobalsFile()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CMigrateGlobalsFile_003Ed__17 _003CMigrateGlobalsFile_003Ed__ = default(_003CMigrateGlobalsFile_003Ed__17);
		_003CMigrateGlobalsFile_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CMigrateGlobalsFile_003Ed__._003C_003E4__this = this;
		_003CMigrateGlobalsFile_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CMigrateGlobalsFile_003Ed__._003C_003Et__builder)).Start<_003CMigrateGlobalsFile_003Ed__17>(ref _003CMigrateGlobalsFile_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CMigrateGlobalsFile_003Ed__._003C_003Et__builder)).Task;
	}

	private static string? GetMigratedCrashRptValue(string selectedValue)
	{
		string result = default(string);
		if (!CrashReportPref.TryGetValue(selectedValue, ref result))
		{
			return null;
		}
		return result;
	}

	private static string? GetMigratedNavDb(string selectedValue)
	{
		string result = default(string);
		if (!NavDBPref.TryGetValue(selectedValue, ref result))
		{
			return null;
		}
		return result;
	}

	private string GlobalSettingsPath()
	{
		return Path.Combine(_pathProvider.DeploymentPath, "globals.json");
	}

	private void SaveSettings()
	{
		try
		{
			_modifiableGlobals.SaveAllSettings(_globalPersistence);
		}
		catch (System.Exception e)
		{
			Logger.Log(e, "Could not save migrated global settings");
			throw;
		}
	}

	private System.Threading.Tasks.Task<V1PersistedGlobals> DeserializeGlobalsFile()
	{
		return _persistence.Load<V1PersistedGlobals>(GlobalSettingsPath());
	}

	private void RemoveOldGlobalsFile()
	{
		_fileSystem.DeleteIfExists(GlobalSettingsPath());
	}

	static GlobalsMigration()
	{
		Dictionary<string, string> obj = new Dictionary<string, string>();
		obj.Add("Always ask before sending a crash report", "AlwaysAsk");
		obj.Add("Always send crash reports", "AlwaysSend");
		obj.Add("Never send crash reports", "NeverSend");
		CrashReportPref = obj;
		Dictionary<string, string> obj2 = new Dictionary<string, string>();
		obj2.Add("Garmin Navigation Database (North America)", "Garmin");
		obj2.Add("Jeppesen Navigation Database (Worldwide)", "Jeppesen");
		NavDBPref = obj2;
	}
}
