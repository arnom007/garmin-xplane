using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Utility;

public class CertificateChainInfo
{
	[field: CompilerGenerated]
	public System.Collections.Generic.IReadOnlyList<CertificateChainLinkInfo> Chain
	{
		[CompilerGenerated]
		get;
	}

	public CertificateChainInfo(System.Collections.Generic.IReadOnlyList<CertificateChainLinkInfo> chain)
	{
		Chain = chain;
	}
}
