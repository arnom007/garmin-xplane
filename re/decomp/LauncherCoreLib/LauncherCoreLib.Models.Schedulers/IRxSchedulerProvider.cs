using System.Reactive.Concurrency;

namespace LauncherCoreLib.Models.Schedulers;

public interface IRxSchedulerProvider
{
	IScheduler IOThreads { get; }
}
