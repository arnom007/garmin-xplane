using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using GTI.Net;
using LOLA.Logging;
using LauncherCoreLib.Models.AppLifeCycle;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class CatalogRefugeDirectoryProvider : ITrainerPackageDirectoryProvider
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003CSearchDirectories_003Eb__4_1_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IObservable<PotentialPackageDirectory>> _003C_003Et__builder;

		public CatalogRefugeDirectoryProvider _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CatalogRefugeDirectoryProvider catalogRefugeDirectoryProvider = _003C_003E4__this;
			IObservable<PotentialPackageDirectory> result;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = catalogRefugeDirectoryProvider._startUpTask.ConfigureAwait(false);
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CSearchDirectories_003Eb__4_1_003Ed>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
				}
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				result = catalogRefugeDirectoryProvider.ListRescuedDirectories();
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly ICatalogPackageRefuge _packageRefuge;

	private readonly ILauncherGtiProvider _gtiProvider;

	private readonly System.Threading.Tasks.Task _startUpTask;

	public CatalogRefugeDirectoryProvider(ICatalogPackageRefuge refuge, ILauncherGtiProvider launcherGtiProvider, IStartupRegistry startup)
	{
		_packageRefuge = refuge;
		_gtiProvider = launcherGtiProvider;
		_startUpTask = startup.WaitForStartupActionToComplete<SyncCatalogRefugeStartup>();
	}

	public IObservable<PotentialPackageDirectory> SearchDirectories()
	{
		return Observable.Defer<PotentialPackageDirectory>((Func<IObservable<PotentialPackageDirectory>>)([CompilerGenerated] () => (IObservable<PotentialPackageDirectory>)(_startUpTask.IsCompleted ? ((object)ListRescuedDirectories()) : ((object)ListRescuedDirectories().Concat<PotentialPackageDirectory>(Observable.DeferAsync<PotentialPackageDirectory>([AsyncStateMachine(typeof(_003C_003CSearchDirectories_003Eb__4_1_003Ed))] [CompilerGenerated] async (CancellationToken _) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			await _startUpTask.ConfigureAwait(false);
			return ListRescuedDirectories();
		})).Distinct<PotentialPackageDirectory, string>((PotentialPackageDirectory p) => p.PackageDirectory.FullPath)))));
	}

	private IObservable<PotentialPackageDirectory> ListRescuedDirectories()
	{
		return _gtiProvider.GtiExeAsSingleValueInStream().Select<ILauncherGtiRunner, IObservable<PotentialPackageDirectory>>((Func<ILauncherGtiRunner, IObservable<PotentialPackageDirectory>>)([CompilerGenerated] (ILauncherGtiRunner gtiExe) => gtiExe.List().GetListEntries().Where<GtiListingEntry>((Func<GtiListingEntry, bool>)((GtiListingEntry entry) => entry.IsTheCatalog))
			.Take<GtiListingEntry>(1)
			.Catch<GtiListingEntry, GtiProcessException>(delegate(GtiProcessException e)
			{
				Logger.LogFormattedException(e, "Retrieving the current catalog version to filter rescue directories.");
				return Observable.Empty<GtiListingEntry>();
			})
			.Select<GtiListingEntry, IObservable<PotentialPackageDirectory>>((Func<GtiListingEntry, IObservable<PotentialPackageDirectory>>)([CompilerGenerated] (GtiListingEntry catalogEntry) => _packageRefuge.ListRescuedPackages().Where<IRescuedCatalogPackage>((Func<IRescuedCatalogPackage, bool>)((IRescuedCatalogPackage rescue) => rescue.CatalogVersions[0] <= catalogEntry.InstalledVersion)).Select<IRescuedCatalogPackage, PotentialPackageDirectory>((Func<IRescuedCatalogPackage, PotentialPackageDirectory>)((IRescuedCatalogPackage rescue) => rescue.RescuedDirectory))))
			.Switch<PotentialPackageDirectory>())).Switch<PotentialPackageDirectory>();
	}
}
