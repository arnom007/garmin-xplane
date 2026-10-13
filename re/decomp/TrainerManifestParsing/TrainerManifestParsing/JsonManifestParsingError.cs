using System.Runtime.CompilerServices;

namespace TrainerManifestParsing;

public class JsonManifestParsingError : ManifestParsingError
{
	[field: CompilerGenerated]
	public JsonParseError JsonError
	{
		[CompilerGenerated]
		get;
	}

	public JsonManifestParsingError(JsonParseError jsonError)
	{
		JsonError = jsonError;
	}

	public virtual string ToString()
	{
		return JsonError.ErrorMessage;
	}
}
