namespace LauncherCoreLib.Models.AppLifeCycle.Migrations;

public interface IRunMigrations
{
	BatchMigration ScheduleMigrations(params ILauncherMigration[] migrations);
}
