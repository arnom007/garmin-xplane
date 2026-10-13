using System;
using System.Collections.Generic;

namespace LauncherCoreLib.Models.Simulated.Airport;

public interface ISquawkBoard
{
	System.Collections.Generic.IReadOnlyCollection<Squawk> Squawks { get; }

	event EventHandler<SquawkChangeEventArgs> OnBoardChange;

	void Add(Squawk squawk);

	void Remove(Squawk squawk);

	void Clear();
}
