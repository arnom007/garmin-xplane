using System.IO;
using System.Threading.Tasks;
using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherCoreLib.Models.CrashReporting;

public static class InProgressCrashScopeExtensions
{
	public static string CrashReportingDirectoryFor(this IInProgressCrashScope inProgressCrashScope, TrainerSessionData trainerSessionData)
	{
		return Path.Combine(inProgressCrashScope.ContainedScope.BaseReportingPath, trainerSessionData.EndpointName.IdentStringChecked);
	}

	public static bool CrashOccurred(this IInProgressCrashScope inProgressCrashScope)
	{
		return inProgressCrashScope.ContainedScope.ContainsCrashes();
	}

	public static System.Threading.Tasks.Task CopyApplicationLogToSession(this IInProgressCrashScope inProgressCrashScope, string logPath)
	{
		return inProgressCrashScope.ContainedScope.CreateCopyOfLogForReporting(logPath);
	}
}
