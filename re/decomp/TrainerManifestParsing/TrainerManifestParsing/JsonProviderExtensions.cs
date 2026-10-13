using System;
using Newtonsoft.Json.Linq;

namespace TrainerManifestParsing;

public static class JsonProviderExtensions
{
	private const string ROOT_NODE_JSONPATH = "$";

	public static JToken CopyJson(this IJsonProvider json)
	{
		return json.CopyJsonPath("$").IfNone((Func<JToken>)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new InvalidOperationException();
		});
	}
}
