using System.Collections.Generic;
using LauncherCoreLib.Models.AbstractUi;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class UiAliasesMap : ITrainerUiAliasProvider, IUiAliasProvider
{
	private readonly UiAliasRegistry _trainerRegistry;

	public System.Collections.Generic.IEnumerable<string> AliasKeys => _trainerRegistry.AliasKeys;

	public UiAliasesMap(TrainerPackageIdentifier packageId, IManifestDataSource manifestDataSource)
	{
		_trainerRegistry = new UiAliasRegistry((IDictionary<string, string>)(object)manifestDataSource.GetManifestData<Dictionary<string, string>>("UiAliases"));
		_trainerRegistry.RegistryName = packageId.IdentStringChecked;
	}

	public UiAlias GetAlias(string uiAliasKey)
	{
		return _trainerRegistry.GetAlias(uiAliasKey);
	}

	public virtual string ToString()
	{
		return ((object)_trainerRegistry).ToString();
	}
}
