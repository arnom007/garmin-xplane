using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using LOLA.Certificates;

namespace LauncherCoreLib.Utility;

public class CertificateChainErrorLink : CertificateChainLinkInfo
{
	[field: CompilerGenerated]
	public System.Collections.Generic.IReadOnlyList<X509ChainStatus> ErrorStatus
	{
		[CompilerGenerated]
		get;
	}

	public CertificateChainErrorLink(PublicCertInfo certificateInfo, System.Collections.Generic.IReadOnlyList<X509ChainStatus> errors)
		: base(certificateInfo)
	{
		ErrorStatus = errors;
	}

	public string FormatErrors()
	{
		return string.Join(", ", Enumerable.Select<X509ChainStatus, string>((System.Collections.Generic.IEnumerable<X509ChainStatus>)ErrorStatus, (Func<X509ChainStatus, string>)((X509ChainStatus e) => $"{((X509ChainStatus)(ref e)).Status} -> {((X509ChainStatus)(ref e)).StatusInformation}")));
	}

	public virtual string ToString()
	{
		return "[" + base.CertificateInfo.Thumbprint + "] " + FormatErrors();
	}
}
