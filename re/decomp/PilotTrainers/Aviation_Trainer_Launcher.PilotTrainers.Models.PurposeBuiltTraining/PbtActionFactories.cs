using Domain.PurposeBuiltTraining;
using LOLA.Redux;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public record PbtActionFactories(ResetActionBuilder BuildReset)
{
	public delegate IAction ResetActionBuilder(TrainingItemKey key);
}
