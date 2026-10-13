using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using Aviation_Trainer_Launcher.LauncherUILib;
using Aviation_Trainer_Launcher.Models;
using Aviation_Trainer_Launcher.PilotTrainers;
using Aviation_Trainer_Launcher.PilotTrainers.Models;
using Aviation_Trainer_Launcher.ScreenshotModule;
using Aviation_Trainer_Launcher.Views;
using FeedbackModule;
using HelpResourceModule;
using LauncherCoreLib.Models;
using LauncherCoreLib.Models.AppLifeCycle;
using LauncherCoreLib.Models.AppLifeCycle.Migrations;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.TrainerPackages;
using LruSoftwareUpdateModule;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Unity;
using VersionManagerModule;

namespace Aviation_Trainer_Launcher;

public class TrainerLauncherApp : PrismUnityLauncherAppBase
{
	[CompilerGenerated]
	private sealed class _003CResolveShutdownActions_003Ed__8 : System.Collections.Generic.IEnumerable<IShutdownAction>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<IShutdownAction>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private IShutdownAction _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		public TrainerLauncherApp _003C_003E4__this;

		IShutdownAction System.Collections.Generic.IEnumerator<IShutdownAction>.Current
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
		public _003CResolveShutdownActions_003Ed__8(int _003C_003E1__state)
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
			int num = _003C_003E1__state;
			TrainerLauncherApp trainerLauncherApp = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = trainerLauncherApp.Container.Resolve<FinishPendingTasksShutdownAction>();
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003C_003E2__current = trainerLauncherApp.Container.Resolve<PersistHangarOnShutdown>();
				_003C_003E1__state = 2;
				return true;
			case 2:
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
		System.Collections.Generic.IEnumerator<IShutdownAction> System.Collections.Generic.IEnumerable<IShutdownAction>.GetEnumerator()
		{
			_003CResolveShutdownActions_003Ed__8 result;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				result = this;
			}
			else
			{
				result = new _003CResolveShutdownActions_003Ed__8(0)
				{
					_003C_003E4__this = _003C_003E4__this
				};
			}
			return result;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<IShutdownAction>)this).GetEnumerator();
		}
	}

	[field: CompilerGenerated]
	public static ISuiteNameProvider Suite
	{
		[CompilerGenerated]
		get;
	} = new SuiteNameProvider("Trainers");


	public TrainerLauncherApp()
		: base(Suite, "de61db70-8904-48ce-bfeb-94dfa05f3deb", "Garmin Aviation Trainers", CrashReportDialogOptions.CleanupToolEnabled)
	{
	}

	protected override void RegisterTypes(IContainerRegistry containerRegistry)
	{
		IUnityContainer container = containerRegistry.GetContainer();
		IoCRegistration.RegisterTypes(container, new TrainerTypes());
		IoCRegistration.RegisterTypes(container, new TrainerServiceTypes());
		base.RegisterTypes(containerRegistry);
	}

	protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
	{
		base.ConfigureModuleCatalog(moduleCatalog);
		moduleCatalog.AddModule(typeof(FeedbackPrismModule), InitializationMode.WhenAvailable);
		moduleCatalog.AddModule(typeof(LruSoftwareUpdateModule.ModuleInfo), InitializationMode.WhenAvailable);
		moduleCatalog.AddModule(typeof(VersionManagerModuleInfo), InitializationMode.WhenAvailable);
		moduleCatalog.AddModule(typeof(ScreenshotModuleInfo), InitializationMode.WhenAvailable);
		moduleCatalog.AddModule(typeof(HelpModule), InitializationMode.WhenAvailable);
		moduleCatalog.AddModule(typeof(PilotTrainersModuleInfo), InitializationMode.WhenAvailable);
	}

	protected override Window ResolveMainWindow()
	{
		return (Window)(object)base.Container.Resolve<MainWindow>();
	}

	protected override IStartupAction[] ResolveStartupActions()
	{
		return new IStartupAction[11]
		{
			base.Container.Resolve<FirstRunBackgroundAction>(),
			base.Container.Resolve<CheckForGtiOperationStartup>(),
			base.Container.Resolve<AutoInitCatalogStartup>(),
			base.Container.Resolve<SyncCatalogRefugeStartup>(),
			base.Container.Resolve<SystemInfoLoggingStartupAction>(),
			base.Container.Resolve<CopyInstallationLogsStartupAction>(),
			base.Container.Resolve<CreateDefaultAircraftStartupAction>(),
			base.Container.Resolve<BuiltInMigrations>(),
			base.Container.Resolve<BackgroundHangarPersistenceBehavior>(),
			base.Container.Resolve<BackgroundPowerUpStatsBehavior>(),
			base.Container.Resolve<CompletePowerDownShutdownAction>()
		};
	}

	[IteratorStateMachine(typeof(_003CResolveShutdownActions_003Ed__8))]
	protected override System.Collections.Generic.IEnumerable<IShutdownAction> ResolveShutdownActions()
	{
		yield return base.Container.Resolve<FinishPendingTasksShutdownAction>();
		yield return base.Container.Resolve<PersistHangarOnShutdown>();
	}
}
