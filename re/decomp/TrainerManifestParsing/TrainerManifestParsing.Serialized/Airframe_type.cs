using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace TrainerManifestParsing.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public enum Airframe_type
{
	[EnumMember(Value = "UNCONFIGURED")]
	UNCONFIGURED,
	[EnumMember(Value = "SINGLE")]
	SINGLE,
	[EnumMember(Value = "MULTI")]
	MULTI,
	[EnumMember(Value = "ROTOR")]
	ROTOR,
	[EnumMember(Value = "TURBO")]
	TURBO,
	[EnumMember(Value = "SINGLETURBO")]
	SINGLETURBO,
	[EnumMember(Value = "MULTITURBOFAN")]
	MULTITURBOFAN
}
