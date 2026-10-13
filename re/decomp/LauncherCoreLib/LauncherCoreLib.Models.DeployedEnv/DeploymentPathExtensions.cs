using System;
using System.Globalization;
using System.IO;
using LOLA.Processes;
using LauncherCoreLib.Properties;

namespace LauncherCoreLib.Models.DeployedEnv;

public static class DeploymentPathExtensions
{
	public const string LOG_FILE_PREFIX = "trainer_launcher_";

	private static readonly Lazy<string> LOG_FILENAME = new Lazy<string>((Func<string>)UniqueFileName, true);

	private static readonly Lazy<string> THIS_PROCESS_ID = new Lazy<string>((Func<string>)ProcessId, false);

	public static string LogDirectoryPath(this IDeploymentPathProvider deploymentPaths)
	{
		return Path.Combine(deploymentPaths.AppTempDirectoryPath, "Logs");
	}

	public static string LogFilePath(this IDeploymentPathProvider deploymentPaths)
	{
		return Path.Combine(deploymentPaths.LogDirectoryPath(), LOG_FILENAME.Value);
	}

	public static string SessionBaseDirectoryPath(this IDeploymentPathProvider deployment)
	{
		return Path.Combine(deployment.AppTempDirectoryPath, "Sessions");
	}

	public static string PackagesDirectoryPath(this IDeploymentPathProvider deployment)
	{
		return Path.Combine(deployment.RootBinPath, Resources.PACKAGES_DIR_NAME);
	}

	public static string HangarDirectoryPath(this IDeploymentPathProvider deployment)
	{
		return Path.Combine(deployment.UserAppDataDirPath, "Hangar");
	}

	public static string OldWinfsDirectoryPath(this IDeploymentPathProvider deployment)
	{
		return Path.Combine(deployment.UserAppDataDirPath, "Winfs");
	}

	public static string DatabasesDirectoryPath(this IDeploymentPathProvider deployment)
	{
		return Path.Combine(deployment.RootDataPath, "Databases");
	}

	public static string PerProcessTempDirectory(this IDeploymentPathProvider deployment)
	{
		return Path.Combine(deployment.AppTempDirectoryPath, "process", THIS_PROCESS_ID.Value);
	}

	public static string AsciiSafeDirectory(this IDeploymentPathProvider deployment)
	{
		return Path.Combine(deployment.RootDataPath, "ascii");
	}

	private static string UniqueFileName()
	{
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "trainer_launcher_{0}.log", (object)System.DateTime.UtcNow.ToString("o", (IFormatProvider)(object)CultureInfo.InvariantCulture).Replace(":", "_", (StringComparison)4));
	}

	private static string ProcessId()
	{
		return ProcessOps.Instance.ThisProcess.ProcessId.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture);
	}
}
