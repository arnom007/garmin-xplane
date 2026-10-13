using System.Runtime.CompilerServices;
using System.Text;
using Domain.UtilTypes;
using LanguageExt;

namespace TrainerManifestParsing;

public readonly record struct TrainerManifestIdentifier(StringBasedId InnerId)
{
	public static Validation<ManifestParsingError, TrainerManifestIdentifier> Parse(string id)
	{
		return StringBasedId.Parse(id).Map<TrainerManifestIdentifier>((StringBasedId id) => new TrainerManifestIdentifier(id)).MapFail<ManifestParsingError>((StringIdentifierError e) => new IdentifierErrorWrapper(e));
	}

	public override string ToString()
	{
		return ((object)InnerId).ToString();
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("InnerId = ");
		builder.Append(((object)InnerId).ToString());
		return true;
	}
}
