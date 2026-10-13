using System;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle.Migrations;

public interface ILauncherMigration
{
	string MigrationName { get; }

	System.Threading.Tasks.Task<bool> IsMigrationRequired();

	IObservable<MigrationStatusUpdate> MigrationSteps();
}
