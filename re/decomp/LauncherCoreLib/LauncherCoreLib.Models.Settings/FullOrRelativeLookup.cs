using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Settings;

public class FullOrRelativeLookup<TValue> : IHaveSettingId where TValue : IHaveSettingId
{
	private readonly Dictionary<SettingIdentifier, TValue> _lookup = (Dictionary<SettingIdentifier, TValue>)(object)new Dictionary<SettingIdentifier, SettingIdentifier>();

	public SettingIdentifier SettingId
	{
		[CompilerGenerated]
		get;
	}

	public int Count => ((Dictionary<SettingIdentifier, SettingIdentifier>)(object)_lookup).Count;

	public System.Collections.Generic.IEnumerable<TValue> Values => (System.Collections.Generic.IEnumerable<TValue>)((Dictionary<SettingIdentifier, SettingIdentifier>)(object)_lookup).Values;

	public FullOrRelativeLookup(SettingIdentifier baseIdentifier)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (baseIdentifier == null)
		{
			throw new ArgumentNullException("baseIdentifier");
		}
		if (baseIdentifier.Equals(SettingIdentifier.Empty))
		{
			throw new ArgumentException("An empty SettingIdentifier is not valid.");
		}
		SettingId = baseIdentifier;
	}

	public bool ContainsIdentifier(SettingIdentifier settingIdentifier)
	{
		return ((Dictionary<SettingIdentifier, SettingIdentifier>)(object)_lookup).ContainsKey(settingIdentifier.LocalId);
	}

	public TValue? GetValue(SettingIdentifier valueIdentifier)
	{
		TValue identifierOrNull = GetIdentifierOrNull(valueIdentifier);
		if (EqualityComparer<TValue>.Default.Equals(identifierOrNull, default(TValue)))
		{
			SettingIdentifier settingIdentifier = RelativeToThisSetting(valueIdentifier);
			if (settingIdentifier != null)
			{
				identifierOrNull = GetIdentifierOrNull(settingIdentifier);
			}
		}
		return identifierOrNull;
	}

	public void Add(TValue value)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		SettingIdentifier? settingIdentifier = RelativeToThisSetting(value);
		if (settingIdentifier == null)
		{
			throw new InvalidOperationException("The setting must be relative to '" + SettingId + "'");
		}
		if (settingIdentifier.Depth > 1)
		{
			throw new InvalidOperationException("Must not be nested within another identifier.");
		}
		((Dictionary<SettingIdentifier, SettingIdentifier>)(object)_lookup)[value.SettingId.LocalId] = (SettingIdentifier)value;
	}

	internal bool Remove(SettingIdentifier settingId)
	{
		return ((Dictionary<SettingIdentifier, SettingIdentifier>)(object)_lookup).Remove(settingId);
	}

	internal void Clear()
	{
		((Dictionary<SettingIdentifier, SettingIdentifier>)(object)_lookup).Clear();
	}

	private unsafe TValue? GetIdentifierOrNull(SettingIdentifier valueIdentifier)
	{
		TValue result = default(TValue);
		((Dictionary<SettingIdentifier, SettingIdentifier>)(object)_lookup).TryGetValue(valueIdentifier, ref *(SettingIdentifier*)(&result));
		return result;
	}

	private SettingIdentifier? RelativeToThisSetting(SettingIdentifier valueDefinitionId)
	{
		return valueDefinitionId.RelativeTo(SettingId);
	}

	private SettingIdentifier? RelativeToThisSetting(IHaveSettingId settingProvider)
	{
		return RelativeToThisSetting(settingProvider.SettingId);
	}
}
