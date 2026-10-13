using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Settings;

namespace LauncherCoreLib.Models.Persistence.Models;

[Serializable]
public class PersistedGlobals
{
	[field: CompilerGenerated]
	public Dictionary<string, string> GlobalSettings
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = new Dictionary<string, string>();


	public void UpdatePersisted(IActiveSetting setting)
	{
		if (!string.IsNullOrEmpty((string)setting.LocalId))
		{
			GlobalSettings[(string)setting.LocalId] = setting.CurrentValue;
		}
	}
}
