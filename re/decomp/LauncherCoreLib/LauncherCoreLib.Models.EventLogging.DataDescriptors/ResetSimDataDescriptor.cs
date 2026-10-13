using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public sealed class ResetSimDataDescriptor : GenericTrackedEventDataDescriptor
{
	private readonly ReactiveSimulator _simulator;

	protected override PropertiesBuilder BuildProperties => [CompilerGenerated] () => (System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>)ImmutableList.Create<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>().AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Release_And_Model"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(Enumerable.Skip<string>((System.Collections.Generic.IEnumerable<string>)_simulator.AircraftAndSim.InstallationKey.SerializedKey.Split('-', (StringSplitOptions)0), 1).Reduce((string s1, string s2) => s1 + "-" + s2)));

	protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("Reset_Sim"), EventTargetApps.Launcher);

	public ResetSimDataDescriptor(ReactiveSimulator simulator)
	{
		_simulator = simulator;
	}

	public override TrackedEventContainer Instance()
	{
		return EventPrototype.ToInstance().With(BuildProperties, null, EventUserDataCensor.None);
	}
}
