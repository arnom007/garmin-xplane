using System;
using System.Collections.Immutable;

namespace LauncherCoreLib.Models.Settings;

public interface ISettingValueDefinition : IEquatable<ISettingValueDefinition>
{
	SettingIdentifier ValueId { get; }

	string DisplayText { get; }

	string Description { get; }

	int DocumentOrderIndex { get; }

	string SimulatorValue { get; }

	ImmutableArray<ValueConstraintDefinition> Constraints { get; }
}
