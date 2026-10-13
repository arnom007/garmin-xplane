using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using LOLA.Extensions;
using RecordShims;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Panel;

public sealed class FamilyDataLookup : IRecord<FamilyDataLookup>, IEquatable<FamilyDataLookup>
{
	private sealed class DeepEquality : EqualityComparer<FamilyDataLookup>
	{
		public override bool Equals(FamilyDataLookup? x, FamilyDataLookup? y)
		{
			if (x != y)
			{
				return x?.Equals(y) ?? false;
			}
			return true;
		}

		public override int GetHashCode(FamilyDataLookup obj)
		{
			return ((object)obj).GetHashCode();
		}
	}

	private readonly int _hash;

	[field: CompilerGenerated]
	public static IEqualityComparer<FamilyDataLookup> DEEP_EQUALS
	{
		[CompilerGenerated]
		get;
	} = (IEqualityComparer<FamilyDataLookup>)(object)new DeepEquality();


	[field: CompilerGenerated]
	public ImmutableDictionary<TrainerEmbeddedReleaseId, FamilyData> FamilyDataByRelease
	{
		[CompilerGenerated]
		get;
	}

	public FamilyDataLookup()
		: this(ImmutableDictionary<TrainerEmbeddedReleaseId, FamilyData>.Empty)
	{
	}

	public FamilyDataLookup(ImmutableDictionary<TrainerEmbeddedReleaseId, FamilyData> familyDataLookup)
	{
		FamilyDataByRelease = familyDataLookup ?? ImmutableDictionary<TrainerEmbeddedReleaseId, FamilyData>.Empty;
		_hash = Enumerable.Aggregate<TrainerEmbeddedReleaseId, int>(FamilyDataByRelease.Keys, 1, (Func<int, TrainerEmbeddedReleaseId, int>)((int acc, TrainerEmbeddedReleaseId next) => acc ^ ((object)next).GetHashCode()));
		ThrowIfConstraintsAreViolated(this);
	}

	public FamilyData GetFamilyDataOrEmpty(TrainerEmbeddedReleaseId releaseId)
	{
		FamilyData result = default(FamilyData);
		if (!FamilyDataByRelease.TryGetValue(releaseId, ref result))
		{
			return new FamilyData();
		}
		return result;
	}

	public FamilyData? GetFamilyDataOrNull(TrainerEmbeddedReleaseId releaseId)
	{
		FamilyData result = default(FamilyData);
		if (!FamilyDataByRelease.TryGetValue(releaseId, ref result))
		{
			return null;
		}
		return result;
	}

	public FamilyDataLookup ShallowCopy()
	{
		return new FamilyDataLookup(FamilyDataByRelease);
	}

	public void ThrowIfConstraintsAreViolated(FamilyDataLookup record)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (record.FamilyDataByRelease == null)
		{
			throw new ArgumentNullException("record", "FamilyDataByRelease");
		}
	}

	public FamilyDataLookup Replace(TrainerEmbeddedReleaseId releaseId, FamilyData data)
	{
		return new FamilyDataLookup(FamilyDataByRelease.SetItem(releaseId, data));
	}

	public FamilyDataLookup Remove(TrainerEmbeddedReleaseId releaseId)
	{
		return new FamilyDataLookup(FamilyDataByRelease.Remove(releaseId));
	}

	public bool Equals(FamilyDataLookup? other)
	{
		if (this != other)
		{
			if (other != null)
			{
				return ((System.Collections.Generic.ICollection<KeyValuePair<TrainerEmbeddedReleaseId, FamilyData>>)((System.Collections.Generic.IEnumerable<KeyValuePair<TrainerEmbeddedReleaseId, FamilyData>>)FamilyDataByRelease).SymmetricDifference((System.Collections.Generic.IEnumerable<KeyValuePair<TrainerEmbeddedReleaseId, FamilyData>>)other.FamilyDataByRelease)).Count == 0;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is FamilyDataLookup other)
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
