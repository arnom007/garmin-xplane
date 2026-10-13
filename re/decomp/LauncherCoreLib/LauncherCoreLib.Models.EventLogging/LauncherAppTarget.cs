using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging;

public class LauncherAppTarget : TrackedTargetAppBase
{
	[field: CompilerGenerated]
	public static LauncherAppTarget Instance
	{
		[CompilerGenerated]
		get;
	} = new LauncherAppTarget();

}
