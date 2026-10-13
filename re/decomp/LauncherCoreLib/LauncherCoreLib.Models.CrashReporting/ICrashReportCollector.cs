using System;
using System.Threading.Tasks;
using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherCoreLib.Models.CrashReporting;

public interface ICrashReportCollector
{
	event EventHandler ReportFound;

	System.Threading.Tasks.Task CheckForOutstandingReports();

	System.Threading.Tasks.Task SendReports();

	IInProgressCrashScope CreateCrashScope(ISessionInfo sessionInfo);

	System.Threading.Tasks.Task ClearReports();
}
