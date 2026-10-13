using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Panel;

public sealed class InstallationSlot : IEquatable<InstallationSlot>, IComparable<InstallationSlot>
{
	public static readonly InstallationSlot Invalid = new InstallationSlot();

	[field: CompilerGenerated]
	public AvionicsType Category
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public int InstanceId
	{
		[CompilerGenerated]
		get;
	}

	public string SerializedSlot => ((object)this).ToString();

	public string InstanceString => InstanceId.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture);

	public InstallationSlot(int instanceId, AvionicsType category)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (instanceId < 0)
		{
			throw new ArgumentOutOfRangeException("instanceId", "Instance Id must be greater or equal to 0.");
		}
		InstanceId = instanceId;
		Category = category;
	}

	private InstallationSlot()
	{
		InstanceId = 0;
		Category = AvionicsType.Unknown;
	}

	public static InstallationSlot Parse(string serializedSlot)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (serializedSlot == null)
		{
			throw new ArgumentNullException("serializedSlot");
		}
		string[] array = serializedSlot.Split('_', (StringSplitOptions)0);
		if (array.Length != 2)
		{
			throw new FormatException(serializedSlot + " is not in the correct format.");
		}
		AvionicsType category = default(AvionicsType);
		if (!System.Enum.TryParse<AvionicsType>(array[0], ref category))
		{
			throw new FormatException(array[0] + " is not a valid avionics type.");
		}
		int instanceId = default(int);
		if (!int.TryParse(array[1], ref instanceId))
		{
			throw new FormatException(array[1] + " is not a valid instance id.");
		}
		return new InstallationSlot(instanceId, category);
	}

	public static bool TryParse(string serializedSlot, out InstallationSlot slot)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (serializedSlot == null)
		{
			throw new ArgumentNullException("serializedSlot");
		}
		slot = Invalid;
		string[] array = serializedSlot.Split('_', (StringSplitOptions)0);
		if (array.Length != 2)
		{
			return false;
		}
		AvionicsType category = default(AvionicsType);
		if (!System.Enum.TryParse<AvionicsType>(array[0], ref category))
		{
			return false;
		}
		int instanceId = default(int);
		if (!int.TryParse(array[1], ref instanceId))
		{
			return false;
		}
		slot = new InstallationSlot(instanceId, category);
		return true;
	}

	public static InstallationSlot ParseOrInvalid(string serializedSlot)
	{
		if (!TryParse(serializedSlot, out InstallationSlot slot))
		{
			return Invalid;
		}
		return slot;
	}

	public static bool operator ==(InstallationSlot? lhs, InstallationSlot? rhs)
	{
		return EqualityComparer<InstallationSlot>.Default.Equals(lhs, rhs);
	}

	public static bool operator !=(InstallationSlot? lhs, InstallationSlot? rhs)
	{
		return !EqualityComparer<InstallationSlot>.Default.Equals(lhs, rhs);
	}

	public static bool operator <(InstallationSlot left, InstallationSlot right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) < 0;
		}
		return (object)right != null;
	}

	public static bool operator <=(InstallationSlot left, InstallationSlot right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) <= 0;
		}
		return true;
	}

	public static bool operator >(InstallationSlot left, InstallationSlot right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) > 0;
		}
		return false;
	}

	public static bool operator >=(InstallationSlot left, InstallationSlot right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) >= 0;
		}
		return (object)right == null;
	}

	public int ToArrayIndex()
	{
		return InstanceId - 1;
	}

	public bool Equals(InstallationSlot? other)
	{
		if ((object)this != other)
		{
			if (other != null && Category == other.Category)
			{
				return InstanceId == other.InstanceId;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is InstallationSlot other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine<AvionicsType, int>(Category, InstanceId);
	}

	public override string ToString()
	{
		return ((object)Category).ToString() + "_" + InstanceString;
	}

	public string ToUiString()
	{
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0} {1}", (object)Category, (object)InstanceString);
	}

	public int CompareTo(InstallationSlot? other)
	{
		if ((object)other == null)
		{
			return -1;
		}
		int num = Comparer<AvionicsType>.Default.Compare(Category, other.Category);
		if (num == 0)
		{
			return Comparer<int>.Default.Compare(InstanceId, other.InstanceId);
		}
		return num;
	}
}
