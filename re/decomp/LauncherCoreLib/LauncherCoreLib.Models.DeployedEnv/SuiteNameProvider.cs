using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace LauncherCoreLib.Models.DeployedEnv;

public class SuiteNameProvider : ISuiteNameProvider
{
	[field: CompilerGenerated]
	public string SuiteName
	{
		[CompilerGenerated]
		get;
	}

	public SuiteNameProvider(string suiteName)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (suiteName == null || !Regex.IsMatch(suiteName, "^\\w{3,}$"))
		{
			throw new FormatException("The suite name is invalid.");
		}
		SuiteName = suiteName;
	}
}
