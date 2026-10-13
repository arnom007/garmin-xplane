using System;
using System.Collections.Generic;

namespace LauncherCoreLib.Models.DeployedEnv;

public class CommandLineProvider : ICommandLineProvider
{
	public System.Collections.Generic.IReadOnlyList<string> CommandLineArgs => Environment.GetCommandLineArgs();
}
