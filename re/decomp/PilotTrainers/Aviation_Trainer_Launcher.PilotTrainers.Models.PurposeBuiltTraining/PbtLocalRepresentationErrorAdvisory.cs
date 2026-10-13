using System;
using Aviation_Trainer_Launcher.LauncherAdvisories;
using Domain.PurposeBuiltTraining;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public class PbtLocalRepresentationErrorAdvisory : ContentFreeAdvisoryInstance
{
	public PbtAdvisoryContext Context { get; }

	public PbtAircraftError? LocalError { get; }

	public PbtLocalRepresentationErrorAdvisory(PbtAdvisoryContext context, PbtAircraftError? error, DateTimeOffset createdAt)
		: base(new AdvisoryInstanceId("PBT_LOCAL_REP_ERROR", createdAt), (!error.HasValue) ? SelfCheckState.WithinParameters : SelfCheckState.OutOfRange, AdvisorySeverity.Error)
	{
		Context = context;
		LocalError = error;
	}
}
