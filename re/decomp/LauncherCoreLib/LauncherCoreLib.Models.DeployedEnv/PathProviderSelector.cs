using System.Runtime.CompilerServices;
using LOLA.FileSystem;

namespace LauncherCoreLib.Models.DeployedEnv;

public class PathProviderSelector : IDeploymentPathProvider
{
	[field: CompilerGenerated]
	public IDeploymentPathProvider ActivePathProvider
	{
		[CompilerGenerated]
		get;
	}

	public string DeploymentPath => ActivePathProvider.DeploymentPath;

	public string UserAppDataDirPath => ActivePathProvider.UserAppDataDirPath;

	public string AppTempDirectoryPath => ActivePathProvider.AppTempDirectoryPath;

	public string RootBinPath => ActivePathProvider.RootBinPath;

	public string RootDataPath => ActivePathProvider.RootDataPath;

	public PathProviderSelector(ICommandLineProvider commandLine, IExecutionPathProvider exePathProvider, IInstallationVariableProvider installationVariableProvider, ISuiteNameProvider suiteNameProvider, IFileSystem fileSystem)
	{
		ActivePathProvider = new DeploymentPathProvider(installationVariableProvider, suiteNameProvider);
	}

	public static PathProviderSelector CreatePathProvider(ISuiteNameProvider suiteNameProvider, IFileSystem fileSystem)
	{
		return new PathProviderSelector(new CommandLineProvider(), new ExecutionPathProvider(), new WindowsRegistryInstallationVariables(suiteNameProvider), suiteNameProvider, fileSystem);
	}
}
