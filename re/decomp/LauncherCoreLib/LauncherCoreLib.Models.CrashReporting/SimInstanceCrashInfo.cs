using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.CrashReporting;

public class SimInstanceCrashInfo
{
	[field: CompilerGenerated]
	public string TrainerLogPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string CrashRptPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string SessionZipPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public SessionCrashScope ParentScope
	{
		[CompilerGenerated]
		get;
	}

	public SimInstanceCrashInfo(string trainerLog, string crashRpt, string sessionzip, SessionCrashScope parent)
	{
		TrainerLogPath = trainerLog;
		CrashRptPath = crashRpt;
		SessionZipPath = sessionzip;
		ParentScope = parent;
	}
}
