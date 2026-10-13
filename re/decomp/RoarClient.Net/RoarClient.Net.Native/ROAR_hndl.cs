using System;

namespace RoarClient.Net.Native;

public struct ROAR_hndl : IEquatable<ROAR_hndl>
{
	public static readonly ROAR_hndl INVALID = new ROAR_hndl
	{
		_hndl = 0
	};

	private int _hndl;

	internal int Value => _hndl;

	public static bool operator ==(ROAR_hndl lhs, ROAR_hndl rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(ROAR_hndl lhs, ROAR_hndl rhs)
	{
		return !lhs.Equals(rhs);
	}

	public static ROAR_hndl Fabricate(int handleValue)
	{
		ROAR_hndl result = default(ROAR_hndl);
		result._hndl = handleValue;
		return result;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is ROAR_hndl other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _hndl.GetHashCode();
	}

	public bool Equals(ROAR_hndl other)
	{
		return _hndl == other._hndl;
	}
}
