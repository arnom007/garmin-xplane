namespace TrainerManifestParsing;

public abstract class JsonPropertyError : JsonParseError
{
	public abstract string TargetPath { get; }
}
