using System;
using System.Reactive.Concurrency;
using System.Runtime.ExceptionServices;

namespace LauncherCoreLib.Utility;

public static class ManualCrash
{
	public static void TakeDownApp(System.Exception e)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		ExceptionDispatchInfo exceptionInfo = ExceptionDispatchInfo.Capture(e);
		Scheduler.Schedule((IScheduler)Scheduler.Default, (Action)delegate
		{
			exceptionInfo.Throw();
		});
	}
}
