using System;
using System.Reactive.Linq;
using Aviation_Trainer_Launcher.LauncherAdvisories;
using Domain.PurposeBuiltTraining;
using LanguageExt;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public static class PbtAdvisories
{
	public static IObservable<IAdvisoryInstance> DetectedAdvisoriesOverTime(IInternalMonitorUtilities utilities, PbtAdvisoryContext context)
	{
		DateTimeOffset currentTime = utilities.CurrentTime;
		BasicAdvisoryInstance basicAdvisoryInstance = new BasicAdvisoryInstance(new AdvisoryInstanceId("PBT_UNKNOWN_TRAINING_ITEM", currentTime), ((object)context.AircraftState != null) ? SelfCheckState.WithinParameters : SelfCheckState.OutOfRange, new BasicAdvisoryContent
		{
			CategoryInfo = new EnumAdvisoryCategory(AdvisoryCategory.Compatability),
			Severity = AdvisorySeverity.Error,
			DisplayText = "Internal Error. This training item has no associated data. Report this to Garmin support."
		});
		PbtAircraftError? error = Prelude.Optional(context.AircraftState).Bind((PbtAircraftState a) => a.LocalRepresentation.ErrorInfo).ToNullable();
		PbtLocalRepresentationErrorAdvisory pbtLocalRepresentationErrorAdvisory = new PbtLocalRepresentationErrorAdvisory(context, error, utilities.CurrentTime);
		return new IAdvisoryInstance[2] { basicAdvisoryInstance, pbtLocalRepresentationErrorAdvisory }.ToObservable();
	}
}
