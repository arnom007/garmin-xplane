using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using Aviation_Trainer_Launcher.PilotTrainers.Models;
using LauncherCoreLib.Models.AbstractUi;
using LauncherCoreLib.Models.AppLifeCycle;
using LruSoftwareUpdateModule.Models;

namespace Aviation_Trainer_Launcher.PilotTrainers;

public static class PilotAppLifecycleActionNames
{
	public static FormatUiActionName PilotAmendment(ISignificantNounProvider significantNouns)
	{
		ISignificantNounProvider significantNouns2 = significantNouns;
		return delegate(IAppLifeCycleAction a)
		{
			if (a is CreateDefaultAircraftStartupAction)
			{
				return "Creating default " + significantNouns2.GetAircraftNoun().Plural + "...";
			}
			if (a is BackgroundCatalogUpdateChecks)
			{
				return "Checking for " + significantNouns2.GetTrainerNoun().Singular + " updates...";
			}
			return (a is PilotMigrationStartupAction) ? ("Migrating " + significantNouns2.GetAircraftNoun().Plural + " data...") : string.Empty;
		};
	}
}
