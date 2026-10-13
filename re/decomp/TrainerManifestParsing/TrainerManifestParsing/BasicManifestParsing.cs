using System;
using System.Collections.Generic;
using System.Linq;
using Domain.TrainerPackages;
using LanguageExt;

namespace TrainerManifestParsing;

public static class BasicManifestParsing
{
	public static Validation<ManifestParsingError, BasicManifestData> ParseManifest(TrainerManifestV2OrGreaterRootJson v2Manifest)
	{
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		TrainerManifestV2OrGreaterRootJson v2Manifest2 = v2Manifest;
		return new ValueTuple<Validation<ManifestParsingError, TrainerFamilyIdentifier>, Validation<ManifestParsingError, TrainerVersionIdentifier>, Validation<ManifestParsingError, Version>, Validation<ManifestParsingError, string>, Validation<ManifestParsingError, string>, Validation<ManifestParsingError, RelativePackagePath>, Validation<ManifestParsingError, RelativePackagePath>, ValueTuple<Validation<ManifestParsingError, System.Collections.Generic.IEnumerable<AirframeCategoryClassType>>, Validation<ManifestParsingError, AvionicsType>, Validation<ManifestParsingError, int>>>(v2Manifest2.ParseFamilyId().ToValidation(), v2Manifest2.ParseVersionId().ToValidation(), v2Manifest2.ParseBinaryVersion().ToValidation(), v2Manifest2.ParseDisplayName().ToValidation(), v2Manifest2.ParseVersionString().ToValidation(), v2Manifest2.ParseExePath().ToValidation(), v2Manifest2.ParseIconPath().ToValidation(), new ValueTuple<Validation<ManifestParsingError, System.Collections.Generic.IEnumerable<AirframeCategoryClassType>>, Validation<ManifestParsingError, AvionicsType>, Validation<ManifestParsingError, int>>(Enumerable.Select<Either<ManifestParsingError, AirframeCategoryClassType>, Validation<ManifestParsingError, AirframeCategoryClassType>>(v2Manifest2.EnumerateSupportedAirframesForAllModels(), (Func<Either<ManifestParsingError, AirframeCategoryClassType>, Validation<ManifestParsingError, AirframeCategoryClassType>>)((Either<ManifestParsingError, AirframeCategoryClassType> e) => e.ToValidation())).Sequence(), v2Manifest2.ParseAvionicsType().ToValidation(), v2Manifest2.ParseMaxInstances().ToValidation())).Apply<ManifestParsingError, TrainerFamilyIdentifier, TrainerVersionIdentifier, Version, string, string, RelativePackagePath, RelativePackagePath, System.Collections.Generic.IEnumerable<AirframeCategoryClassType>, AvionicsType, int, BasicManifestData>((TrainerFamilyIdentifier fid, TrainerVersionIdentifier vid, Version bv, string name, string vs, RelativePackagePath exe, RelativePackagePath icon, System.Collections.Generic.IEnumerable<AirframeCategoryClassType> airframes, AvionicsType avionics, int max) => new BasicManifestData(v2Manifest2.SchemaVersion, fid, vid, bv, name, vs, exe, icon, (System.Collections.Generic.IReadOnlyList<AirframeCategoryClassType>)Enumerable.ToList<AirframeCategoryClassType>(airframes), avionics, max));
	}

	public static Validation<ManifestParsingError, BasicModelData> ParseModel(ModelJsonWrapper model)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		return new ValueTuple<Validation<ManifestParsingError, TrainerModelIdentifier>, Validation<ManifestParsingError, string>, Validation<ManifestParsingError, Option<string>>, Validation<ManifestParsingError, RelativePackagePath>, Validation<ManifestParsingError, int>, Validation<ManifestParsingError, System.Collections.Generic.IEnumerable<AirframeCategoryClassType>>>(model.GetModelId().ToValidation(), model.GetModelName().ToValidation(), model.GetModelDescription().ToValidation(), model.GetModelIconPath().ToValidation(), model.GetBaconIndex().ToValidation(), model.EnumerateModelSupportedAirframesIfTheyExist().Sequence().ToValidation()).Apply<ManifestParsingError, TrainerModelIdentifier, string, Option<string>, RelativePackagePath, int, System.Collections.Generic.IEnumerable<AirframeCategoryClassType>, BasicModelData>((TrainerModelIdentifier id, string name, Option<string> maybeDescription, RelativePackagePath icon, int bacon, System.Collections.Generic.IEnumerable<AirframeCategoryClassType> additonalAirframes) => new BasicModelData(id, name, maybeDescription.IfNone(string.Empty), icon, new DisplayOrderIndex(bacon), additonalAirframes.ToArr()));
	}
}
