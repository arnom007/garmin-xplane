using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace LauncherCoreLib.Models.DeployedEnv;

public class ConfidentialAlias
{
	[field: CompilerGenerated]
	public string Secret
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public Regex SecretRegex
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string Alias
	{
		[CompilerGenerated]
		get;
	}

	public ConfidentialAlias(string secret, string alias)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		Secret = secret;
		Alias = alias;
		SecretRegex = new Regex("\\b" + Regex.Escape(secret) + "\\b", (RegexOptions)8);
	}
}
