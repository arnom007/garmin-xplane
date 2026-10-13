using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LOLA.Certificates;
using LOLA.Logging;
using LauncherCoreLib.Models.EventLogging;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;

namespace LauncherCoreLib.Utility;

public class LauncherLayerFileTrustProvider : ILayeredFileTrustProvider
{
	private sealed class FileTrustErrorEvent : GenericTrackedEventDataDescriptor
	{
		private readonly string _filePath;

		private readonly string _reason;

		protected override PropertiesBuilder BuildProperties => [CompilerGenerated] () => new KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>[2]
		{
			KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(AppCenterPropertyName.ParseAndThrowOnErrors("File"), AppCenterPropertyValue.ParseTruncateFromBeginningAndHandleErrors(_filePath)),
			KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(AppCenterPropertyName.ParseAndThrowOnErrors("Reason"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(_reason))
		};

		protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("FILE_TRUST_ERROR"), LauncherAppTarget.Instance);

		public override TrackedEventContainer Instance()
		{
			return EventPrototype.ToInstance().With(BuildProperties);
		}

		public FileTrustErrorEvent(string filePath, string reason)
		{
			_filePath = filePath;
			_reason = reason;
		}
	}

	private readonly ILayeredFileTrustProvider _innerFileTrust;

	public LauncherLayerFileTrustProvider(ILayeredFileTrustProvider innerLayer)
	{
		_innerFileTrust = innerLayer;
	}

	public IFileTrustLayer GetFileTrust(string filePath)
	{
		IFileTrustLayer fileTrust = _innerFileTrust.GetFileTrust(filePath);
		SwitchProviderLayer result = new SwitchProviderLayer(fileTrust, this);
		if (!fileTrust.IsTrusted)
		{
			ISignedFileTrustCertificates signedFileTrustCertificates = Enumerable.FirstOrDefault<ISignedFileTrustCertificates>(Enumerable.OfType<ISignedFileTrustCertificates>((System.Collections.IEnumerable)fileTrust.EnumerateLayers()));
			if (signedFileTrustCertificates != null)
			{
				System.Collections.Generic.IEnumerator<PublicCertInfo> enumerator = Enumerable.Select<FileCertificateInfo, PublicCertInfo>(Enumerable.Where<FileCertificateInfo>((System.Collections.Generic.IEnumerable<FileCertificateInfo>)signedFileTrustCertificates.Certificates, (Func<FileCertificateInfo, bool>)((FileCertificateInfo c) => !c.IsTrusted)), (Func<FileCertificateInfo, PublicCertInfo>)((FileCertificateInfo c) => c.Certificate)).GetEnumerator();
				try
				{
					while (((System.Collections.IEnumerator)enumerator).MoveNext())
					{
						PublicCertInfo current = enumerator.Current;
						Logger.Log(LogLevel.Warning, $"The '{filePath}' file is signed but not by a trusted source ({current.SubjectName.Name}). Thumbprint: {current.Thumbprint}");
					}
				}
				finally
				{
					((System.IDisposable)enumerator)?.Dispose();
				}
			}
			string text = Enumerable.FirstOrDefault<IFileDistrustReasonProvider>(Enumerable.OfType<IFileDistrustReasonProvider>((System.Collections.IEnumerable)fileTrust.EnumerateLayers()))?.DistrustReason ?? "None";
			Logger.Log(LogLevel.Warning, "The '{0}' is not trusted. Reason given: {1}", filePath, text);
			TrackedEvent.Log(new FileTrustErrorEvent(filePath, text));
		}
		return result;
	}

	private static bool IsAuthenticodeSigned(IFileTrustLayer fileTrust)
	{
		ISignedFileTrustCertificates signedFileTrustCertificates = Enumerable.FirstOrDefault<ISignedFileTrustCertificates>(Enumerable.OfType<ISignedFileTrustCertificates>((System.Collections.IEnumerable)fileTrust.EnumerateLayers()));
		if (signedFileTrustCertificates != null)
		{
			return Enumerable.Any<FileCertificateInfo>((System.Collections.Generic.IEnumerable<FileCertificateInfo>)signedFileTrustCertificates.Certificates, (Func<FileCertificateInfo, bool>)((FileCertificateInfo fc) => fc.Source == CertificateSource.AuthenticodeOrOtherCodeSigning));
		}
		return false;
	}
}
