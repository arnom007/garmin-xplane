using System.Runtime.CompilerServices;

namespace TrainerManifestParsing;

public class InvalidJsonPropertyFormatErrorWrapper : JsonPropertyError
{
	[field: CompilerGenerated]
	public JsonParseError ActualError
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public override string TargetPath
	{
		[CompilerGenerated]
		get;
	}

	public override string ErrorMessage => ActualError.ErrorMessage;

	public InvalidJsonPropertyFormatErrorWrapper(JsonParseError actualError, string targetPath)
	{
		ActualError = actualError;
		TargetPath = targetPath;
	}
}
