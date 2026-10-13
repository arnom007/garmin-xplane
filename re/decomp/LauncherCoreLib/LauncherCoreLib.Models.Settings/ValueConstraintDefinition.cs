using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using TrainerPackageLib.Serialized;

namespace LauncherCoreLib.Models.Settings;

public sealed class ValueConstraintDefinition : IEquatable<ValueConstraintDefinition>
{
	[field: CompilerGenerated]
	public SettingIdentifier SettingToObserve
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ImmutableArray<string> EnableOnValues
	{
		[CompilerGenerated]
		get;
	} = ImmutableArray<string>.Empty;


	[field: CompilerGenerated]
	public ImmutableArray<string> DisableOnValues
	{
		[CompilerGenerated]
		get;
	} = ImmutableArray<string>.Empty;


	public ValueConstraintDefinition(SettingIdentifier toObserve, ImmutableArray<string> enable, ImmutableArray<string> disable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		SettingToObserve = toObserve ?? throw new ArgumentNullException("toObserve");
		EnableOnValues = enable;
		DisableOnValues = disable;
	}

	public static System.Collections.Generic.IEnumerable<ValueConstraintDefinition> FromJsonConstraintArray(System.Collections.Generic.IEnumerable<Setting_constraint> constraints)
	{
		return Enumerable.Select<Setting_constraint, ValueConstraintDefinition>(constraints, (Func<Setting_constraint, ValueConstraintDefinition>)((Setting_constraint c) => new ValueConstraintDefinition(c.SettingToObserve, ImmutableArray.ToImmutableArray<string>((System.Collections.Generic.IEnumerable<string>)c.EnableOnValues), ImmutableArray.ToImmutableArray<string>((System.Collections.Generic.IEnumerable<string>)c.DisableOnValues))));
	}

	public bool AreValuesIdentifiers(Func<SettingIdentifier, SettingDefinition> settingLookup)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return (settingLookup.Invoke(SettingToObserve) ?? throw new InvalidOperationException("settingLookup could not find " + SettingToObserve)).ControlType.ControlUsesIdentifiers();
	}

	public bool Equals(ValueConstraintDefinition? other)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (this != other)
		{
			if (other != null && SettingToObserve == other.SettingToObserve && ImmutableArrayExtensions.SequenceEqual<string, string>(EnableOnValues, other.EnableOnValues, (IEqualityComparer<string>)null))
			{
				return ImmutableArrayExtensions.SequenceEqual<string, string>(DisableOnValues, other.DisableOnValues, (IEqualityComparer<string>)null);
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is ValueConstraintDefinition other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)SettingToObserve).GetHashCode();
	}
}
