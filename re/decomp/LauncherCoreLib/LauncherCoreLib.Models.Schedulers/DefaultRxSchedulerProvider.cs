using System.Reactive.Concurrency;

namespace LauncherCoreLib.Models.Schedulers;

public class DefaultRxSchedulerProvider : ConstRxSchedulerProvider
{
	public DefaultRxSchedulerProvider()
		: base(Scheduler.Default)
	{
	}
}
