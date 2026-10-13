using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LOLA.Extensions;

namespace LauncherCoreLib.Models.Settings;

public sealed class SettingDefinition : ISettingDefinition, IEquatable<ISettingDefinition>, IEquatable<SettingDefinition>, IHaveSettingId
{
	private readonly FullOrRelativeLookup<SettingValueDefinition> _lookup;

	[field: CompilerGenerated]
	public SettingUIControlType ControlType
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
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
	public string DefaultValue
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	public SettingIdentifier SettingId => _lookup.SettingId;

	public System.Collections.Generic.IEnumerable<ISettingValueDefinition> Values => (System.Collections.Generic.IEnumerable<ISettingValueDefinition>)_lookup.Values;

	public SettingDefinition(SettingIdentifier settingIdentifier)
	{
		_lookup = new FullOrRelativeLookup<SettingValueDefinition>(settingIdentifier);
	}

	public SettingDefinition(SettingIdentifier packageNamespace, SettingIdentifier localIdentifier)
		: this(packageNamespace + localIdentifier)
	{
	}

	public bool ContainsValue(SettingIdentifier valueIdentifier)
	{
		return _lookup.ContainsIdentifier(valueIdentifier);
	}

	public SettingDefinition Add(SettingValueDefinition valueDefinition)
	{
		_lookup.Add(valueDefinition);
		return this;
	}

	public SettingDefinition AddRange(System.Collections.Generic.IEnumerable<SettingValueDefinition> valueDefinitions)
	{
		System.Collections.Generic.IEnumerator<SettingValueDefinition> enumerator = valueDefinitions.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				SettingValueDefinition current = enumerator.Current;
				Add(current);
			}
			return this;
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	public SettingDefinition Build(Func<SettingIdentifier, SettingValueDefinition> valueDefinitions)
	{
		Func<SettingIdentifier, SettingValueDefinition> valueDefinitions2 = valueDefinitions;
		return Build((Func<SettingIdentifier, System.Collections.Generic.IEnumerable<SettingValueDefinition>>)((SettingIdentifier si) => valueDefinitions2.Invoke(si).Yield()));
	}

	public SettingDefinition Build(Func<SettingIdentifier, System.Collections.Generic.IEnumerable<SettingValueDefinition>> valueDefinitions)
	{
		return AddRange(valueDefinitions.Invoke(SettingId));
	}

	public ISettingValueDefinition? GetValue(SettingIdentifier valueIdentifier)
	{
		return _lookup.GetValue(valueIdentifier);
	}

	public override string ToString()
	{
		return _lookup.SettingId;
	}

	public bool Equals(ISettingDefinition? other)
	{
		if (this != other)
		{
			if (other != null && SettingId == other.SettingId && DisplayText == other.DisplayText && Description == other.Description && DefaultValue == other.DefaultValue && ControlType == other.ControlType)
			{
				return ((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)_lookup.Values).SetEquals(other.Values);
			}
			return false;
		}
		return true;
	}

	public bool Equals(SettingDefinition? other)
	{
		return Equals((ISettingDefinition?)other);
	}

	public override bool Equals(object? obj)
	{
		if (obj is SettingDefinition other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)SettingId).GetHashCode();
	}
}
