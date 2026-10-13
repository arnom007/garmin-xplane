using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LauncherCoreLib.Models.AbstractUi;

public static class UiAliasProviderExtensions
{
	public static bool IsKnownAlias(this IUiAliasProvider aliasProvider, string aliasKey, params IUiAliasProvider[] fallbacks)
	{
		return UiAlias.IsValid(GetFirstValidAliasOrInvalid(aliasKey, Enumerable.Concat<IUiAliasProvider>((System.Collections.Generic.IEnumerable<IUiAliasProvider>)new IUiAliasProvider[1] { aliasProvider }, (System.Collections.Generic.IEnumerable<IUiAliasProvider>)fallbacks)));
	}

	public static UiAlias GetKnownAlias(this IUiAliasProvider primaryProvider, string aliasKey, IUiAliasProvider secondaryProvider, params IUiAliasProvider[] fallbacks)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		System.Collections.Generic.IEnumerable<IUiAliasProvider> aliasProviders = Enumerable.Concat<IUiAliasProvider>((System.Collections.Generic.IEnumerable<IUiAliasProvider>)new IUiAliasProvider[2] { primaryProvider, secondaryProvider }, (System.Collections.Generic.IEnumerable<IUiAliasProvider>)fallbacks);
		UiAlias firstValidAliasOrInvalid = GetFirstValidAliasOrInvalid(aliasKey, aliasProviders);
		if (!UiAlias.IsValid(firstValidAliasOrInvalid))
		{
			throw new KeyNotFoundException("Could not find an alias for " + aliasKey + " in any of the providers.");
		}
		return firstValidAliasOrInvalid;
	}

	public static UiAlias GetFirstValidAliasOrInvalid(string aliasKey, System.Collections.Generic.IEnumerable<IUiAliasProvider> aliasProviders)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		System.Collections.Generic.IEnumerator<IUiAliasProvider> enumerator = aliasProviders.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				IUiAliasProvider current = enumerator.Current;
				UiAlias alias = current.GetAlias(aliasKey);
				if (UiAlias.IsValid(alias))
				{
					if (alias.AliasKey != aliasKey)
					{
						throw new ArgumentException(((object)current)?.ToString() + " returned a valid alias with a mismatched key: " + alias.AliasKey + ".");
					}
					return alias;
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		return UiAlias.Invalid;
	}
}
