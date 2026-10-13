using System.IO;
using GTI.Net;
using LOLA.Certificates;
using LOLA.FileSystem;
using LOLA.Processes;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.TrainerPackages;

public class GtiRunnerFactory
{
	private sealed class ExeTrustProvider : IExeTrustProvider
	{
		private readonly ILayeredFileTrustProvider _layeredFileTrustProvider;

		public ExeTrustProvider(ILayeredFileTrustProvider layeredFileTrustProvider)
		{
			_layeredFileTrustProvider = layeredFileTrustProvider;
		}

		public bool IsExeTrusted(string exePath)
		{
			return _layeredFileTrustProvider.GetFileTrust(exePath).LogDistrustReasons().IsTrusted;
		}
	}

	private readonly IExecutionPathProvider _executionPath;

	private readonly IDeploymentPathProvider _deployment;

	private readonly IOsProcessOperations _processes;

	private readonly IFileSystem _fileSystem;

	private readonly ILayeredFileTrustProvider _fileTrustProvider;

	public GtiRunnerFactory(IExecutionPathProvider execution, IDeploymentPathProvider deployment, IOsProcessOperations processes, IFileSystem fileSystem, ILayeredFileTrustProvider fileTrustProvider)
	{
		_executionPath = execution;
		_deployment = deployment;
		_processes = processes;
		_fileSystem = fileSystem;
		_fileTrustProvider = fileTrustProvider;
	}

	public IGtiRunner CreateDefaultRunner()
	{
		return CreateRunner("Production");
	}

	private GtiRunnerV2 CreateRunner(string gtiDirectoryName)
	{
		string text = Path.Combine(_executionPath.ExeExecutionPath, "GTI", "GTI.exe");
		string gtiRoot = Path.Combine(_deployment.RootDataPath, "GTI", gtiDirectoryName);
		_fileTrustProvider.GetFileTrust(text).LogDistrustReasons().ThrowIfNotTrusted();
		return new GtiRunnerV2(text, gtiRoot, new ExeTrustProvider(_fileTrustProvider), _processes);
	}
}
