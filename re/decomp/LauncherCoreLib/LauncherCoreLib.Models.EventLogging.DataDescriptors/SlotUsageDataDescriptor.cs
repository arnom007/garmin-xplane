using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.Simulated.Panel;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public sealed class SlotUsageDataDescriptor : GenericTrackedEventDataDescriptor
{
	private readonly System.Collections.Generic.IReadOnlyList<SimProcessSession> _simulators;

	protected override PropertiesBuilder BuildProperties => [CompilerGenerated] () =>
	{
		AvionicsModel avionicsModel = Enumerable.FirstOrDefault<AvionicsModel>(Enumerable.Select<SimProcessSession, AvionicsModel>(Enumerable.Where<SimProcessSession>((System.Collections.Generic.IEnumerable<SimProcessSession>)_simulators, (Func<SimProcessSession, bool>)((SimProcessSession sim) => sim.Simulator.PackageInfo.AvionicsType == AvionicsType.Display)), (Func<SimProcessSession, AvionicsModel>)((SimProcessSession sim) => sim.Simulator.PackageInfo)));
		AvionicsModel avionicsModel2 = Enumerable.FirstOrDefault<AvionicsModel>(Enumerable.Select<SimProcessSession, AvionicsModel>(Enumerable.Where<SimProcessSession>((System.Collections.Generic.IEnumerable<SimProcessSession>)_simulators, (Func<SimProcessSession, bool>)((SimProcessSession sim) => sim.Simulator.PackageInfo.AvionicsType == AvionicsType.Navigator)), (Func<SimProcessSession, AvionicsModel>)((SimProcessSession sim) => sim.Simulator.PackageInfo)));
		int maxSimCount;
		string simFamilyId;
		if ((object)avionicsModel != null)
		{
			int maxInstances = avionicsModel.MaxInstances;
			string text = ((object)avionicsModel.FamilyId).ToString();
			maxSimCount = maxInstances;
			simFamilyId = text;
		}
		else
		{
			maxSimCount = 0;
			simFamilyId = "Row";
		}
		int maxSimCount2;
		string simFamilyId2;
		if ((object)avionicsModel2 != null)
		{
			int maxInstances = avionicsModel2.MaxInstances;
			string text = ((object)avionicsModel2.FamilyId).ToString();
			maxSimCount2 = maxInstances;
			simFamilyId2 = text;
		}
		else
		{
			maxSimCount2 = 0;
			simFamilyId2 = "Row";
		}
		return (System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>)((IImmutableDictionary<AppCenterPropertyName, AppCenterPropertyValue>)(object)ImmutableDictionary.ToImmutableDictionary<SimInstallKey, AppCenterPropertyName, AppCenterPropertyValue>(Enumerable.Select<SimProcessSession, SimInstallKey>((System.Collections.Generic.IEnumerable<SimProcessSession>)_simulators, (Func<SimProcessSession, SimInstallKey>)((SimProcessSession sim) => sim.Simulator.InstallData.InstallationKey)), (Func<SimInstallKey, AppCenterPropertyName>)((SimInstallKey k) => AppCenterPropertyName.ParseTruncateAndHandleErrors($"{"Slot_"}{k.Slot}")), (Func<SimInstallKey, AppCenterPropertyValue>)((SimInstallKey k) => AppCenterPropertyValue.ParseTruncateAndHandleErrors($"{k.ReleaseId.FamilyId}__{k.ModelId}")))).FillEmptySlots("Slot_Display_", simFamilyId, maxSimCount).FillEmptySlots("Slot_Navigator_", simFamilyId2, maxSimCount2);
	};

	protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("Slot_Usage"), EventTargetApps.Launcher);

	public SlotUsageDataDescriptor(System.Collections.Generic.IReadOnlyList<SimProcessSession> simulators)
	{
		_simulators = simulators;
	}

	public override TrackedEventContainer Instance()
	{
		return EventPrototype.ToInstance().With(BuildProperties, null, EventUserDataCensor.None);
	}
}
