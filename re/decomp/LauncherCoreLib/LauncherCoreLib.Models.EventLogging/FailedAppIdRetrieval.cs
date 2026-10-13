namespace LauncherCoreLib.Models.EventLogging;

public class FailedAppIdRetrieval : TrackedEventError
{
	public virtual string ToString()
	{
		return "Failed to retrieve app center app ID for tracked event";
	}
}
