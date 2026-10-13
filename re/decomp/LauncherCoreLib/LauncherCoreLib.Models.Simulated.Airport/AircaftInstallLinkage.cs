using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LOLA.Extensions;
using LOLA.Reactive;
using LauncherCoreLib.Models.TrainerPackages;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class AircaftInstallLinkage : IAircraftInstallationLinker, IAircraftLinkedInstallations
{
	private readonly ConcurrentDictionary<AircraftIdentifier, ConcurrentDictionary<TrainerPackageIdentifier, ITrainerPackage>> _linkedPackages = new ConcurrentDictionary<AircraftIdentifier, ConcurrentDictionary<TrainerPackageIdentifier, ITrainerPackage>>();

	private readonly ISubject<AircraftIdentifier> _aircraftChanged = new Subject<AircraftIdentifier>();

	public IObservable<ISet<ITrainerPackage>> CurrentPackageInstallsLinkedToAircraft(AircraftIdentifier aircraftIdentifier)
	{
		ConcurrentDictionary<TrainerPackageIdentifier, ITrainerPackage> dictionary = default(ConcurrentDictionary<TrainerPackageIdentifier, ITrainerPackage>);
		return (IObservable<ISet<ITrainerPackage>>)(object)TimingEvent.StartStreamAndConcat<AircraftIdentifier>((IObservable<AircraftIdentifier>)_aircraftChanged).Select<TimingEvent, HashSet<ITrainerPackage>>((Func<TimingEvent, HashSet<ITrainerPackage>>)((TimingEvent _) => (!_linkedPackages.TryGetValue(aircraftIdentifier, ref dictionary)) ? new HashSet<ITrainerPackage>() : Enumerable.ToHashSet<ITrainerPackage>(dictionary.NonBlockingValues<TrainerPackageIdentifier, ITrainerPackage>())));
	}

	public System.IDisposable LinkAircraftToPackageInstall(AircraftIdentifier aircraftIdentifier, ITrainerPackage trainerPackage)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		if (!trainerPackage.IsACatalogPackage())
		{
			throw new ArgumentException("Can only link cataloged packages.");
		}
		ConcurrentDictionary<TrainerPackageIdentifier, ITrainerPackage> aircraftSet = _linkedPackages.GetOrAdd(aircraftIdentifier, (Func<AircraftIdentifier, ConcurrentDictionary<TrainerPackageIdentifier, ITrainerPackage>>)((AircraftIdentifier _) => new ConcurrentDictionary<TrainerPackageIdentifier, ITrainerPackage>()));
		ITrainerPackage pack = aircraftSet.GetOrAdd(trainerPackage.TrainerPackageId, trainerPackage);
		RaiseChangeEvent();
		return Disposable.Create((Action)delegate
		{
			ITrainerPackage trainerPackage2 = default(ITrainerPackage);
			if (aircraftSet.TryRemove(pack.TrainerPackageId, ref trainerPackage2))
			{
				RaiseChangeEvent();
			}
		});
		void RaiseChangeEvent()
		{
			lock (_aircraftChanged)
			{
				((IObserver<AircraftIdentifier>)_aircraftChanged).OnNext(aircraftIdentifier);
			}
		}
	}
}
