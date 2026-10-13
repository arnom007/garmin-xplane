namespace LauncherCoreLib.Models.DeployedEnv;

public interface IDeploymentPathProvider
{
	string RootBinPath { get; }

	string RootDataPath { get; }

	string DeploymentPath { get; }

	string UserAppDataDirPath { get; }

	string AppTempDirectoryPath { get; }
}
