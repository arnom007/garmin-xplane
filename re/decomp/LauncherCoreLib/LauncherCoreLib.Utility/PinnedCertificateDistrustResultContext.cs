using System.Runtime.CompilerServices;
using LOLA.Certificates;

namespace LauncherCoreLib.Utility;

public sealed class PinnedCertificateDistrustResultContext : CertificateTrustResultContext
{
	[field: CompilerGenerated]
	public PinnedCertificateChainDistrust DistrustReason
	{
		[CompilerGenerated]
		get;
	}

	public PinnedCertificateDistrustResultContext(PinnedCertificateChainDistrust distrustReason)
	{
		DistrustReason = distrustReason;
	}
}
