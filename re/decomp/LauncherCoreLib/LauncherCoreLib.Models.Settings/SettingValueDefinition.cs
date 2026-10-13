using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Settings;

public sealed class SettingValueDefinition : IEquatable<SettingValueDefinition>, ISettingValueDefinition, IEquatable<ISettingValueDefinition>, IHaveSettingId
{
	[field: CompilerGenerated]
	public SettingIdentifier ValueId
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string DisplayText
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[field: CompilerGenerated]
	public string Description
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[field: CompilerGenerated]
	public int DocumentOrderIndex
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public string SimulatorValue
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[field: CompilerGenerated]
	public ImmutableArray<ValueConstraintDefinition> Constraints
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = ImmutableArray<ValueConstraintDefinition>.Empty;


	SettingIdentifier IHaveSettingId.SettingId => ValueId;

	public SettingValueDefinition(SettingIdentifier fullValueIdentifier)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ValueId = fullValueIdentifier;
	}

	public SettingValueDefinition(SettingIdentifier settingDefinitionIdentifier, SettingIdentifier valueIdentifier)
		: this(settingDefinitionIdentifier + valueIdentifier)
	{
	}

	public override string ToString()
	{
		return ValueId;
	}

	public override int GetHashCode()
	{
		return ((object)ValueId).GetHashCode();
	}

	public bool Equals(SettingValueDefinition? other)
	{
		return Equals((ISettingValueDefinition?)other);
	}

	public bool Equals(ISettingValueDefinition? other)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (this != other)
		{
			if (other != null && ValueId == other.ValueId && DisplayText == other.DisplayText && Description == other.Description && SimulatorValue == other.SimulatorValue)
			{
				return ImmutableArrayExtensions.SequenceEqual<ValueConstraintDefinition, ValueConstraintDefinition>(Constraints, other.Constraints, (IEqualityComparer<ValueConstraintDefinition>)null);
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is SettingValueDefinition other)
		{
			return Equals(other);
		}
		return false;
	}
}
