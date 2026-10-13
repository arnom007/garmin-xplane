using System;
using System.Collections.Generic;

namespace LauncherCoreLib.Models.Settings;

public interface ISettingDefinition : IEquatable<ISettingDefinition>
{
	SettingIdentifier SettingId { get; }

	SettingUIControlType ControlType { get; }

	string DisplayText { get; }

	string Description { get; }

	int DocumentOrderIndex { get; }

	string DefaultValue { get; }

	System.Collections.Generic.IEnumerable<ISettingValueDefinition> Values { get; }

	bool ContainsValue(SettingIdentifier valueIdentifier);

	ISettingValueDefinition? GetValue(SettingIdentifier valueIdentifier);
}
