using System.Runtime.CompilerServices;

namespace TrainerManifestParsing;

public class ModelJsonWrapper
{
	[field: CompilerGenerated]
	public JsonObjectProvider Json
	{
		[CompilerGenerated]
		get;
	}

	public ModelJsonWrapper(JsonObjectProvider json)
	{
		Json = json;
	}
}
