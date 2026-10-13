using System.Threading;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Airport;

public interface IHangarMigrationMonitor
{
	System.Threading.Tasks.Task<bool> AreMigrationsNeeded();

	System.Threading.Tasks.Task StartMigrations(CancellationToken token = default(CancellationToken));
}
