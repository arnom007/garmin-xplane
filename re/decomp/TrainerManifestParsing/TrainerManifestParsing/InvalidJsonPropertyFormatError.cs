using System.Runtime.CompilerServices;

namespace TrainerManifestParsing;

public class InvalidJsonPropertyFormatError : JsonPropertyError
{
	[field: CompilerGenerated]
	public override string TargetPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public override string ErrorMessage
	{
		[CompilerGenerated]
		get;
	}

	public InvalidJsonPropertyFormatError(string targetPath, string errorMessage)
	{
		TargetPath = targetPath;
		ErrorMessage = errorMessage;
	}
}
