namespace TrainerManifestParsing;

public static class JsonParseErrorExtensions
{
	public static JsonParseError AsParseError(this JsonParseError parseError)
	{
		return parseError;
	}

	public static JsonPropertyError AsPropertyError(this JsonPropertyError propertyError)
	{
		return propertyError;
	}

	public static string FormatErrorString(this JsonParseError error)
	{
		if (error is JsonPropertyError error2)
		{
			return error2.FormatErrorString();
		}
		return "Unknown: " + ((object)error).ToString();
	}
}
