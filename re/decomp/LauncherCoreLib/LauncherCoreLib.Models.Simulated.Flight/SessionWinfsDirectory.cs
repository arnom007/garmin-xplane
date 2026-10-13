using System;
using System.IO;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class SessionWinfsDirectory
{
	public const string PROGRAM_DATA_WINFS_DIRECTORY_NAME = "winfs";

	public static string GetSimDirPath(IDeploymentPathProvider deploymentPathProvider, IUserIdProvider userIdProvider, SimAircraftKey simAircraftKey)
	{
		return Path.Combine(new string[5]
		{
			deploymentPathProvider.AsciiSafeDirectory(),
			"winfs",
			GetUniqueUserIdOrUnknown(userIdProvider),
			simAircraftKey.AircraftIdentifier.IdentStringChecked,
			simAircraftKey.InstallationKey.ToStandardRelativePath()
		});
	}

	private static string GetUniqueUserIdOrUnknown(IUserIdProvider userIdProvider)
	{
		try
		{
			return userIdProvider.UniqueUserId;
		}
		catch (NotSupportedException)
		{
			return "UNKNOWN";
		}
	}
}
