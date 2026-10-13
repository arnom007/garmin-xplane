using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Settings;

public interface IGlobalsMigration
{
	bool HasOldGlobalsFile();

	System.Threading.Tasks.Task MigrateGlobalsFile();
}
