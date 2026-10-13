using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging;

public class AppCenterStringFailedRegex : AppCenterSemanticsError
{
	[field: CompilerGenerated]
	public override string Value
	{
		[CompilerGenerated]
		get;
	}

	public override string ErrorMessage => $"{Value} event name string was not formatted properly. Refer to this regex {ValidatorUtil.AppCenterIdentifierRegex}";

	public AppCenterStringFailedRegex(string value)
	{
		Value = value;
	}
}
