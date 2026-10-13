using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.DeployedEnv;

internal class ExecutionPathProvider : IExecutionPathProvider
{
	[field: CompilerGenerated]
	public string ExeExecutionPath
	{
		[CompilerGenerated]
		get;
	}

	public ExecutionPathProvider()
	{
		ExeExecutionPath = AppDomain.CurrentDomain.BaseDirectory;
	}
}
