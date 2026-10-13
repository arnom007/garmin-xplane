using System;

namespace LauncherCoreLib.Models.AppLifeCycle.Migrations;

public interface IMigrationSource
{
	IObservable<BatchMigration> MigrationsBeingApplied { get; }
}
