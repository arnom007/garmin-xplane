using System;
using System.Reactive;
using System.Reactive.Linq;
using LOLA.Reactive;

namespace LauncherCoreLib.Models.TrainerPackages;

public static class LauncherGtiRunnerExtensions
{
	public static IObservable<TimingEvent> WhenMutationCommandCompletes(this ILauncherGtiRunner runner)
	{
		ILauncherGtiRunner runner2 = runner;
		return Observable.FromEventPattern(delegate(EventHandler h)
		{
			runner2.MutationCommandCompleted += h;
		}, delegate(EventHandler h)
		{
			runner2.MutationCommandCompleted -= h;
		}).ToTimingEventStream<EventPattern<object>>("Mutation command completed");
	}
}
