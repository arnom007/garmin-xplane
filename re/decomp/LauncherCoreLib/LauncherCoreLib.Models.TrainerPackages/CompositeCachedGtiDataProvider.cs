using System;
using System.Collections;
using System.Collections.Generic;

namespace LauncherCoreLib.Models.TrainerPackages;

public class CompositeCachedGtiDataProvider : ICachedGtiDataProvider
{
	private readonly System.Collections.Generic.IEnumerable<ICachedGtiDataProvider> _cacheProviders;

	public CompositeCachedGtiDataProvider(System.Collections.Generic.IEnumerable<ICachedGtiDataProvider> cacheProviders)
	{
		_cacheProviders = cacheProviders;
	}

	public void NotifyOfCatalogUpdate(Version catalogVersion)
	{
		System.Collections.Generic.IEnumerator<ICachedGtiDataProvider> enumerator = _cacheProviders.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				enumerator.Current.NotifyOfCatalogUpdate(catalogVersion);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}
}
