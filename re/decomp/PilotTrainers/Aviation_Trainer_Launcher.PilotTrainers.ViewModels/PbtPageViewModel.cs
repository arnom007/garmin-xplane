using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Aviation_Trainer_Launcher.LauncherUILib.Navigation;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;
using Domain.PurposeBuiltTraining;
using Domain.UtilTypes;
using LOLA.Logging;
using LOLA.Reactive;
using LanguageExt;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.EventLogging;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;
using LauncherCoreLib.Models.Simulated.Airport;
using Prism.Commands;
using Prism.Regions;
using TrainerUILib.ViewModels;

namespace Aviation_Trainer_Launcher.PilotTrainers.ViewModels;

public class PbtPageViewModel : FullPageViewModel
{
	private readonly IUnitOfWork _unitOfWork;

	private readonly PbtAircraftApi _pbtApi;

	private readonly PbtPowerButtonViewModel _mainActionVm;

	private readonly IWizardDialogService _wizards;

	private PbtSelectionListViewModel? _listViewModel;

	private bool _selectedTrainingItemIsPowered;

	private string _panelTitle = string.Empty;

	private DelegateCommand _resetTrainingCommand = CommandUtil.Disabled;

	private bool _showErrorImage;

	public PbtSelectionListViewModel? ListViewModel
	{
		get
		{
			return _listViewModel;
		}
		set
		{
			SetProperty(ref _listViewModel, value, "ListViewModel");
		}
	}

	public AdvisoryListExpanderViewModel<PbtAdvisoryContext> Advisories { get; }

	public PbtPanelViewModel Panel { get; }

	public string PanelTitle
	{
		get
		{
			return _panelTitle;
		}
		set
		{
			SetProperty(ref _panelTitle, value, "PanelTitle");
		}
	}

	public bool ShowErrorImage
	{
		get
		{
			return _showErrorImage;
		}
		set
		{
			SetProperty(ref _showErrorImage, value, "ShowErrorImage");
		}
	}

	public DelegateCommand ResetItemCommand
	{
		get
		{
			return _resetTrainingCommand;
		}
		set
		{
			SetProperty(ref _resetTrainingCommand, value, "ResetItemCommand");
		}
	}

	public bool SelectedTrainingItemIsPowered
	{
		get
		{
			return _selectedTrainingItemIsPowered;
		}
		set
		{
			SetProperty(ref _selectedTrainingItemIsPowered, value, "SelectedTrainingItemIsPowered");
		}
	}

	public AircraftInstallsViewModel AircraftInstalls { get; }

	public PbtPageViewModel(PbtPanelViewModel panel, IUnitOfWork unitOfWork, PbtAircraftApi pbtApi, AdvisoryListExpanderViewModel<PbtAdvisoryContext> advisories, PbtPowerButtonViewModel mainActionVm, IWizardDialogService wizards, AircraftInstallsViewModel aircraftInstalls)
	{
		Panel = panel;
		_unitOfWork = unitOfWork;
		_pbtApi = pbtApi;
		Advisories = advisories;
		_mainActionVm = mainActionVm;
		_wizards = wizards;
		AircraftInstalls = aircraftInstalls;
	}

	protected override IObservable<System.Reactive.Unit> CreatePageEventStream(NavigationContext navigationContext, CommonPageInterfaces commonPageInterfaces)
	{
		CommonPageInterfaces commonPageInterfaces2 = commonPageInterfaces;
		return Observable.Create(delegate(IObserver<System.Reactive.Unit> o)
		{
			IObservable<PbtPageState> source = _pbtApi.Store.StateOverTime.Replay(1).RefCount();
			PbtSelectionListViewModel pbtSelectionListViewModel = new PbtSelectionListViewModel(selectedKeysOverTime: source.Select((PbtPageState s) => s.SelectedItem.ToNullable()), items: PbtSelectionListViewModel.ConvertToChangeSet(source.Select((PbtPageState s) => s.ListState.Menu).DistinctUntilChanged()), onSelected: delegate(TrainingItemKey? k)
			{
				Prelude.Optional(k).IfSome(delegate(TrainingItemKey k)
				{
					_pbtApi.Store.Dispatch(new PbtReduxActions.SelectTrainingItemAction(k));
				});
			});
			ListViewModel = pbtSelectionListViewModel;
			IDisposable disposable = source.Select(delegate(PbtPageState t)
			{
				TrainingItemKey? trainingItemKey2 = t.SelectedItem.ToNullable();
				PbtAircraftListState listState = t.ListState;
				TrainingItemKey? selected = trainingItemKey2;
				PbtAircraftListState state = listState;
				Option<TrainingItemListInfo> item = state.Menu.ListItems.Find(delegate(TrainingItemListInfo i)
				{
					TrainingItemKey key = i.Key;
					TrainingItemKey? trainingItemKey4 = selected;
					return key == trainingItemKey4;
				});
				Option<PbtAircraftState> item2 = item.Bind((TrainingItemListInfo i) => state.AircraftState.Lookup.Find(i.Key));
				bool item3 = t.RunningAircraft.Bind((PbtRunningAircraftInfo r) => r.TrainingItem).Map(delegate(TrainingItemKey i)
				{
					TrainingItemKey? trainingItemKey3 = selected;
					return i == trainingItemKey3;
				}).IfNone(noneValue: false);
				return (selected: selected, listInfoOrNone: item, aircraftStateOrNone: item2, isPoweredOn: item3);
			}).DistinctUntilChanged().ObserveOn(commonPageInterfaces2.Schedulers.UIThread)
				.Do(delegate((TrainingItemKey? selected, Option<TrainingItemListInfo> listInfoOrNone, Option<PbtAircraftState> aircraftStateOrNone, bool isPoweredOn) t)
				{
					PanelTitle = t.listInfoOrNone.Map((TrainingItemListInfo i) => i.Title.RawValue).IfNone(string.Empty);
				})
				.Do(delegate((TrainingItemKey? selected, Option<TrainingItemListInfo> listInfoOrNone, Option<PbtAircraftState> aircraftStateOrNone, bool isPoweredOn) t)
				{
					ResetItemCommand = (t.isPoweredOn ? CommandUtil.Disabled : CreateResetCommand(t.listInfoOrNone, t.aircraftStateOrNone.Map((PbtAircraftState s) => s.AircraftDescriptor)));
				})
				.Do(delegate((TrainingItemKey? selected, Option<TrainingItemListInfo> listInfoOrNone, Option<PbtAircraftState> aircraftStateOrNone, bool isPoweredOn) t)
				{
					SelectedTrainingItemIsPowered = t.isPoweredOn;
				})
				.Select(delegate((TrainingItemKey? selected, Option<TrainingItemListInfo> listInfoOrNone, Option<PbtAircraftState> aircraftStateOrNone, bool isPoweredOn) t)
				{
					ReactiveAircraft localAircraft = t.aircraftStateOrNone.Bind((PbtAircraftState s) => s.LocalRepresentation.AircraftIdentifierOrNone).Map((PbtLocalAircraftIdentifier ai) => new ReactiveAircraft(_unitOfWork, ai.ToStorageAircraftId())).ToNullableClass();
					var (trainingItemKey, _, _, _) = t;
					return (!trainingItemKey.HasValue) ? null : new PbtAdvisoryContext(t.selected.Value, t.aircraftStateOrNone.ToNullableClass(), localAircraft);
				})
				.Replay(1)
				.RefCount()
				.Publish((IObservable<PbtAdvisoryContext> s) => s.Select((PbtAdvisoryContext c) => Panel.SetupPanel(c?.LocalAircraft).TakeUntil(c?.LocalAircraft?.WhenAircraftIsDeleted() ?? Observable.Return(TimingEvent.Default)).ToSideEffectStream()
					.OnSubscribe(delegate
					{
						ShowErrorImage = false;
					})
					.Merge(_mainActionVm.DriveUi(commonPageInterfaces2, c?.LocalAircraft))
					.Merge(AircraftInstalls.DisplayAircraftInstalls(c?.LocalAircraft))
					.Merge(AircraftInstalls.EnableErrorNavigation(commonPageInterfaces2))
					.CatchAndComplete(delegate(Exception e)
					{
						TrackedEvent.Log(InlineEventDataDescriptor.Create(TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("PBT_AIRCRAFT_PANEL_ERROR"), LauncherAppTarget.Instance), EventProperties.StaticList((AppCenterPropertyName.ParseAndThrowOnErrors("EXCEPTION_TYPE"), AppCenterPropertyValue.ParseTruncateFromBeginningAndHandleErrors(e.GetType()?.FullName ?? "No Name")), (AppCenterPropertyName.ParseAndThrowOnErrors("EXCEPTION_MESSAGE"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(e.Message))), EventUserDataCensor.CensorProperties(AppCenterPropertyName.ParseAndThrowOnErrors("EXCEPTION_MESSAGE"))));
						ShowErrorImage = true;
						Logger.LogFormattedException(e, "[PBT] Aircraft Panel");
					})
					.Merge(((object)c != null) ? Advisories.CreateMountSubscription(c) : Observable.Empty<System.Reactive.Unit>())).Switch())
				.ToSideEffectStream()
				.SubscribeSafe(o);
			return new CompositeDisposable(pbtSelectionListViewModel, disposable);
		});
		DelegateCommand CreateResetCommand(Option<TrainingItemListInfo> listInfo, Option<PbtAircraftDescriptor> aircraft)
		{
			return (first: listInfo, second: aircraft).Map2((TrainingItemListInfo l, PbtAircraftDescriptor a) => new DelegateCommand(async delegate
			{
				if ((bool)(await _wizards.DisplayModalConfirmation(new YesNoConfirmation
				{
					Title = "Reset Training Item",
					Prompt = $"Would you like to reset the {l.Title} back to its default state?",
					AcceptMessage = "_Reset it",
					DeclineMessage = "_Keep it",
					ShowAcceptAsRed = true
				})))
				{
					_pbtApi.Store.Dispatch(_pbtApi.ActionFactories.BuildReset(l.Key));
				}
			})).IfNone(CommandUtil.Disabled);
		}
	}
}
