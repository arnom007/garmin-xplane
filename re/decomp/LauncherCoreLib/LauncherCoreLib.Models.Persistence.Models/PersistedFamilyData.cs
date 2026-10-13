using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Persistence.Models;

public class PersistedFamilyData
{
	[field: CompilerGenerated]
	public Dictionary<string, string> Settings
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = new Dictionary<string, string>();

}
