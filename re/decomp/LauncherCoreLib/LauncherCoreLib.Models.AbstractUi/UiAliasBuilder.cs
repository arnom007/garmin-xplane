using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.AbstractUi;

public sealed class UiAliasBuilder
{
	private readonly UiAlias _defaultAlias;

	[field: CompilerGenerated]
	public UiAlias SelectedAlias
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	private UiAliasBuilder(UiAlias uiAlias)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!UiAlias.IsValid(uiAlias))
		{
			throw new ArgumentException("Cannot start with an invalid alias.");
		}
		SelectedAlias = (_defaultAlias = uiAlias);
	}

	public static UiAliasBuilder WithDefault(UiAlias alias)
	{
		return new UiAliasBuilder(alias);
	}

	public UiAliasBuilder ResolveFromProviders(params IUiAliasProvider[] providers)
	{
		UiAlias firstValidAliasOrInvalid = UiAliasProviderExtensions.GetFirstValidAliasOrInvalid(_defaultAlias.AliasKey, providers);
		if (UiAlias.IsValid(firstValidAliasOrInvalid))
		{
			SelectedAlias = firstValidAliasOrInvalid;
		}
		return this;
	}
}
