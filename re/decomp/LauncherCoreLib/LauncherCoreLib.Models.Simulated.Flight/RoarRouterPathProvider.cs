using System;
using System.IO;
using LauncherCoreLib.Models.DeployedEnv;

namespace LauncherCoreLib.Models.Simulated.Flight;

[Obsolete("Roar router is now dynamic because we have to support side by side versions of it.")]
public class RoarRouterPathProvider
{
	public const string ROAR_PROCESS_NAME = "roar";

	private readonly IExecutionPathProvider _executionPathProvider;

	public string RouterExePath => Path.Combine(_executionPathProvider.ExeExecutionPath, "roar.exe");

	public RoarRouterPathProvider(IExecutionPathProvider executionPathProvider)
	{
		_executionPathProvider = executionPathProvider;
	}
}
