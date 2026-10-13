using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using LauncherCoreLib.Models.Settings;

namespace LauncherCoreLib.Models.CrashReporting;

public static class CrashReportingSetting
{
	public const string CRASH_REPORT_PREF_NAME = "When a crash occurs";

	public const string CRASH_REPORT_PREF_ASK = "Always ask before sending a crash report";

	public const string CRASH_REPORT_PREF_ALWAYS_SEND = "Always send crash reports";

	public const string CRASH_REPORT_PREF_NEVER_SEND = "Never send crash reports";

	public const string CRASH_REPORT_SETTING_ID = "CrashSendUserPref";

	public static SettingDefinition CreateCrashReportingSetting()
	{
		SettingDefinition settingDef = new SettingDefinition(SettingIdentifier.FromString("CrashSendUserPref"))
		{
			DisplayText = "When a crash occurs",
			Description = "Default behavior for handling crash reports when they are detected.",
			ControlType = SettingUIControlType.Picklist,
			DefaultValue = "AlwaysAsk"
		};
		settingDef.AddRange(Enumerable.Select<CrashSendUserPref, SettingValueDefinition>(Enumerable.Cast<CrashSendUserPref>((System.Collections.IEnumerable)System.Enum.GetValues(typeof(CrashSendUserPref))), (Func<CrashSendUserPref, SettingValueDefinition>)((CrashSendUserPref e) => new SettingValueDefinition(settingDef.SettingId, SettingIdentifier.FromString(((object)e).ToString()))
		{
			DisplayText = GetDescription(e),
			SimulatorValue = ((object)e).ToString()
		})));
		return settingDef;
	}

	public static void AddCrashReportingSetting(this IModifiableGlobalSettingsTable globals)
	{
		SettingDefinition settingDefinition = CreateCrashReportingSetting();
		globals.AddSetting(settingDefinition.SettingId.LocalId, settingDefinition);
	}

	public static IActiveSetting? GetCrashReportingSetting(this ISettingsProvider settings)
	{
		return settings.GetSetting(SettingIdentifier.FromString("CrashSendUserPref"));
	}

	public static CrashSendUserPref CurrentlySelectedReportingBehavior(IActiveSetting activeSetting)
	{
		ThrowIfNotCrashReportingSetting(activeSetting, "get the selected crash reporting behavior");
		return ConvertSettingIdToEnum(SettingIdentifier.FromString(activeSetting.CurrentValue).LocalId);
	}

	public static void SelectCrashReportingBehavior(IActiveSetting setting, CrashSendUserPref crashSendUserPref)
	{
		ThrowIfNotCrashReportingSetting(setting, "selecting " + ((object)crashSendUserPref).ToString());
		setting.SelectByLocalValueId(SettingIdentifier.FromString(((object)crashSendUserPref).ToString()));
	}

	public static CrashSendUserPref ConvertValueToPreference(ISettingValueDefinition value)
	{
		return ConvertSettingIdToEnum(value.ValueId.LocalId);
	}

	private static CrashSendUserPref ConvertSettingIdToEnum(SettingIdentifier enumString)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		CrashSendUserPref result = default(CrashSendUserPref);
		if (!System.Enum.TryParse<CrashSendUserPref>((string)enumString, ref result))
		{
			throw new NotSupportedException(enumString + " is not a supported crash report value.");
		}
		return result;
	}

	private static void ThrowIfNotCrashReportingSetting(IActiveSetting setting, string settingAction)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		string text = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "The '{0}' setting is required to {1}.", (object)"CrashSendUserPref", (object)settingAction);
		if (setting == null)
		{
			throw new ArgumentNullException(text);
		}
		if (setting.LocalId != "CrashSendUserPref")
		{
			throw new InvalidOperationException(text);
		}
	}

	private static string GetDescription(CrashSendUserPref e)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		MemberInfo[] member = ((object)e).GetType().GetMember(((object)e).ToString());
		if (member != null && member.Length == 1)
		{
			object[] customAttributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (customAttributes != null && customAttributes.Length == 1)
			{
				return ((DescriptionAttribute)customAttributes[0]).Description;
			}
		}
		return ((object)e).ToString();
	}
}
