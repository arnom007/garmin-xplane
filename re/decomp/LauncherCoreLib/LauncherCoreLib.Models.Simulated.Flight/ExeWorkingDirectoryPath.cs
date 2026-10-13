using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Simulated.Flight;

public readonly struct ExeWorkingDirectoryPath : IEquatable<ExeWorkingDirectoryPath>
{
	[field: CompilerGenerated]
	public string RawPath
	{
		[CompilerGenerated]
		get;
	}

	public ExeWorkingDirectoryPath(string rawPath)
	{
		RawPath = rawPath;
	}

	public static bool operator ==(ExeWorkingDirectoryPath left, ExeWorkingDirectoryPath right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ExeWorkingDirectoryPath left, ExeWorkingDirectoryPath right)
	{
		return !(left == right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is ExeWorkingDirectoryPath exeWorkingDirectoryPath)
		{
			return this == exeWorkingDirectoryPath;
		}
		return false;
	}

	public bool Equals(ExeWorkingDirectoryPath other)
	{
		return string.Equals(RawPath, other.RawPath, (StringComparison)4);
	}

	public override int GetHashCode()
	{
		return RawPath.GetHashCode((StringComparison)4);
	}
}
