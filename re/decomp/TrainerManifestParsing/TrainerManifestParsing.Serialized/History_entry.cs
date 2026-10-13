using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace TrainerManifestParsing.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public class History_entry
{
	[JsonProperty("BinaryVersion", Required = Required.Always)]
	[Required(AllowEmptyStrings = true)]
	[RegularExpression("^((\\d+\\.){0,3})(\\d+)$")]
	[field: CompilerGenerated]
	public string BinaryVersion
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty("Description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	[StringLength(2147483647, MinimumLength = 1)]
	[field: CompilerGenerated]
	public string Description
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty("HasBreakingChanges", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	[field: CompilerGenerated]
	public bool HasBreakingChanges
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[JsonProperty("NotableChanges", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	[field: CompilerGenerated]
	public System.Collections.Generic.IList<string> NotableChanges
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = (System.Collections.Generic.IList<string>)new List<string>();


	[JsonProperty("ReleaseNotesPath", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	[field: CompilerGenerated]
	public string? ReleaseNotesPath
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	public static History_entry? FromJson(string data)
	{
		return JsonConvert.DeserializeObject<History_entry>(data);
	}

	public string ToJson()
	{
		return JsonConvert.SerializeObject(this);
	}
}
