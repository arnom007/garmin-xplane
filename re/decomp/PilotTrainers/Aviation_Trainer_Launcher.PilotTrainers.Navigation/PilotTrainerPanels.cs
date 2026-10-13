using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCenter.Wpf;
using Aviation_Trainer_Launcher.LauncherUILib.Models;
using Aviation_Trainer_Launcher.LauncherUILib.Navigation;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using Aviation_Trainer_Launcher.LauncherUILib.Views;
using Aviation_Trainer_Launcher.PilotTrainers.ViewModels;
using Aviation_Trainer_Launcher.PilotTrainers.Views;
using Aviation_Trainer_Launcher.ScreenshotModule.Navigation;
using HelpResourceModule.Navigation;
using LauncherCoreLib.Models.AbstractUi;
using LauncherCoreLib.Models.AppLifeCycle;
using VersionManagerModule.Navigation;

namespace Aviation_Trainer_Launcher.PilotTrainers.Navigation;

public class PilotTrainerPanels : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	private readonly INavigatePanels _panelNavigator;

	private readonly IDefaultPanelParameters _panelParams;

	private readonly INavBarButtonRepository _navbarRepo;

	private readonly ISignificantNounProvider _significantNouns;

	private readonly IUserPanelTracker _userPanelTracker;

	public PilotTrainerPanels(INavigatePanels panelNavigator, DefaultNavParameters panelParameters, INavBarButtonRepository buttonrepository, ISignificantNounProvider significantNouns, IUserPanelTracker userPanelTracker)
	{
		_panelNavigator = panelNavigator;
		_panelParams = panelParameters;
		_navbarRepo = buttonrepository;
		_significantNouns = significantNouns;
		base.ActionName = "Pilot Trainer navigation buttons";
		_userPanelTracker = userPanelTracker;
	}

	public async Task OnStartup(CancellationToken shutdown)
	{
		List<RegionPanel> list = CreatePanels().ToList();
		foreach (RegionPanel item in list)
		{
			item.BindNavigation(_panelNavigator);
		}
		foreach (NavBarButton item2 in list.Where((RegionPanel p) => p.ShowOnNavBar).Select(delegate(RegionPanel p)
		{
			NavParams navParams = _panelParams.GetDefaultPanelParameters(p);
			return new NavBarButton(new UserAction(p.FriendlyName, p.Description, delegate
			{
				p.NavigateToIfNotCurrent(navParams);
			}), p, navParams);
		}).ToList())
		{
			_navbarRepo.InsertButton(item2);
		}
		await InitialPanelLogic.Create(delegate(InitialPanelNavApi api)
		{
			api.PanelNavigator.NavTo(api.NavBar.CurrentNavBar[0].AssociatedPanel);
		}).AugmentIf(async (InitialPanelNavApi _) => !(await WpfAppCrashReporter.CrashOccurredLastSession().ConfigureAwait(continueOnCapturedContext: false)), (InitialPanelNavigation defaultNav) => defaultNav.RestoreUsingRepo(_userPanelTracker))(new InitialPanelNavApi(_panelNavigator, _navbarRepo)).ConfigureAwait(continueOnCapturedContext: true);
	}

	private IEnumerable<RegionPanel> CreatePanels()
	{
		yield return new RegionPanel("Avionics _Demos", "Try a combination of Garmin avionics that most closely matches your panel", typeof(SelectedAvionicsViewModel), typeof(SelectedAvionicsView));
		yield return new RegionPanel("Aircraft _Trainers", "Train with an aircraft panel configured by Garmin.", typeof(PbtPageViewModel), typeof(PbtPage));
		yield return new RegionPanel("_Settings", "Application settings", typeof(SettingsPageViewModel), typeof(SettingsPageView));
		yield return VersionManagerPanel.CreatePanel(_significantNouns.GetTrainerNoun());
		yield return ScreenshotPanel.CreatePanel();
		yield return HelpResourcesPanel.CreatePanel();
	}
}
