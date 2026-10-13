using System;
using System.Globalization;

namespace RoarClient.Net.Native;

public struct ROAR_rte_chnl_type : IEquatable<ROAR_rte_chnl_type>
{
	private ushort _channel;

	public ROAR_rte_chnl_type(ushort chnl)
	{
		_channel = chnl;
	}

	public static implicit operator ushort(ROAR_rte_chnl_type chan)
	{
		return chan._channel;
	}

	public static implicit operator ROAR_rte_chnl_type(ushort chanNumber)
	{
		ROAR_rte_chnl_type result = default(ROAR_rte_chnl_type);
		result._channel = chanNumber;
		return result;
	}

	public static bool operator ==(ROAR_rte_chnl_type lhs, ROAR_rte_chnl_type rhs)
	{
		return lhs._channel == rhs._channel;
	}

	public static bool operator !=(ROAR_rte_chnl_type lhs, ROAR_rte_chnl_type rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator ==(ROAR_rte_chnl_type lhs, ushort rhs)
	{
		return lhs._channel == rhs;
	}

	public static bool operator !=(ROAR_rte_chnl_type lhs, ushort rhs)
	{
		return !(lhs == rhs);
	}

	public static ROAR_rte_chnl_type FromUInt16(ushort channel)
	{
		ROAR_rte_chnl_type result = default(ROAR_rte_chnl_type);
		result._channel = channel;
		return result;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is ROAR_rte_chnl_type other)
		{
			return Equals(other);
		}
		if (obj is IConvertible)
		{
			return _channel.Equals(Convert.ToUInt16(obj, CultureInfo.InvariantCulture));
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _channel.GetHashCode();
	}

	public override string ToString()
	{
		return _channel.ToString(CultureInfo.InvariantCulture);
	}

	public ushort ToUInt16()
	{
		return _channel;
	}

	public bool Equals(ROAR_rte_chnl_type other)
	{
		return _channel == other._channel;
	}
}
