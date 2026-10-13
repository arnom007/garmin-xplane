using System.Collections.Generic;

namespace LauncherCoreLib.Models.DeployedEnv;

public interface ICommandLineProvider
{
	System.Collections.Generic.IReadOnlyList<string> CommandLineArgs { get; }
}
