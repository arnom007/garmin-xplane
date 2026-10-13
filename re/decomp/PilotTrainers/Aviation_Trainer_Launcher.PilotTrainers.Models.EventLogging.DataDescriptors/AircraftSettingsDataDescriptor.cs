using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using LanguageExt;
using LauncherCoreLib.Models.EventLogging;
using LauncherCoreLib.Models.EventLogging.BackgroundLoggers;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.EventLogging.DataDescriptors;

public sealed class AircraftSettingsDataDescriptor : GenericTrackedEventDataDescriptor
{
	private readonly IActiveSetting _dbSettings;

	private readonly IReadOnlyList<SimProcessSession> _simulators;

	private readonly AircraftSnapshotData _aircraft;

	protected override PropertiesBuilder BuildProperties => () => (IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>)ImmutableList.ToImmutableList<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>(_simulators.Select((SimProcessSession sim) => new KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>(AppCenterPropertyName.ParseTruncateAndHandleErrors($"Slot_{sim.Simulator.InstallData.InstallationKey.Slot}"), AppCenterPropertyValue.ParseTruncateAndHandleErrors($"{sim.Simulator.InstallData.InstallationKey.ReleaseId}__{sim.Simulator.InstallData.InstallationKey.ModelId}")))).AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Simulator_Count"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(_simulators.Count.ToString(CultureInfo.CurrentCulture))).AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Airframe"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(_aircraft.ListData.AirframeCCT.ToString()))
		.AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Database"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(_dbSettings.CurrentValue.Split('.', StringSplitOptions.None).Last()));

	protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("Aircraft_Settings"), EventTargetApps.Launcher);

	public AircraftSettingsDataDescriptor(IActiveSetting dbSettings, IReadOnlyList<SimProcessSession> simulators, AircraftSnapshotData aircraft)
	{
		_dbSettings = dbSettings;
		_simulators = simulators;
		_aircraft = aircraft;
	}

	public static SimSettingsInjector AircraftSettingsInjector()
	{
		return (IActiveSetting? dbSettings, ITrainingSession? trainingSession, AircraftSnapshotData? aircraft) => (dbSettings != null && trainingSession != null && aircraft != null) ? ((Option<TrackedEventDataDescriptor>)new AircraftSettingsDataDescriptor(dbSettings, trainingSession.Simulators, aircraft)) : Option<TrackedEventDataDescriptor>.None;
	}

	public override TrackedEventContainer Instance()
	{
		return EventPrototype.ToInstance().With(BuildProperties, null, EventUserDataCensor.None);
	}
}
