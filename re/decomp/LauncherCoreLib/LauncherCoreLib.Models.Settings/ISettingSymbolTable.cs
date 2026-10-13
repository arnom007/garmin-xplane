using System.Collections.Generic;

namespace LauncherCoreLib.Models.Settings;

public interface ISettingSymbolTable : ISettingsProvider
{
	System.Collections.Generic.IEnumerable<IActiveSetting> AllSettings { get; }

	IActiveSetting? GetByScopedIdentifier(SettingIdentifier id);

	ISettingSymbolTable? GetNestedTable(SettingIdentifier localIdentifier);
}
