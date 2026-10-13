using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public class ResourceOpenedDataDescriptor : GenericTrackedEventDataDescriptor
{
	private readonly string _trainerAndResourceName;

	private readonly EventStatus _status;

	protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("Resource_Opened"), EventTargetApps.Launcher);

	protected override PropertiesBuilder BuildProperties => [CompilerGenerated] () => (System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>)ImmutableList.Create<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>().AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Trainer_And_Resource_Name"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(_trainerAndResourceName ?? "")).AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Status"), AppCenterPropertyValue.ParseTruncateAndHandleErrors($"{_status}"));

	public ResourceOpenedDataDescriptor(string trainerAndResourceName, EventStatus status)
	{
		_trainerAndResourceName = trainerAndResourceName;
		_status = status;
	}

	public override TrackedEventContainer Instance()
	{
		return EventPrototype.ToInstance().With(BuildProperties, null, EventUserDataCensor.None);
	}
}
