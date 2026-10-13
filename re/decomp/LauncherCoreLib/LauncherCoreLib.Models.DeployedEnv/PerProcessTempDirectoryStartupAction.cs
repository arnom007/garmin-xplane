using System.Threading;
using System.Threading.Tasks;
using LauncherCoreLib.Models.AppLifeCycle;

namespace LauncherCoreLib.Models.DeployedEnv;

public class PerProcessTempDirectoryStartupAction : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	private readonly PerProcessTempDirectoryHandling _handling;

	public PerProcessTempDirectoryStartupAction(PerProcessTempDirectoryHandling handling)
	{
		_handling = handling;
		base.ActionName = "Stale Per Process Cleanup";
	}

	public System.Threading.Tasks.Task OnStartup(CancellationToken shutdown)
	{
		_handling.CleanupStaleDirectories();
		return System.Threading.Tasks.Task.CompletedTask;
	}
}
