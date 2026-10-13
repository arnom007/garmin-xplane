using System;

namespace RoarDiscovery;

public struct RoarExePath : IEquatable<RoarExePath>, IEquatable<string>
{
	public string ExePath { get; }

	public RoarExePath(string exePath)
	{
		ExePath = exePath ?? throw new ArgumentNullException("exePath");
	}

	public static implicit operator string(RoarExePath roarExePath)
	{
		return roarExePath.ExePath;
	}

	public static bool operator ==(RoarExePath left, RoarExePath right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(RoarExePath left, RoarExePath right)
	{
		return !(left == right);
	}

	public RoarExePath ThrowIfDefault(string argumentName)
	{
		if (ExePath == null)
		{
			throw new ArgumentNullException(argumentName);
		}
		return this;
	}

	public bool Equals(RoarExePath other)
	{
		return ExePath == other.ExePath;
	}

	public bool Equals(string other)
	{
		return ExePath == other;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is RoarExePath other) || !Equals(other))
		{
			if (obj is string other2)
			{
				return Equals(other2);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return (ExePath?.GetHashCode()).GetValueOrDefault();
	}

	public override string ToString()
	{
		return ExePath;
	}
}
