using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace LauncherCoreLib.Models.DeployedEnv;

public class WindowsRegistryInstallationVariables : IInstallationVariableProvider
{
	private const string WIN_REG_KEY_PATH_FORMAT = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Garmin\\{0}\\";

	private const string ALT_WIN_REG_KEY_PATH_FORMAT = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Garmin\\{0}\\";

	private readonly string _mainWinRegKeyPath;

	private readonly string _64BitWinRegKeyPath;

	public WindowsRegistryInstallationVariables(ISuiteNameProvider suiteNameProvider)
	{
		_mainWinRegKeyPath = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "HKEY_LOCAL_MACHINE\\SOFTWARE\\Garmin\\{0}\\", (object)suiteNameProvider.SuiteName);
		_64BitWinRegKeyPath = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Garmin\\{0}\\", (object)suiteNameProvider.SuiteName);
	}

	public System.Collections.Generic.IEnumerable<string> GetExpectedPaths(string variableName)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		string variableName2 = variableName;
		if (variableName2 == null)
		{
			throw new ArgumentNullException("variableName");
		}
		return Enumerable.Select<string, string>((System.Collections.Generic.IEnumerable<string>)new string[2] { _mainWinRegKeyPath, _64BitWinRegKeyPath }, (Func<string, string>)((string prefix) => prefix + variableName2));
	}

	public string? GetString(string variableName)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (variableName == null)
		{
			throw new ArgumentNullException("variableName");
		}
		try
		{
			return (Registry.GetValue(_mainWinRegKeyPath, variableName, (object)null) ?? Registry.GetValue(_64BitWinRegKeyPath, variableName, (object)null)) as string;
		}
		catch (IOException)
		{
			return null;
		}
	}
}
