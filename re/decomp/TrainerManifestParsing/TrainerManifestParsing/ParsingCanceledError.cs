using System.Runtime.CompilerServices;
using System.Threading;

namespace TrainerManifestParsing;

public sealed class ParsingCanceledError : JsonParseError
{
	[field: CompilerGenerated]
	public CancellationToken Token
	{
		[CompilerGenerated]
		get;
	}

	public override string ErrorMessage => "Parsing was canceled.";

	public ParsingCanceledError(CancellationToken token)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Token = token;
	}
}
