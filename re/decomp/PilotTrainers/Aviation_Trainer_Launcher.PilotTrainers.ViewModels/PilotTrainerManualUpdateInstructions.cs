using System.Threading;
using System.Threading.Tasks;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using LOLA.Processes;
using LauncherUpdates;

namespace Aviation_Trainer_Launcher.PilotTrainers.ViewModels;

public static class PilotTrainerManualUpdateInstructions
{
	public static ManualInstructionsFactory Create(IOsProcessOperations processes)
	{
		IOsProcessOperations processes2 = processes;
		return (ILauncherUpdateCheckResult c, LauncherBasicUpdateInfoViewModel s, CancellationToken t) => Task.FromResult((ManualUpdateInstructionViewModel)new PilotLauncherManualUpdateInstructionsViewModel(processes2));
	}
}
