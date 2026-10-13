using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using LOLA.Reactive;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class CompositeAirworthiness : IAirworthiness
{
	private readonly System.Collections.Generic.IEnumerable<IAirworthiness> _worthinessChecks;

	public CompositeAirworthiness(System.Collections.Generic.IEnumerable<IAirworthiness> providers)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_worthinessChecks = providers ?? throw new ArgumentNullException("providers");
	}

	public CompositeAirworthiness(params IAirworthiness[] providers)
		: this((System.Collections.Generic.IEnumerable<IAirworthiness>)providers)
	{
	}

	public IObservable<bool> IsAirworthyOverTime(ReactiveAircraft aircraft)
	{
		ReactiveAircraft aircraft2 = aircraft;
		return Observable.Defer<bool>((Func<IObservable<bool>>)(() => Enumerable.Select<IAirworthiness, IObservable<bool>>(_worthinessChecks, (Func<IAirworthiness, IObservable<bool>>)((IAirworthiness p) => p.IsAirworthyOverTime(aircraft2))).CombineLatestOrEmpty().Select<System.Collections.Generic.IList<bool>, bool>((Func<System.Collections.Generic.IList<bool>, bool>)((System.Collections.Generic.IList<bool> combinedResults) => ((System.Collections.Generic.IEnumerable<bool>)combinedResults).AllEqual(operand: true)))));
	}
}
