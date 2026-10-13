using System;
using System.Collections.Immutable;

namespace LauncherCoreLib.Models.Settings;

public static class SettingDefinitionCollectionExtensions
{
	public static ImmutableDictionary<SettingIdentifier, string> ToDefaultPersistenceDictonary(this ISettingDefinitionCollection settingDefinitions)
	{
		return ImmutableDictionary.ToImmutableDictionary<ISettingDefinition, SettingIdentifier, string>(settingDefinitions.Definitions, (Func<ISettingDefinition, SettingIdentifier>)((ISettingDefinition s) => s.SettingId.LocalId), (Func<ISettingDefinition, string>)((ISettingDefinition s) => s.DefaultValue));
	}
}
