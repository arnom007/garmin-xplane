using System.Runtime.CompilerServices;

namespace TrainerManifestParsing;

public class MissingJsonPathError : JsonPropertyError
{
	[field: CompilerGenerated]
	public string MissingPath
	{
		[CompilerGenerated]
		get;
	}

	public override string TargetPath => MissingPath;

	public override string ErrorMessage => "JSON local path '" + MissingPath + "' was not found.";

	public MissingJsonPathError(string expectedpath)
	{
		MissingPath = expectedpath;
	}
}
