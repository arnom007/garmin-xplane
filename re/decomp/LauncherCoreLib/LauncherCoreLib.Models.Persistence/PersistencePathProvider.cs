using System;
using LauncherCoreLib.Models.DeployedEnv;

namespace LauncherCoreLib.Models.Persistence;

public class PersistencePathProvider : IPersistencePathProvider
{
	private readonly IDeploymentPathProvider _pathProvider;

	public string PersistenceDirectoryPath => _pathProvider.UserAppDataDirPath;

	public PersistencePathProvider(IDeploymentPathProvider deploymentProvider)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_pathProvider = deploymentProvider ?? throw new ArgumentNullException("deploymentProvider");
	}
}
