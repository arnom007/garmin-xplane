using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using LOLA.Reactive;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class CachedPackageDirectoryProvider : ITrainerPackageDirectoryProvider
{
	private readonly ITrainerPackageDirectoryProvider _wrappedProvider;

	private readonly ConcurrentDictionary<string, PotentialPackageDirectory> _cache = new ConcurrentDictionary<string, PotentialPackageDirectory>();

	public CachedPackageDirectoryProvider(ITrainerPackageDirectoryProvider trainerPackageDirectoryProvider)
	{
		_wrappedProvider = trainerPackageDirectoryProvider;
	}

	public IObservable<PotentialPackageDirectory> SearchDirectories()
	{
		return CachedDirectories().Concat<PotentialPackageDirectory>(DirectoriesFromProvider());
	}

	private static string GetCacheKey(PotentialPackageDirectory potentialPackageDir)
	{
		return potentialPackageDir.PackageDirectory.FullPath;
	}

	private static System.Collections.Generic.IEnumerable<string> StaleEntries(HashSet<string> initialKeys, HashSet<string> foundInThisQuery)
	{
		HashSet<string> obj = new HashSet<string>((System.Collections.Generic.IEnumerable<string>)initialKeys);
		obj.ExceptWith((System.Collections.Generic.IEnumerable<string>)foundInThisQuery);
		return (System.Collections.Generic.IEnumerable<string>)obj;
	}

	private IObservable<PotentialPackageDirectory> DirectoriesFromProvider()
	{
		return Observable.Defer<PotentialPackageDirectory>((Func<IObservable<PotentialPackageDirectory>>)([CompilerGenerated] () =>
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Expected O, but got Unknown
			HashSet<string> intialKeys = new HashSet<string>(Enumerable.Select<KeyValuePair<string, PotentialPackageDirectory>, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, PotentialPackageDirectory>>)_cache, (Func<KeyValuePair<string, PotentialPackageDirectory>, string>)((KeyValuePair<string, PotentialPackageDirectory> kv) => kv.Key)));
			HashSet<string> foundInThisQuery = new HashSet<string>();
			return _wrappedProvider.SearchDirectories().Do<PotentialPackageDirectory>((Action<PotentialPackageDirectory>)delegate(PotentialPackageDirectory potentialPackageDir)
			{
				AddOrUpdateCache(potentialPackageDir, foundInThisQuery);
			}).OnCompleted<PotentialPackageDirectory>((Action)delegate
			{
				RemoveStaleCacheEntries(intialKeys, foundInThisQuery);
			});
		}));
	}

	private IObservable<PotentialPackageDirectory> CachedDirectories()
	{
		return Enumerable.Select<KeyValuePair<string, PotentialPackageDirectory>, PotentialPackageDirectory>((System.Collections.Generic.IEnumerable<KeyValuePair<string, PotentialPackageDirectory>>)_cache, (Func<KeyValuePair<string, PotentialPackageDirectory>, PotentialPackageDirectory>)((KeyValuePair<string, PotentialPackageDirectory> kv) => kv.Value)).ToObservable();
	}

	private void AddOrUpdateCache(PotentialPackageDirectory potentialPackageDir, HashSet<string> foundInThisQuery)
	{
		PotentialPackageDirectory potentialPackageDir2 = potentialPackageDir;
		string cacheKey = GetCacheKey(potentialPackageDir2);
		foundInThisQuery.Add(cacheKey);
		_cache.AddOrUpdate(cacheKey, potentialPackageDir2, (Func<string, PotentialPackageDirectory, PotentialPackageDirectory>)((string _, PotentialPackageDirectory __) => potentialPackageDir2));
	}

	private void RemoveStaleCacheEntries(HashSet<string> initialKeys, HashSet<string> foundInThisQuery)
	{
		System.Collections.Generic.IEnumerator<string> enumerator = StaleEntries(initialKeys, foundInThisQuery).GetEnumerator();
		try
		{
			PotentialPackageDirectory potentialPackageDirectory = default(PotentialPackageDirectory);
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				string current = enumerator.Current;
				_cache.TryRemove(current, ref potentialPackageDirectory);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}
}
