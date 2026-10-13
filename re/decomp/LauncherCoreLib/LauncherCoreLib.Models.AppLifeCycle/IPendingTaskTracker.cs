using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public interface IPendingTaskTracker
{
	bool IsAcceptingNewTasks { get; }

	System.Threading.Tasks.Task<TResult> TrackTask<TResult>(Func<System.Threading.Tasks.Task<TResult>> startTask, [CallerMemberName] string? taskName = null);

	IObservable<TResult> TrackStream<TResult>(IObservable<TResult> stream, [CallerMemberName] string? streamName = null);
}
