using System.Threading;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public interface IStartupAction : IAppLifeCycleAction
{
	System.Threading.Tasks.Task OnStartup(CancellationToken shutdown);
}
