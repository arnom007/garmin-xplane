using System.Threading.Tasks;

namespace LauncherCoreLib.Models.CrashReporting;

public interface ICrashReportInstallationIdProvider
{
	System.Threading.Tasks.Task<string> GetInstallationId();

	System.Threading.Tasks.Task<string?> GetUserId();
}
