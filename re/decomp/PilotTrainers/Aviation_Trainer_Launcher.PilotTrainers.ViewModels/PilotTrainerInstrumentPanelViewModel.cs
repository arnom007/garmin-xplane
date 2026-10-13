using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Aviation_Trainer_Launcher.LauncherUILib.Navigation;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using Aviation_Trainer_Launcher.PilotTrainers.Models;
using LOLA.Logging;
using LOLA.Reactive;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using Prism.Commands;
using TrainerPackageLib;

namespace Aviation_Trainer_Launcher.PilotTrainers.ViewModels;

public class PilotTrainerInstrumentPanelViewModel : InstrumentPanelViewModel
{
	private readonly IUnitOfWork _unitOfWork;

	private readonly IWizardDialogService _wizards;

	private readonly StartDownloadIfNetworkAvailableBehavior _startifOnlineBehavior;

	private readonly DelegateCommand<SimPlaceholderViewModel> _baseAddLruCommand;

	public PilotTrainerInstrumentPanelViewModel(INavigatePanels navigator, ISquawkBoard squawks, IWizardDialogService wizardService, IUnitOfWork unitOfWork, StartDownloadIfNetworkAvailableBehavior startifOnlineBehavior)
		: base(navigator, squawks, wizardService)
	{
		_unitOfWork = unitOfWork;
		_wizards = wizardService;
		_startifOnlineBehavior = startifOnlineBehavior;
		_baseAddLruCommand = base.AddLRUCommand;
		base.AddLRUCommand = new DelegateCommand<SimPlaceholderViewModel>(AddPilotLru, (SimPlaceholderViewModel _) => base.CanChangeSlots).ObservesProperty(() => CanChangeSlots);
		base.SlotMainCommand = base.AddLRUCommand;
	}

	private async void AddPilotLru(SimPlaceholderViewModel placeholder)
	{
		SimPlaceholderViewModel placeholder2 = placeholder;
		ReactiveAircraft ba = base.BoundAircraft;
		if (ba == null)
		{
			Logger.Log(LogLevel.Warning, "Cannot add an LRU because this panel is not bound to an aircraft.");
			return;
		}
		if (PlaceholderIsInEmptyRow(placeholder2))
		{
			SelectionStepperViewModel selectionStepperViewModel = CreateAddLruWizard();
			selectionStepperViewModel.ShowSteps = true;
			FamilySelectionStepViewModel familySelectionStepViewModel = new FamilySelectionStepViewModel
			{
				StepName = "Product",
				StepDescription = "Select the product family of the desired Trainer."
			};
			selectionStepperViewModel.Steps.Add(familySelectionStepViewModel);
			VersionSelectionStepViewModel versionSelectionStepViewModel = new VersionSelectionStepViewModel
			{
				StepName = "Version",
				StepDescription = "Select the airborne software version of the desired Trainer."
			};
			selectionStepperViewModel.Steps.Add(versionSelectionStepViewModel);
			ModelSelectionWizardStepViewModel modelStep = InstrumentPanelViewModel.CreateModelSelectionStep(placeholder2);
			try
			{
				modelStep.StepName = "Model";
				selectionStepperViewModel.Steps.Add(modelStep);
				AvionicsModel selectedModel = null;
				Task<AirframeCategoryClassType> categoryTask = (from ld in ba.ListData.GetCachedThenUpdatedValues().WhereClassNotNull()
					select ld.AirframeCCT).FirstAsync().ToTask();
				IObservable<ITrainerPackage> packages = _unitOfWork.Packages.Packages().WhereAsync(async delegate(ITrainerPackage p)
				{
					AirframeCategoryClassType c = await categoryTask.ConfigureAwait(continueOnCapturedContext: false);
					return p.Models.Any((ITrainerModelInfo m) => m.Category == placeholder2.Category && m.SupportedAirframes.Contains(c));
				});
				using (familySelectionStepViewModel.ListFamilies(packages).Merge(new FamilyAndVersionSelectorStepAdapter(familySelectionStepViewModel, versionSelectionStepViewModel).ListVersions(packages, (FamilySelectorViewModel? newFamily) => (newFamily != null && newFamily.FamilyName != null) ? ("Select the " + newFamily.FamilyName + " airborne software version to install.") : "Select the airborne software version to install.")).Merge((from version in versionSelectionStepViewModel.WhenSelectedVersionChanges().Do(delegate(PackageVersionViewModel version)
					{
						if (version != null)
						{
							modelStep.StepDescription = string.Format(CultureInfo.InvariantCulture, "Select a {0} model to install into the {1} slot.", version.Package.BaseInfo.FormatFullSoftwareString(), placeholder2.Slot.ToUiString());
						}
					})
					select modelStep.SelectModel(base.BoundAircraft, placeholder2.Category, (IReadOnlyCollection<ITrainerPackage>)(object)((version == null) ? Array.Empty<ITrainerPackage>() : new ITrainerPackage[1] { version.Package }))).Switch().Do(delegate(AvionicsModel model)
				{
					selectedModel = model;
				}).ToSideEffectStream())
					.Subscribe())
				{
					if (!(await _wizards.DisplayWizard(selectionStepperViewModel)))
					{
						return;
					}
					if ((object)selectedModel == null)
					{
						throw new InvalidOperationException("Internal wizard error: A model must be selected when it is completed successfully.");
					}
					ReactiveSimulator installedSim;
					if (!InstrumentPanelViewModel.IsSameModel(placeholder2, selectedModel.Key))
					{
						installedSim = await LoadingHarness(async delegate
						{
							Logger.Log(LogLevel.Info, $"[{ba.AircraftIdentifier}] Installing {selectedModel} into {placeholder2.Slot}.");
							ReactiveSimulator result = await ba.Install(selectedModel, placeholder2.Slot);
							Logger.Log(LogLevel.Info, $"[{ba.AircraftIdentifier}] Installed {selectedModel} into {placeholder2.Slot}.");
							return result;
						});
						ITrainerPackage trainerPackage = await Observable.Return(selectedModel.Package).Concat(GetInstalledPackage()).WhereClassNotNull()
							.FirstOrDefaultAsync((ITrainerPackage p) => p.IsACatalogPackage());
						if (trainerPackage != null)
						{
							_startifOnlineBehavior.StartInstallIfOnline(trainerPackage);
						}
					}
					IObservable<ITrainerPackage> GetInstalledPackage()
					{
						return Observable.DeferAsync(async delegate
						{
							if (installedSim != null)
							{
								SimInstallData cachedValue = installedSim.SimData.CachedValue;
								if (cachedValue != null)
								{
									TrainerPackageIdentifier trainerPackageIdentifier = cachedValue.FullPackageIdOrThrow();
									ITrainerPackage trainerPackage2 = await installedSim.UnitOfWork.PackageRepos.Packages.GetPackageById(trainerPackageIdentifier).ConfigureAwait(continueOnCapturedContext: false);
									if (trainerPackage2 != null)
									{
										return Observable.Return(trainerPackage2);
									}
								}
							}
							return Observable.Empty<ITrainerPackage>();
						});
					}
				}
				return;
			}
			finally
			{
				if (modelStep != null)
				{
					((IDisposable)modelStep).Dispose();
				}
			}
		}
		_baseAddLruCommand.Execute(placeholder2);
	}

	private bool PlaceholderIsInEmptyRow(SimPlaceholderViewModel obj)
	{
		SimPlaceholderViewModel obj2 = obj;
		if (obj2.InstanceId == 1)
		{
			return base.PlaceHoldersByCategory.First((AvionicsCategoryGroupViewModel<SimPlaceholderViewModel> row) => row.Avionics.Contains(obj2)).Avionics.Count == 1;
		}
		return false;
	}
}
