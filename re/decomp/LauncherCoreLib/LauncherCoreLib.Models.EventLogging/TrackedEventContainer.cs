using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging;

public class TrackedEventContainer
{
	[field: CompilerGenerated]
	public AppCenterEventName EventName
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public PropertiesBuilder PropertiesBuilder
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public TrackedTargetAppBase TargetApp
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ExtraTrackedEventData Extras
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ShouldCensorUserDataInValueForProperty ShouldCensorUserData
	{
		[CompilerGenerated]
		get;
	}

	public TrackedEventContainer(AppCenterEventName eventName, PropertiesBuilder propertiesBuilder, TrackedTargetAppBase targetApp, ExtraTrackedEventData? extras = null, ShouldCensorUserDataInValueForProperty? shouldCensorUserData = null)
	{
		EventName = eventName;
		PropertiesBuilder = propertiesBuilder;
		TargetApp = targetApp;
		Extras = extras ?? NoExtraTrackedEventData.Instance;
		ShouldCensorUserData = shouldCensorUserData ?? EventUserDataCensor.All;
	}

	public TrackedEventContainer With(PropertiesBuilder? properties = null, ExtraTrackedEventData? extras = null, ShouldCensorUserDataInValueForProperty? shouldCensorUserData = null)
	{
		return new TrackedEventContainer(EventName, properties ?? PropertiesBuilder, TargetApp, extras ?? Extras, shouldCensorUserData ?? ShouldCensorUserData);
	}
}
