using System.Runtime.CompilerServices;
using LanguageExt;
using Newtonsoft.Json.Linq;

namespace TrainerManifestParsing;

public class JsonProvider : IUnsafeJsonProviderAccess, IJsonProvider
{
	[field: CompilerGenerated]
	private JToken Seed
	{
		[CompilerGenerated]
		get;
	}

	public JsonProvider(JToken json)
	{
		Seed = json;
	}

	public Option<JToken> CopyJsonPath(string jsonPath)
	{
		return Seed.SelectJsonPath(jsonPath).Map<JToken>((JToken t) => t.DeepClone());
	}

	public JToken UnsafeAccess()
	{
		return Seed;
	}
}
