namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public abstract class TimeRangeTrackedEventDataDescriptor : TrackedEventDataDescriptor
{
	public abstract TrackedEventContainer Instance(ITimeRangeProvider timeRangeProvider);

	protected abstract PropertiesBuilder BuildProperties(ITimeRangeProvider timeRangeProvider);
}
