using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Simulated.Flight;

public readonly struct ExeFilePath : IEquatable<ExeFilePath>
{
	[field: CompilerGenerated]
	public string RawPath
	{
		[CompilerGenerated]
		get;
	}

	public ExeFilePath(string rawPath)
	{
		RawPath = rawPath;
	}

	public static bool operator ==(ExeFilePath left, ExeFilePath right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ExeFilePath left, ExeFilePath right)
	{
		return !(left == right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is ExeFilePath exeFilePath)
		{
			return this == exeFilePath;
		}
		return false;
	}

	public bool Equals(ExeFilePath other)
	{
		return string.Equals(RawPath, other.RawPath, (StringComparison)4);
	}

	public override int GetHashCode()
	{
		return RawPath.GetHashCode((StringComparison)4);
	}
}
