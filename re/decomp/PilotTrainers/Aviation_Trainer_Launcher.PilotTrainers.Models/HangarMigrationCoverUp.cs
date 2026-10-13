using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Tasks;
using LauncherCoreLib.Models.AppLifeCycle.Migrations;
using LauncherCoreLib.Models.Simulated.Airport;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models;

public class HangarMigrationCoverUp : MigrationBase, ICancelableMigration, ILauncherMigration
{
	private readonly IHangarMigrationMonitor _hangarMigrationMonitor;

	public HangarMigrationCoverUp(IHangarMigrationMonitor hangarMigrationMonitor)
		: base("Hangar migration cover up")
	{
		_hangarMigrationMonitor = hangarMigrationMonitor;
	}

	public override async Task<bool> IsMigrationRequired()
	{
		return await _hangarMigrationMonitor.AreMigrationsNeeded().ConfigureAwait(continueOnCapturedContext: false);
	}

	public IObservable<MigrationStatusUpdate> MigrationSteps(CancellationToken token)
	{
		return Observable.Create(async delegate(IObserver<MigrationStatusUpdate> o, CancellationToken t)
		{
			using ICancellation combined = CancellationFactory.CreateLinked(t, token);
			MigrationHarness harness = new MigrationHarness(o, base.MigrationName, combined.Token);
			Exception ex = await DoMigration(harness).DetachContext();
			if (ex != null)
			{
				o.OnError(ex);
			}
			else
			{
				o.OnCompleted();
			}
		});
	}

	protected override async Task<Exception?> DoMigration(MigrationHarness harness)
	{
		harness.CurrentStatus = new MigrationStatusUpdate("Updating aircraft data from previous Launcher.");
		harness.ReportCurrentStatus();
		try
		{
			await _hangarMigrationMonitor.StartMigrations(harness.CancelToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception result)
		{
			return result;
		}
		return null;
	}
}
