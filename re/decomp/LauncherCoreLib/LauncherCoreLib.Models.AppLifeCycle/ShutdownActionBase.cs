using System.Threading.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public abstract class ShutdownActionBase : AppLifeCycleActionBase, IShutdownAction, IAppLifeCycleAction
{
	public abstract System.Threading.Tasks.Task OnShutdown();
}
