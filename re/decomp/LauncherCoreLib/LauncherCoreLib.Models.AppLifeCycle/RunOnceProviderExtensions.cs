namespace LauncherCoreLib.Models.AppLifeCycle;

public static class RunOnceProviderExtensions
{
	public const string APP_OPERATION_NAME = "app";

	public static IRunOnce GetAppFirstRunInfo(this IRunOnceProvider runOnceProvider)
	{
		return runOnceProvider.GetByOperationNameSync("app");
	}
}
