using System;
using System.Collections.Generic;

namespace LauncherCoreLib.Models.Settings;

public interface IConstraintWatcher
{
	IObservable<ISettingValueDefinition[]> WatchConstraints(System.Collections.Generic.IEnumerable<ISettingValueDefinition> values);
}
