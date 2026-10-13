using System;
using Aviation_Trainer_Launcher.LauncherUILib.Models;
using Aviation_Trainer_Launcher.LauncherUILib.Models.EventLogging;
using Aviation_Trainer_Launcher.PilotTrainers.Models;
using Aviation_Trainer_Launcher.PilotTrainers.Navigation;
using LOLA.Logging;
using LauncherCoreLib.Models;
using LauncherCoreLib.Models.AppLifeCycle;
using LauncherCoreLib.Models.EventLogging.BackgroundLoggers;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherSimCommandProtocol;
using LauncherUpdates;
using LruSoftwareUpdateModule.Models;
using Prism.Ioc;
using Prism.Modularity;
using TrainerUILib;
using TrainerUILib.Extensions;
using Unity;
using Unity.Resolution;

namespace Aviation_Trainer_Launcher.PilotTrainers;

[ModuleDependency("ModuleInfo")]
[ModuleDependency("VersionManagerModuleInfo")]
[ModuleDependency("ScreenshotModuleInfo")]
[ModuleDependency("HelpModule")]
public class PilotTrainersModuleInfo : IModule
{
	private readonly IUnityContainer _container;

	public PilotTrainersModuleInfo(IUnityContainer container)
	{
		_container = container;
	}

	public void OnInitialized(IContainerProvider containerProvider)
	{
		Logger.Log(LogLevel.Info, "Initializing Pilot Trainer module.");
		AddPilotStylesToApplication();
		ITrainingSessionServiceRegistry trainingSessionServiceRegistry = _container.Resolve<ITrainingSessionServiceRegistry>(Array.Empty<ResolverOverride>());
		trainingSessionServiceRegistry.RegisterServiceFactory(serviceFactory: _container.Resolve<TrainingSessionRdmpProviderFactory>(Array.Empty<ResolverOverride>()), targetType: typeof(ITrainingSessionRdmpProvider));
		trainingSessionServiceRegistry.RegisterServiceFactory(serviceFactory: _container.Resolve<TrainingSessionLscpProviderFactory>(Array.Empty<ResolverOverride>()), targetType: typeof(ITrainingSessionLauncherSimMasterProvider));
		trainingSessionServiceRegistry.RegisterServiceFactory(serviceFactory: _container.Resolve<TrainingSessionLauncherSimCommanderProviderFactory>(Array.Empty<ResolverOverride>()), targetType: typeof(ITrainingSessionLauncherSimCommanderProvider));
		_container.Resolve<IModifiableGlobalSettingsTable>(Array.Empty<ResolverOverride>()).AddAutoUpdateCheckSetting();
		_container.Resolve<IMainAppWindow>(Array.Empty<ResolverOverride>()).WindowTitle = "Garmin Aviation Trainer Launcher";
		StartupActions.StartAndThrowOnImmediateError(_container.Resolve<PilotMigrationStartupAction>(Array.Empty<ResolverOverride>()), _container.Resolve<PilotTrainerPanels>(Array.Empty<ResolverOverride>()), _container.Resolve<BackgroundLauncherUpdateChecks>(Array.Empty<ResolverOverride>()), _container.Resolve<BackgroundCatalogUpdateChecks>(Array.Empty<ResolverOverride>()), _container.Resolve<AutomaticUpdateCheckAdvertisement>(Array.Empty<ResolverOverride>()), _container.Resolve<LogTrainerToAppCenterStartupAction>(Array.Empty<ResolverOverride>()), _container.Resolve<LogLifecycleEventsStartupAction>(Array.Empty<ResolverOverride>()), _container.Resolve<LogUIEventsStartupAction>(Array.Empty<ResolverOverride>()), _container.Resolve<BackgroundSimWindowWatcherAction>(Array.Empty<ResolverOverride>()), _container.Resolve<StoreCurrentPageAsItChanges>(Array.Empty<ResolverOverride>()));
	}

	public void RegisterTypes(IContainerRegistry containerRegistry)
	{
		IoCRegistration.RegisterTypes(_container, new PilotTrainerTypes());
	}

	private void AddPilotStylesToApplication()
	{
		_container.Resolve<IMergeDictionaryRegistry>(Array.Empty<ResolverOverride>()).AddDictionaryResource(PackUrl.FormatPackUri("PilotTrainers", "/PilotTrainerStyles.xaml"));
	}
}
