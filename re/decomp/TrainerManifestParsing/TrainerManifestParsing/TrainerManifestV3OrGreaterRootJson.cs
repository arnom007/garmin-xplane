using System;
using System.Runtime.CompilerServices;
using Domain.TrainerPackages;
using LanguageExt;

namespace TrainerManifestParsing;

public class TrainerManifestV3OrGreaterRootJson : IRootManifestJsonProvider
{
	[field: CompilerGenerated]
	public IRootManifestJsonProvider ActualProvider
	{
		[CompilerGenerated]
		get;
	}

	public JsonObjectProvider Json => ActualProvider.Json;

	private TrainerManifestV3OrGreaterRootJson(IRootManifestJsonProvider validatedRoot)
	{
		ActualProvider = validatedRoot;
	}

	public static Validation<ManifestParsingError, TrainerManifestV3OrGreaterRootJson> Parse(IRootManifestJsonProvider manifest)
	{
		IRootManifestJsonProvider manifest2 = manifest;
		return manifest2.ParseManifestSchema().Bind<TrainerManifestV3OrGreaterRootJson>(delegate(ManifestSchemaVersion schemaVersion)
		{
			ManifestSchemaVersion manifestSchemaVersion = new ManifestSchemaVersion(3);
			return (schemaVersion >= manifestSchemaVersion) ? ((Either<ManifestParsingError, TrainerManifestV3OrGreaterRootJson>)Prelude.Right(new TrainerManifestV3OrGreaterRootJson(manifest2))) : ((Either<ManifestParsingError, TrainerManifestV3OrGreaterRootJson>)Prelude.Left(new ManifestSchemaVersionErrorWrapper(new ManifestSchemaVersionError(schemaVersion, manifestSchemaVersion)).AsBase()));
		}).ToValidation();
	}

	public TrainerManifestV2OrGreaterRootJson ToV2()
	{
		return this.ToV2OrGreater().IfFail((Func<Seq<ManifestParsingError>, TrainerManifestV2OrGreaterRootJson>)delegate(Seq<ManifestParsingError> e)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			throw new InvalidOperationException(((object)e).ToString());
		});
	}
}
