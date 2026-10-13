using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Settings;

public sealed class SettingHasOptionsChanged
{
	[field: CompilerGenerated]
	public IActiveSetting Setting
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public System.Collections.Generic.IReadOnlyList<ISettingValueDefinition> Options
	{
		[CompilerGenerated]
		get;
	}

	public bool HasOptions => Setting.SettingHasOptions((System.Collections.Generic.IReadOnlyCollection<ISettingValueDefinition>)Options);

	public SettingHasOptionsChanged(IActiveSetting setting, System.Collections.Generic.IReadOnlyList<ISettingValueDefinition> options)
	{
		Setting = setting;
		Options = options;
	}
}
