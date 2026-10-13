using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using LOLA.Certificates;
using LOLA.Logging;

namespace LauncherCoreLib.Utility;

public static class LolaCertificates
{
	public static ParseFileCertificates CreateParserForSource(this IFileSignatureParser p, CertificateSource source)
	{
		IFileSignatureParser p2 = p;
		return (string fp) => Enumerable.Select<PublicCertInfo, ValueTuple<PublicCertInfo, CertificateSource>>((System.Collections.Generic.IEnumerable<PublicCertInfo>)p2.GetSignatureCertificates(fp), (Func<PublicCertInfo, ValueTuple<PublicCertInfo, CertificateSource>>)((PublicCertInfo c) => new ValueTuple<PublicCertInfo, CertificateSource>(c, source)));
	}

	public static DetermineCertificateTrust CheckForAnyPinnedCertInEntireChain(ISigningCertificatesProvider pinnedCerts)
	{
		ISigningCertificatesProvider pinnedCerts2 = pinnedCerts;
		return delegate(CertificateTrustRequest request)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			X509Certificate2 val3 = request.Certificate.LoadBinaryCert();
			try
			{
				ValueTuple<X509Chain, bool> val4 = BuildAndVerifyCertificateChain(val3);
				X509Chain item = val4.Item1;
				bool item2 = val4.Item2;
				X509Chain val5 = item;
				try
				{
					List<CertificateChainLinkInfo> certChainLinks = Enumerable.ToList<CertificateChainLinkInfo>(Enumerable.Select<X509ChainElement, CertificateChainLinkInfo>(Enumerable.Cast<X509ChainElement>((System.Collections.IEnumerable)item.ChainElements), (Func<X509ChainElement, CertificateChainLinkInfo>)delegate(X509ChainElement ce)
					{
						PublicCertInfo certificateInfo = PublicCertInfo.LoadFromCert(ce.Certificate);
						return Enumerable.Any<X509ChainStatus>((System.Collections.Generic.IEnumerable<X509ChainStatus>)ce.ChainElementStatus, (Func<X509ChainStatus, bool>)((X509ChainStatus s) => (int)((X509ChainStatus)(ref s)).Status > 0)) ? ((CertificateChainLinkInfo)new CertificateChainErrorLink(certificateInfo, ce.ChainElementStatus)) : ((CertificateChainLinkInfo)new CertificateChainTrustedLink(certificateInfo));
					}));
					if (item2)
					{
						if (Enumerable.FirstOrDefault<CertificateChainLinkInfo>((System.Collections.Generic.IEnumerable<CertificateChainLinkInfo>)certChainLinks, (Func<CertificateChainLinkInfo, bool>)((CertificateChainLinkInfo ce) => pinnedCerts2.ThumbprintIsKnown(ce.CertificateInfo.Thumbprint))) == null)
						{
							Logger.Log(LogLevel.Error, $"The chain attached to {request.Context} is valid but it does not contain a known certificate:\n\t{FormatChain()}");
							return new CertificateTrustResult(isTrusted: false, new PinnedCertificateDistrustResultContext(PinnedCertificateChainDistrust.NoCertInChainMatches));
						}
						return new CertificateTrustResult(isTrusted: true);
					}
					if (pinnedCerts2.ThumbprintIsKnown(request.Certificate.Thumbprint))
					{
						return new CertificateTrustResult(isTrusted: true);
					}
					Logger.Log(LogLevel.Error, $"The chain attached to {request.Context} is invalid and the leaf certificate is not a known certificate:\n\t{FormatChain()}");
					return new CertificateTrustResult(isTrusted: false, new PinnedCertificateDistrustResultContext(PinnedCertificateChainDistrust.ChainIsNotTrustedByOS));
					[CompilerGenerated]
					string FormatChain()
					{
						return string.Join("\n\t", Enumerable.Select<CertificateChainLinkInfo, string>((System.Collections.Generic.IEnumerable<CertificateChainLinkInfo>)certChainLinks, (Func<CertificateChainLinkInfo, string>)delegate(CertificateChainLinkInfo c)
						{
							string text = ((c is CertificateChainErrorLink certificateChainErrorLink) ? ("\t" + certificateChainErrorLink.FormatErrors()) : string.Empty);
							return c.CertificateInfo.Thumbprint + ": " + c.CertificateInfo.SubjectName.Name + text;
						}));
					}
				}
				finally
				{
					((System.IDisposable)val5)?.Dispose();
				}
			}
			finally
			{
				((System.IDisposable)val3)?.Dispose();
			}
		};
		[CompilerGenerated]
		static ValueTuple<X509Chain, bool> BuildAndVerifyCertificateChain(X509Certificate2 cert)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			System.DateTime notAfter = cert.NotAfter;
			X509Chain val = new X509Chain();
			val.ChainPolicy.VerificationTime = notAfter;
			try
			{
				val.ChainPolicy.RevocationMode = (X509RevocationMode)2;
				bool flag = val.Build(cert);
				if (!flag && IsRevokationUnknown(val))
				{
					val.Dispose();
					X509Chain val2 = new X509Chain();
					val2.ChainPolicy.VerificationTime = notAfter;
					val2.ChainPolicy.RevocationMode = (X509RevocationMode)1;
					val2.ChainPolicy.UrlRetrievalTimeout = TimeSpan.FromSeconds(10.0);
					return new ValueTuple<X509Chain, bool>(val2, val2.Build(cert));
				}
				return new ValueTuple<X509Chain, bool>(val, flag);
			}
			catch
			{
				val.Dispose();
				throw;
			}
		}
		[CompilerGenerated]
		static bool IsRevokationUnknown(X509Chain offlineChain)
		{
			return Enumerable.Any<X509ChainElement>(Enumerable.Cast<X509ChainElement>((System.Collections.IEnumerable)offlineChain.ChainElements), (Func<X509ChainElement, bool>)((X509ChainElement ce) => Enumerable.Any<X509ChainStatus>((System.Collections.Generic.IEnumerable<X509ChainStatus>)ce.ChainElementStatus, (Func<X509ChainStatus, bool>)((X509ChainStatus s) => ((System.Enum)((X509ChainStatus)(ref s)).Status).HasFlag((System.Enum)(object)(X509ChainStatusFlags)64) || ((System.Enum)((X509ChainStatus)(ref s)).Status).HasFlag((System.Enum)(object)(X509ChainStatusFlags)16777216)))));
		}
	}
}
