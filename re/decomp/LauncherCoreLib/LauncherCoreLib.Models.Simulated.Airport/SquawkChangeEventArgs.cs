using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class SquawkChangeEventArgs : EventArgs
{
	[field: CompilerGenerated]
	public System.Collections.Generic.IReadOnlyCollection<Squawk> Added
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public System.Collections.Generic.IReadOnlyCollection<Squawk> Removed
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	private SquawkChangeEventArgs()
	{
		System.Collections.Generic.IReadOnlyCollection<Squawk> removed = (Added = (System.Collections.Generic.IReadOnlyCollection<Squawk>)(object)System.Array.Empty<Squawk>());
		Removed = removed;
	}

	public static SquawkChangeEventArgs CreateAdded(System.Collections.Generic.IEnumerable<Squawk> squawks)
	{
		return new SquawkChangeEventArgs
		{
			Added = (System.Collections.Generic.IReadOnlyCollection<Squawk>)(object)Enumerable.ToArray<Squawk>(squawks)
		};
	}

	public static SquawkChangeEventArgs CreateRemoved(System.Collections.Generic.IEnumerable<Squawk> squawks)
	{
		return new SquawkChangeEventArgs
		{
			Removed = (System.Collections.Generic.IReadOnlyCollection<Squawk>)(object)Enumerable.ToArray<Squawk>(squawks)
		};
	}
}
