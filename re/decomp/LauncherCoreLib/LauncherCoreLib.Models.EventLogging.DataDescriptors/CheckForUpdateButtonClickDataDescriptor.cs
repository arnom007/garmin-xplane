using System.Collections.Generic;
using System.Linq;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public sealed class CheckForUpdateButtonClickDataDescriptor : GenericTrackedEventDataDescriptor
{
	protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("Check_For_Updates"), EventTargetApps.Launcher);

	protected override PropertiesBuilder BuildProperties => () => Enumerable.Empty<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>();

	public override TrackedEventContainer Instance()
	{
		return EventPrototype.ToInstance().With(BuildProperties);
	}
}
