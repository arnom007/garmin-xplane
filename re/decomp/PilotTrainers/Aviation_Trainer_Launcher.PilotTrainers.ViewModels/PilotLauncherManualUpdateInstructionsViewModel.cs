using System;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using LOLA.Processes;
using Prism.Commands;

namespace Aviation_Trainer_Launcher.PilotTrainers.ViewModels;

public class PilotLauncherManualUpdateInstructionsViewModel : ManualUpdateInstructionViewModel
{
	public DelegateCommand OpenGarminPortalCommand { get; }

	public PilotLauncherManualUpdateInstructionsViewModel(IOsProcessOperations processes)
	{
		IOsProcessOperations processes2 = processes;
		base._002Ector();
		OpenGarminPortalCommand = new DelegateCommand(AvoidDoubleClicks(delegate
		{
			OpenPortalInBrowser(processes2);
		}));
	}

	private static void OpenPortalInBrowser(IOsProcessOperations processes)
	{
		processes.ForkFromCurrentDirectory("explorer.exe", "\"https://www8.garmin.com/support/download_details.jsp?id=12373\"").Dispose();
	}

	private static Action AvoidDoubleClicks(Action a)
	{
		Action a2 = a;
		DateTimeOffset lastRun = DateTimeOffset.Now;
		return delegate
		{
			DateTimeOffset now = DateTimeOffset.Now;
			if (now - lastRun > TimeSpan.FromSeconds(1.0))
			{
				lastRun = now;
				a2();
			}
		};
	}
}
