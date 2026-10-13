namespace LauncherCoreLib.Models.AppLifeCycle;

public static class RunOnceExtensions
{
	public static bool HasCompleted(this IRunOnce runOnce)
	{
		return runOnce.CompletedAt.HasValue;
	}
}
