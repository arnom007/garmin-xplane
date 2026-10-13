using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdvancedProcessCreation;
using DataRouterMatrix;
using IO.Win32.Events;
using LOLA.Certificates;
using LOLA.Compression;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Processes;
using LauncherCoreLib.Models;
using LauncherCoreLib.Models.AbstractUi;
using LauncherCoreLib.Models.AppLifeCycle;
using LauncherCoreLib.Models.AppLifeCycle.Migrations;
using LauncherCoreLib.Models.CrashReporting;
using LauncherCoreLib.Models.CurrentMainAction;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Manuals;
using LauncherCoreLib.Models.OnlineResources;
using LauncherCoreLib.Models.Persistence;
using LauncherCoreLib.Models.Schedulers;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using LauncherCoreLib.Models.Winfs;
using LauncherCoreLib.Utility;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace LauncherCoreLib;

public class LibraryTypes : IoCTypesBase
{
	[CompilerGenerated]
	private sealed class _003CSingletons_003Ed__1 : System.Collections.Generic.IEnumerable<TypeMapping>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<TypeMapping>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private TypeMapping _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		TypeMapping System.Collections.Generic.IEnumerator<TypeMapping>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object System.Collections.IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CSingletons_003Ed__1(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ISquawkBoard, SquawkBoard>();
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IRunSimulators, SimRunnerCoordinatorAdapter>();
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ITrainingSessionSource, TrainingSessionSource>();
				_003C_003E1__state = 3;
				return true;
			case 3:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<DefaultTrainingSessionServiceProvider>(new System.Type[2]
				{
					typeof(ITrainingSessionServiceProvider),
					typeof(ITrainingSessionServiceRegistry)
				});
				_003C_003E1__state = 4;
				return true;
			case 4:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IDeploymentPathProvider, PathProviderSelector>();
				_003C_003E1__state = 5;
				return true;
			case 5:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IExecutionPathProvider, ExecutionPathProvider>();
				_003C_003E1__state = 6;
				return true;
			case 6:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IPersistencePathProvider, PersistencePathProvider>();
				_003C_003E1__state = 7;
				return true;
			case 7:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IPersistArbitraryObjects, ArbitraryObjectPersistence>();
				_003C_003E1__state = 8;
				return true;
			case 8:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IGlobalSettingsPersistence, GlobalSettingsPersistence>();
				_003C_003E1__state = 9;
				return true;
			case 9:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IManualProvider, ManualProvider>();
				_003C_003E1__state = 10;
				return true;
			case 10:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IOnlineResourceProvider, OnlineResourceProvider>();
				_003C_003E1__state = 11;
				return true;
			case 11:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IMainActionSource, MainActionSource>();
				_003C_003E1__state = 12;
				return true;
			case 12:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ICommandLineProvider, CommandLineProvider>();
				_003C_003E1__state = 13;
				return true;
			case 13:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IFlightSessionCounter, PersistedSessionCounter>();
				_003C_003E1__state = 14;
				return true;
			case 14:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ICrashReportCollector, CrashReportCollector>();
				_003C_003E1__state = 15;
				return true;
			case 15:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ISystemInfoCache, SystemInfoCache>();
				_003C_003E1__state = 16;
				return true;
			case 16:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IFlightSessionFactory, DefaultFlightSessionFactory>();
				_003C_003E1__state = 17;
				return true;
			case 17:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ISessionWorkflowProvider, DefaultWorkflowProvider>();
				_003C_003E1__state = 18;
				return true;
			case 18:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IEnhanceSession, EnhancedSessionWorkflow>();
				_003C_003E1__state = 19;
				return true;
			case 19:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ISessionRouteTableFactory, DefaultSessionRouteTableFactory>();
				_003C_003E1__state = 20;
				return true;
			case 20:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IRouterCommandConnectionFactory, RoarRouterCommandConnectionFactory>();
				_003C_003E1__state = 21;
				return true;
			case 21:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IRouterEndpointFactory, RoarEndpointFactory>();
				_003C_003E1__state = 22;
				return true;
			case 22:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IOsProcessOperations, ProcessOps>();
				_003C_003E1__state = 23;
				return true;
			case 23:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IOsWindowOperations, Win32Windows>();
				_003C_003E1__state = 24;
				return true;
			case 24:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ISimSessionFactory, DefaultSimSessionFactory>();
				_003C_003E1__state = 25;
				return true;
			case 25:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IPowerStatsRepo, PowerStatsRepo>();
				_003C_003E1__state = 26;
				return true;
			case 26:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IAvionicsRepo, AvionicsRepo>();
				_003C_003E1__state = 27;
				return true;
			case 27:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ISettingsRepo, SettingsRepo>();
				_003C_003E1__state = 28;
				return true;
			case 28:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IReactiveHangar, ReactiveHangar>();
				_003C_003E1__state = 29;
				return true;
			case 29:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ShutdownRegistry>(new System.Type[2]
				{
					typeof(IRootShutdown),
					typeof(IShutdownRegistry)
				});
				_003C_003E1__state = 30;
				return true;
			case 30:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IPendingTaskTracker, FinishPendingTasksShutdownAction>();
				_003C_003E1__state = 31;
				return true;
			case 31:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<LauncherWindow>(new System.Type[1] { typeof(IMainAppWindow) });
				_003C_003E1__state = 32;
				return true;
			case 32:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IGlobalsMigration, GlobalsMigration>();
				_003C_003E1__state = 33;
				return true;
			case 33:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<MigrationSource>(new System.Type[3]
				{
					typeof(IMigrationSource),
					typeof(IRunMigrations),
					typeof(IRunCancelableMigrations)
				});
				_003C_003E1__state = 34;
				return true;
			case 34:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ISigningCertificatesProvider, LauncherCertificates>();
				_003C_003E1__state = 35;
				return true;
			case 35:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IFileVerification, Win32FileVerification>();
				_003C_003E1__state = 36;
				return true;
			case 36:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IInstallerLauncher, InstallerLauncher>();
				_003C_003E1__state = 37;
				return true;
			case 37:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IBaseGtiRunnerProvider, BaseGtiRunnerProvider>();
				_003C_003E1__state = 38;
				return true;
			case 38:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ILauncherGtiProvider, LauncherGtiProvider>();
				_003C_003E1__state = 39;
				return true;
			case 39:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IFileTrustProvider, LayeredTrustBackFill>();
				_003C_003E1__state = 40;
				return true;
			case 40:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IRootUiAliasRegistry, RootUiAliasRegistry>();
				_003C_003E1__state = 41;
				return true;
			case 41:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IRunOnceProvider, RunOnceProvider>();
				_003C_003E1__state = 42;
				return true;
			case 42:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IRxSchedulerProvider, DefaultRxSchedulerProvider>();
				_003C_003E1__state = 43;
				return true;
			case 43:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ICrashReportInstallationIdProvider, AnonymousUserIdProvider>();
				_003C_003E1__state = 44;
				return true;
			case 44:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ISimSessionCoordinator, SimSessionCoordinator>();
				_003C_003E1__state = 45;
				return true;
			case 45:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<AircaftInstallLinkage>(new System.Type[2]
				{
					typeof(IAircraftInstallationLinker),
					typeof(IAircraftLinkedInstallations)
				});
				_003C_003E1__state = 46;
				return true;
			case 46:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ICatalogPackageRefuge, CatalogPackageRefuge>();
				_003C_003E1__state = 47;
				return true;
			case 47:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IGtiListingProvider, CachedGtiListingProvider>();
				_003C_003E1__state = 48;
				return true;
			case 48:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IInstallerListingInfoProvider, GtiStatusProvider>();
				_003C_003E1__state = 49;
				return true;
			case 49:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IRetrieveStringFromExe, RetrieveStringFromExe>();
				_003C_003E1__state = 50;
				return true;
			case 50:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<DedicatedMessagePumpThread>(System.Array.Empty<System.Type>());
				_003C_003E1__state = 51;
				return true;
			case 51:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IMessagePumpThreadDispatcher, DedicatedMessagePumpThread>();
				_003C_003E1__state = 52;
				return true;
			case 52:
				_003C_003E1__state = -1;
				return false;
			}
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<TypeMapping> System.Collections.Generic.IEnumerable<TypeMapping>.GetEnumerator()
		{
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				return this;
			}
			return new _003CSingletons_003Ed__1(0);
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<TypeMapping>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _003CTransient_003Ed__0 : System.Collections.Generic.IEnumerable<TypeMapping>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<TypeMapping>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private TypeMapping _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		TypeMapping System.Collections.Generic.IEnumerator<TypeMapping>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object System.Collections.IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CTransient_003Ed__0(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IZipCompression, DotNetZipCompression>();
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<INetworkDetector, NetworkDetector>();
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IRealityXpInstallationDetector, RealityXpDetector>();
				_003C_003E1__state = 3;
				return true;
			case 3:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IBrokenPackageReportFormatter, BrokenPackageReportFormatter>();
				_003C_003E1__state = 4;
				return true;
			case 4:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IInstallationVariableProvider, WindowsRegistryInstallationVariables>();
				_003C_003E1__state = 5;
				return true;
			case 5:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IManipulateRunningSims, SimManipulator>();
				_003C_003E1__state = 6;
				return true;
			case 6:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IProtectedPathProvider, ProtectedPathProvider>();
				_003C_003E1__state = 7;
				return true;
			case 7:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IDataRouterAssemblyTypes, LauncherDataRouterAssemblyTypes>();
				_003C_003E1__state = 8;
				return true;
			case 8:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ISimSessionRouterInstallationAdapter, SimSessionRouterInstallAdapter>();
				_003C_003E1__state = 9;
				return true;
			case 9:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IDataRouterSessionFactory, DefaultDataRouterSessionFactory>();
				_003C_003E1__state = 10;
				return true;
			case 10:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ILauncherRoarClientFactory, LauncherRoarClientFactory>();
				_003C_003E1__state = 11;
				return true;
			case 11:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IDataRouterInstallationResolver, DataRouterInstallationResolver>();
				_003C_003E1__state = 12;
				return true;
			case 12:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IDataRouterVersionResolver, DataRouterVersionResolver>();
				_003C_003E1__state = 13;
				return true;
			case 13:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IDataRouterCompatabilityChecker, DataRouterInstallationCompatabilityChecker>();
				_003C_003E1__state = 14;
				return true;
			case 14:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IFeedbackLogArchiveBuilder, CompressInMemoryLogArchiveBuilder>();
				_003C_003E1__state = 15;
				return true;
			case 15:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IWinVerifyTrustNativeApi, Win32WinVerifyTrustNativeApi>();
				_003C_003E1__state = 16;
				return true;
			case 16:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IFileSignatureParser, SignedBinarySignatureParser>();
				_003C_003E1__state = 17;
				return true;
			case 17:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IAircraftPackageInstallationProvider, AircraftPackageInstallationProvider>();
				_003C_003E1__state = 18;
				return true;
			case 18:
				_003C_003E1__state = -1;
				return false;
			}
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<TypeMapping> System.Collections.Generic.IEnumerable<TypeMapping>.GetEnumerator()
		{
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				return this;
			}
			return new _003CTransient_003Ed__0(0);
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<TypeMapping>)this).GetEnumerator();
		}
	}

	[IteratorStateMachine(typeof(_003CTransient_003Ed__0))]
	public override System.Collections.Generic.IEnumerable<TypeMapping> Transient()
	{
		yield return TypeMapping.Create<IZipCompression, DotNetZipCompression>();
		yield return TypeMapping.Create<INetworkDetector, NetworkDetector>();
		yield return TypeMapping.Create<IRealityXpInstallationDetector, RealityXpDetector>();
		yield return TypeMapping.Create<IBrokenPackageReportFormatter, BrokenPackageReportFormatter>();
		yield return TypeMapping.Create<IInstallationVariableProvider, WindowsRegistryInstallationVariables>();
		yield return TypeMapping.Create<IManipulateRunningSims, SimManipulator>();
		yield return TypeMapping.Create<IProtectedPathProvider, ProtectedPathProvider>();
		yield return TypeMapping.Create<IDataRouterAssemblyTypes, LauncherDataRouterAssemblyTypes>();
		yield return TypeMapping.Create<ISimSessionRouterInstallationAdapter, SimSessionRouterInstallAdapter>();
		yield return TypeMapping.Create<IDataRouterSessionFactory, DefaultDataRouterSessionFactory>();
		yield return TypeMapping.Create<ILauncherRoarClientFactory, LauncherRoarClientFactory>();
		yield return TypeMapping.Create<IDataRouterInstallationResolver, DataRouterInstallationResolver>();
		yield return TypeMapping.Create<IDataRouterVersionResolver, DataRouterVersionResolver>();
		yield return TypeMapping.Create<IDataRouterCompatabilityChecker, DataRouterInstallationCompatabilityChecker>();
		yield return TypeMapping.Create<IFeedbackLogArchiveBuilder, CompressInMemoryLogArchiveBuilder>();
		yield return TypeMapping.Create<IWinVerifyTrustNativeApi, Win32WinVerifyTrustNativeApi>();
		yield return TypeMapping.Create<IFileSignatureParser, SignedBinarySignatureParser>();
		yield return TypeMapping.Create<IAircraftPackageInstallationProvider, AircraftPackageInstallationProvider>();
	}

	[IteratorStateMachine(typeof(_003CSingletons_003Ed__1))]
	public override System.Collections.Generic.IEnumerable<TypeMapping> Singletons()
	{
		yield return TypeMapping.Create<ISquawkBoard, SquawkBoard>();
		yield return TypeMapping.Create<IRunSimulators, SimRunnerCoordinatorAdapter>();
		yield return TypeMapping.Create<ITrainingSessionSource, TrainingSessionSource>();
		yield return TypeMapping.Create<DefaultTrainingSessionServiceProvider>(new System.Type[2]
		{
			typeof(ITrainingSessionServiceProvider),
			typeof(ITrainingSessionServiceRegistry)
		});
		yield return TypeMapping.Create<IDeploymentPathProvider, PathProviderSelector>();
		yield return TypeMapping.Create<IExecutionPathProvider, ExecutionPathProvider>();
		yield return TypeMapping.Create<IPersistencePathProvider, PersistencePathProvider>();
		yield return TypeMapping.Create<IPersistArbitraryObjects, ArbitraryObjectPersistence>();
		yield return TypeMapping.Create<IGlobalSettingsPersistence, GlobalSettingsPersistence>();
		yield return TypeMapping.Create<IManualProvider, ManualProvider>();
		yield return TypeMapping.Create<IOnlineResourceProvider, OnlineResourceProvider>();
		yield return TypeMapping.Create<IMainActionSource, MainActionSource>();
		yield return TypeMapping.Create<ICommandLineProvider, CommandLineProvider>();
		yield return TypeMapping.Create<IFlightSessionCounter, PersistedSessionCounter>();
		yield return TypeMapping.Create<ICrashReportCollector, CrashReportCollector>();
		yield return TypeMapping.Create<ISystemInfoCache, SystemInfoCache>();
		yield return TypeMapping.Create<IFlightSessionFactory, DefaultFlightSessionFactory>();
		yield return TypeMapping.Create<ISessionWorkflowProvider, DefaultWorkflowProvider>();
		yield return TypeMapping.Create<IEnhanceSession, EnhancedSessionWorkflow>();
		yield return TypeMapping.Create<ISessionRouteTableFactory, DefaultSessionRouteTableFactory>();
		yield return TypeMapping.Create<IRouterCommandConnectionFactory, RoarRouterCommandConnectionFactory>();
		yield return TypeMapping.Create<IRouterEndpointFactory, RoarEndpointFactory>();
		yield return TypeMapping.Create<IOsProcessOperations, ProcessOps>();
		yield return TypeMapping.Create<IOsWindowOperations, Win32Windows>();
		yield return TypeMapping.Create<ISimSessionFactory, DefaultSimSessionFactory>();
		yield return TypeMapping.Create<IPowerStatsRepo, PowerStatsRepo>();
		yield return TypeMapping.Create<IAvionicsRepo, AvionicsRepo>();
		yield return TypeMapping.Create<ISettingsRepo, SettingsRepo>();
		yield return TypeMapping.Create<IReactiveHangar, ReactiveHangar>();
		yield return TypeMapping.Create<ShutdownRegistry>(new System.Type[2]
		{
			typeof(IRootShutdown),
			typeof(IShutdownRegistry)
		});
		yield return TypeMapping.Create<IPendingTaskTracker, FinishPendingTasksShutdownAction>();
		yield return TypeMapping.Create<LauncherWindow>(new System.Type[1] { typeof(IMainAppWindow) });
		yield return TypeMapping.Create<IGlobalsMigration, GlobalsMigration>();
		yield return TypeMapping.Create<MigrationSource>(new System.Type[3]
		{
			typeof(IMigrationSource),
			typeof(IRunMigrations),
			typeof(IRunCancelableMigrations)
		});
		yield return TypeMapping.Create<ISigningCertificatesProvider, LauncherCertificates>();
		yield return TypeMapping.Create<IFileVerification, Win32FileVerification>();
		yield return TypeMapping.Create<IInstallerLauncher, InstallerLauncher>();
		yield return TypeMapping.Create<IBaseGtiRunnerProvider, BaseGtiRunnerProvider>();
		yield return TypeMapping.Create<ILauncherGtiProvider, LauncherGtiProvider>();
		yield return TypeMapping.Create<IFileTrustProvider, LayeredTrustBackFill>();
		yield return TypeMapping.Create<IRootUiAliasRegistry, RootUiAliasRegistry>();
		yield return TypeMapping.Create<IRunOnceProvider, RunOnceProvider>();
		yield return TypeMapping.Create<IRxSchedulerProvider, DefaultRxSchedulerProvider>();
		yield return TypeMapping.Create<ICrashReportInstallationIdProvider, AnonymousUserIdProvider>();
		yield return TypeMapping.Create<ISimSessionCoordinator, SimSessionCoordinator>();
		yield return TypeMapping.Create<AircaftInstallLinkage>(new System.Type[2]
		{
			typeof(IAircraftInstallationLinker),
			typeof(IAircraftLinkedInstallations)
		});
		yield return TypeMapping.Create<ICatalogPackageRefuge, CatalogPackageRefuge>();
		yield return TypeMapping.Create<IGtiListingProvider, CachedGtiListingProvider>();
		yield return TypeMapping.Create<IInstallerListingInfoProvider, GtiStatusProvider>();
		yield return TypeMapping.Create<IRetrieveStringFromExe, RetrieveStringFromExe>();
		yield return TypeMapping.Create<DedicatedMessagePumpThread>(System.Array.Empty<System.Type>());
		yield return TypeMapping.Create<IMessagePumpThreadDispatcher, DedicatedMessagePumpThread>();
	}

	public override void Complex(IUnityContainer container)
	{
		container.RegisterInstance(StartupActions.Registry);
		container.RegisterInstance((IConfidentialAliasProvider)ConfidentialAliasProvider.Instance);
		RegisterFilesystem(container);
		RegisterGlobalSettingsTable(container);
		RegisterHangarRepos(container);
		RegisterUnitOfWork(container);
		RegisterRoarCompatibilityLookup(container);
		RegisterUserIdProvider(container);
		RegisterResetter(container);
		container.RegisterFactory<ILayeredFileTrustProvider>((Func<IUnityContainer, object>)delegate(IUnityContainer c)
		{
			WinVerifyTrustLayeredTrustProvider innerLayerProvider = new WinVerifyTrustLayeredTrustProvider(c.Resolve<IWinVerifyTrustNativeApi>(System.Array.Empty<ResolverOverride>()));
			IFileSignatureParser p = c.Resolve<IFileSignatureParser>(System.Array.Empty<ResolverOverride>());
			return new LauncherLayerFileTrustProvider(new SignedFileCertCrossReferenceLayeredTrustProviderV2(innerLayerProvider, p.CreateParserForSource(CertificateSource.AuthenticodeOrOtherCodeSigning), LolaCertificates.CheckForAnyPinnedCertInEntireChain(c.Resolve<ISigningCertificatesProvider>(System.Array.Empty<ResolverOverride>()))));
		}, (IFactoryLifetimeManager)null);
		container.RegisterSingleton<GtiStatusProvider>(System.Array.Empty<InjectionMember>());
		container.RegisterSingleton<CachedGtiListingProvider>(System.Array.Empty<InjectionMember>());
		container.RegisterFactory<ICachedGtiDataProvider>((Func<IUnityContainer, object>)((IUnityContainer c) => new CompositeCachedGtiDataProvider(new ICachedGtiDataProvider[2]
		{
			c.Resolve<GtiStatusProvider>(System.Array.Empty<ResolverOverride>()),
			c.Resolve<CachedGtiListingProvider>(System.Array.Empty<ResolverOverride>())
		})), (IFactoryLifetimeManager)new ContainerControlledLifetimeManager());
		container.RegisterFactory<ForkTrainer>((Func<IUnityContainer, object>)((IUnityContainer c) => TrainerProcess.CreateDirectForkStrategy(AdvancedProcess.Fork((IAdvancedProcessStartupInfo startup) => (startup is Win32ProcessStartInfoAdapter startup3) ? AdvancedProcess.CreateSuspendedProcessInAJob(c.Resolve<IOsProcessOperations>(System.Array.Empty<ResolverOverride>()), NewThreadScheduler.Default).LogForkAndExit(LogLevel.Debug, (Func<Win32ProcessStartInfoAdapter, IProcessSession, string>)(object)AdvancedProcess.ProcessNameAndId).CloseStdIn()
			.ResumeIfSuspended()
			.LogOutputs(LogLevel.Debug, (Func<Win32ProcessStartInfoAdapter, IProcessSession, string>)(object)AdvancedProcess.ProcessNameAndId)(startup3) : AdvancedProcess.ForkUsingLegacyLola(c.Resolve<IOsProcessOperations>(System.Array.Empty<ResolverOverride>()))(startup))).LogOutputs().MoveMainWindowToSpecifiedLocation(c.Resolve<IOsWindowOperations>(System.Array.Empty<ResolverOverride>()), c.Resolve<DedicatedMessagePumpThread>(System.Array.Empty<ResolverOverride>()))), (IFactoryLifetimeManager)null);
		container.RegisterFactory<IDataRouterStarter>((Func<IUnityContainer, object>)((IUnityContainer c) => new DefaultDataRouterStarter(new DataRouterProcessApi(c.Resolve<IOsProcessOperations>(System.Array.Empty<ResolverOverride>()), AdvancedProcess.Fork((IAdvancedProcessStartupInfo startup) => (startup is Win32ProcessStartInfoAdapter startup2) ? AdvancedProcess.CreateSuspendedProcessInAJob(c.Resolve<IOsProcessOperations>(System.Array.Empty<ResolverOverride>()), NewThreadScheduler.Default).LogForkAndExit(LogLevel.Debug, (Func<Win32ProcessStartInfoAdapter, IProcessSession, string>)(object)AdvancedProcess.ProcessNameAndId).ResumeIfSuspended()(startup2) : AdvancedProcess.ForkUsingLegacyLola(c.Resolve<IOsProcessOperations>(System.Array.Empty<ResolverOverride>()))(startup)), ProcessExePathHelper.FromProcessSession(c.Resolve<IOsProcessOperations>(System.Array.Empty<ResolverOverride>())).FallBackOn(ProcessExePathHelper.WmiQuery())), c.Resolve<IFileSystem>(System.Array.Empty<ResolverOverride>()), c.Resolve<IFileVerification>(System.Array.Empty<ResolverOverride>()), c.Resolve<IDataRouterAssemblyTypes>(System.Array.Empty<ResolverOverride>()), c.Resolve<IDataRouterSessionFactory>(System.Array.Empty<ResolverOverride>()))), (IFactoryLifetimeManager)new SingletonLifetimeManager());
		container.RegisterFactory<GetAircraftWinFsSessionWorkflowMode>((Func<IUnityContainer, object>)((IUnityContainer _) => WinfsSessionWorkflow.DefaultWorkflowModeSelector), (IFactoryLifetimeManager)null);
		container.RegisterFactory<GetAircraftSessionBehaviorMode>((Func<IUnityContainer, object>)((IUnityContainer _) => SerializeSessionJsonWorkflow.DefaultAircraftSessionBehaviorMode), (IFactoryLifetimeManager)null);
		container.RegisterFactory<WatchTrainerWindowRectangle>((Func<IUnityContainer, object>)((IUnityContainer c) => WindowWatcher.CreateEventWatcher(c.Resolve<IMessagePumpThreadDispatcher>(System.Array.Empty<ResolverOverride>()), c.Resolve<IOsWindowOperations>(System.Array.Empty<ResolverOverride>())).FallbackOn(WindowWatcher.CreatePollingWatcher(c.Resolve<DefaultRxSchedulerProvider>(System.Array.Empty<ResolverOverride>()).IOThreads, c.Resolve<IOsWindowOperations>(System.Array.Empty<ResolverOverride>()), TimeSpan.FromSeconds(5.0)))), (IFactoryLifetimeManager)null);
		container.RegisterFactory<GetDesiredTrainerWindow>((Func<IUnityContainer, object>)((IUnityContainer c) => (GetDesiredTrainerWindow)((ISessionEnvironment e, TrainerSessionData tsd, CancellationToken c) => System.Threading.Tasks.Task.FromResult<TrainerWindowScreenCoordinates>(tsd.InstallData.ParseWindowRect()))), (IFactoryLifetimeManager)null);
	}

	private static void RegisterUserIdProvider(IUnityContainer container)
	{
		container.RegisterFactory<IUserIdProvider>((Func<IUnityContainer, object>)((IUnityContainer _) => UniqueUserIdentifierProvider.Provider), (IFactoryLifetimeManager)null);
	}

	private static void RegisterResetter(IUnityContainer container)
	{
		container.RegisterFactory<IResetSimulators>((Func<IUnityContainer, object>)((IUnityContainer c) => new CompositeSimResetter(c.Resolve<SimulatorResetter>(System.Array.Empty<ResolverOverride>()), c.Resolve<SessionWinfsSimResetter>(System.Array.Empty<ResolverOverride>()))), (IFactoryLifetimeManager)null);
	}

	private static void RegisterFilesystem(IUnityContainer container)
	{
		container.RegisterInstance(typeof(IFileSystem), LolaFileSystem.Instance);
	}

	private static void RegisterGlobalSettingsTable(IUnityContainer container)
	{
		ContainerControlledLifetimeManager lifetimeManager = new ContainerControlledLifetimeManager();
		string name = "BaseGlobals";
		container.RegisterType<GlobalSettingTable>(lifetimeManager, System.Array.Empty<InjectionMember>());
		container.RegisterType<IGlobalSettingsProvider, GlobalSettingTable>(name, System.Array.Empty<InjectionMember>());
		lifetimeManager = new ContainerControlledLifetimeManager();
		container.RegisterType<AutoPersistGlobals>(lifetimeManager, new InjectionMember[1]
		{
			new InjectionConstructor(new object[2]
			{
				new ResolvedParameter<IGlobalSettingsProvider>(name),
				new ResolvedParameter<IGlobalSettingsPersistence>()
			})
		});
		container.RegisterType<IGlobalSettingsProvider, AutoPersistGlobals>(System.Array.Empty<InjectionMember>());
		container.RegisterType<IModifiableGlobalSettingsTable, AutoPersistGlobals>(System.Array.Empty<InjectionMember>());
	}

	private static void RegisterHangarRepos(IUnityContainer container)
	{
		container.RegisterFactory<UserHangarDirectoryRepos>((Func<IUnityContainer, object>)((IUnityContainer c) => new UserHangarDirectoryRepos(c.Resolve<ILowLevelDal>(System.Array.Empty<ResolverOverride>()))), (IFactoryLifetimeManager)new ContainerControlledLifetimeManager());
		container.RegisterType<IAircraftRepoEvolved, UserHangarDirectoryRepos>(new ContainerControlledLifetimeManager(), System.Array.Empty<InjectionMember>());
		container.RegisterType<IAircraftHistoryRepo, UserHangarDirectoryRepos>(new ContainerControlledLifetimeManager(), System.Array.Empty<InjectionMember>());
		container.RegisterType<IAircraftHistoryRepoV2, UserHangarDirectoryRepos>(new ContainerControlledLifetimeManager(), System.Array.Empty<InjectionMember>());
		container.RegisterType<IFamilyDataRepo, UserHangarDirectoryRepos>(new ContainerControlledLifetimeManager(), System.Array.Empty<InjectionMember>());
		container.RegisterType<IHangarMigrationMonitor, UserHangarDirectoryRepos>(new ContainerControlledLifetimeManager(), System.Array.Empty<InjectionMember>());
		container.RegisterType<ISimulatorRepository, UserHangarDirectoryRepos>(new ContainerControlledLifetimeManager(), System.Array.Empty<InjectionMember>());
	}

	private static void RegisterUnitOfWork(IUnityContainer container)
	{
		ContainerControlledLifetimeManager lifetimeManager = new ContainerControlledLifetimeManager();
		container.RegisterFactory<LowLevelDal>((Func<IUnityContainer, object>)((IUnityContainer c) => new LowLevelDal
		{
			Deployment = c.Resolve<IDeploymentPathProvider>(System.Array.Empty<ResolverOverride>()),
			FileSystem = c.Resolve<IFileSystem>(System.Array.Empty<ResolverOverride>()),
			Compression = c.Resolve<IZipCompression>(System.Array.Empty<ResolverOverride>()),
			ObjectPersistence = c.Resolve<IPersistArbitraryObjects>(System.Array.Empty<ResolverOverride>())
		}), (IFactoryLifetimeManager)lifetimeManager);
		container.RegisterType<ILowLevelDal, LowLevelDal>(System.Array.Empty<InjectionMember>());
		lifetimeManager = new ContainerControlledLifetimeManager();
		container.RegisterFactory<UnitOfWork>((Func<IUnityContainer, object>)((IUnityContainer c) => new UnitOfWork
		{
			LowLevel = c.Resolve<ILowLevelDal>(System.Array.Empty<ResolverOverride>()),
			Aircraft = c.Resolve<IAircraftRepoEvolved>(System.Array.Empty<ResolverOverride>()),
			Simulators = c.Resolve<ISimulatorRepository>(System.Array.Empty<ResolverOverride>()),
			FamilyData = c.Resolve<IFamilyDataRepo>(System.Array.Empty<ResolverOverride>()),
			AircraftHistory = c.Resolve<IAircraftHistoryRepoV2>(System.Array.Empty<ResolverOverride>()),
			PowerStats = c.Resolve<IPowerStatsRepo>(System.Array.Empty<ResolverOverride>()),
			Packages = c.Resolve<ITrainerPackageRepoEvolved>(System.Array.Empty<ResolverOverride>()),
			PackageRepos = c.Resolve<PackageRepositories>(System.Array.Empty<ResolverOverride>()),
			Avionics = c.Resolve<IAvionicsRepo>(System.Array.Empty<ResolverOverride>()),
			Settings = c.Resolve<ISettingsRepo>(System.Array.Empty<ResolverOverride>())
		}), (IFactoryLifetimeManager)lifetimeManager);
		container.RegisterType<IUnitOfWork, UnitOfWork>(System.Array.Empty<InjectionMember>());
	}

	private static void RegisterRoarCompatibilityLookup(IUnityContainer container)
	{
		container.RegisterFactory<IDataRouterVersionLookupProvider>((Func<IUnityContainer, object>)((IUnityContainer _) => new ConstantDataRouterVersionLookupProvider(new DataRouterVersionLookup(VersionLookupFormat.RouterBased, VersionRangeSelector.ParseJsonWithVersionKey("{\r\n\"18.1.5.0\": \"[18.1.2.0,19.2.0.0)\",\r\n\"19.2.5.1\": \"[19.2.0.0,19.4.0.0)\",\r\n\"19.5.1.0\": \"[19.2.0.0,19.9.1.0]\",\r\n}")))), (IFactoryLifetimeManager)null);
	}
}
