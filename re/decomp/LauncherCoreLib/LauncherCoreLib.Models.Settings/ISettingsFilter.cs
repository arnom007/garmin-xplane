using System;

namespace LauncherCoreLib.Models.Settings;

public interface ISettingsFilter
{
	event EventHandler FilterChanged;

	bool FilterAppliesToProvider(ISettingsProvider provider);

	bool IsSettingValid(IActiveSetting activeSetting);
}
