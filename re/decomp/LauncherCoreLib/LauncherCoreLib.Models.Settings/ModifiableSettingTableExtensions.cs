namespace LauncherCoreLib.Models.Settings;

public static class ModifiableSettingTableExtensions
{
	public static void AddOrReplaceIfDefinitionDoesntMatch(this IModifiableSettingSymbolTable simulatorSettings, ISettingDefinition settingDef)
	{
		SettingIdentifier localId = settingDef.SettingId.LocalId;
		IActiveSetting setting = simulatorSettings.GetSetting(localId);
		if (setting == null || setting.Definition != settingDef)
		{
			simulatorSettings.AddSetting(localId, settingDef);
		}
	}
}
