using LOLA.Certificates;

namespace LauncherCoreLib.Utility;

public class CertificateChainTrustedLink : CertificateChainLinkInfo
{
	public CertificateChainTrustedLink(PublicCertInfo certificateInfo)
		: base(certificateInfo)
	{
	}
}
