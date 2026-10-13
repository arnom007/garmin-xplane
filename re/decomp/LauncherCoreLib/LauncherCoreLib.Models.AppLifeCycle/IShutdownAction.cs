using System.Threading.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public interface IShutdownAction : IAppLifeCycleAction
{
	System.Threading.Tasks.Task OnShutdown();
}
