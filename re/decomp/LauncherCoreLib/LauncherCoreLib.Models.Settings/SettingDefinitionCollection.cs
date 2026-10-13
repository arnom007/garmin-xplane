using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LauncherCoreLib.Models.Settings;

public sealed class SettingDefinitionCollection : System.Collections.Generic.ICollection<SettingDefinition>, System.Collections.Generic.IEnumerable<SettingDefinition>, System.Collections.IEnumerable, ISettingDefinitionCollection, IEquatable<ISettingDefinitionCollection>, IEquatable<SettingDefinitionCollection>
{
	private readonly FullOrRelativeLookup<SettingDefinition> _lookup;

	public SettingIdentifier BaseIdentifier => _lookup.SettingId;

	public System.Collections.Generic.IEnumerable<ISettingDefinition> Definitions => (System.Collections.Generic.IEnumerable<ISettingDefinition>)_lookup.Values;

	public bool IsReadOnly => false;

	public int Count => _lookup.Count;

	public SettingDefinitionCollection(SettingIdentifier baseIdentifier)
	{
		_lookup = new FullOrRelativeLookup<SettingDefinition>(baseIdentifier);
	}

	public bool ContainsSetting(SettingIdentifier settingIdentifier)
	{
		return _lookup.ContainsIdentifier(settingIdentifier);
	}

	public ISettingDefinition? GetSetting(SettingIdentifier settingIdentifier)
	{
		return _lookup.GetValue(settingIdentifier);
	}

	public void Add(SettingDefinition item)
	{
		_lookup.Add(item);
	}

	public SettingDefinitionCollection Build(Func<SettingIdentifier, SettingDefinition> buildSetting)
	{
		Add(buildSetting.Invoke(BaseIdentifier));
		return this;
	}

	public SettingDefinitionCollection Build(Func<SettingIdentifier, System.Collections.Generic.IEnumerable<SettingDefinition>> definitions)
	{
		return AddRange(definitions.Invoke(BaseIdentifier));
	}

	public SettingDefinitionCollection AddRange(System.Collections.Generic.IEnumerable<SettingDefinition> definitions)
	{
		System.Collections.Generic.IEnumerator<SettingDefinition> enumerator = definitions.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				SettingDefinition current = enumerator.Current;
				Add(current);
			}
			return this;
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	public void Clear()
	{
		_lookup.Clear();
	}

	public bool Contains(SettingDefinition item)
	{
		return _lookup.GetValue(item.SettingId)?.Equals(item) ?? false;
	}

	public void CopyTo(SettingDefinition[] array, int arrayIndex)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (array.Length - arrayIndex < Count)
		{
			throw new InvalidOperationException("Cannot copy. Not enough room.");
		}
		System.Collections.Generic.IEnumerable<SettingDefinition> values = _lookup.Values;
		int num = arrayIndex;
		System.Collections.Generic.IEnumerator<SettingDefinition> enumerator = values.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				SettingDefinition current = enumerator.Current;
				array[num++] = current;
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	public System.Collections.Generic.IEnumerator<SettingDefinition> GetEnumerator()
	{
		return _lookup.Values.GetEnumerator();
	}

	public bool Remove(SettingDefinition item)
	{
		if (Contains(item))
		{
			return _lookup.Remove(item.SettingId);
		}
		return false;
	}

	public bool Equals(ISettingDefinitionCollection? other)
	{
		if (this != other)
		{
			if (other != null)
			{
				return Enumerable.SequenceEqual<ISettingDefinition>(Definitions, other.Definitions);
			}
			return false;
		}
		return true;
	}

	public bool Equals(SettingDefinitionCollection? other)
	{
		return Equals((ISettingDefinitionCollection?)other);
	}

	public override bool Equals(object? obj)
	{
		if (obj is SettingDefinitionCollection other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)BaseIdentifier).GetHashCode() ^ Count.GetHashCode();
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return (System.Collections.IEnumerator)GetEnumerator();
	}
}
