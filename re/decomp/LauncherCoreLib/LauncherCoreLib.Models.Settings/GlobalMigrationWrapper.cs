using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LauncherCoreLib.Models.AppLifeCycle.Migrations;

namespace LauncherCoreLib.Models.Settings;

public class GlobalMigrationWrapper : ILauncherMigration
{
	private readonly IGlobalsMigration _globals;

	public string MigrationName => "Global Settings";

	public GlobalMigrationWrapper(IGlobalsMigration globalMigration)
	{
		_globals = globalMigration;
	}

	public System.Threading.Tasks.Task<bool> IsMigrationRequired()
	{
		return System.Threading.Tasks.Task.FromResult<bool>(_globals.HasOldGlobalsFile());
	}

	public IObservable<MigrationStatusUpdate> MigrationSteps()
	{
		return Observable.Return(new MigrationStatusUpdate(MigrationName)).Concat<MigrationStatusUpdate>(Observable.FromAsync((Func<System.Threading.Tasks.Task>)([CompilerGenerated] () => _globals.MigrateGlobalsFile())).Select<Unit, MigrationStatusUpdate>((Func<Unit, MigrationStatusUpdate>)([CompilerGenerated] (Unit _) => new MigrationStatusUpdate(MigrationName, 1))));
	}
}
