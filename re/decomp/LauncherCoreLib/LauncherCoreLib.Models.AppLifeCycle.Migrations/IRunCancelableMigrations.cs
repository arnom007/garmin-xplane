using System.Threading;

namespace LauncherCoreLib.Models.AppLifeCycle.Migrations;

public interface IRunCancelableMigrations
{
	BatchMigration ScheduleMigrations(CancellationToken token, params ICancelableMigration[] migrations);
}
