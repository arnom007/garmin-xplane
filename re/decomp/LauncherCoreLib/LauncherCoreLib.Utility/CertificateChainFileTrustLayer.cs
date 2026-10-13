using System.Runtime.CompilerServices;
using LOLA.Certificates;

namespace LauncherCoreLib.Utility;

public class CertificateChainFileTrustLayer : DecoratedFileTrustLayerBase
{
	[field: CompilerGenerated]
	public CertificateChainInfo CertificateChain
	{
		[CompilerGenerated]
		get;
	}

	public CertificateChainFileTrustLayer(IFileTrustLayer innerLayer, CertificateChainInfo chain)
		: base(innerLayer)
	{
		CertificateChain = chain;
	}
}
