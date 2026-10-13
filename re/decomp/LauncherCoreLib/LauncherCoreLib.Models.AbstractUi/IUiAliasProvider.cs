using System.Collections.Generic;

namespace LauncherCoreLib.Models.AbstractUi;

public interface IUiAliasProvider
{
	System.Collections.Generic.IEnumerable<string> AliasKeys { get; }

	UiAlias GetAlias(string uiAliasKey);
}
