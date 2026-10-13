namespace LauncherCoreLib.Models.DeployedEnv;

public interface IProtectedPathProvider
{
	bool IsPathProtected(string path);
}
