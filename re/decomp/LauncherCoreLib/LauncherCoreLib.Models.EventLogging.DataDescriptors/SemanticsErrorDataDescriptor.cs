using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public sealed class SemanticsErrorDataDescriptor : GenericTrackedEventDataDescriptor
{
	private readonly string _eventName;

	private readonly string _errorMessage;

	private readonly AppCenterPropertyName _errorMessagePropName = AppCenterPropertyName.ParseAndThrowOnErrors("Error_Message");

	protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("Semantics_Error"), EventTargetApps.Launcher);

	protected override PropertiesBuilder BuildProperties => [CompilerGenerated] () => (System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>)ImmutableList.Create<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>().AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Event_Name"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(_eventName)).AddNewKvp(_errorMessagePropName, AppCenterPropertyValue.ParseTruncateAndHandleErrors(_errorMessage));

	public SemanticsErrorDataDescriptor(string eventName, string errorMessage)
	{
		_eventName = eventName;
		_errorMessage = errorMessage;
	}

	public override TrackedEventContainer Instance()
	{
		return EventPrototype.ToInstance().With(BuildProperties, null, EventUserDataCensor.CensorProperties(_errorMessagePropName));
	}
}
