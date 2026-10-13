using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public sealed class ScreenshotTakenDataDescriptor : GenericTrackedEventDataDescriptor
{
	private readonly EventStatus _status;

	protected override PropertiesBuilder BuildProperties => [CompilerGenerated] () => (System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>)ImmutableList.Create<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>().AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Screenshot_Status"), AppCenterPropertyValue.ParseTruncateAndHandleErrors($"{_status}"));

	protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("Screenshot_Taken"), EventTargetApps.Launcher);

	public ScreenshotTakenDataDescriptor(EventStatus status)
	{
		_status = status;
	}

	public override TrackedEventContainer Instance()
	{
		return EventPrototype.ToInstance().With(BuildProperties, null, EventUserDataCensor.None);
	}
}
