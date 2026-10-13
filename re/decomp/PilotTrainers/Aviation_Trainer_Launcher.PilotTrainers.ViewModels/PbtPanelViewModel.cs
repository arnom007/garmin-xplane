using Aviation_Trainer_Launcher.LauncherUILib.Navigation;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using LauncherCoreLib.Models.Simulated.Airport;

namespace Aviation_Trainer_Launcher.PilotTrainers.ViewModels;

public class PbtPanelViewModel : InstrumentPanelViewModel
{
	public PbtPanelViewModel(INavigatePanels navigator, ISquawkBoard squawks, IWizardDialogService wizardService)
		: base(navigator, squawks, wizardService)
	{
		base.DisplayEmptySlots = false;
		base.CanChangeSlots = false;
	}
}
