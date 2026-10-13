using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace TrainerManifestParsing.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public class Winfs_mount_setup
{
	[JsonProperty("copy", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	[field: CompilerGenerated]
	public System.Collections.Generic.IList<Winfs_copy_instruction> Copy
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = (System.Collections.Generic.IList<Winfs_copy_instruction>)new List<Winfs_copy_instruction>();


	[JsonProperty("links", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	[field: CompilerGenerated]
	public System.Collections.Generic.IList<Winfs_link_instruction> Links
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = (System.Collections.Generic.IList<Winfs_link_instruction>)new List<Winfs_link_instruction>();


	public static Winfs_mount_setup? FromJson(string data)
	{
		return JsonConvert.DeserializeObject<Winfs_mount_setup>(data);
	}

	public string ToJson()
	{
		return JsonConvert.SerializeObject(this);
	}
}
