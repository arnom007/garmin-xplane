using System.Runtime.CompilerServices;

namespace TrainerManifestParsing;

public class InvalidJsonFormatError : JsonParseError
{
	[field: CompilerGenerated]
	public string ErrorHint
	{
		[CompilerGenerated]
		get;
	}

	public override string ErrorMessage => ErrorHint;

	public InvalidJsonFormatError(string errorHint)
	{
		ErrorHint = errorHint;
	}
}
