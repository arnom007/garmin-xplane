using System.Runtime.CompilerServices;
using Domain.TrainerPackages;
using LanguageExt;

namespace TrainerManifestParsing;

public class TrainerManifestV2OrGreaterRootJson : IRootManifestJsonProvider
{
	[field: CompilerGenerated]
	public IRootManifestJsonProvider ActualProvider
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ManifestSchemaVersion SchemaVersion
	{
		[CompilerGenerated]
		get;
	}

	public JsonObjectProvider Json => ActualProvider.Json;

	private TrainerManifestV2OrGreaterRootJson(IRootManifestJsonProvider validatedRoot, ManifestSchemaVersion schemaVersion)
	{
		ActualProvider = validatedRoot;
		SchemaVersion = schemaVersion;
	}

	public static Validation<ManifestParsingError, TrainerManifestV2OrGreaterRootJson> Parse(IRootManifestJsonProvider manifest)
	{
		IRootManifestJsonProvider manifest2 = manifest;
		ManifestSchemaVersion requiredVersion = new ManifestSchemaVersion(2);
		return manifest2.ParseManifestSchema().Bind<TrainerManifestV2OrGreaterRootJson>((ManifestSchemaVersion schemaVersion) => (schemaVersion >= requiredVersion) ? ((Either<ManifestParsingError, TrainerManifestV2OrGreaterRootJson>)new TrainerManifestV2OrGreaterRootJson(manifest2, schemaVersion)) : ((Either<ManifestParsingError, TrainerManifestV2OrGreaterRootJson>)Prelude.Left(new ManifestSchemaVersionErrorWrapper(new ManifestSchemaVersionError(schemaVersion, requiredVersion)).AsBase()))).ToValidation();
	}
}
