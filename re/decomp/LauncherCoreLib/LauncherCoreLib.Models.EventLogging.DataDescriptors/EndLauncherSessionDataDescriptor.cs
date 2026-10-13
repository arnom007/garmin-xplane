using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public sealed class EndLauncherSessionDataDescriptor : TimeRangeTrackedEventDataDescriptor
{
	private readonly int _powerCycles;

	private readonly DateTimeOffset _startSessionOffset;

	protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("End_Session"), EventTargetApps.Launcher);

	public EndLauncherSessionDataDescriptor(int powerCycles, DateTimeOffset startSessionOffset)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		_powerCycles = powerCycles;
		_startSessionOffset = startSessionOffset;
	}

	public override TrackedEventContainer Instance(ITimeRangeProvider timeRangeProvider)
	{
		return EventPrototype.ToInstance(BuildProperties(timeRangeProvider), EventUserDataCensor.None);
	}

	protected override PropertiesBuilder BuildProperties(ITimeRangeProvider timeRangeProvider)
	{
		ITimeRangeProvider timeRangeProvider2 = timeRangeProvider;
		return () => (System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>)ImmutableList.Create<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>().AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Power_Cycles"), AppCenterPropertyValue.ParseTruncateAndHandleErrors($"{_powerCycles}")).AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Session_Duration"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(timeRangeProvider2?.GetTimeRangeString(DateTimeOffset.Now - _startSessionOffset) ?? ""));
	}
}
