using System;

namespace LauncherCoreLib.Models.DeployedEnv;

public interface IInstallerLauncher
{
	IObservable<bool> LauncherIsRunning();

	void LaunchInstaller();
}
