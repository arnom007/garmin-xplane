using System;
using System.Collections.Generic;

namespace LauncherCoreLib.Models.AppLifeCycle;

public interface IStartupRegistry
{
	event EventHandler StartupActionAdded;

	System.Collections.Generic.IReadOnlyCollection<AppLifeCycleActionRecord> GetStartupActions(string? actionName = null, System.Type? actionType = null);
}
