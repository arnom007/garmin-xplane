using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using GTI.Net;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Logging.Reactive;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Utility;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class GtiPackageDirectoryProvider : ITrainerPackageDirectoryProvider
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003CSearchDirectories_003Eb__8_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IObservable<PotentialPackageDirectory>> _003C_003Et__builder;

		public GtiPackageDirectoryProvider _003C_003E4__this;

		private ConfiguredTaskAwaiter<ILauncherGtiRunner> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			GtiPackageDirectoryProvider gtiPackageDirectoryProvider = _003C_003E4__this;
			IObservable<PotentialPackageDirectory> result2;
			try
			{
				ConfiguredTaskAwaiter<ILauncherGtiRunner> awaiter;
				if (num != 0)
				{
					awaiter = gtiPackageDirectoryProvider._gtiProvider.GtiExe.DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ILauncherGtiRunner>, _003C_003CSearchDirectories_003Eb__8_0_003Ed>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ILauncherGtiRunner>);
					num = (_003C_003E1__state = -1);
				}
				ILauncherGtiRunner result = awaiter.GetResult();
				object cacheLock = gtiPackageDirectoryProvider._cacheLock;
				bool flag = false;
				try
				{
					Monitor.Enter(cacheLock, ref flag);
					IObservable<PotentialPackageDirectory> val = gtiPackageDirectoryProvider._cachedQuery;
					if (val == null)
					{
						IConnectableObservable<PotentialPackageDirectory> connectableObservable = (IConnectableObservable<PotentialPackageDirectory>)(gtiPackageDirectoryProvider._cachedQuery = (IObservable<PotentialPackageDirectory>?)result.List().GetListEntries().Where<GtiListingEntry>((Func<GtiListingEntry, bool>)((GtiListingEntry e) => !e.IsTheCatalog))
							.SelectMany<GtiListingEntry, PotentialPackageDirectory>((Func<GtiListingEntry, IObservable<PotentialPackageDirectory>>)gtiPackageDirectoryProvider.ConvertListEntryToPotentialDirectories)
							.Replay<PotentialPackageDirectory>());
						gtiPackageDirectoryProvider._currentQueryConnection = connectableObservable.Connect();
						val = connectableObservable;
					}
					result2 = val.LogError<PotentialPackageDirectory>("Listing GTI packages").CatchAndComplete<PotentialPackageDirectory>().Merge<PotentialPackageDirectory>(IssueWarningIfThereAreNoEntries(val));
				}
				finally
				{
					if (num < 0 && flag)
					{
						Monitor.Exit(cacheLock);
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[CompilerGenerated]
	private sealed class _003CEnumeratePotentials_003Ed__13 : System.Collections.Generic.IEnumerable<PotentialPackageDirectory>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<PotentialPackageDirectory>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private PotentialPackageDirectory _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private GtiListingEntry listingEntry;

		public GtiListingEntry _003C_003E3__listingEntry;

		public GtiPackageDirectoryProvider _003C_003E4__this;

		PotentialPackageDirectory System.Collections.Generic.IEnumerator<PotentialPackageDirectory>.Current
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
		public _003CEnumeratePotentials_003Ed__13(int _003C_003E1__state)
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
			GtiPackageDirectoryProvider gtiPackageDirectoryProvider = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (IsPathValid(listingEntry.InstalledPath))
				{
					_003C_003E2__current = new PotentialPackageDirectory(TrainerPackageType.InstalledByGti, gtiPackageDirectoryProvider._fileSystem.StatIfExists(listingEntry.InstalledPath).Entry);
					_003C_003E1__state = 1;
					return true;
				}
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			return false;
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
		System.Collections.Generic.IEnumerator<PotentialPackageDirectory> System.Collections.Generic.IEnumerable<PotentialPackageDirectory>.GetEnumerator()
		{
			_003CEnumeratePotentials_003Ed__13 _003CEnumeratePotentials_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CEnumeratePotentials_003Ed__ = this;
			}
			else
			{
				_003CEnumeratePotentials_003Ed__ = new _003CEnumeratePotentials_003Ed__13(0)
				{
					_003C_003E4__this = _003C_003E4__this
				};
			}
			_003CEnumeratePotentials_003Ed__.listingEntry = _003C_003E3__listingEntry;
			return _003CEnumeratePotentials_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<PotentialPackageDirectory>)this).GetEnumerator();
		}
	}

	private readonly ILauncherGtiProvider _gtiProvider;

	private readonly IFileSystem _fileSystem;

	private readonly IDeploymentPathProvider _deployment;

	private readonly System.IDisposable _mutationSubscription;

	private readonly object _cacheLock = new object();

	private IObservable<PotentialPackageDirectory>? _cachedQuery;

	private System.IDisposable _currentQueryConnection = Disposable.Empty;

	public GtiPackageDirectoryProvider(ILauncherGtiProvider gti, IFileSystem fileSystem, IDeploymentPathProvider deployment)
	{
		_gtiProvider = gti;
		_fileSystem = fileSystem;
		_deployment = deployment;
		_mutationSubscription = gti.GtiExeAsSingleValueInStream().Select<ILauncherGtiRunner, IObservable<TimingEvent>>((Func<ILauncherGtiRunner, IObservable<TimingEvent>>)((ILauncherGtiRunner launcherGti) => launcherGti.WhenMutationCommandCompletes())).Switch<TimingEvent>()
			.Do<TimingEvent>((Action<TimingEvent>)([CompilerGenerated] (TimingEvent _) =>
			{
				ClearCache();
			}))
			.Subscribe<TimingEvent>();
	}

	public IObservable<PotentialPackageDirectory> SearchDirectories()
	{
		return Observable.DeferAsync<PotentialPackageDirectory>([AsyncStateMachine(typeof(_003C_003CSearchDirectories_003Eb__8_0_003Ed))] [CompilerGenerated] async (CancellationToken cancel) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ILauncherGtiRunner runner = await _gtiProvider.GtiExe.DetachContext();
			lock (_cacheLock)
			{
				IObservable<PotentialPackageDirectory> val = _cachedQuery;
				if (val == null)
				{
					IConnectableObservable<PotentialPackageDirectory> connectableObservable = (IConnectableObservable<PotentialPackageDirectory>)(_cachedQuery = (IObservable<PotentialPackageDirectory>?)runner.List().GetListEntries().Where<GtiListingEntry>((Func<GtiListingEntry, bool>)((GtiListingEntry e) => !e.IsTheCatalog))
						.SelectMany<GtiListingEntry, PotentialPackageDirectory>((Func<GtiListingEntry, IObservable<PotentialPackageDirectory>>)ConvertListEntryToPotentialDirectories)
						.Replay<PotentialPackageDirectory>());
					_currentQueryConnection = connectableObservable.Connect();
					val = connectableObservable;
				}
				return val.LogError<PotentialPackageDirectory>("Listing GTI packages").CatchAndComplete<PotentialPackageDirectory>().Merge<PotentialPackageDirectory>(IssueWarningIfThereAreNoEntries(val));
			}
		});
	}

	private static IObservable<PotentialPackageDirectory> IssueWarningIfThereAreNoEntries(IObservable<PotentialPackageDirectory> sharedEntries)
	{
		return sharedEntries.Any<PotentialPackageDirectory>().WhereEqual(operand: false).Do<bool>((Action<bool>)delegate
		{
			Logger.Log(LogLevel.Warning, "[GTI] There are no entries in the catalog.");
		})
			.IgnoreElements<bool>()
			.SelectDefault<bool, PotentialPackageDirectory>(null)
			.CatchAndComplete<PotentialPackageDirectory>();
	}

	private static bool IsPathValid(string? path)
	{
		if (path != null)
		{
			return path != "ERROR";
		}
		return false;
	}

	private void ClearCache()
	{
		lock (_cacheLock)
		{
			_currentQueryConnection.Dispose();
			_cachedQuery = null;
		}
	}

	private IObservable<PotentialPackageDirectory> ConvertListEntryToPotentialDirectories(GtiListingEntry listingEntry)
	{
		return EnumeratePotentials(listingEntry).ToObservable();
	}

	[IteratorStateMachine(typeof(_003CEnumeratePotentials_003Ed__13))]
	private System.Collections.Generic.IEnumerable<PotentialPackageDirectory> EnumeratePotentials(GtiListingEntry listingEntry)
	{
		if (IsPathValid(listingEntry.InstalledPath))
		{
			yield return new PotentialPackageDirectory(TrainerPackageType.InstalledByGti, _fileSystem.StatIfExists(listingEntry.InstalledPath).Entry);
		}
	}
}
