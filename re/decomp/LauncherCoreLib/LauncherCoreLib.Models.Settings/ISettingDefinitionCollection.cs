using System;
using System.Collections.Generic;

namespace LauncherCoreLib.Models.Settings;

public interface ISettingDefinitionCollection : IEquatable<ISettingDefinitionCollection>
{
	System.Collections.Generic.IEnumerable<ISettingDefinition> Definitions { get; }

	bool ContainsSetting(SettingIdentifier settingIdentifier);

	ISettingDefinition? GetSetting(SettingIdentifier settingIdentifier);
}
