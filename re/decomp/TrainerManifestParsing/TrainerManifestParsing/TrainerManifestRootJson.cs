using System.Runtime.CompilerServices;

namespace TrainerManifestParsing;

public class TrainerManifestRootJson : IRootManifestJsonProvider
{
	[field: CompilerGenerated]
	public JsonObjectProvider Json
	{
		[CompilerGenerated]
		get;
	}

	public TrainerManifestRootJson(JsonObjectProvider json)
	{
		Json = json;
	}
}
