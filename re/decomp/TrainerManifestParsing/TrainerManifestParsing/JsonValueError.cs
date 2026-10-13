using System.Runtime.CompilerServices;

namespace TrainerManifestParsing;

public class JsonValueError : JsonParseError
{
	[field: CompilerGenerated]
	public override string ErrorMessage
	{
		[CompilerGenerated]
		get;
	}

	public JsonValueError(string message)
	{
		ErrorMessage = message;
	}
}
