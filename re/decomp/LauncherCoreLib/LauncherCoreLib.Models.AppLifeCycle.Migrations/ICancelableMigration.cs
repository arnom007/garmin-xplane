using System;
using System.Threading;

namespace LauncherCoreLib.Models.AppLifeCycle.Migrations;

public interface ICancelableMigration : ILauncherMigration
{
	IObservable<MigrationStatusUpdate> MigrationSteps(CancellationToken token);
}
