using System;
using System.Globalization;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public sealed class SnapshotIdentifier : IEquatable<SnapshotIdentifier>, IComparable<SnapshotIdentifier>
{
	private readonly TrainerIdentifier _identifier;

	public string Serialized => _identifier.IdentStringChecked;

	public SnapshotIdentifier(TrainerIdentifier identifier)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		_identifier = identifier.ThrowIfInvalid((System.Exception?)new ArgumentException("Must be a valid trainer identifier."));
	}

	public SnapshotIdentifier(DateTimeOffset snapshotCreatedAt)
		: this((TrainerIdentifier)((DateTimeOffset)(ref snapshotCreatedAt)).UtcDateTime.ToString("yyyy_MM_dd_HH_mm_ss_FFFF", (IFormatProvider)(object)CultureInfo.InvariantCulture))
	{
	}

	public static SnapshotIdentifier Parse(string serialized)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (serialized == null)
		{
			throw new ArgumentNullException("serialized");
		}
		return new SnapshotIdentifier(new TrainerIdentifier(serialized));
	}

	public static bool IsValid(string input)
	{
		return TrainerIdentifier.IsValidIdentifier(input);
	}

	public static bool TryParse(string input, out SnapshotIdentifier snapshotIdentifier)
	{
		if (IsValid(input))
		{
			snapshotIdentifier = Parse(input);
			return true;
		}
		snapshotIdentifier = null;
		return false;
	}

	public static SnapshotIdentifier SnapshotCreatedNow()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return new SnapshotIdentifier(DateTimeOffset.Now);
	}

	public static bool operator ==(SnapshotIdentifier? left, SnapshotIdentifier? right)
	{
		return left?.Equals(right) ?? ((object)right == null);
	}

	public static bool operator !=(SnapshotIdentifier? left, SnapshotIdentifier? right)
	{
		return !(left == right);
	}

	public static bool operator <(SnapshotIdentifier left, SnapshotIdentifier right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) < 0;
		}
		return (object)right != null;
	}

	public static bool operator <=(SnapshotIdentifier left, SnapshotIdentifier right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) <= 0;
		}
		return true;
	}

	public static bool operator >(SnapshotIdentifier left, SnapshotIdentifier right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) > 0;
		}
		return false;
	}

	public static bool operator >=(SnapshotIdentifier left, SnapshotIdentifier right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) >= 0;
		}
		return (object)right == null;
	}

	public bool Equals(SnapshotIdentifier? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null)
			{
				return _identifier == other._identifier;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is SnapshotIdentifier)
		{
			return Equals((SnapshotIdentifier)obj);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)_identifier).GetHashCode() * 1871;
	}

	public override string ToString()
	{
		return ((object)_identifier).ToString();
	}

	public int CompareTo(SnapshotIdentifier? other)
	{
		return _identifier.CompareTo(other?._identifier ?? default(TrainerIdentifier));
	}
}
