namespace LauncherCoreLib.Models.Settings;

public class GlobalSettingTable : SettingSymbolTable, IModifiableGlobalSettingsTable, IGlobalSettingsProvider, ISettingsProvider, IModifiableSettingSymbolTable, ISettingSymbolTable
{
	public GlobalSettingTable()
		: base("Application")
	{
		this.AddNavigationDatabaseSetting();
	}
}
