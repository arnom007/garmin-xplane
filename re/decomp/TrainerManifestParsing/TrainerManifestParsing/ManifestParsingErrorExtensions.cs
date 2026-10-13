namespace TrainerManifestParsing;

public static class ManifestParsingErrorExtensions
{
	public static ManifestParsingError AsBase(this ManifestParsingError implementee)
	{
		return implementee;
	}

	public static ManifestParsingError ToManifestError(this JsonParseError jsonError)
	{
		return new JsonManifestParsingError(jsonError);
	}
}
