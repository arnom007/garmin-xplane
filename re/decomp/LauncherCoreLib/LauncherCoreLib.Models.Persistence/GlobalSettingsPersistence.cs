using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LOLA.FileSystem;
using LauncherCoreLib.Models.Persistence.Models;
using LauncherCoreLib.Models.Settings;

namespace LauncherCoreLib.Models.Persistence;

public class GlobalSettingsPersistence : PersistenceBase<PersistedGlobals>, IGlobalSettingsPersistence, IPersistence
{
	public const string GLOBAL_SETTINGS_PERSISTENCE_FILE = "globals";

	private PersistedGlobals CurrentGlobals => base.Container ?? throw new InvalidOperationException("The persistence container is null.");

	public GlobalSettingsPersistence(IPersistencePathProvider pathProvider, IFileSystem fileSystem)
		: base(pathProvider, fileSystem, "globals")
	{
	}

	public void Store(IActiveSetting setting)
	{
		ThrowIfSettingIsNull(setting);
		lock (base.FileMutex)
		{
			CurrentGlobals.UpdatePersisted(setting);
		}
	}

	public void Store(System.Collections.Generic.IEnumerable<IActiveSetting> settings)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (settings == null)
		{
			throw new ArgumentNullException("settings");
		}
		lock (base.FileMutex)
		{
			System.Collections.Generic.IEnumerator<IActiveSetting> enumerator = Enumerable.Where<IActiveSetting>(settings, (Func<IActiveSetting, bool>)((IActiveSetting s) => s != null)).GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					IActiveSetting current = enumerator.Current;
					CurrentGlobals.UpdatePersisted(current);
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		}
	}

	public bool Hydrate(IActiveSetting setting)
	{
		ThrowIfSettingIsNull(setting);
		string persistedSetting = GetPersistedSetting(setting);
		if (persistedSetting != null)
		{
			setting.CurrentValue = persistedSetting;
			return true;
		}
		return false;
	}

	public void Clear(SettingIdentifier settingIdentifer)
	{
		lock (base.FileMutex)
		{
			CurrentGlobals.GlobalSettings.Remove((string)settingIdentifer);
		}
	}

	protected override PersistedGlobals CreateDefaultContainer()
	{
		return new PersistedGlobals();
	}

	private static void ThrowIfSettingIsNull(IActiveSetting setting)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (setting == null)
		{
			throw new ArgumentNullException("setting");
		}
	}

	private string? GetPersistedSetting(IActiveSetting setting)
	{
		lock (base.FileMutex)
		{
			string result = default(string);
			CurrentGlobals.GlobalSettings.TryGetValue((string)setting.LocalId, ref result);
			return result;
		}
	}
}
