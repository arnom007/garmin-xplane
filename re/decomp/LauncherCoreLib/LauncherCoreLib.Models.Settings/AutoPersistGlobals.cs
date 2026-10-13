using System;
using System.Collections;
using System.Collections.Generic;
using LauncherCoreLib.Models.Persistence;

namespace LauncherCoreLib.Models.Settings;

public class AutoPersistGlobals : IModifiableGlobalSettingsTable, IGlobalSettingsProvider, ISettingsProvider, IModifiableSettingSymbolTable, ISettingSymbolTable, System.IDisposable
{
	private readonly IGlobalSettingsProvider _actualSettings;

	private readonly IModifiableGlobalSettingsTable _modifiableGlobals;

	private readonly IGlobalSettingsPersistence _persistence;

	private bool _disposed;

	public System.Collections.Generic.IEnumerable<IActiveSetting> Settings => _actualSettings.Settings;

	public string SettingsName => _actualSettings.SettingsName;

	public System.Collections.Generic.IEnumerable<IActiveSetting> AllSettings => _modifiableGlobals.AllSettings;

	public AutoPersistGlobals(IGlobalSettingsProvider globals, IGlobalSettingsPersistence persistence)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		_actualSettings = globals ?? throw new ArgumentNullException("globals");
		_persistence = persistence ?? throw new ArgumentNullException("persistence");
		_modifiableGlobals = (globals as IModifiableGlobalSettingsTable) ?? throw new InvalidOperationException("Expected the global to also implement IModifiableGlobalSettingsTable.");
		HydrateAllSettings();
	}

	public bool ContainsSetting(SettingIdentifier settingIdentifier)
	{
		return _actualSettings.ContainsSetting(settingIdentifier);
	}

	public IActiveSetting? GetSetting(SettingIdentifier settingIdentifier)
	{
		return _actualSettings.GetSetting(settingIdentifier);
	}

	public IModifiableSettingSymbolTable CreateNestedTable(SettingIdentifier localIdentifier)
	{
		return _modifiableGlobals.CreateNestedTable(localIdentifier);
	}

	public T AddOrUpdateNestedTable<T>(SettingIdentifier localIdentifier, T nestedTable) where T : ISettingSymbolTable
	{
		return _modifiableGlobals.AddOrUpdateNestedTable(localIdentifier, nestedTable);
	}

	public ISettingSymbolTable? GetNestedTable(SettingIdentifier localIdentifier)
	{
		return _modifiableGlobals.GetNestedTable(localIdentifier);
	}

	public ISettingSymbolTable? RemoveNestedTable(SettingIdentifier localIdentifier)
	{
		return _modifiableGlobals.RemoveNestedTable(localIdentifier);
	}

	public IActiveSetting AddSetting(SettingIdentifier localIdentifier, ISettingDefinition settingDefinition)
	{
		IActiveSetting activeSetting = _modifiableGlobals.AddSetting(localIdentifier, settingDefinition);
		_persistence.Hydrate(activeSetting);
		return activeSetting;
	}

	public IActiveSetting? RemoveSetting(SettingIdentifier localIdentifier)
	{
		IActiveSetting activeSetting = _modifiableGlobals.RemoveSetting(localIdentifier);
		if (activeSetting != null)
		{
			_persistence.Clear(activeSetting.LocalId);
		}
		return activeSetting;
	}

	public IActiveSetting? GetByScopedIdentifier(SettingIdentifier id)
	{
		return _modifiableGlobals.GetByScopedIdentifier(id);
	}

	private void HydrateAllSettings()
	{
		System.Collections.Generic.IEnumerator<IActiveSetting> enumerator = Settings.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				IActiveSetting current = enumerator.Current;
				_persistence.Hydrate(current);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	private void StoreAllSettings()
	{
		System.Collections.Generic.IEnumerator<IActiveSetting> enumerator = Settings.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				IActiveSetting current = enumerator.Current;
				_persistence.Store(current);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize((object)this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			_disposed = true;
			if (disposing)
			{
				StoreAllSettings();
				_persistence.Commit();
			}
		}
	}
}
