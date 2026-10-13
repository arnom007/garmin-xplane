using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace TrainerManifestParsing.Serialized;

public class Winfs_copy_instruction
{
	[JsonProperty("packagePath", Required = Required.Always)]
	[Required(AllowEmptyStrings = true)]
	[field: CompilerGenerated]
	public string PackagePath
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty("winfsPath", Required = Required.Always)]
	[Required(AllowEmptyStrings = true)]
	[field: CompilerGenerated]
	public string WinfsPath
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty("globs", Required = Required.Always)]
	[Required]
	[field: CompilerGenerated]
	public System.Collections.Generic.IList<string> Globs
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = (System.Collections.Generic.IList<string>)new List<string>();


	public static Winfs_copy_instruction? FromJson(string data)
	{
		return JsonConvert.DeserializeObject<Winfs_copy_instruction>(data);
	}

	public string ToJson()
	{
		return JsonConvert.SerializeObject(this);
	}
}
