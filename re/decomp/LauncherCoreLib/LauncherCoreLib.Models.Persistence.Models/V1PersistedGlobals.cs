using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Persistence.Models;

public class V1PersistedGlobals
{
	[field: CompilerGenerated]
	public System.Collections.Generic.IList<V1SelectedOption> GlobalSettings
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	public V1PersistedGlobals()
	{
		GlobalSettings = (System.Collections.Generic.IList<V1SelectedOption>)new List<V1SelectedOption>();
	}
}
