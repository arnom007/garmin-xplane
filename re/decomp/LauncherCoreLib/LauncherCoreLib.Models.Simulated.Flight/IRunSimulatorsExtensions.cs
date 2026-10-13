using System;
using System.Reactive.Linq;
using LauncherCoreLib.Models.AbstractUi;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class IRunSimulatorsExtensions
{
	public static IObservable<IUserAction> SimActions(this IRunSimulators simRunner)
	{
		return simRunner.WhenActionChanges;
	}

	public static IObservable<bool> SafeWhenIsRunningChanges(this IRunSimulators simRunner)
	{
		return simRunner.WhenIsRunningChanges.Retry<bool>().DistinctUntilChanged<bool>();
	}
}
