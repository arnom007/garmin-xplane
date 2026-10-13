using System.Runtime.CompilerServices;
using Domain.UtilTypes;

namespace TrainerManifestParsing;

public class IdentifierErrorWrapper : ManifestParsingError
{
	[field: CompilerGenerated]
	public StringIdentifierError IdError
	{
		[CompilerGenerated]
		get;
	}

	public IdentifierErrorWrapper(StringIdentifierError idError)
	{
		IdError = idError;
	}
}
