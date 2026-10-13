using LauncherCoreLib.Models.CrashReporting;

namespace Aviation_Trainer_Launcher.Models.Identity;

internal class TrainerLauncherAppIdProvider : ICrashReportAppIdentityProvider, IFeedbackAppIdentityProvider
{
	public string GetAppIdForNativeCrashreporting()
	{
		return "3cf6f0c2-ab8f-44bc-8b04-563f6100023e";
	}

	public string GetFeedbackTargetAppIdentity()
	{
		return "6e448fcc-d350-4be1-9ddb-69bf4f9def31";
	}
}
