using System;

namespace LauncherCoreLib.Models.EventLogging;

public interface ITimeRangeProvider
{
	string GetTimeRangeString(TimeSpan duration);
}
