using Domain.PurposeBuiltTraining;
using LanguageExt;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public readonly record struct PbtRunningAircraftInfo(PbtLocalAircraftIdentifier LocalId, Option<TrainingItemKey> TrainingItem);
