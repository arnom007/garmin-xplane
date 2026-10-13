using Domain.PurposeBuiltTraining;
using LOLA.Redux;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public static class PbtReduxActions
{
	public record SelectTrainingItemAction(TrainingItemKey Key) : IAction;

	public record UpdateAircraftStateAction(PbtAircraftState UpdatedState) : IAction;

	public record UpdateRunningAircraftAction(PbtRunningAircraftInfo? Running) : IAction;
}
