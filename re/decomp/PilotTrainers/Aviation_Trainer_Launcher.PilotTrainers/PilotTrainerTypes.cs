using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using AppCenter.EventLogging;
using Aviation_Trainer_Launcher.LauncherAdvisories;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using Aviation_Trainer_Launcher.PilotTrainers.Models;
using Aviation_Trainer_Launcher.PilotTrainers.Models.Advisories;
using Aviation_Trainer_Launcher.PilotTrainers.Models.EventLogging.DataDescriptors;
using Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;
using Aviation_Trainer_Launcher.PilotTrainers.ViewModels;
using CommonAdvisories;
using Domain.PurposeBuiltTraining;
using FeatureMatrix;
using FeedbackModule.Models;
using IO.AircraftDescriptor;
using LOLA.Processes;
using LOLA.Redux;
using LanguageExt;
using LauncherCoreLib.Models;
using LauncherCoreLib.Models.AbstractUi;
using LauncherCoreLib.Models.AutoUpdate;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.EventLogging.BackgroundLoggers;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.TrainerPackages;
using LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;
using LauncherCoreLib.Models.Winfs;
using LauncherFeatureMatrix;
using LauncherFeatureMatrixAdvisories;
using LruSoftwareUpdateModule.Models;
using LruSoftwareUpdateModule.ViewModels;
using PilotTrainers.Models;
using TrainerPackageLib;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Aviation_Trainer_Launcher.PilotTrainers;

public class PilotTrainerTypes : IoCTypesBase
{
	public override IEnumerable<TypeMapping> Transient()
	{
		yield return TypeMapping.Create<IInternalMonitorUtilities, MonitorUtilities>();
		yield return TypeMapping.Create<IPackageFeatureCompatibilityResolverProvider, PackageCompatibilityResolverProvider>();
		yield return TypeMapping.Create<IAutomaticLegacyPackageUpgrades, LegacyTrainerPackageUpgrades>();
		yield return TypeMapping.Create<IPackageScanner, NoopScanner>();
		yield return TypeMapping.Create<IPackageReferenceFilter, AutomaticBugfixCompatibilityGate>();
		yield return TypeMapping.Create<IAircraftPackageUpdater, PilotAircraftPackageUpdater>();
		yield return TypeMapping.Create<HangarViewModel, PilotTrainerHangarViewModel>();
		yield return TypeMapping.Create<IAircraftProvider, AircraftProvider>();
	}

	public override IEnumerable<TypeMapping> Singletons()
	{
		yield return TypeMapping.Create<IAutomaticShutdownStrategy, ShutdownWhenAllSimsExit>();
	}

	public override void Complex(IUnityContainer container)
	{
		IUnityContainer container2 = container;
		container2.RegisterFactory<ISignificantNounProvider>((IUnityContainer c) => c.Resolve<SignificantNounProvider>(Array.Empty<ResolverOverride>()).SetLauncherNoun(new SignificantNoun("Launcher", "Launchers")).SetTrainerNoun(new SignificantNoun("Trainer", "Trainers"))
			.SetAircraftNoun(new SignificantNoun("Aircraft", "Aircraft")), new ContainerControlledLifetimeManager());
		RegisterAdvancedAdvisories(container2);
		container2.RegisterFactory<CompositeAirworthiness>((IUnityContainer c) => new CompositeAirworthiness(c.Resolve<BasicAirworthiness>(Array.Empty<ResolverOverride>()), c.Resolve<ErrorAdvisoryAirworthiness>(Array.Empty<ResolverOverride>()), c.Resolve<PbtLocalAircraftAirworthiness>(Array.Empty<ResolverOverride>())), new ContainerControlledLifetimeManager());
		container2.RegisterType<IAirworthiness, CompositeAirworthiness>(Array.Empty<InjectionMember>());
		container2.RegisterPanelFactoryAndTap(() => container2.Resolve<PilotTrainerInstrumentPanelViewModel>(Array.Empty<ResolverOverride>()));
		container2.RegisterFactory<ILauncherCoreFeatureProvider>((IUnityContainer c) => new LauncherCoreFeatureProvider(CoreLauncherFeatures.CreateLauncherCoreFeatureId(CoreLauncherFeatures.ParseSuiteFeatureId(c.Resolve<ISuiteNameProvider>(Array.Empty<ResolverOverride>()).SuiteName))));
		container2.RegisterFactory<ILauncherFeatureSetProvider>(delegate(IUnityContainer c)
		{
			ILauncherCoreFeatureProvider launcherCoreFeatureProvider = c.Resolve<ILauncherCoreFeatureProvider>(Array.Empty<ResolverOverride>());
			return new ConstFeatureSetProvider(FeatureSetBuilder.Start().AddFeature(launcherCoreFeatureProvider.LauncherCoreFeatureId, "[1.0]", "Pilot Trainer Launcher feature set. This is not tied to the launcher assembly version.").AddPinnedCertificateListFeature(CoreLauncherFeatures.ParseSuiteFeatureId(c.Resolve<ISuiteNameProvider>(Array.Empty<ResolverOverride>()).SuiteName))
				.AddFeature(FeatureIdentifier.Parse("Multi_Turbo_Aircraft"), "[1.0]", "This launcher lists Multi-Turbo in the available aircraft list.")
				.AddFeature(FeatureIdentifier.Parse("Multi_Turbofan_Aircraft"), "[1.0]", "This launcher lists Multi-Turbofan in the available aircraft list.")
				.ToFeatureSet());
		});
		container2.RegisterSingleton<IManifestValidator, PreV3LauncherFeaturesValidator>("PreLauncherFeatures", Array.Empty<InjectionMember>());
		string server = "https://trainers-feedback-aviation.garmin.com";
		string destinationToken = "55E608E6-9E4D-484E-8F36-04739C2D06CB";
		container2.RegisterFactory<ITrainerFeedbackApiConfiguration>((IUnityContainer c) => new FeedbackApiConfiguration(c.Resolve<ISuiteNameProvider>(Array.Empty<ResolverOverride>()).SuiteName, "Launcher", destinationToken, new Uri(server)));
		ContainerControlledLifetimeManager lifetimeManager = new ContainerControlledLifetimeManager();
		container2.RegisterFactory<IUpdateUrlProvider>((IUnityContainer c) => new HourlyCacheUpdateFeedProvider(new DevelopmentUpdateFeedDecorator(new ImmutableUrlFeedProvider(ModuleSuiteConstants.SouffleUrl), c.Resolve<ICommandLineProvider>(Array.Empty<ResolverOverride>()))), lifetimeManager);
		container2.RegisterFactory<FormatUiActionName>((IUnityContainer c) => AppLifecycleActionUiNames.CreateCoreActionNamer(c.Resolve<ISignificantNounProvider>(Array.Empty<ResolverOverride>())).Amend(PilotAppLifecycleActionNames.PilotAmendment(c.Resolve<ISignificantNounProvider>(Array.Empty<ResolverOverride>()))));
		container2.RegisterFactory<IProvideLegacyAppId>((IUnityContainer _) => new ProvideLegacyAppId(typeof(PilotTrainersModuleInfo).Assembly, "legacy_app_center_apps.json"));
		container2.RegisterFactory<SimSettingsInjector>((IUnityContainer _) => AircraftSettingsDataDescriptor.AircraftSettingsInjector());
		container2.RegisterFactory<PbtLocalAircraftRecognizer>((IUnityContainer c) => PbtAircraft.CreateRecognizer());
		container2.RegisterFactory<GetAircraftWinFsSessionWorkflowMode>((IUnityContainer c) => PbtAircraft.CreatePbtWinfsModeSelector(c.Resolve<PbtLocalAircraftRecognizer>(Array.Empty<ResolverOverride>())));
		container2.RegisterFactory<GetAircraftSessionBehaviorMode>((IUnityContainer c) => PbtAircraft.CreatePbtSessionProp(c.Resolve<PbtLocalAircraftRecognizer>(Array.Empty<ResolverOverride>())));
		container2.RegisterFactory<LocalAircraftIdentifierLookup>((IUnityContainer c) => PbtAircraft.CreateLocalAircraftLookup());
		container2.RegisterFactory<PbtLocalAircraftLocator>((IUnityContainer c) => PbtAircraft.CreateLocalAircraftLocator(c.Resolve<IUnitOfWork>(Array.Empty<ResolverOverride>()), c.Resolve<LocalAircraftIdentifierLookup>(Array.Empty<ResolverOverride>()), c.Resolve<PbtLocalAircraftRecognizer>(Array.Empty<ResolverOverride>()), c.Resolve<PbtAircraftBugFixUpdateChecker>(Array.Empty<ResolverOverride>())));
		container2.RegisterFactory<PurposeBuiltAircraftCreator>((IUnityContainer c) => PbtAircraft.BuildPurposeBuiltAircraftCreator(c.Resolve<IUnitOfWork>(Array.Empty<ResolverOverride>()), c.Resolve<LocalAircraftIdentifierLookup>(Array.Empty<ResolverOverride>())));
		container2.RegisterFactory<PurposeBuiltAircraftResetter>((IUnityContainer c) => PbtAircraft.BuildPurposeBuiltAircraftResetter(c.Resolve<IUnitOfWork>(Array.Empty<ResolverOverride>()), c.Resolve<LocalAircraftIdentifierLookup>(Array.Empty<ResolverOverride>()), c.Resolve<PurposeBuiltAircraftCreator>(Array.Empty<ResolverOverride>())));
		container2.RegisterFactory<PbtAircraftBugFixUpdateChecker>((IUnityContainer c) => PbtAircraft.BugFixUpdateChecker(c.Resolve<IAircraftPackageReferenceLocker>(Array.Empty<ResolverOverride>()), c.Resolve<IPackageReferenceFilter>(Array.Empty<ResolverOverride>())));
		container2.RegisterFactory<PbtActionFactories>((IUnityContainer c) => new PbtActionFactories(PbtReduxMiddleWare.CreateResetTrainingItemAction(c.Resolve<PurposeBuiltAircraftResetter>(Array.Empty<ResolverOverride>()))));
		container2.RegisterFactory<PbtReduxStore>((IUnityContainer c) => PbtReduxStore.CreatePackageStore(MiddleWare.Apply<PbtPageState>(MiddleWare.Thunk<PbtPageState>(), MiddleWare.RxActionTap(PbtReduxMiddleWare.InitialLoad(c.Resolve<IAircraftProvider>(Array.Empty<ResolverOverride>()))), MiddleWare.RxActionTap(PbtReduxMiddleWare.LocalAircraftSubscription(c.Resolve<IUnitOfWork>(Array.Empty<ResolverOverride>()), c.Resolve<LocalAircraftIdentifierLookup>(Array.Empty<ResolverOverride>()), c.Resolve<PbtLocalAircraftLocator>(Array.Empty<ResolverOverride>()))), MiddleWare.RxActionTap(PbtReduxMiddleWare.LoadOnSelection(c.Resolve<PbtLocalAircraftLocator>(Array.Empty<ResolverOverride>()), c.Resolve<PurposeBuiltAircraftCreator>(Array.Empty<ResolverOverride>()))), MiddleWare.RxActionTap(PbtReduxMiddleWare.AutoMigrateMissingBugFixPackageReferences(c.Resolve<IUnitOfWork>(Array.Empty<ResolverOverride>()), c.Resolve<LocalAircraftIdentifierLookup>(Array.Empty<ResolverOverride>()), c.Resolve<PbtAircraftBugFixUpdateChecker>(Array.Empty<ResolverOverride>()), c.Resolve<PbtLocalAircraftLocator>(Array.Empty<ResolverOverride>()))), MiddleWare.RxActionTap(PbtReduxMiddleWare.UpdatePbtRunningItem(() => c.Resolve<ISimSessionCoordinator>(Array.Empty<ResolverOverride>()), c.Resolve<PbtLocalAircraftRecognizer>(Array.Empty<ResolverOverride>()))), MiddleWare.RxActionTap(PbtReduxMiddleWare.StoreAndLoadLastSelectedItem(c.Resolve<ILowLevelDal>(Array.Empty<ResolverOverride>()))), MiddleWare.ThreadedDispatch<PbtPageState>(ViewModelSchedulers.UIThread), MiddleWare.Identity<PbtPageState>())), new SingletonLifetimeManager());
		container2.RegisterFactory<PbtAircraftApi>((IUnityContainer c) => new PbtAircraftApi(c.Resolve<PbtReduxStore>(Array.Empty<ResolverOverride>()), c.Resolve<PbtActionFactories>(Array.Empty<ResolverOverride>())));
	}

	private static void RegisterAdvancedAdvisories(IUnityContainer container)
	{
		container.RegisterFactory<IContextualProvider<ReactiveAircraft, IAdvisoryMonitor>>((IUnityContainer c) => new CachedContextualProvider<ReactiveAircraft, AircraftIdentifier, IAdvisoryMonitor>(FunctionalContextualProvider.Create((FunctionalContextualProvider<ReactiveAircraft, IAdvisoryMonitor>.FactoryMethod)delegate(ReactiveAircraft aircraft)
		{
			ReactiveAircraft aircraft3 = aircraft;
			return new CompositeAdvisoryMonitor(FunctionalMonitor.Create(() => c.Resolve<GetGoodComputerAdvisoryMonitor>(Array.Empty<ResolverOverride>()).MonitorAircraft(aircraft3, c.Resolve<IInternalMonitorUtilities>(Array.Empty<ResolverOverride>()))), FunctionalMonitor.Create(() => c.Resolve<DataRouterAdvisoryMonitor>(Array.Empty<ResolverOverride>()).MonitorAircraft(aircraft3, c.Resolve<IInternalMonitorUtilities>(Array.Empty<ResolverOverride>()))), FunctionalMonitor.Create(() => c.Resolve<NoActiveSimsInAircraftAdvisoryMonitor>(Array.Empty<ResolverOverride>()).MonitorAircraft(aircraft3, c.Resolve<IInternalMonitorUtilities>(Array.Empty<ResolverOverride>()))), FunctionalMonitor.Create(() => c.Resolve<DownloadRequiredPackagesAdvisoryMonitor>(Array.Empty<ResolverOverride>()).MonitorAircraft(aircraft3, c.Resolve<IInternalMonitorUtilities>(Array.Empty<ResolverOverride>()))), FunctionalMonitor.Create(() => c.Resolve<BrokenPackagesAdvisoryMonitor>(Array.Empty<ResolverOverride>()).MonitorAircraft(aircraft3, c.Resolve<IInternalMonitorUtilities>(Array.Empty<ResolverOverride>()))), FunctionalMonitor.Create(() => c.Resolve<AircraftPackageCompatibilityAdvisoryMonitor>(Array.Empty<ResolverOverride>()).MonitorAircraft(aircraft3, c.Resolve<IInternalMonitorUtilities>(Array.Empty<ResolverOverride>()))), FunctionalMonitor.Create(() => c.Resolve<EmbeddedReleaseUpdateAdvisoryMonitor>(Array.Empty<ResolverOverride>()).MonitorAircraft(aircraft3, c.Resolve<IInternalMonitorUtilities>(Array.Empty<ResolverOverride>()))), FunctionalMonitor.Create(() => c.Resolve<BugFixUpdateAdvisoryMonitor>(Array.Empty<ResolverOverride>()).MonitorAircraft(aircraft3, c.Resolve<IInternalMonitorUtilities>(Array.Empty<ResolverOverride>()))));
		}), ReactiveAircraftCacheKeySelector, ReactiveAircraftCacheEviction), new ContainerControlledLifetimeManager());
		container.RegisterFactory<IContextualProvider<ReactiveAircraft, IAdvisoryCache>>((IUnityContainer _) => new CachedContextualProvider<ReactiveAircraft, AircraftIdentifier, IAdvisoryCache>(FunctionalContextualProvider.Create((FunctionalContextualProvider<ReactiveAircraft, IAdvisoryCache>.FactoryMethod)((ReactiveAircraft __) => new AdvisoryCache())), ReactiveAircraftCacheKeySelector, ReactiveAircraftCacheEviction), new ContainerControlledLifetimeManager());
		container.RegisterFactory<IContextualProvider<ReactiveAircraft, IAdvisoryUserActionLog>>((Func<IUnityContainer, object>)delegate
		{
			IdBasedActionLog value2 = new IdBasedActionLog();
			Dictionary<string, IAdvisoryUserActionLog> sharedMap2 = new Dictionary<string, IAdvisoryUserActionLog>
			{
				{ "HIGH_PERFORMANCE_COMPUTER_NEEDED", value2 },
				{ "BROKEN_PACKS_FOR_AIRCRAFT", value2 }
			};
			return new CachedContextualProvider<ReactiveAircraft, AircraftIdentifier, IAdvisoryUserActionLog>(FunctionalContextualProvider.Create((FunctionalContextualProvider<ReactiveAircraft, IAdvisoryUserActionLog>.FactoryMethod)((ReactiveAircraft __) => new LoggingUserActionLog(MappingActionLog.Create(sharedMap2, new InMemAdvisoryActionLog())))), ReactiveAircraftCacheKeySelector, ReactiveAircraftCacheEviction);
		}, (IFactoryLifetimeManager)new ContainerControlledLifetimeManager());
		container.RegisterFactory<IBasicAdvisoryActionFactory>((IUnityContainer c) => new AggregateAdvisoryActionFactory(c.Resolve<CommonAdvisoryActionFactory>(Array.Empty<ResolverOverride>()), c.Resolve<PilotTrainerActionFactory>(Array.Empty<ResolverOverride>())));
		container.RegisterFactory<IAdvisoryUiContentFactory>((IUnityContainer c) => new CompositeAdvisoryUiContentFactory(c.Resolve<BasicAdvisoryUiFactory>(Array.Empty<ResolverOverride>()), c.Resolve<BrokenPackagesUiFactory>(Array.Empty<ResolverOverride>()), c.Resolve<DownloadRequiredPackageAdvisoryUiFactory>(Array.Empty<ResolverOverride>()), c.Resolve<LruSoftwareUpdateAdvisoryUiFactory>(Array.Empty<ResolverOverride>()), c.Resolve<AircraftPackageCompatibilityAdvisoryUiFactory>(Array.Empty<ResolverOverride>()), c.Resolve<PbtAdvisoryUiFactory>(Array.Empty<ResolverOverride>())));
		container.RegisterFactory<IAutoExpandAdvisoryExpanderBehavior>((IUnityContainer c) => new CompositeAutoExpandBehavior(c.Resolve<AutoExpandOnErrorBehavior>(Array.Empty<ResolverOverride>()), c.Resolve<AutoExpandOnRequiredDownloadsBehavior>(Array.Empty<ResolverOverride>()), c.Resolve<AutoExpandOnUpdateBehavior>(Array.Empty<ResolverOverride>())));
		container.RegisterFactory<ManualInstructionsFactory>((IUnityContainer c) => PilotTrainerManualUpdateInstructions.Create(c.Resolve<IOsProcessOperations>(Array.Empty<ResolverOverride>())));
		container.RegisterFactory<AdvisoryUnitOfWork<PbtAdvisoryContext>>(delegate(IUnityContainer c)
		{
			FunctionalContextualProvider<PbtAdvisoryContext, IAdvisoryMonitor> advisories = FunctionalContextualProvider.Create<PbtAdvisoryContext, IAdvisoryMonitor>(CreatePurposeBuiltStateMonitors);
			CachedContextualProvider<PbtAdvisoryContext, TrainingItemKey, IAdvisoryCache> caches = new CachedContextualProvider<PbtAdvisoryContext, TrainingItemKey, IAdvisoryCache>(FunctionalContextualProvider.Create((FunctionalContextualProvider<PbtAdvisoryContext, IAdvisoryCache>.FactoryMethod)((PbtAdvisoryContext __) => new AdvisoryCache())), KeySelector, CacheEviction);
			IdBasedActionLog value = new IdBasedActionLog();
			Dictionary<string, IAdvisoryUserActionLog> sharedMap = new Dictionary<string, IAdvisoryUserActionLog>
			{
				{ "HIGH_PERFORMANCE_COMPUTER_NEEDED", value },
				{ "BROKEN_PACKS_FOR_AIRCRAFT", value }
			};
			CachedContextualProvider<PbtAdvisoryContext, TrainingItemKey, IAdvisoryUserActionLog> actions = new CachedContextualProvider<PbtAdvisoryContext, TrainingItemKey, IAdvisoryUserActionLog>(FunctionalContextualProvider.Create((FunctionalContextualProvider<PbtAdvisoryContext, IAdvisoryUserActionLog>.FactoryMethod)((PbtAdvisoryContext __) => new LoggingUserActionLog(MappingActionLog.Create(sharedMap, new InMemAdvisoryActionLog())))), KeySelector, CacheEviction);
			return new AdvisoryUnitOfWork<PbtAdvisoryContext>(advisories, caches, actions);
			IEnumerable<IAdvisoryMonitor> CreateLocalAircraftMonitors(ReactiveAircraft? aircraft)
			{
				ReactiveAircraft aircraft2 = aircraft;
				if (aircraft2 != null)
				{
					IInternalMonitorUtilities util = c.Resolve<IInternalMonitorUtilities>(Array.Empty<ResolverOverride>());
					yield return FunctionalMonitor.Create(() => c.Resolve<GetGoodComputerAdvisoryMonitor>(Array.Empty<ResolverOverride>()).MonitorAircraft(aircraft2, util));
					yield return FunctionalMonitor.Create(() => c.Resolve<DownloadRequiredPackagesAdvisoryMonitor>(Array.Empty<ResolverOverride>()).MonitorAircraft(aircraft2, util));
					yield return FunctionalMonitor.Create(() => c.Resolve<AircraftPackageCompatibilityAdvisoryMonitor>(Array.Empty<ResolverOverride>()).MonitorAircraft(aircraft2, util));
					yield return FunctionalMonitor.Create(() => c.Resolve<BugFixUpdateAdvisoryMonitor>(Array.Empty<ResolverOverride>()).MonitorAircraft(aircraft2, util));
				}
			}
			IAdvisoryMonitor CreatePurposeBuiltStateMonitors(PbtAdvisoryContext context)
			{
				PbtAdvisoryContext context2 = context;
				Seq<IAdvisoryMonitor> empty = Seq<IAdvisoryMonitor>.Empty;
				empty = empty.Add(FunctionalMonitor.Create(() => PbtAdvisories.DetectedAdvisoriesOverTime(c.Resolve<IInternalMonitorUtilities>(Array.Empty<ResolverOverride>()), context2)));
				if ((object)context2.AircraftState != null && !context2.AircraftState.LocalRepresentation.IsBroken)
				{
					empty = empty.Concat(CreateLocalAircraftMonitors(context2.LocalAircraft));
				}
				return new CompositeAdvisoryMonitor(empty);
			}
		}, new SingletonLifetimeManager());
		container.RegisterFactory<GetScreenRectangle>((IUnityContainer c) => ScreenRectangleProvider.Instance);
		container.RegisterFactory<GetDesiredTrainerWindow>((IUnityContainer c) => PbtPanelLayout.CreateSimWindowLocator(c.Resolve<PbtAircraftApi>(Array.Empty<ResolverOverride>()), c.Resolve<PbtLocalAircraftRecognizer>(Array.Empty<ResolverOverride>()), c.Resolve<GetScreenRectangle>(Array.Empty<ResolverOverride>())));
		static Task CacheEviction(PbtAdvisoryContext c)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return new TaskCompletionSource().Task;
		}
		static TrainingItemKey KeySelector(PbtAdvisoryContext c)
		{
			return c.Key;
		}
	}

	private static IEnumerable<IAirworthiness> DelayResolution(params Func<IAirworthiness>[] factoryMethods)
	{
		foreach (Func<IAirworthiness> func in factoryMethods)
		{
			yield return func();
		}
	}

	private static AircraftIdentifier ReactiveAircraftCacheKeySelector(ReactiveAircraft aircraft)
	{
		return aircraft.AircraftIdentifier;
	}

	private static Task ReactiveAircraftCacheEviction(ReactiveAircraft aircraft)
	{
		return aircraft.WhenAircraftIsDeleted().FirstAsync().ToTask();
	}
}
