using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace LauncherCoreLib.Models.Persistence.Models;

public class V1SelectedOption
{
	[JsonProperty(PropertyName = "SettingName")]
	[field: CompilerGenerated]
	public string SettingName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty(PropertyName = "SelectedDisplayName")]
	[field: CompilerGenerated]
	public string SelectedDisplayName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;

}
