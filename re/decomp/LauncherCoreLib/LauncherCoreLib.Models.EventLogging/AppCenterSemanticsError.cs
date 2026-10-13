namespace LauncherCoreLib.Models.EventLogging;

public abstract class AppCenterSemanticsError
{
	public abstract string Value { get; }

	public abstract string ErrorMessage { get; }
}
