using System;
using System.Reactive;
using System.Reactive.Linq;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using LOLA.UI;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;

namespace Aviation_Trainer_Launcher.PilotTrainers.ViewModels;

internal class PilotTrainerHangarViewModel : HangarViewModel
{
	private readonly IStartingAircraftFactory _startingAircraft;

	public PilotTrainerHangarViewModel(IReactiveHangar hangar, ISquawkBoard squawks, IWizardDialogService wizards, IRunSimulators simRunner, IAircraftUIController aircraftUIController, IFileDialogs fileDialogs, IAircraftSnapshotDialogPresenter snapshotDialogPresenter, IStartingAircraftFactory startingAircraft)
		: base(hangar, squawks, wizards, simRunner, aircraftUIController, fileDialogs, snapshotDialogPresenter)
	{
		_startingAircraft = startingAircraft;
	}

	public override IObservable<Unit> ShowHangar()
	{
		return Observable.Defer(delegate
		{
			foreach (AircraftListData item in _startingAircraft.CreateStartingAircraftInfos())
			{
				if (GetListItemForId(item.AircraftIdentifier) == null)
				{
					base.SupportedAircraft.Add(new AircraftListItemViewModel(item));
				}
			}
			return base.ShowHangar();
		}).SubscribeOn(ViewModelSchedulers.UIThread);
	}
}
