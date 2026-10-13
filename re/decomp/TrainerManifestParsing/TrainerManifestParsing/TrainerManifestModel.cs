using System;
using System.Collections.Generic;
using System.Linq;
using Domain.TrainerPackages;
using LanguageExt;
using Newtonsoft.Json.Linq;
using TrainerManifestParsing.Serialized;

namespace TrainerManifestParsing;

public static class TrainerManifestModel
{
	private const string MODELS_PROPERTY_NAME = "Models";

	public static System.Collections.Generic.IEnumerable<Either<ManifestParsingError, ModelJsonWrapper>> EnumerateModels(this IRootManifestJsonProvider manifest)
	{
		return Enumerable.Select<Either<JsonParseError, JObject>, Either<ManifestParsingError, ModelJsonWrapper>>(manifest.Json.EnumerateRequiredJObjectLookupValuesAtPath("Models"), (Func<Either<JsonParseError, JObject>, Either<ManifestParsingError, ModelJsonWrapper>>)((Either<JsonParseError, JObject> e) => e.Map<ModelJsonWrapper>((JObject o) => new ModelJsonWrapper(o.ToJsonObjectProvider())).MapJsonErrorToManifestError()));
	}

	public static Either<ManifestParsingError, Option<ModelJsonWrapper>> GetModelById(this IRootManifestJsonProvider manifest, TrainerModelIdentifier manifestModelId)
	{
		return manifest.Json.GetWrapperAtPathOrNone<ModelJsonWrapper>("Models/" + manifestModelId.RawString, (JObject mo) => new ModelJsonWrapper(mo.ToJsonObjectProvider())).MapJsonErrorToManifestError();
	}

	public static Either<ManifestParsingError, TrainerModelIdentifier> GetModelId(this ModelJsonWrapper modelJson)
	{
		return modelJson.Json.ParseTrainerIdentifierAtPath("id").Map<TrainerModelIdentifier>((TrainerManifestIdentifier id) => new TrainerModelIdentifier(id.InnerId));
	}

	public static Either<ManifestParsingError, string> GetModelName(this ModelJsonWrapper model)
	{
		return model.GetRequiredModelStringProperty("Name");
	}

	public static Either<ManifestParsingError, Option<string>> GetModelDescription(this ModelJsonWrapper model)
	{
		return model.GetModelStringProperty("ModelDescription");
	}

	public static Either<ManifestParsingError, RelativePackagePath> GetModelIconPath(this ModelJsonWrapper model)
	{
		return model.GetRequiredModelStringProperty("IconImagePath").ParseAsRelativePath("IconImagePath");
	}

	public static Either<ManifestParsingError, int> GetBaconIndex(this ModelJsonWrapper model)
	{
		return model.Json.CopyRequiredJsonPath("DisplayOrderIndex").ConvertToType<int>().AttachJsonPath("DisplayOrderIndex")
			.MapJsonErrorToManifestError();
	}

	public static Either<ManifestParsingError, Option<Data_formats>> GetDataFormats(this ModelJsonWrapper model)
	{
		return model.Json.GetDataFormatsProperty();
	}

	public static System.Collections.Generic.IEnumerable<Either<ManifestParsingError, AirframeCategoryClassType>> EnumerateModelSupportedAirframes(this ModelJsonWrapper model)
	{
		return model.Json.EnumerateSupportedAirframesProperty();
	}

	public static System.Collections.Generic.IEnumerable<Either<ManifestParsingError, AirframeCategoryClassType>> EnumerateModelSupportedAirframesIfTheyExist(this ModelJsonWrapper model)
	{
		return model.Json.CopyJsonPath("SupportedAirframes").IfNone((Func<JToken>)(() => new JArray())).AsJsonArray()
			.MapJsonErrorToManifestError()
			.Map<System.Collections.Generic.IEnumerable<Either<ManifestParsingError, AirframeCategoryClassType>>>((JArray ja) => ja.EnumerateAirframes())
			.LeftToRightEnumerable();
	}

	public static Either<ManifestParsingError, string> GetRequiredModelStringProperty(this ModelJsonWrapper model, string jsonPath)
	{
		string jsonPath2 = jsonPath;
		return model.GetModelStringProperty(jsonPath2).Bind<string>((Option<string> maybe) => maybe.ToEither<JsonParseError>(() => new MissingJsonPathError(jsonPath2).AsParseError()).MapJsonErrorToManifestError());
	}

	public static Either<ManifestParsingError, Option<string>> GetModelStringProperty(this ModelJsonWrapper model, string jsonPath)
	{
		return model.Json.CopyJsonPath(jsonPath).Map<Either<JsonParseError, string>>((JToken maybeStr) => maybeStr.ConvertToType<string>()).Sequence()
			.MapJsonErrorToManifestError();
	}
}
