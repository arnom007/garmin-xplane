using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Logging;
using LauncherCoreLib.Models.AppLifeCycle;
using LauncherCoreLib.Models.AppLifeCycle.Migrations;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models;

public class PilotMigrationStartupAction : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	private readonly IRunCancelableMigrations _migrationRunner;

	private readonly HangarMigrationCoverUp _hangarMigrationCoverUp;

	public PilotMigrationStartupAction(IRunCancelableMigrations migrations, HangarMigrationCoverUp hangarConverUp)
	{
		_migrationRunner = migrations;
		_hangarMigrationCoverUp = hangarConverUp;
	}

	public async Task OnStartup(CancellationToken shutdown)
	{
		using (shutdown.Register(delegate
		{
			Log(LogLevel.Warning, "Shutdown initiated during migration. Data corruption may occur if a shutdown timeout elapses.");
		}, useSynchronizationContext: false))
		{
			await _migrationRunner.ScheduleMigrations(shutdown, _hangarMigrationCoverUp).StatusUpdates.LastOrDefaultAsync().ToTask().ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
