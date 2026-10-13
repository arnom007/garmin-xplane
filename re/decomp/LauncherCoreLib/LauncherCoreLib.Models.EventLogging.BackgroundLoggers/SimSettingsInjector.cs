using LanguageExt;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherCoreLib.Models.EventLogging.BackgroundLoggers;

public delegate Option<TrackedEventDataDescriptor> SimSettingsInjector(IActiveSetting? dbSettings, ITrainingSession? session, AircraftSnapshotData? aircraft);
