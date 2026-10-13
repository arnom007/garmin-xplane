using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LauncherCoreLib.Models.DeployedEnv;

namespace LauncherCoreLib.Models.Winfs;

public class DatabaseDeploymentPathProvider : IDatabaseDeploymentPathProvider
{
	private readonly IDeploymentPathProvider _deployment;

	private readonly Dictionary<string, RelativePath> _databaseKeys = BuildDefaultInternalDatabaseMap();

	public DatabaseDeploymentPathProvider(IDeploymentPathProvider deploymentPathProvider)
	{
		_deployment = deploymentPathProvider;
	}

	public static Dictionary<string, RelativePath> BuildDefaultInternalDatabaseMap()
	{
		return Enumerable.ToDictionary<SupportedWinfsDatabase, string, RelativePath>(Enumerable.Where<SupportedWinfsDatabase>(Enumerable.Cast<SupportedWinfsDatabase>((System.Collections.IEnumerable)System.Enum.GetValues(typeof(SupportedWinfsDatabase))), (Func<SupportedWinfsDatabase, bool>)((SupportedWinfsDatabase e) => e != SupportedWinfsDatabase.Invalid)), (Func<SupportedWinfsDatabase, string>)((SupportedWinfsDatabase swd) => ((object)swd).ToString()), (Func<SupportedWinfsDatabase, RelativePath>)((SupportedWinfsDatabase swd) => DatabaseDeployedRelativePathAttribute.GetDeployedPath(swd) ?? throw new InvalidOperationException()));
	}

	public string GetDatabasePath(string winfsKey)
	{
		return Path.Combine(_deployment.DatabasesDirectoryPath(), (string)_databaseKeys[winfsKey]);
	}
}
