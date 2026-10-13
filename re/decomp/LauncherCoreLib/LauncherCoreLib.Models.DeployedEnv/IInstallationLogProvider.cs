using System;

namespace LauncherCoreLib.Models.DeployedEnv;

public interface IInstallationLogProvider
{
	IObservable<string> InstallationLogFilePaths { get; }
}
