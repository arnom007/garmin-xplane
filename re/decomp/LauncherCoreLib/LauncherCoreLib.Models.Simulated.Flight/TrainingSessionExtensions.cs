using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using LOLA.Processes;
using LOLA.Tasks;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class TrainingSessionExtensions
{
	public static System.Collections.Generic.IEnumerable<RunningSim> GetCurrentRunningSims(this ITrainingSession trainingSession, IOsWindowOperations windows)
	{
		IOsWindowOperations windows2 = windows;
		return Enumerable.Select<SimProcessSession, RunningSim>((System.Collections.Generic.IEnumerable<SimProcessSession>)trainingSession.Simulators, (Func<SimProcessSession, RunningSim>)((SimProcessSession ps) => ps.Process.HasExited() ? new RunningSim(ps, OsWindow.DEFAULT) : new RunningSim(ps, windows2.GetCurrentWindow(ps.Process.MainWindowHandle))));
	}

	public static IObservable<System.Collections.Generic.IList<RunningSim>> WatchSimWindows<TTiming>(this ITrainingSession trainingSession, IOsWindowOperations windows, IObservable<TTiming> timingSequence)
	{
		ITrainingSession trainingSession2 = trainingSession;
		IOsWindowOperations windows2 = windows;
		return (IObservable<System.Collections.Generic.IList<RunningSim>>)(object)timingSequence.TakeUntil<TTiming, Unit>(trainingSession2.AllSimsExited.Catch().ToObservable()).Select<TTiming, List<RunningSim>>((Func<TTiming, List<RunningSim>>)((TTiming _) => Enumerable.ToList<RunningSim>(trainingSession2.GetCurrentRunningSims(windows2))));
	}
}
