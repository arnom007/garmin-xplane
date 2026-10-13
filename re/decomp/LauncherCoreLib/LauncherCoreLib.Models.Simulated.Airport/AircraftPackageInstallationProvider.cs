using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using LOLA.Extensions;
using LauncherCoreLib.Models.TrainerPackages;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class AircraftPackageInstallationProvider : IAircraftPackageInstallationProvider
{
	private readonly IAircraftLinkedInstallations _aircraftLinks;

	public AircraftPackageInstallationProvider(IAircraftLinkedInstallations aircraftLinks)
	{
		_aircraftLinks = aircraftLinks;
	}

	public IObservable<System.Collections.Generic.IReadOnlyCollection<ITrainerPackage>> GetRelevantPackages(ReactiveAircraft aircraft)
	{
		return (IObservable<System.Collections.Generic.IReadOnlyCollection<ITrainerPackage>>)(object)Observable.CombineLatest<HashSet<ITrainerPackage>, ISet<ITrainerPackage>, HashSet<ITrainerPackage>>(aircraft.ActivePackagesInAircraftOverTime().Select<System.Collections.Generic.IList<ITrainerPackage>, HashSet<ITrainerPackage>>((Func<System.Collections.Generic.IList<ITrainerPackage>, HashSet<ITrainerPackage>>)((System.Collections.Generic.IList<ITrainerPackage> activeAircraftPackages) => Enumerable.ToHashSet<ITrainerPackage>(Enumerable.Where<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)activeAircraftPackages, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => p.IsACatalogPackage()))))).DistinctUntilChanged<HashSet<ITrainerPackage>>((IEqualityComparer<HashSet<ITrainerPackage>>)(object)new SetEquality<ITrainerPackage>()), _aircraftLinks.CurrentPackageInstallsLinkedToAircraft(aircraft.AircraftIdentifier), (HashSet<ITrainerPackage> activeAircraftPackages, ISet<ITrainerPackage> linkedPackages) => Enumerable.ToHashSet<ITrainerPackage>(Enumerable.Concat<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)activeAircraftPackages, (System.Collections.Generic.IEnumerable<ITrainerPackage>)linkedPackages)));
	}
}
