using System;
using System.Collections.Generic;

namespace LauncherCoreLib.Models.Settings;

public interface IActiveSetting
{
	SettingIdentifier LocalId { get; }

	ISettingDefinition Definition { get; }

	System.Collections.Generic.IReadOnlyList<ISettingValueDefinition> CachedAvailableOptions { get; }

	IObservable<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> WhenAvailableOptionsChange { get; }

	string CurrentValue { get; set; }

	event EventHandler<SettingValueChangedEventArgs> ValueChanged;
}
