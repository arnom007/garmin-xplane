using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class RunningSimObserverExtensions
{
	public static IObservable<System.Collections.Generic.IList<RunningSim>> WatchSimWindows<TTiming>(this ITrainingSessionSource trainingSessionSource, IOsWindowOperations windows, IObservable<TTiming> timingSequence)
	{
		IOsWindowOperations windows2 = windows;
		IObservable<TTiming> timingSequence2 = timingSequence;
		return trainingSessionSource.TrainingSessions().Select<ITrainingSession, IObservable<System.Collections.Generic.IList<RunningSim>>>((Func<ITrainingSession, IObservable<System.Collections.Generic.IList<RunningSim>>>)((ITrainingSession ts) => EmptyObservableOnNull<TTiming>(windows2, timingSequence2, ts))).Switch<System.Collections.Generic.IList<RunningSim>>();
	}

	private static IObservable<System.Collections.Generic.IList<RunningSim>> EmptyObservableOnNull<TTiming>(IOsWindowOperations windows, IObservable<TTiming> timingSequence, ITrainingSession? ts)
	{
		if (ts != null)
		{
			return ts.WatchSimWindows<TTiming>(windows, timingSequence);
		}
		return Observable.Empty<System.Collections.Generic.IList<RunningSim>>();
	}
}
