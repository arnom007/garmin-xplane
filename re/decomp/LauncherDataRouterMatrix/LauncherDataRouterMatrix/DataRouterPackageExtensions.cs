using System;
using TrainerPackageLib;
using TrainerPackageLib.Serialized;

namespace LauncherDataRouterMatrix;

public static class DataRouterPackageExtensions
{
	private static readonly object VERSION_KEY = new object();

	public static Version? GetDataRouterClientVersionOrNull(this ITrainerPackage package)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		Version val = new Version();
		Version dataRouterClientVersionOr = package.GetDataRouterClientVersionOr(val);
		if (!(dataRouterClientVersionOr == val))
		{
			return dataRouterClientVersionOr;
		}
		return null;
	}

	public static Version GetDataRouterClientVersionOr(this ITrainerPackage package, Version invalidVersion)
	{
		ITrainerPackage package2 = package;
		return package2.GetExtensionData<Version>(VERSION_KEY, delegate
		{
			Data_formats data_formats = package2.BaseInfo.Manifest.CopyDataFormats();
			Version val = default(Version);
			return (data_formats == null || !Version.TryParse(data_formats.DataRouterClientVersion, ref val)) ? null : val;
		}) ?? invalidVersion;
	}
}
