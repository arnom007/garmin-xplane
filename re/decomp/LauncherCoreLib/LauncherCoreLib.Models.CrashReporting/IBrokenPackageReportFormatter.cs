using System.Collections.Generic;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.CrashReporting;

public interface IBrokenPackageReportFormatter
{
	string FormatFeedbackAttachment(System.Collections.Generic.IEnumerable<TrainerPackageLoadException> errors, int characterLimit);
}
