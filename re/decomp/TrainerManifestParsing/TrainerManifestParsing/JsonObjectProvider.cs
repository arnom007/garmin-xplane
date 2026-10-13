using System.Runtime.CompilerServices;
using LanguageExt;
using Newtonsoft.Json.Linq;

namespace TrainerManifestParsing;

public sealed class JsonObjectProvider : IJsonProvider
{
	[field: CompilerGenerated]
	public IJsonProvider JsonProvider
	{
		[CompilerGenerated]
		get;
	}

	public JsonObjectProvider(JObject json)
		: this(new JsonProvider(json))
	{
	}

	private JsonObjectProvider(IJsonProvider jsonProvider)
	{
		JsonProvider = jsonProvider;
	}

	public static Option<JsonObjectProvider> Parse(IJsonProvider jsonProvider)
	{
		if (jsonProvider is IUnsafeJsonProviderAccess unsafeJsonProviderAccess && unsafeJsonProviderAccess.UnsafeAccess() is JObject)
		{
			return new JsonObjectProvider(jsonProvider);
		}
		if (jsonProvider.CopyJson() is JObject)
		{
			return new JsonObjectProvider(jsonProvider);
		}
		return Prelude.None;
	}

	public JObject CopyJsonObject()
	{
		return (JObject)JsonProvider.CopyJson();
	}

	public Option<JToken> CopyJsonPath(string jsonPath)
	{
		return JsonProvider.CopyJsonPath(jsonPath);
	}
}
