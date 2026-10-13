using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.TrainerPackages;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public sealed class TrainerInstallOrUninstallDataDescriptor : GenericTrackedEventDataDescriptor
{
	private readonly CachedCustomAppIdTargetApp _cachedAppId;

	private readonly TrainerPackageOperationState _status;

	private readonly string _eventName;

	protected override PropertiesBuilder BuildProperties => [CompilerGenerated] () => (System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>)ImmutableList.Create<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>().AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Status"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(((object)_status).ToString())).AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors(_eventName + "_Version"), AppCenterPropertyValue.ParseTruncateAndHandleErrors($"{_cachedAppId.AppVersion}"));

	[field: CompilerGenerated]
	protected override TrackedEventPrototype EventPrototype
	{
		[CompilerGenerated]
		get;
	}

	public TrainerInstallOrUninstallDataDescriptor(CachedCustomAppIdTargetApp cachedAppId, TrainerPackageOperationState status, string eventName)
	{
		_cachedAppId = cachedAppId;
		_status = status;
		_eventName = eventName;
		EventPrototype = TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors(_eventName), cachedAppId);
	}

	public override TrackedEventContainer Instance()
	{
		return EventPrototype.ToInstance().With(BuildProperties, null, EventUserDataCensor.None);
	}
}
