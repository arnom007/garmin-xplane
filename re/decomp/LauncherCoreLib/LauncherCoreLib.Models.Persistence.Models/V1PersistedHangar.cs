using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Persistence.Models;

[Serializable]
public class V1PersistedHangar
{
	[field: CompilerGenerated]
	public string? SelectedAircraft
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public System.Collections.Generic.IList<V1PersistedAircraft> Aircraft
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	public V1PersistedHangar()
	{
		Aircraft = (System.Collections.Generic.IList<V1PersistedAircraft>)new List<V1PersistedAircraft>();
	}
}
