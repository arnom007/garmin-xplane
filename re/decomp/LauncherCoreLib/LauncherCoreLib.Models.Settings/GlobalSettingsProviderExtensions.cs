using LauncherCoreLib.Models.Persistence;

namespace LauncherCoreLib.Models.Settings;

internal static class GlobalSettingsProviderExtensions
{
	public static void SaveAllSettings(this IGlobalSettingsProvider globalSettingsProvider, IGlobalSettingsPersistence persist)
	{
		persist.Store(globalSettingsProvider.Settings);
		persist.Commit();
	}
}
