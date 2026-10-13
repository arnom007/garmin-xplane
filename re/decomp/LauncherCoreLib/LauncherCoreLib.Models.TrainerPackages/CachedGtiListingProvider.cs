using System;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using GTI.Net;
using LOLA.Reactive;

namespace LauncherCoreLib.Models.TrainerPackages;

public class CachedGtiListingProvider : IGtiListingProvider, ICachedGtiDataProvider
{
	private readonly ILauncherGtiProvider _gtiProvider;

	private readonly object _cacheLock = new object();

	private IObservable<GtiListingEntry>? _cachedReplay;

	public CachedGtiListingProvider(ILauncherGtiProvider gtiProvider)
	{
		_gtiProvider = gtiProvider;
	}

	public IObservable<GtiListingEntry> GtiListEntries()
	{
		return Observable.Defer<GtiListingEntry>((Func<IObservable<GtiListingEntry>>)([CompilerGenerated] () =>
		{
			lock (_cacheLock)
			{
				if (_cachedReplay == null)
				{
					_cachedReplay = CreateNewReplayStream();
				}
				return _cachedReplay;
			}
		}));
	}

	public void NotifyOfCatalogUpdate(Version catalogVersion)
	{
		ResetCache();
	}

	private IObservable<GtiListingEntry> CreateNewReplayStream()
	{
		return _gtiProvider.GtiExeAsSingleValueInStream().Select<ILauncherGtiRunner, IObservable<GtiListingEntry>>((Func<ILauncherGtiRunner, IObservable<GtiListingEntry>>)([CompilerGenerated] (ILauncherGtiRunner gtiExe) => gtiExe.List().GetListEntries().CatchDoRethrow<GtiListingEntry, GtiProcessException>([CompilerGenerated] (GtiProcessException _) =>
		{
			ResetCache();
		}))).Merge<GtiListingEntry>()
			.Replay<GtiListingEntry>()
			.RefCount();
	}

	private void ResetCache()
	{
		lock (_cacheLock)
		{
			_cachedReplay = null;
		}
	}
}
