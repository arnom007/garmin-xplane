using Aviation_Trainer_Launcher.LauncherUILib.Navigation;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using LauncherCoreLib.Models.AppLifeCycle;

namespace Aviation_Trainer_Launcher.ViewModels;

internal class MainWindowViewModel : MainWindowViewModelBase
{
	public MainWindowViewModel(IMainAppWindow mainWindow, INavigatePanels panels, IScreenBoundaryProvider screenBoundary)
		: base(mainWindow, panels, new DefaultWindowSizeProvider(1200, 800), screenBoundary)
	{
	}
}
