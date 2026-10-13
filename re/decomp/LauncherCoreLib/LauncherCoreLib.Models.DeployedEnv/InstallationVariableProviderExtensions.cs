using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LauncherCoreLib.Models.DeployedEnv;

public static class InstallationVariableProviderExtensions
{
	public static string? GetString(this IInstallationVariableProvider installationVariables, InstallationVariable variable)
	{
		return installationVariables.GetString(variable.ToVariableKey());
	}

	public static System.Collections.Generic.IEnumerable<string> GetExpectedPaths(this IInstallationVariableProvider installationVariables, InstallationVariable variable)
	{
		return installationVariables.GetExpectedPaths(variable.ToVariableKey());
	}

	public static bool VariableWasDeployed(this IInstallationVariableProvider installationVariables, string variable)
	{
		return installationVariables.GetString(variable) != null;
	}

	public static bool VariableWasDeployed(this IInstallationVariableProvider installationVariables, InstallationVariable variable)
	{
		return installationVariables.VariableWasDeployed(variable.ToVariableKey());
	}

	public static string ToVariableKey(this InstallationVariable variable)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (variable == InstallationVariable.Invalid)
		{
			throw new NotSupportedException("Invalid variable key.");
		}
		return ((object)variable).ToString();
	}

	public static System.Collections.Generic.IEnumerable<InstallationVariable> ValidVariables()
	{
		return Enumerable.Where<InstallationVariable>(Enumerable.Cast<InstallationVariable>((System.Collections.IEnumerable)System.Enum.GetValues(typeof(InstallationVariable))), (Func<InstallationVariable, bool>)((InstallationVariable iv) => iv != InstallationVariable.Invalid));
	}
}
