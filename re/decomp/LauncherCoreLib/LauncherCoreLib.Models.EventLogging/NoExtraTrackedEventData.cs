using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging;

public class NoExtraTrackedEventData : ExtraTrackedEventData
{
	[field: CompilerGenerated]
	public static NoExtraTrackedEventData Instance
	{
		[CompilerGenerated]
		get;
	} = new NoExtraTrackedEventData();

}
