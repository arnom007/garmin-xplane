using System.Threading.Tasks;

namespace LauncherCoreLib.Models.CrashReporting;

public interface IFeedbackLogArchiveBuilder
{
	System.Threading.Tasks.Task<byte[]> CreateLogArchive(int maxSize);
}
