using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Persistence.Models;

public class V1PersistedSimulator
{
	[field: CompilerGenerated]
	public int InstallId
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public string ModelName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[field: CompilerGenerated]
	public int Category
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public System.Collections.Generic.IList<V1SelectedOption> Options
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = (System.Collections.Generic.IList<V1SelectedOption>)new List<V1SelectedOption>();

}
