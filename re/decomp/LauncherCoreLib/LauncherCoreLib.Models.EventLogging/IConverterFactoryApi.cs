namespace LauncherCoreLib.Models.EventLogging;

public interface IConverterFactoryApi
{
	TrackedEventConverters.TrackedEventConverter Convert { get; }
}
