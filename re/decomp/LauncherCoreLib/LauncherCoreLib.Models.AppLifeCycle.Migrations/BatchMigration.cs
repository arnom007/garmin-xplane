using System;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.AppLifeCycle.Migrations;

public sealed class BatchMigration
{
	[field: CompilerGenerated]
	public IObservable<MigrationData<MigrationStatusUpdate>> StatusUpdates
	{
		[CompilerGenerated]
		get;
	}

	public BatchMigration(IObservable<MigrationData<MigrationStatusUpdate>> statusUpdatesForbatch)
	{
		StatusUpdates = statusUpdatesForbatch.Replay<MigrationData<MigrationStatusUpdate>>(1).RefCount();
	}
}
