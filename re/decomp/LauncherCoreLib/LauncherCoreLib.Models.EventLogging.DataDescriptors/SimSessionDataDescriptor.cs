using System;
using System.Collections.Generic;
using System.Linq;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public sealed class SimSessionDataDescriptor : GenericTrackedEventDataDescriptor
{
	private readonly DateTimeOffset _startSessionTime;

	private readonly DateTimeOffset _endSessionTime;

	private readonly ITrainerPackage _trainerPackage;

	protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("Trainer_Session"), EventTargetApps.TrainerByPackage(_trainerPackage));

	protected override PropertiesBuilder BuildProperties => () => Enumerable.Empty<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>();

	public SimSessionDataDescriptor(ITrainerPackage trainerPackage, DateTimeOffset startSessionTime, DateTimeOffset endSessionTime)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		_trainerPackage = trainerPackage;
		_startSessionTime = startSessionTime;
		_endSessionTime = endSessionTime;
	}

	public override TrackedEventContainer Instance()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return EventPrototype.ToInstance().With(null, new TrainerSessionExtraTrackedEventData(_startSessionTime, _endSessionTime), EventUserDataCensor.None);
	}
}
