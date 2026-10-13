using System.Threading.Tasks;

namespace LauncherCoreLib.Models.CrashReporting;

public interface IErrorReportMediator
{
	System.Threading.Tasks.Task PostCrashReport(SimInstanceCrashInfo simCrash, CrashRptInfo cri);
}
