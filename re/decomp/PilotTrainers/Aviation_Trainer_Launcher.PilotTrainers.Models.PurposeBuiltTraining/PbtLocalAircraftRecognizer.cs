using LanguageExt;
using LauncherCoreLib.Models.Simulated.Airport;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public delegate Option<PbtDataStoredInLocalAircraft> PbtLocalAircraftRecognizer(AircraftSnapshotData snapshot);
