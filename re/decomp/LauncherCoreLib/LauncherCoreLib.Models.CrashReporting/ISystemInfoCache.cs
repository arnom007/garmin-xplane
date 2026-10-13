using System.Threading;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.CrashReporting;

public interface ISystemInfoCache
{
	System.Threading.Tasks.Task<SystemInfo?> GetSystemInfo(CancellationToken cancel = default(CancellationToken));
}
