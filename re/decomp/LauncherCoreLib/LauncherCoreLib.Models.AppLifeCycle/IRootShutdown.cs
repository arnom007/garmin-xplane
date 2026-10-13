using System.Threading.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public interface IRootShutdown
{
	System.Threading.Tasks.Task OnShutdown();
}
