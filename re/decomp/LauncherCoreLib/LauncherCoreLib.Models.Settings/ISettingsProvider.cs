using System.Collections.Generic;

namespace LauncherCoreLib.Models.Settings;

public interface ISettingsProvider
{
	string SettingsName { get; }

	System.Collections.Generic.IEnumerable<IActiveSetting> Settings { get; }

	bool ContainsSetting(SettingIdentifier settingIdentifier);

	IActiveSetting? GetSetting(SettingIdentifier settingIdentifier);
}
