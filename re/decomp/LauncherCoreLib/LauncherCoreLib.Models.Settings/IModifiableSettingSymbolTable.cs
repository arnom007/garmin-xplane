namespace LauncherCoreLib.Models.Settings;

public interface IModifiableSettingSymbolTable : ISettingSymbolTable, ISettingsProvider
{
	IModifiableSettingSymbolTable CreateNestedTable(SettingIdentifier localIdentifier);

	T AddOrUpdateNestedTable<T>(SettingIdentifier localIdentifier, T nestedTable) where T : ISettingSymbolTable;

	ISettingSymbolTable? RemoveNestedTable(SettingIdentifier localIdentifier);

	IActiveSetting AddSetting(SettingIdentifier localIdentifier, ISettingDefinition settingDefinition);

	IActiveSetting? RemoveSetting(SettingIdentifier localIdentifier);
}
