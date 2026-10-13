using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrainerManifestParsing.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public class Launcher_feature_descriptor
{
	[JsonProperty("Version", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	[RegularExpression("^(\\d+\\.){0,3}(\\d+)$")]
	[field: CompilerGenerated]
	public string Version
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty("Criticality", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	[JsonConverter(typeof(StringEnumConverter))]
	[field: CompilerGenerated]
	public Launcher_feature_descriptorCriticality Criticality
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	public static Launcher_feature_descriptor? FromJson(string data)
	{
		return JsonConvert.DeserializeObject<Launcher_feature_descriptor>(data);
	}

	public string ToJson()
	{
		return JsonConvert.SerializeObject(this);
	}
}
