using System.Runtime.CompilerServices;
using AppCenter.EventLogging;
using LOLA.Logging;
using LanguageExt;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.EventLogging;

public static class TrackedEvent
{
	public delegate Option<AppCenterAppId> SelectAppIdBasedOnTrainer(ITrainerPackage trainerPackage);

	public delegate void LogTrackedEvent(TrackedEventDataDescriptor dataDescriptor);

	[field: CompilerGenerated]
	public static LogTrackedEvent Noop
	{
		[CompilerGenerated]
		get;
	} = delegate
	{
	};


	[field: CompilerGenerated]
	public static LogTrackedEvent Log
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = CreateDefaultLogger();


	public static LogTrackedEvent SetGlobalSingleton(this LogTrackedEvent sender)
	{
		return Log = sender;
	}

	private static LogTrackedEvent CreateDefaultLogger()
	{
		return delegate(TrackedEventDataDescriptor e)
		{
			Logger.Log(LogLevel.Error, $"Event Logged before setup: {e}");
		};
	}
}
