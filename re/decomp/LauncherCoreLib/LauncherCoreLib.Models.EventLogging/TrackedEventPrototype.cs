using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging;

public sealed class TrackedEventPrototype
{
	[field: CompilerGenerated]
	public AppCenterEventName EventName
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

	public TrackedEventPrototype(AppCenterEventName eventName, TrackedTargetAppBase targetApp)
	{
		EventName = eventName;
		TargetApp = targetApp;
	}

	public static TrackedEventPrototype Create(AppCenterEventName eventName, TrackedTargetAppBase targetApp)
	{
		return new TrackedEventPrototype(eventName, targetApp);
	}

	public TrackedEventContainer ToInstance()
	{
		return ToInstance((PropertiesBuilder)Enumerable.Empty<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>);
	}

	public TrackedEventContainer ToInstance(PropertiesBuilder properties)
	{
		return ToInstance(properties, EventUserDataCensor.All);
	}

	public TrackedEventContainer ToInstance(PropertiesBuilder properties, ShouldCensorUserDataInValueForProperty shouldCensorUserData)
	{
		return new TrackedEventContainer(EventName, properties, TargetApp, NoExtraTrackedEventData.Instance, shouldCensorUserData);
	}
}
