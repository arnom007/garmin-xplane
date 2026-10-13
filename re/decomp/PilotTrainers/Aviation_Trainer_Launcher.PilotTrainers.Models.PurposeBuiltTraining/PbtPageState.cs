using System.Runtime.CompilerServices;
using Domain.PurposeBuiltTraining;
using LanguageExt;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public record PbtPageState
{
	public static PbtPageState Empty { get; } = new PbtPageState(PbtAircraftListState.Empty, Prelude.None, Prelude.None);


	public PbtAircraftListState ListState { get; init; }

	public Option<TrainingItemKey> SelectedItem { get; init; }

	public Option<PbtRunningAircraftInfo> RunningAircraft { get; init; }

	public PbtPageState(PbtAircraftListState listState, Option<TrainingItemKey> selectedItem, Option<PbtRunningAircraftInfo> runningAircraft)
	{
		ListState = listState;
		SelectedItem = selectedItem;
		RunningAircraft = runningAircraft;
	}

	[CompilerGenerated]
	protected PbtPageState(PbtPageState original)
	{
		ListState = original.ListState;
		SelectedItem = original.SelectedItem;
		RunningAircraft = original.RunningAircraft;
	}
}
