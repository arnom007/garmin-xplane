namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public abstract class TrackedEventDataDescriptor
{
	protected abstract TrackedEventPrototype EventPrototype { get; }
}
