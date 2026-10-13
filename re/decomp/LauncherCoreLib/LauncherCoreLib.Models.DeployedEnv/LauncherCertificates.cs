using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using LOLA.Certificates;

namespace LauncherCoreLib.Models.DeployedEnv;

public class LauncherCertificates : ISigningCertificatesProvider
{
	private const string PUBLIC_CERTS_RESOURCE_PREFIX = "LauncherCoreLib.Resources.public_certs.";

	private static readonly Lazy<IReadOnlyDictionary<string, CertificateMetaWrapper>> LAZY_CERTS = new Lazy<IReadOnlyDictionary<string, CertificateMetaWrapper>>((Func<IReadOnlyDictionary<string, CertificateMetaWrapper>>)GetCertsOnce);

	public System.Collections.Generic.IEnumerable<CertificateMetaWrapper> KnownCertificates()
	{
		return LAZY_CERTS.Value.Values;
	}

	public CertificateMetaWrapper GetCertificateByThumbprint(string thumbprint)
	{
		return LAZY_CERTS.Value[thumbprint];
	}

	public bool ThumbprintIsKnown(string thumbprint)
	{
		return LAZY_CERTS.Value.ContainsKey(thumbprint);
	}

	private static IReadOnlyDictionary<string, CertificateMetaWrapper> GetCertsOnce()
	{
		Assembly thisAssembly = typeof(LauncherCertificates).Assembly;
		return (IReadOnlyDictionary<string, CertificateMetaWrapper>)(object)Enumerable.ToDictionary<CertificateMetaWrapper, string>(Enumerable.Select<string, CertificateMetaWrapper>(Enumerable.Where<string>((System.Collections.Generic.IEnumerable<string>)thisAssembly.GetManifestResourceNames(), (Func<string, bool>)((string rn) => rn.StartsWith("LauncherCoreLib.Resources.public_certs.", (StringComparison)4))), (Func<string, CertificateMetaWrapper>)((string resourceName) => LoadCertificateFromStream(thisAssembly, resourceName))), (Func<CertificateMetaWrapper, string>)((CertificateMetaWrapper cw) => cw.CertInfo.Thumbprint));
		[CompilerGenerated]
		static CertificateMetaWrapper LoadCertificateFromStream(Assembly assembly, string resourceName)
		{
			Stream manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
			try
			{
				byte[] certBytes = ConvertStreamToBytes(manifestResourceStream);
				return new CertificateMetaWrapper(resourceName, PublicCertInfo.LoadFromCertAndDispose((Func<X509Certificate2>)(() => new X509Certificate2(certBytes))));
			}
			finally
			{
				((System.IDisposable)manifestResourceStream)?.Dispose();
			}
		}
	}

	private static byte[] ConvertStreamToBytes(Stream stream)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		MemoryStream val = new MemoryStream();
		try
		{
			stream.CopyTo((Stream)(object)val);
			return val.ToArray();
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
	}
}
