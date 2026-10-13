using System.Collections.Generic;
using LauncherCoreLib.Models.Settings;

namespace LauncherCoreLib.Models.Persistence;

public interface IGlobalSettingsPersistence : IPersistence
{
	void Store(IActiveSetting setting);

	void Store(System.Collections.Generic.IEnumerable<IActiveSetting> settings);

	bool Hydrate(IActiveSetting setting);

	void Clear(SettingIdentifier settingIdentifer);
}
