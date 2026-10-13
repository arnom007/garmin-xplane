using System;
using System.IO;

namespace RoarClient.Net;

public struct DllFilePath : IEquatable<DllFilePath>
{
	public string FullPath { get; }

	public DllFilePath(string fullPath)
	{
		FullPath = fullPath ?? throw new ArgumentNullException("fullPath");
	}

	public static bool operator ==(DllFilePath left, DllFilePath right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(DllFilePath left, DllFilePath right)
	{
		return !(left == right);
	}

	public bool MatchesRoarClientFileNamePattern()
	{
		return Path.GetFileName(FullPath).StartsWith("roarclient", StringComparison.OrdinalIgnoreCase);
	}

	public bool Equals(DllFilePath other)
	{
		return FullPath == other.FullPath;
	}

	public override bool Equals(object obj)
	{
		if (obj is DllFilePath other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (FullPath?.GetHashCode()).GetValueOrDefault();
	}

	public override string ToString()
	{
		return FullPath ?? string.Empty;
	}
}
