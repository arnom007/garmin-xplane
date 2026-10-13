using LauncherCoreLib.Models.Settings;
using TrainerPackageLib;
using TrainerPackageLib.Serialized;

namespace LauncherCoreLib.Models.Simulated.Airport;

public static class AircraftSettingUtils
{
	public static void SetupAirframeSetting(IModifiableSettingSymbolTable aircraftSettings, AirframeCategoryClassType airframe)
	{
		string text = ConvertToExpectedJSONString(airframe);
		SettingDefinition settingDefinition = new SettingDefinition("Airframe")
		{
			DisplayText = "Airframe",
			Description = "The airframe type",
			ControlType = SettingUIControlType.Picklist,
			DefaultValue = text
		};
		settingDefinition.Add(new SettingValueDefinition(settingDefinition.SettingId, text)
		{
			DisplayText = ((object)airframe).ToString(),
			SimulatorValue = text
		});
		aircraftSettings.AddSetting(settingDefinition.SettingId, settingDefinition);
	}

	public static string ConvertToExpectedJSONString(AirframeCategoryClassType airframe)
	{
		if (airframe == AirframeCategoryClassType.None)
		{
			return "NONE";
		}
		return ((object)TrainerManifest.ConvertActualToSerial(airframe)).ToString();
	}
}
