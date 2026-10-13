using Domain.UtilTypes;
using LanguageExt;

namespace TrainerManifestParsing;

public static class ManifestParsingAugmentation
{
	public delegate Validation<ManifestParsingError, IRootManifestJsonProvider> ManifestJsonAugmentation(IRootManifestJsonProvider manifestJson);

	public static ManifestJsonAugmentation LoadOverrideIfAvailable(NonEmptyString overrideName)
	{
		NonEmptyString overrideName2 = overrideName;
		return (IRootManifestJsonProvider json) => json.UseOverrideIfExists(overrideName2);
	}
}
