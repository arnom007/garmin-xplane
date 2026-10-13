using System.Collections.Generic;

namespace LauncherCoreLib.Models.DeployedEnv;

public interface IConfidentialAliasProvider
{
	System.Collections.Generic.IEnumerable<ConfidentialAlias> Aliases { get; }
}
