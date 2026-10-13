using System;
using System.Collections.Generic;
using System.Linq;

namespace LauncherCoreLib.Models.Settings;

public static class SettingDefinitionExtensions
{
	public static System.Collections.Generic.IEnumerable<SettingIdentifier> AllSettingsConstrainingDefinition(this ISettingDefinition settingDefinition)
	{
		return Enumerable.Distinct<SettingIdentifier>(Enumerable.SelectMany<ISettingValueDefinition, SettingIdentifier>(settingDefinition.Values, (Func<ISettingValueDefinition, System.Collections.Generic.IEnumerable<SettingIdentifier>>)((ISettingValueDefinition v) => ImmutableArrayExtensions.Select<ValueConstraintDefinition, SettingIdentifier>(v.Constraints, (Func<ValueConstraintDefinition, SettingIdentifier>)((ValueConstraintDefinition c) => c.SettingToObserve)))));
	}

	public static SettingIdentifier FullDefaultValueId(this ISettingDefinition settingDefinition)
	{
		return settingDefinition.SettingId + (SettingIdentifier)settingDefinition.DefaultValue;
	}
}
