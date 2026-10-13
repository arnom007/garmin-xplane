using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging;

public class ConverterFactoryApi : IConverterFactoryApi
{
	[field: CompilerGenerated]
	public TrackedEventConverters.TrackedEventConverter Convert
	{
		[CompilerGenerated]
		get;
	}

	public ConverterFactoryApi(TrackedEventConverters.TrackedEventConverter convert)
	{
		Convert = convert;
	}
}
