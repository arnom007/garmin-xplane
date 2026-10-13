using System.Runtime.CompilerServices;
using LOLA.Certificates;

namespace LauncherCoreLib.Utility;

public abstract class CertificateChainLinkInfo
{
	[field: CompilerGenerated]
	public PublicCertInfo CertificateInfo
	{
		[CompilerGenerated]
		get;
	}

	protected CertificateChainLinkInfo(PublicCertInfo certificateInfo)
	{
		CertificateInfo = certificateInfo;
	}
}
