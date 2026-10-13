using TrainerPackageLib;

namespace LauncherCoreLib.Models.EventLogging;

public static class EventTargetApps
{
	public static TrackedTargetAppBase Launcher => LauncherAppTarget.Instance;

	public static TrackedTargetAppBase TrainerByPackage(ITrainerPackage package)
	{
		return TrainerAppTarget.Instance(package);
	}
}
