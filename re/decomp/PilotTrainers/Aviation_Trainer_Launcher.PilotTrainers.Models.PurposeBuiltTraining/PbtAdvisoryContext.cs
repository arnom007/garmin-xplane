using Domain.PurposeBuiltTraining;
using LauncherCoreLib.Models.Simulated.Airport;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public record PbtAdvisoryContext(TrainingItemKey Key, PbtAircraftState? AircraftState, ReactiveAircraft? LocalAircraft);
