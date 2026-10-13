using System.Runtime.CompilerServices;

namespace TrainerManifestParsing;

public sealed class SettingValueJsonWrapper
{
	[field: CompilerGenerated]
	public JsonObjectProvider Json
	{
		[CompilerGenerated]
		get;
	}

	public SettingValueJsonWrapper(JsonObjectProvider json)
	{
		Json = json;
	}
}
