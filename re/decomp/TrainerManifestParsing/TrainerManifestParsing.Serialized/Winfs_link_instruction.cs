using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace TrainerManifestParsing.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public class Winfs_link_instruction
{
	[JsonProperty("winfs", Required = Required.Always)]
	[Required(AllowEmptyStrings = true)]
	[field: CompilerGenerated]
	public string Winfs
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


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


	public static Winfs_link_instruction? FromJson(string data)
	{
		return JsonConvert.DeserializeObject<Winfs_link_instruction>(data);
	}

	public string ToJson()
	{
		return JsonConvert.SerializeObject(this);
	}
}
