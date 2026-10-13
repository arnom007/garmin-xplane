using System;
using System.Globalization;
using LauncherCoreLib.Models.Winfs;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Settings;

public static class NavigationDatabaseSetting
{
	public const string NAVIGATION_DB_SETTING_ID = "NavigationDatabase";

	public const string JEPPESEN_VALUE_ID = "Jeppesen";

	public const string JEPPESEN_SIM_VALUE = "Jeppesen";

	public const string JEPPESEN_WORLDWIDE_VALUE_ID = "JeppesenWorldwide";

	public const string JEPPESEN_WORLDWIDE_SIM_VALUE = "JeppesenWorldwide";

	public const string GARMIN_VALUE_ID = "Garmin";

	public const string GARMIN_SIM_VALUE = "Garmin";

	public static SettingDefinition CreateNavDatabaseSetting()
	{
		SettingDefinition obj = new SettingDefinition("NavigationDatabase")
		{
			DisplayText = "Navigation Database",
			Description = "Preferred Navigation Database source to load in trainers.",
			ControlType = SettingUIControlType.Picklist,
			DefaultValue = "Jeppesen"
		};
		obj.Add(new SettingValueDefinition(obj.SettingId, "JeppesenWorldwide")
		{
			DisplayText = "Jeppesen (Worldwide Fixed Wing / Worldwide Rotorcraft)",
			Description = "Load Jeppesen navigation data in trainers.",
			SimulatorValue = "JeppesenWorldwide"
		});
		obj.Add(new SettingValueDefinition(obj.SettingId, "Jeppesen")
		{
			DisplayText = "Jeppesen (Worldwide Fixed Wing / Americas Rotorcraft)",
			Description = "Load Jeppesen navigation data in trainers.",
			SimulatorValue = "Jeppesen"
		});
		obj.Add(new SettingValueDefinition(obj.SettingId, "Garmin")
		{
			DisplayText = "Garmin (North America Fixed Wing / North America Rotorcraft)",
			Description = "Load Garmin navigation data in trainers.",
			SimulatorValue = "Garmin"
		});
		return obj;
	}

	public static void AddNavigationDatabaseSetting(this IModifiableGlobalSettingsTable globals)
	{
		SettingDefinition settingDefinition = CreateNavDatabaseSetting();
		globals.AddSetting(settingDefinition.SettingId.LocalId, settingDefinition);
	}

	public static IActiveSetting? GetNavigationDatabaseSettingFromParent(this ISettingSymbolTable settingsProvider)
	{
		return settingsProvider.GetByScopedIdentifier("NavigationDatabase");
	}

	public static IActiveSetting? GetNavigationDatabaseSetting(this IGlobalSettingsProvider settingsProvider)
	{
		return settingsProvider.GetSetting("NavigationDatabase");
	}

	public static void SelectJeppesenNavigation(IActiveSetting setting)
	{
		ThrowIfNotNavDbSetting(setting, "select Jeppesen Navigation");
		setting.SelectByLocalValueId("Jeppesen");
	}

	public static void SelectGarminNavigation(IActiveSetting setting)
	{
		ThrowIfNotNavDbSetting(setting, "select Garmin Navigation");
		setting.SelectByLocalValueId("Garmin");
	}

	public static SupportedWinfsDatabase CurrentlySelectedDatabase(IActiveSetting activeSetting)
	{
		ThrowIfNotNavDbSetting(activeSetting, "get the selected database");
		if (((SettingIdentifier)activeSetting.CurrentValue).LocalId == "Garmin")
		{
			return SupportedWinfsDatabase.GARMIN_NAV;
		}
		return SupportedWinfsDatabase.JEPP_NAV;
	}

	public static SupportedWinfsDatabase CurrentlySelectedDatabaseForAircraftType(IActiveSetting activeSetting, AirframeCategoryClassType airframeCategoryClassType)
	{
		string text = ((SettingIdentifier)activeSetting.CurrentValue).LocalId;
		if (!(text == "Garmin"))
		{
			if (!(text == "Jeppesen"))
			{
				if (!(text == "JeppesenWorldwide"))
				{
				}
				return SupportedWinfsDatabase.JEPP_NAV;
			}
			if (airframeCategoryClassType != AirframeCategoryClassType.Rotorcraft)
			{
				return SupportedWinfsDatabase.JEPP_NAV;
			}
			return SupportedWinfsDatabase.JEPP_ROTOR_NAV;
		}
		if (airframeCategoryClassType != AirframeCategoryClassType.Rotorcraft)
		{
			return SupportedWinfsDatabase.GARMIN_NAV;
		}
		return SupportedWinfsDatabase.GARMIN_ROTOR_NAV;
	}

	private static void ThrowIfNotNavDbSetting(IActiveSetting setting, string settingAction)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		string text = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "The '{0}' setting is required to {1}.", (object)"NavigationDatabase", (object)settingAction);
		if (setting == null)
		{
			throw new ArgumentNullException(text);
		}
		if (setting.LocalId != "NavigationDatabase")
		{
			throw new InvalidOperationException(text);
		}
	}
}
