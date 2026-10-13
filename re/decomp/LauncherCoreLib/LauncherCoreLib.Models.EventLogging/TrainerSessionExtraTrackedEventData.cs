using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging;

public class TrainerSessionExtraTrackedEventData : ExtraTrackedEventData
{
	[field: CompilerGenerated]
	public DateTimeOffset StartSessionTime
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public DateTimeOffset EndSessionTime
	{
		[CompilerGenerated]
		get;
	}

	public TrainerSessionExtraTrackedEventData(DateTimeOffset startSessionTime, DateTimeOffset endSessionTime)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		StartSessionTime = startSessionTime;
		EndSessionTime = endSessionTime;
	}
}
