using System.Collections.Generic;

namespace LauncherCoreLib.Models.DeployedEnv;

public interface IInstallationVariableProvider
{
	string? GetString(string variableName);

	System.Collections.Generic.IEnumerable<string> GetExpectedPaths(string variableName);
}
