using System;
using System.Globalization;

namespace RoarClient.Net.Native;

public struct ROAR_port_type : IEquatable<ROAR_port_type>
{
	private ushort _port;

	public bool IsInvalidOrWildcard => _port == 0;

	public static implicit operator ushort(ROAR_port_type chan)
	{
		return chan._port;
	}

	public static implicit operator ROAR_port_type(ushort chanNumber)
	{
		ROAR_port_type result = default(ROAR_port_type);
		result._port = chanNumber;
		return result;
	}

	public static ROAR_port_type FromUInt16(ushort port)
	{
		ROAR_port_type result = default(ROAR_port_type);
		result._port = port;
		return result;
	}

	public static bool operator ==(ROAR_port_type left, ROAR_port_type right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ROAR_port_type left, ROAR_port_type right)
	{
		return !(left == right);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is ROAR_port_type other)
		{
			return Equals(other);
		}
		if (obj is IConvertible)
		{
			return _port.Equals(Convert.ToUInt16(obj, CultureInfo.InvariantCulture));
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _port.GetHashCode();
	}

	public bool Equals(ROAR_port_type other)
	{
		return _port == other._port;
	}

	public ushort ToUInt16()
	{
		return _port;
	}
}
