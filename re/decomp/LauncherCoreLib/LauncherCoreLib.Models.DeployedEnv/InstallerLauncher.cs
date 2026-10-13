using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Processes;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.DeployedEnv;

public sealed class InstallerLauncher : IInstallerLauncher, System.IDisposable
{
	private readonly IInstallationVariableProvider _installationVariableProvider;

	private readonly IDeploymentPathProvider _deployment;

	private readonly IOsProcessOperations _processes;

	private readonly BehaviorSubject<IProcessSession?> _bootstrapperProcesses;

	public InstallerLauncher(IInstallationVariableProvider installationVariables, IDeploymentPathProvider deploymentPathProvider, IOsProcessOperations processes)
	{
		_installationVariableProvider = installationVariables;
		_deployment = deploymentPathProvider;
		_processes = processes;
		_bootstrapperProcesses = new BehaviorSubject<IProcessSession>(null);
	}

	public void LaunchInstaller()
	{
		if (IsRunning(_bootstrapperProcesses.Value))
		{
			Logger.Log(LogLevel.Debug, "[Warning] Implement installer refocus.");
			return;
		}
		string @string = _installationVariableProvider.GetString(InstallationVariable.BootstrapperLocation);
		if (@string != null)
		{
			_bootstrapperProcesses.OnNext(_processes.Fork(_deployment.AppTempDirectoryPath, @string, "/modify"));
		}
	}

	public IObservable<bool> LauncherIsRunning()
	{
		return ((IObservable<IProcessSession>)(object)_bootstrapperProcesses).Select<IProcessSession, IObservable<bool>>((Func<IProcessSession, IObservable<bool>>)((IProcessSession p) => IsRunningEvents(p))).Switch<bool>().DistinctUntilChanged<bool>();
	}

	public void Dispose()
	{
		_bootstrapperProcesses.Dispose();
	}

	private static IObservable<bool> IsRunningEvents(IProcessSession? p)
	{
		IProcessSession p2 = p;
		IObservable<bool> val = Observable.Return(IsRunning(p2));
		if (IsRunning(p2))
		{
			val = val.Concat<bool>(Observable.FromAsync<bool>((Func<System.Threading.Tasks.Task<bool>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass8_0._003C_003CIsRunningEvents_003Eb__0_003Ed))] async () =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				await p2.Exited.Catch().DetachContext();
				return false;
			})));
		}
		return val;
	}

	private static bool IsRunning(IProcessSession? p)
	{
		if (p == null)
		{
			return false;
		}
		return !p.HasExited();
	}
}
