namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public abstract class GenericTrackedEventDataDescriptor : TrackedEventDataDescriptor
{
	protected abstract PropertiesBuilder BuildProperties { get; }

	public abstract TrackedEventContainer Instance();
}
