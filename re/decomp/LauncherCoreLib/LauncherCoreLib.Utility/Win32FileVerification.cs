using System;

namespace LauncherCoreLib.Utility;

public class Win32FileVerification : IFileVerification
{
	private readonly LauncherLayerFileTrustProvider _fileTrustProvider;

	public Win32FileVerification(LauncherLayerFileTrustProvider fileTrustProvider)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_fileTrustProvider = fileTrustProvider ?? throw new ArgumentNullException("fileTrustProvider");
	}

	public bool IsFileSignedByGarmin(string filepath)
	{
		return _fileTrustProvider.GetFileTrust(filepath).LogDistrustReasons().IsTrusted;
	}
}
