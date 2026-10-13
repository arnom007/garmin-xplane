using System;

namespace LauncherCoreLib.Models.CrashReporting;

public interface IInProgressCrashScope : System.IDisposable
{
	SessionCrashScope ContainedScope { get; }
}
