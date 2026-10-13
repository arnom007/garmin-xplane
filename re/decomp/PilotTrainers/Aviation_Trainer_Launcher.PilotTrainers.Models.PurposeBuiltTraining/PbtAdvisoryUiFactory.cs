using Aviation_Trainer_Launcher.LauncherAdvisories;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using Domain.PurposeBuiltTraining;
using LanguageExt;
using LauncherCoreLib.Models.AbstractUi;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public class PbtAdvisoryUiFactory : IAdvisoryUiContentFactory
{
	private readonly PbtAircraftApi _pbtApi;

	public PbtAdvisoryUiFactory(PbtAircraftApi api)
	{
		_pbtApi = api;
	}

	public object? CreateUiContent(IAdvisoryInstance instance, IAdvisoryUserActionLog actionLog)
	{
		PbtLocalRepresentationErrorAdvisory localError = instance as PbtLocalRepresentationErrorAdvisory;
		if (localError == null)
		{
			return null;
		}
		BasicAdvisoryContent content = localError.LocalError.ToOption().Map((PbtAircraftError error) => new BasicAdvisoryContent
		{
			CategoryInfo = new EnumAdvisoryCategory(AdvisoryCategory.GeneralInfo),
			Severity = AdvisorySeverity.Error,
			DisplayText = error.State.Match((PbtAircraftError.MissingPackage missingPackage) => $"A required trainer package was not found ({missingPackage.PackagereRef}). Reinstall the launcher and if this persists contact Garmin Support.", (PbtAircraftError.MissingPackageElementReference missingReference) => $"Unresolved package reference: {missingReference.Reference.ElementReference.RawString} in package {missingReference.PackageChecked}", (PbtAircraftError.SettingConstraintViolation constraintViolation) => $"The {constraintViolation.SettingReference.SettingId} setting in the {constraintViolation.SimulatorName}sim references a value({constraintViolation.SettingReference.ValueId}) that is not within constraints.", (PbtAircraftError.BrokenLocalAircraft brokenAircraft) => "This training item is out of sync with its specification. Reset it to restore operation.")
		}).IfNone(() => new BasicAdvisoryContent());
		BasicAdvisoryContentViewModel vm = new BasicAdvisoryContentViewModel(content);
		vm = localError.LocalError.ToOption().Map((PbtAircraftError error) => error.State.Match<BasicAdvisoryContentViewModel>((PbtAircraftError.MissingPackage missingPackage) => vm, (PbtAircraftError.MissingPackageElementReference missingReference) => vm, (PbtAircraftError.SettingConstraintViolation constraintViolation) => vm, (PbtAircraftError.BrokenLocalAircraft brokenAircraft) => vm.AddActions(new LauncherButtonViewModel[1]
		{
			new LauncherButtonViewModel
			{
				ButtonAction = new UserAction("Reset", "Reset this aircraft to its initial state.", delegate
				{
					_pbtApi.Store.Dispatch(_pbtApi.ActionFactories.BuildReset(localError.Context.Key));
				}, ActionStyling.Red)
			}
		}))).IfNone(vm);
		return vm;
	}
}
