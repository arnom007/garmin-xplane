using System;

namespace RoarClient.Net;

public struct SearchDir : IEquatable<SearchDir>
{
	public string FullPath { get; }

	public SearchDir(string fullPath)
	{
		FullPath = fullPath ?? throw new ArgumentNullException("fullPath");
	}

	public static bool operator ==(SearchDir left, SearchDir right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(SearchDir left, SearchDir right)
	{
		return !(left == right);
	}

	public bool Equals(SearchDir other)
	{
		return FullPath == other.FullPath;
	}

	public override bool Equals(object obj)
	{
		if (obj is SearchDir other)
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
