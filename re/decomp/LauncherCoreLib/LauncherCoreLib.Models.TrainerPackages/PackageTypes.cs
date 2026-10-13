using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;
using TrainerPackageLib;
using TrainerPackageLib.ManifestParsing;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace LauncherCoreLib.Models.TrainerPackages;

public class PackageTypes : IoCTypesBase
{
	[CompilerGenerated]
	private sealed class _003CSingletons_003Ed__2 : System.Collections.Generic.IEnumerable<TypeMapping>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<TypeMapping>, System.Collections.IEnumerator, System.IDisposable
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
		public _003CSingletons_003Ed__2(int _003C_003E1__state)
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
				_003C_003E2__current = TypeMapping.Create<IStartingPackageProvider, StartingPackageProvider>();
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ITrainerPackageRepoEvolved, PackageRepoEvolved>();
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ITrainerPackageInstaller, GtiPackageInstaller>();
				_003C_003E1__state = 3;
				return true;
			case 3:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ITrainerPackageDirectoryProvider, AllTrainerDirectoryProviders>();
				_003C_003E1__state = 4;
				return true;
			case 4:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<PackageInstallApprovalCollector>(new System.Type[2]
				{
					typeof(IPackageInstallApprovalCollector),
					typeof(IPackageInstallRequester)
				});
				_003C_003E1__state = 5;
				return true;
			case 5:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IApprovedPackageInstaller, ApprovedPackageInstaller>();
				_003C_003E1__state = 6;
				return true;
			case 6:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<SuppressedGtiDirectoryProvider>(new System.Type[1] { typeof(IGtiDirectorySuppressor) });
				_003C_003E1__state = 7;
				return true;
			case 7:
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
			return new _003CSingletons_003Ed__2(0);
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
				_003C_003E2__current = TypeMapping.Create<IPackageErrorProvider, PackageErrorProvider>();
				_003C_003E1__state = 1;
				return true;
			case 1:
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
		yield return TypeMapping.Create<IPackageErrorProvider, PackageErrorProvider>();
	}

	public override void Complex(IUnityContainer container)
	{
		RegisterPackageUpgradeDecisionRepo(container);
		DecorateDirectoryParserWithValidation(container);
		DecorateGtiInstallerWithDownloadRevokation(container);
		container.RegisterType<IVersionedManifestParsing, V1ManifestParsing>("V1Parsing", System.Array.Empty<InjectionMember>());
		container.RegisterType<IVersionedManifestParsing, V2ManifestParsing>("V2Parsing", System.Array.Empty<InjectionMember>());
		container.RegisterType<IManifestValidator, TrainerExePathValidator>("Trainer EXE Validator", System.Array.Empty<InjectionMember>());
		container.RegisterType<IManifestValidator, DuplicateDisplayOrderIndexWarningValidator>("Duplicate DOI Validator", System.Array.Empty<InjectionMember>());
		container.RegisterType<IManifestValidator, VersionIdMatchesBinaryVersionValidator>("Version Id <-> Binary version Validator", System.Array.Empty<InjectionMember>());
	}

	[IteratorStateMachine(typeof(_003CSingletons_003Ed__2))]
	public override System.Collections.Generic.IEnumerable<TypeMapping> Singletons()
	{
		yield return TypeMapping.Create<IStartingPackageProvider, StartingPackageProvider>();
		yield return TypeMapping.Create<ITrainerPackageRepoEvolved, PackageRepoEvolved>();
		yield return TypeMapping.Create<ITrainerPackageInstaller, GtiPackageInstaller>();
		yield return TypeMapping.Create<ITrainerPackageDirectoryProvider, AllTrainerDirectoryProviders>();
		yield return TypeMapping.Create<PackageInstallApprovalCollector>(new System.Type[2]
		{
			typeof(IPackageInstallApprovalCollector),
			typeof(IPackageInstallRequester)
		});
		yield return TypeMapping.Create<IApprovedPackageInstaller, ApprovedPackageInstaller>();
		yield return TypeMapping.Create<SuppressedGtiDirectoryProvider>(new System.Type[1] { typeof(IGtiDirectorySuppressor) });
	}

	private static void DecorateGtiInstallerWithDownloadRevokation(IUnityContainer container)
	{
		string name = "basePackageInstaller";
		container.RegisterType<GtiPackageInstaller>(new ContainerControlledLifetimeManager(), System.Array.Empty<InjectionMember>());
		container.RegisterType<ITrainerPackageInstaller, GtiPackageInstaller>(name, System.Array.Empty<InjectionMember>());
		container.RegisterType<RevokeDownloadPermissionsInstallerLayer>(new ContainerControlledLifetimeManager(), new InjectionMember[1]
		{
			new InjectionConstructor(new object[2]
			{
				new ResolvedParameter<ITrainerPackageInstaller>(name),
				new ResolvedParameter<IPackageInstallApprovalCollector>()
			})
		});
		container.RegisterType<ITrainerPackageInstaller, RevokeDownloadPermissionsInstallerLayer>(System.Array.Empty<InjectionMember>());
	}

	private static void DecorateDirectoryParserWithValidation(IUnityContainer container)
	{
		ContainerControlledLifetimeManager lifetimeManager = new ContainerControlledLifetimeManager();
		string name = "BaseParser";
		container.RegisterType<DirectoryPackageParser>(lifetimeManager, System.Array.Empty<InjectionMember>());
		container.RegisterType<ITrainerPackageParser, DirectoryPackageParser>(name, System.Array.Empty<InjectionMember>());
		lifetimeManager = new ContainerControlledLifetimeManager();
		container.RegisterType<ExtendedManifestValidationParser>(lifetimeManager, new InjectionMember[1]
		{
			new InjectionConstructor(new object[2]
			{
				new ResolvedParameter<ITrainerPackageParser>(name),
				new ResolvedParameter<IManifestValidator[]>()
			})
		});
		container.RegisterType<ITrainerPackageParser, ExtendedManifestValidationParser>(System.Array.Empty<InjectionMember>());
		container.RegisterType<IManifestValidator, PackageHistoryValidator>("History", System.Array.Empty<InjectionMember>());
	}

	private static void RegisterPackageUpgradeDecisionRepo(IUnityContainer container)
	{
		container.RegisterFactory<PackageUpgradeDecisionRepo>((Func<IUnityContainer, object>)((IUnityContainer c) => new PackageUpgradeDecisionRepo(c.Resolve<ILowLevelDal>(System.Array.Empty<ResolverOverride>()), Scheduler.Default)), (IFactoryLifetimeManager)new ContainerControlledLifetimeManager());
		container.RegisterType<IPackageUpgradeDecisionRepo, PackageUpgradeDecisionRepo>(System.Array.Empty<InjectionMember>());
	}
}
