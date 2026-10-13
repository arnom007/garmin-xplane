using Domain.TrainerPackages;
using LanguageExt;
using Newtonsoft.Json.Linq;
using TrainerManifestParsing.Serialized;

namespace TrainerManifestParsing;

public static class DataFormatParsing
{
	public const string DATA_FORMATS_PROPERTY_NAME = "DataFormats";

	public static Validation<ManifestParsingError, Option<Data_formats>> ParseDataFormats(this IRootManifestJsonProvider manifestDataSource)
	{
		return manifestDataSource.Json.GetDataFormatsProperty().ToValidation();
	}

	public static Validation<ManifestParsingError, Option<Data_formats>> ParseModelDataFormats(this IRootManifestJsonProvider manifestDataSource, TrainerModelIdentifier modelIdentifier)
	{
		return manifestDataSource.GetModelById(modelIdentifier).Bind<Option<Data_formats>>((Option<ModelJsonWrapper> o) => o.Map<Either<ManifestParsingError, Option<Data_formats>>>((ModelJsonWrapper model) => model.Json.GetDataFormatsProperty()).Sequence().Map<Option<Data_formats>>((Option<Option<Data_formats>> o) => o.Flatten())).ToValidation();
	}

	public static Either<ManifestParsingError, Option<Data_formats>> GetDataFormatsProperty(this IJsonProvider manifestOrModel)
	{
		return manifestOrModel.CopyJsonPath("DataFormats").Map<Either<JsonParseError, Data_formats>>((JToken maybeToken) => maybeToken.ConvertToType<Data_formats>()).Sequence()
			.MapJsonErrorToManifestError();
	}
}
