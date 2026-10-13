using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using LOLA.Extensions;
using LauncherCoreLib.Models.Settings;
using RecordShims;

namespace LauncherCoreLib.Models.Simulated.Panel;

public sealed class FamilyData : IRecord<FamilyData>, IEquatable<FamilyData>
{
	private sealed class DeepEquality : EqualityComparer<FamilyData>
	{
		public override bool Equals(FamilyData? x, FamilyData? y)
		{
			if (x != y)
			{
				return x?.Equals(y) ?? false;
			}
			return true;
		}

		public override int GetHashCode(FamilyData obj)
		{
			return ((object)obj).GetHashCode();
		}
	}

	private readonly int _hash;

	[field: CompilerGenerated]
	public static IEqualityComparer<FamilyData> DEEP_EQUALITY
	{
		[CompilerGenerated]
		get;
	} = (IEqualityComparer<FamilyData>)(object)new DeepEquality();


	[field: CompilerGenerated]
	public ImmutableDictionary<SettingIdentifier, string> Settings
	{
		[CompilerGenerated]
		get;
	}

	public FamilyData()
		: this(ImmutableDictionary<SettingIdentifier, string>.Empty)
	{
	}

	public FamilyData(ImmutableDictionary<SettingIdentifier, string> settings)
	{
		Settings = settings ?? ImmutableDictionary<SettingIdentifier, string>.Empty;
		_hash = Enumerable.Aggregate<SettingIdentifier, int>(Settings.Keys, 1, (Func<int, SettingIdentifier, int>)((int acc, SettingIdentifier next) => acc ^ ((object)next).GetHashCode()));
	}

	public FamilyData ShallowCopy()
	{
		return new FamilyData(Settings);
	}

	public void ThrowIfConstraintsAreViolated(FamilyData record)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (record.Settings == null)
		{
			throw new ArgumentNullException("record", "Settings");
		}
	}

	public bool Equals(FamilyData? other)
	{
		if (this != other)
		{
			if (other != null)
			{
				return ((System.Collections.Generic.ICollection<KeyValuePair<SettingIdentifier, string>>)((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)Settings).SymmetricDifference((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)other.Settings)).Count == 0;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is FamilyData other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _hash;
	}
}
