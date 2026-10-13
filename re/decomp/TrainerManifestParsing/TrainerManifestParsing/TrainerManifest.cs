using System;
using System.Collections.Generic;
using System.Linq;
using Domain.TrainerPackages;
using Domain.UtilTypes;
using LanguageExt;
using Newtonsoft.Json.Linq;
using TrainerManifestParsing.Serialized;

namespace TrainerManifestParsing;

public static class TrainerManifest
{
	private const string MANIFEST_SCHEMA_PROPERTY = "ManifestVersion";

	private const string FAMILY_ID_PROPERTY = "FamilyId";

	private const string VERSION_ID_PROPERTY = "VersionId";

	public static Validation<ManifestParsingError, IRootManifestJsonProvider> UseOverrideIfExists(this IRootManifestJsonProvider manifestObjectProvider, NonEmptyString overrideName)
	{
		NonEmptyString overrideName2 = overrideName;
		IRootManifestJsonProvider manifestObjectProvider2 = manifestObjectProvider;
		return manifestObjectProvider2.Json.CopyJsonPath($"Overrides.{overrideName2}").Map<Validation<ManifestParsingError, IRootManifestJsonProvider>>((JToken jt) => jt.AsJsonObject().MapJsonErrorToManifestError().ToValidation()
			.Bind<JObject>((JObject jo) => (from propName in Prelude.Seq("ManifestVersion", "FamilyId", "VersionId", "BinaryVersion")
				select Prelude.Optional(jo.SelectToken(propName)).Map<ManifestParsingError>((JToken _) => new JsonValueError($"'Overrides.{overrideName2}.{propName}' cannot be set. That property cannot be overridden.").ToManifestError())).Somes().Match<Validation<ManifestParsingError, JObject>>((Func<Validation<ManifestParsingError, JObject>>)(() => Prelude.Success<ManifestParsingError, JObject>(jo)), (Func<Seq<ManifestParsingError>, Validation<ManifestParsingError, JObject>>)((Seq<ManifestParsingError> errs) => Prelude.Fail<ManifestParsingError, JObject>(errs))))
			.Bind<IRootManifestJsonProvider>(delegate(JObject jo)
			{
				JObject jObject = manifestObjectProvider2.Json.CopyJsonObject();
				jObject.SelectToken("Overrides").AssertValid().AsJsonObject()
					.AssertValid()
					.Remove(overrideName2);
				jObject.Merge(jo, new JsonMergeSettings
				{
					MergeArrayHandling = MergeArrayHandling.Replace,
					MergeNullValueHandling = MergeNullValueHandling.Merge,
					PropertyNameComparison = (StringComparison)4
				});
				return jObject.ToJsonObjectProvider().ParseAsManifest();
			})).Sequence()
			.Map<IRootManifestJsonProvider>((Option<IRootManifestJsonProvider> o) => o.IfNone(manifestObjectProvider2));
	}

	public static Validation<ManifestParsingError, IRootManifestJsonProvider> ParseAsManifest(this JObject manifestObject)
	{
		return manifestObject.ToJsonObjectProvider().ParseAsManifest();
	}

	public static Validation<ManifestParsingError, IRootManifestJsonProvider> ParseAsManifest(this JsonObjectProvider manifestObjectProvider)
	{
		return (Validation<ManifestParsingError, IRootManifestJsonProvider>)(IRootManifestJsonProvider)new TrainerManifestRootJson(manifestObjectProvider);
	}

	public static Validation<ManifestParsingError, TrainerManifestV2OrGreaterRootJson> ToV2OrGreater(this IRootManifestJsonProvider root)
	{
		return TrainerManifestV2OrGreaterRootJson.Parse(root);
	}

	public static Validation<ManifestParsingError, TrainerManifestV3OrGreaterRootJson> ToV3OrGreater(this IRootManifestJsonProvider root)
	{
		return TrainerManifestV3OrGreaterRootJson.Parse(root);
	}

	public static Either<ManifestParsingError, ManifestSchemaVersion> ParseManifestSchema(this IRootManifestJsonProvider manifest)
	{
		return manifest.Json.CopyRequiredJsonPath("ManifestVersion").Bind<ManifestSchemaVersion>((JToken t) => t.ConvertToType<int>().Bind<ManifestSchemaVersion>((int i) => ManifestSchemaVersion.ConvertToSchemaOrNone(i).ToEither<JsonParseError>(() => new JsonValueError("Must be 1 or greater.").AsParseError()).AttachJsonPath("ManifestVersion"))).MapJsonErrorToManifestError();
	}

	public static Either<ManifestParsingError, TrainerFamilyIdentifier> ParseFamilyId(this IRootManifestJsonProvider manifest)
	{
		return manifest.Json.ParseTrainerIdentifierAtPath("FamilyId").Map<TrainerFamilyIdentifier>((TrainerManifestIdentifier id) => new TrainerFamilyIdentifier(id.InnerId));
	}

	public static Either<ManifestParsingError, TrainerVersionIdentifier> ParseVersionId(this TrainerManifestV2OrGreaterRootJson v2manifest)
	{
		return v2manifest.Json.ParseTrainerIdentifierAtPath("VersionId").Map<TrainerVersionIdentifier>((TrainerManifestIdentifier id) => new TrainerVersionIdentifier(id.InnerId));
	}

	public static Either<ManifestParsingError, string> ParseDisplayName(this IRootManifestJsonProvider manifest)
	{
		return manifest.Json.ParseRequiredManifestStringAtPath("DisplayName");
	}

	public static Either<ManifestParsingError, string> ParseVersionString(this IRootManifestJsonProvider manifest)
	{
		return manifest.Json.ParseRequiredManifestStringAtPath("VersionString");
	}

	public static Either<ManifestParsingError, Version> ParseBinaryVersion(this TrainerManifestV2OrGreaterRootJson v2manifest)
	{
		return v2manifest.Json.CopyRequiredJsonPath("BinaryVersion").ConvertToType<Version>().MapJsonErrorToManifestError();
	}

	public static Either<ManifestParsingError, RelativePackagePath> ParseExePath(this IRootManifestJsonProvider manifest)
	{
		return manifest.Json.ParseRequiredRelativePathAtJsonPath("ExecutablePath");
	}

	public static Either<ManifestParsingError, AvionicsType> ParseAvionicsType(this IRootManifestJsonProvider manifest)
	{
		return manifest.Json.ParseRequiredManifestStringAtPath("AvionicsType").Bind<Avionics_category>(delegate(string s)
		{
			string s2 = s;
			return Prelude.parseEnum<Avionics_category>(s2).ToEither<ManifestParsingError>(() => new JsonValueError("Invalid enum: " + s2).AttachJsonPath("AvionicsType").ToManifestError());
		}).Map<AvionicsType>((Avionics_category s) => PermanentCommonDataTypes.ConvertSerialToActual(s));
	}

	public static Either<ManifestParsingError, RelativePackagePath> ParseIconPath(this IRootManifestJsonProvider manifest)
	{
		return manifest.Json.ParseRequiredRelativePathAtJsonPath("IconImagePath");
	}

	public static Either<ManifestParsingError, int> ParseMaxInstances(this IRootManifestJsonProvider manifest)
	{
		return manifest.ParseRequiredIntegerAtPath("MaxInstances");
	}

	public static System.Collections.Generic.IEnumerable<Either<ManifestParsingError, History_entry>> GetHistory(this IRootManifestJsonProvider manifest)
	{
		return manifest.Json.CopyJsonPath("History").Map<System.Collections.Generic.IEnumerable<Either<JsonParseError, History_entry>>>((JToken t) => t.AsJsonArray().Map<System.Collections.Generic.IEnumerable<Either<JsonParseError, History_entry>>>((JArray ja) => Enumerable.Select<JToken, Either<JsonParseError, History_entry>>((System.Collections.Generic.IEnumerable<JToken>)ja, (Func<JToken, Either<JsonParseError, History_entry>>)((JToken o) => o.ConvertToType<History_entry>()))).LeftToRightEnumerable()).Map<System.Collections.Generic.IEnumerable<Either<ManifestParsingError, History_entry>>>((System.Collections.Generic.IEnumerable<Either<JsonParseError, History_entry>> s) => Enumerable.Select<Either<JsonParseError, History_entry>, Either<ManifestParsingError, History_entry>>(s, (Func<Either<JsonParseError, History_entry>, Either<ManifestParsingError, History_entry>>)((Either<JsonParseError, History_entry> e) => e.MapJsonErrorToManifestError())))
			.IfNone((Func<System.Collections.Generic.IEnumerable<Either<ManifestParsingError, History_entry>>>)(() => Enumerable.Empty<Either<ManifestParsingError, History_entry>>()));
	}

	public static Either<ManifestParsingError, Option<JObject>> GetEtc(this IRootManifestJsonProvider manifest)
	{
		return manifest.Json.CopyJsonPath("etc").Map<Either<JsonParseError, JObject>>((JToken t) => t.AsJsonObject()).Sequence()
			.MapJsonErrorToManifestError();
	}

	public static System.Collections.Generic.IEnumerable<Either<ManifestParsingError, AirframeCategoryClassType>> EnumerateSupportedAirframesForAllModels(this IRootManifestJsonProvider manifest)
	{
		return manifest.Json.EnumerateSupportedAirframesProperty();
	}

	public static System.Collections.Generic.IEnumerable<Either<ManifestParsingError, AirframeCategoryClassType>> EnumerateSupportedAirframesForModel(this IRootManifestJsonProvider manifest, TrainerModelIdentifier modelId)
	{
		IRootManifestJsonProvider manifest2 = manifest;
		return manifest2.GetModelById(modelId).Bind<ModelJsonWrapper>((Option<ModelJsonWrapper> m) => m.ToEither<ManifestParsingError>(() => new MissingJsonPathError($"Models/{modelId}").ToManifestError())).Map<System.Collections.Generic.IEnumerable<Either<ManifestParsingError, AirframeCategoryClassType>>>((ModelJsonWrapper m) => Enumerable.Concat<Either<ManifestParsingError, AirframeCategoryClassType>>(manifest2.Json.EnumerateSupportedAirframesProperty(), m.EnumerateModelSupportedAirframesIfTheyExist()))
			.LeftToRightEnumerable();
	}

	public static System.Collections.Generic.IEnumerable<Either<ManifestParsingError, AirframeCategoryClassType>> EnumerateSupportedAirframesProperty(this IJsonProvider manifestOrModel)
	{
		return manifestOrModel.CopyRequiredJsonPath("SupportedAirframes").Bind<System.Collections.Generic.IEnumerable<Either<ManifestParsingError, AirframeCategoryClassType>>>((JToken t) => t.AsJsonArray().Map<System.Collections.Generic.IEnumerable<Either<ManifestParsingError, AirframeCategoryClassType>>>((JArray ja) => ja.EnumerateAirframes())).MapJsonErrorToManifestError()
			.LeftToRightEnumerable();
	}

	public static System.Collections.Generic.IEnumerable<Either<ManifestParsingError, AirframeCategoryClassType>> EnumerateAirframes(this JArray airframeArray)
	{
		return Enumerable.Select<JToken, Either<ManifestParsingError, AirframeCategoryClassType>>((System.Collections.Generic.IEnumerable<JToken>)airframeArray.Children(), (Func<JToken, Either<ManifestParsingError, AirframeCategoryClassType>>)((JToken t) => t.ConvertToType<Airframe_type>().Map<AirframeCategoryClassType>((Airframe_type s) => PermanentCommonDataTypes.ConvertSerialToActual(s)).MapJsonErrorToManifestError()));
	}

	public static Either<ManifestParsingError, RelativePackagePath> ParseAsRelativePath(this Either<ManifestParsingError, string> either, string jsonPath)
	{
		string jsonPath2 = jsonPath;
		return either.Bind<RelativePackagePath>((string s) => ParseAsRelativePath(s, jsonPath2));
	}

	public static Either<ManifestParsingError, RelativePackagePath> ParseAsRelativePath(string path, string jsonPath)
	{
		string jsonPath2 = jsonPath;
		return RelativePackagePath.Parse(path).ToEither().MapLeft<ManifestParsingError>((Seq<RelativePackagePathError> errors) => new JsonValueError(string.Join<RelativePackagePathError>(" ", (System.Collections.Generic.IEnumerable<RelativePackagePathError>)errors)).AttachJsonPath(jsonPath2).ToManifestError());
	}

	public static Either<ManifestParsingError, int> ParseRequiredIntegerAtPath(this IRootManifestJsonProvider manifest, string jsonPath)
	{
		string jsonPath2 = jsonPath;
		return manifest.Json.CopyRequiredJsonPath(jsonPath2).Bind<int>((JToken j) => j.ConvertToType<int>().AttachJsonPath(jsonPath2)).MapJsonErrorToManifestError();
	}

	public static Either<ManifestParsingError, RelativePackagePath> ParseRequiredRelativePathAtJsonPath(this IJsonProvider json, string jsonPath)
	{
		return json.ParseRequiredManifestStringAtPath(jsonPath).ParseAsRelativePath(jsonPath);
	}

	public static Either<ManifestParsingError, string> ParseRequiredManifestStringAtPath(this IJsonProvider json, string jsonPath)
	{
		return json.ParseRequiredStringAtPath<ManifestParsingError>(jsonPath, MapJsonErrorToManifestError);
	}

	public static Either<ManifestParsingError, TrainerManifestIdentifier> ParseTrainerIdentifierAtPath(this IJsonProvider json, string jsonPath)
	{
		return json.ParseStringIdentifierAtPath(jsonPath).Map<TrainerManifestIdentifier>((StringBasedId s) => new TrainerManifestIdentifier(s)).MapJsonErrorToManifestError();
	}

	public static Either<ManifestParsingError, T> MapJsonErrorToManifestError<T, TJE>(this Either<TJE, T> either) where TJE : JsonParseError
	{
		return either.MapLeft<ManifestParsingError>((TJE e) => e.ToManifestError());
	}

	public static Validation<ManifestParsingError, JObject> ToManifestJObjectValidation(this JToken token)
	{
		return token.AsJsonObject().MapJsonErrorToManifestError().ToValidation();
	}

	public static Validation<ManifestParsingError, IRootManifestJsonProvider> ValidateBasicManifest(this Either<JsonParseError, JToken> initialJsonParse)
	{
		return initialJsonParse.AsJsonObject().MapLeft<ManifestParsingError>((JsonParseError je) => je.ToManifestError()).ToValidation()
			.Bind<IRootManifestJsonProvider>((JObject jobj) => jobj.ParseAsManifest());
	}
}
