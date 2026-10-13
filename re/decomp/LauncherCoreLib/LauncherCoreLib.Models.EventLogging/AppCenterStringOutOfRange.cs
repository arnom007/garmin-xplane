using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging;

public class AppCenterStringOutOfRange : AppCenterSemanticsError
{
	[field: CompilerGenerated]
	public override string Value
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public int MaxChars
	{
		[CompilerGenerated]
		get;
	}

	public override string ErrorMessage => $"{Value} event name string is too long. Must not be longer than {MaxChars} characters";

	public AppCenterStringOutOfRange(string value, int maxChars)
	{
		Value = value;
		MaxChars = maxChars;
	}
}
