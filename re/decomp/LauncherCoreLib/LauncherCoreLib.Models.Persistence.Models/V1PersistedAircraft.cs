using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Persistence.Models;

public class V1PersistedAircraft
{
	[field: CompilerGenerated]
	public string AircraftName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[field: CompilerGenerated]
	public System.Collections.Generic.IList<V1PersistedSimulator> Simulators
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	public V1PersistedAircraft()
	{
		Simulators = (System.Collections.Generic.IList<V1PersistedSimulator>)new List<V1PersistedSimulator>();
	}
}
