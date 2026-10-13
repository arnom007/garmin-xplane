using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Persistence.Models;

[Serializable]
internal class PersistedSimOptions
{
	[field: CompilerGenerated]
	public string CommandLine
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;

}
