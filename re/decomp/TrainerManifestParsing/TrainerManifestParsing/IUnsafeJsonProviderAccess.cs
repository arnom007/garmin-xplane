using Newtonsoft.Json.Linq;

namespace TrainerManifestParsing;

public interface IUnsafeJsonProviderAccess : IJsonProvider
{
	JToken UnsafeAccess();
}
