namespace LauncherCoreLib.Models.Settings;

public static class SettingUIControlTypeExtensions
{
	public static bool IsControlValid(this SettingUIControlType controlType)
	{
		return controlType != SettingUIControlType.None;
	}

	public static bool ControlUsesIdentifiers(this SettingUIControlType controlType)
	{
		return controlType == SettingUIControlType.Picklist;
	}
}
