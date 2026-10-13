using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.AbstractUi;

public sealed class FeatureAvailableIndicator
{
	public static readonly FeatureAvailableIndicator Available = new FeatureAvailableIndicator
	{
		IsFeatureAvailable = true
	};

	[field: CompilerGenerated]
	public bool IsFeatureAvailable
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public System.Collections.Generic.IReadOnlyList<string> DisabledReasons
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public FeatureAvailableIndicator()
	{
		IsFeatureAvailable = false;
		DisabledReasons = (System.Collections.Generic.IReadOnlyList<string>)new List<string>().AsReadOnly();
	}

	public static FeatureAvailableIndicator Disabled(params string[] reasons)
	{
		return Disabled((System.Collections.Generic.IEnumerable<string>)reasons);
	}

	public static FeatureAvailableIndicator Disabled(System.Collections.Generic.IEnumerable<string> reasons)
	{
		ReadOnlyCollection<string> disabledReasons = (IsAlreadyReadonly(reasons) ? ((ReadOnlyCollection<string>)reasons) : Enumerable.ToList<string>(reasons).AsReadOnly());
		return new FeatureAvailableIndicator
		{
			IsFeatureAvailable = false,
			DisabledReasons = (System.Collections.Generic.IReadOnlyList<string>)disabledReasons
		};
	}

	public static FeatureAvailableIndicator Merge(params FeatureAvailableIndicator[] indicators)
	{
		ReadOnlyCollection<string> val = Enumerable.ToList<string>(Enumerable.SelectMany<FeatureAvailableIndicator, string>(Enumerable.Where<FeatureAvailableIndicator>((System.Collections.Generic.IEnumerable<FeatureAvailableIndicator>)indicators, (Func<FeatureAvailableIndicator, bool>)((FeatureAvailableIndicator i) => !i.IsFeatureAvailable)), (Func<FeatureAvailableIndicator, System.Collections.Generic.IEnumerable<string>>)((FeatureAvailableIndicator i) => (System.Collections.Generic.IEnumerable<string>)i.DisabledReasons))).AsReadOnly();
		if (val.Count > 0)
		{
			return Disabled((System.Collections.Generic.IEnumerable<string>)val);
		}
		return Available;
	}

	private static bool IsAlreadyReadonly(System.Collections.Generic.IEnumerable<string> reasons)
	{
		return reasons is ReadOnlyCollection<string>;
	}
}
