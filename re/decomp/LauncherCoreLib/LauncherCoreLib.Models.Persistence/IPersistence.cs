namespace LauncherCoreLib.Models.Persistence;

public interface IPersistence
{
	void Commit();

	void Refresh();
}
