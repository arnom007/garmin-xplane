using System.Runtime.CompilerServices;
using Domain.TrainerPackages;

namespace TrainerManifestParsing;

public class ManifestSchemaVersionErrorWrapper : ManifestParsingError
{
	[field: CompilerGenerated]
	public ManifestSchemaVersionError VersionError
	{
		[CompilerGenerated]
		get;
	}

	public ManifestSchemaVersionErrorWrapper(ManifestSchemaVersionError versionError)
	{
		VersionError = versionError;
	}
}
