namespace LauncherCoreLib.Models.CrashReporting;

public interface ICrashReportAppIdentityProvider
{
	string GetAppIdForNativeCrashreporting();
}
