using System.Collections.Generic;
using System.Linq;

namespace LauncherCoreLib.Models.Settings;

internal class NoSettings : ISettingsProvider
{
	public static readonly ISettingsProvider Instance = new NoSettings();

	public System.Collections.Generic.IEnumerable<IActiveSetting> Settings => Enumerable.Empty<IActiveSetting>();

	public string SettingsName => string.Empty;

	private NoSettings()
	{
	}

	public bool ContainsSetting(SettingIdentifier settingIdentifier)
	{
		return false;
	}

	public IActiveSetting? GetSetting(SettingIdentifier settingIdentifier)
	{
		return null;
	}
}
