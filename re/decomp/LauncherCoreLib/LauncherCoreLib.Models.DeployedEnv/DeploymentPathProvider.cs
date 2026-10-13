using System;
using System.IO;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Properties;

namespace LauncherCoreLib.Models.DeployedEnv;

public class DeploymentPathProvider : IDeploymentPathProvider
{
	public const string LAUNCHER_DATA_DIR_NAME = "Launcher";

	public static readonly string GARMIN_COMPANY_NAME = Resources.COMPANY_NAME;

	[field: CompilerGenerated]
	public string DeploymentPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string UserAppDataDirPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string AppTempDirectoryPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string RootBinPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string RootDataPath
	{
		[CompilerGenerated]
		get;
	}

	public DeploymentPathProvider(IInstallationVariableProvider installationVariables, ISuiteNameProvider suiteNameProvider)
	{
		string text = Path.Combine(Environment.GetFolderPath((SpecialFolder)42), GARMIN_COMPANY_NAME, suiteNameProvider.SuiteName);
		string text2 = Path.Combine(Environment.GetFolderPath((SpecialFolder)35), GARMIN_COMPANY_NAME, suiteNameProvider.SuiteName);
		RootBinPath = GetBinPathFromRegistry(installationVariables) ?? text;
		RootDataPath = GetDataPathFromRegistry(installationVariables) ?? text2;
		DeploymentPath = Path.Combine(RootDataPath, "Launcher");
		UserAppDataDirPath = Path.Combine(Environment.GetFolderPath((SpecialFolder)28), GARMIN_COMPANY_NAME, suiteNameProvider.SuiteName, "Launcher");
		AppTempDirectoryPath = Path.Combine(Path.GetTempPath(), GARMIN_COMPANY_NAME, suiteNameProvider.SuiteName, "Launcher");
	}

	public static string? GetBinPathFromRegistry(IInstallationVariableProvider installationVariables)
	{
		return installationVariables.GetString(InstallationVariable.BinPath);
	}

	public static string? GetDataPathFromRegistry(IInstallationVariableProvider installationVariables)
	{
		return installationVariables.GetString(InstallationVariable.DataPath);
	}
}
