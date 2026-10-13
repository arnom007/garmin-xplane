using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LOLA.Extensions;

namespace LauncherCoreLib.Models.AbstractUi;

public class UiAliasRegistry : IUiAliasRegistry, IUiAliasProvider
{
	private readonly IDictionary<string, string> _aliases;

	[field: CompilerGenerated]
	public string RegistryName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	public System.Collections.Generic.IEnumerable<string> AliasKeys => _aliases.EnumerateKeysWithoutSnapshot<string, string>();

	public UiAliasRegistry()
	{
		_aliases = (IDictionary<string, string>)(object)new ConcurrentDictionary<string, string>();
	}

	public UiAliasRegistry(IDictionary<string, string> useDictionary)
	{
		_aliases = (IDictionary<string, string>)(((object)useDictionary) ?? ((object)new ConcurrentDictionary<string, string>()));
	}

	public UiAlias AddorUpdateAlias(UiAlias uiAlias)
	{
		_aliases[uiAlias.AliasKey] = uiAlias.AliasValue;
		return uiAlias;
	}

	public UiAlias GetAlias(string uiAliasKey)
	{
		string value = default(string);
		if (_aliases.TryGetValue(uiAliasKey, ref value))
		{
			return new UiAlias(uiAliasKey, value);
		}
		return UiAlias.Invalid;
	}

	public virtual string ToString()
	{
		return (RegistryName ?? "Unknown") + "UiAlias Registry";
	}
}
