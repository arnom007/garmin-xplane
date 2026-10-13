using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using LOLA.Extensions;
using LOLA.Logging.Reactive;
using LOLA.Reactive;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class BasicAirworthiness : IAirworthiness
{
	public IObservable<bool> IsAirworthyOverTime(ReactiveAircraft aircraft)
	{
		if (aircraft == null)
		{
			return Observable.Return(value: false);
		}
		return Observable.CombineLatest<int, bool, AircraftIdentifier, bool>(aircraft.Panel.GetCachedThenUpdatedAndThenContinualValues().Select<SimPanel, int>((Func<SimPanel, int>)((SimPanel panel) => ((System.Collections.Generic.IReadOnlyCollection<SimInstallKey>)panel.ActiveSimKeys).Count)), aircraft.ActivePackagesInAircraftOverTime().Select<System.Collections.Generic.IList<ITrainerPackage>, HashSet<ITrainerPackage>>((Func<System.Collections.Generic.IList<ITrainerPackage>, HashSet<ITrainerPackage>>)((System.Collections.Generic.IList<ITrainerPackage> packages) => Enumerable.ToHashSet<ITrainerPackage>(Enumerable.Where<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)packages, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => p.IsACatalogPackage()))))).DistinctUntilChanged<HashSet<ITrainerPackage>>((IEqualityComparer<HashSet<ITrainerPackage>>)(object)new SetEquality<ITrainerPackage>())
			.Select<HashSet<ITrainerPackage>, bool>((Func<HashSet<ITrainerPackage>, bool>)((HashSet<ITrainerPackage> packs) => Enumerable.All<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)packs, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => p.BaseInfo.PackageType.IsCompleteOnDisk())))), aircraft.ListData.GetCachedThenUpdatedAndThenContinualValues().DistinctUntilChanged<AircraftListData>().Select<AircraftListData, AircraftIdentifier>((Func<AircraftListData, AircraftIdentifier>)((AircraftListData a) => a.AircraftIdentifier)), (int simCount, bool allAreInstalled, AircraftIdentifier aircraftId) => simCount > 0 && allAreInstalled && aircraftId != AircraftIdentifier.Default).LogError<bool>("IsAirworthyOverTime").Catch<bool, System.Exception>(delegate(System.Exception e)
		{
			IObservable<bool> val = Observable.Return(value: false);
			return (IObservable<bool>)((e is AircraftRollbackException || e is FormatException) ? ((object)val) : ((object)val.Concat<bool>(Observable.Throw<bool>(e))));
		});
	}
}
