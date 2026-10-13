using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace TrainerManifestParsing.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public enum Avionics_category
{
	[EnumMember(Value = "NONE")]
	NONE,
	[EnumMember(Value = "DISPLAY")]
	DISPLAY,
	[EnumMember(Value = "NAVIGATOR")]
	NAVIGATOR
}
