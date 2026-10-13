namespace LauncherCoreLib.Models.Winfs;

public interface IDatabaseDeploymentPathProvider
{
	string GetDatabasePath(string winfsKey);
}
