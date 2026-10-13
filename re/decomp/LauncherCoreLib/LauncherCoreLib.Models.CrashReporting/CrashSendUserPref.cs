using System.ComponentModel;

namespace LauncherCoreLib.Models.CrashReporting;

public enum CrashSendUserPref
{
	[Description("Always ask before sending a crash report")]
	AlwaysAsk,
	[Description("Always send crash reports")]
	AlwaysSend,
	[Description("Never send crash reports")]
	NeverSend
}
