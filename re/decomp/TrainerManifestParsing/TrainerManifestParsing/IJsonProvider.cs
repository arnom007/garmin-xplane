using LanguageExt;
using Newtonsoft.Json.Linq;

namespace TrainerManifestParsing;

public interface IJsonProvider
{
	Option<JToken> CopyJsonPath(string jsonPath);
}
