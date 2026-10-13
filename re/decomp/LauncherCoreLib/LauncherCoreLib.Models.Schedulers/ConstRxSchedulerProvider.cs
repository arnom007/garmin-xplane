using System;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Schedulers;

public class ConstRxSchedulerProvider : IRxSchedulerProvider
{
	[field: CompilerGenerated]
	public IScheduler IOThreads
	{
		[CompilerGenerated]
		get;
	}

	public ConstRxSchedulerProvider(IScheduler scheduler)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		IOThreads = scheduler ?? throw new ArgumentNullException("scheduler");
	}
}
