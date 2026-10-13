using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public sealed class EndTrainerSessionDataDescriptor : TimeRangeTrackedEventDataDescriptor
{
	private readonly DateTimeOffset _startSessionOffset;

	private readonly DateTimeOffset _endSessionOffset;

	private readonly ITrainerPackage _trainerPackage;

	protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("Trainer_Session_Duration"), EventTargetApps.TrainerByPackage(_trainerPackage));

	public EndTrainerSessionDataDescriptor(DateTimeOffset startSessionOffset, DateTimeOffset endSessionOffset, ITrainerPackage trainerPackage)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		_startSessionOffset = startSessionOffset;
		_endSessionOffset = endSessionOffset;
		_trainerPackage = trainerPackage;
	}

	public override TrackedEventContainer Instance(ITimeRangeProvider timeRangeProvider)
	{
		return EventPrototype.ToInstance().With(BuildProperties(timeRangeProvider), null, EventUserDataCensor.None);
	}

	protected override PropertiesBuilder BuildProperties(ITimeRangeProvider timeRangeProvider)
	{
		ITimeRangeProvider timeRangeProvider2 = timeRangeProvider;
		return () => (System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>)ImmutableList.Create<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>().AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Session_Duration"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(timeRangeProvider2?.GetTimeRangeString(_endSessionOffset - _startSessionOffset) ?? ""));
	}
}
