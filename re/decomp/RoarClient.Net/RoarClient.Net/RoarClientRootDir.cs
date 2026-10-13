using System;
using System.IO;
using System.Text.RegularExpressions;

namespace RoarClient.Net;

public struct RoarClientRootDir : IEquatable<RoarClientRootDir>
{
	private static readonly Regex ROAR_CLIENT_PACKAGE_OUTPUT_DIRECTORY_NAME_FORMAT = new Regex("^roarclient\\.((\\d+\\.){3}\\d+)$");

	public string FullPath { get; }

	public RoarClientRootDir(string fullPath)
	{
		FullPath = fullPath ?? throw new ArgumentNullException("fullPath");
		if (!MatchesDirectoryNamePattern(fullPath))
		{
			throw new ArgumentException("Does not match the directory name pattern.");
		}
	}

	public static bool MatchesDirectoryNamePattern(string directoryPath)
	{
		return ROAR_CLIENT_PACKAGE_OUTPUT_DIRECTORY_NAME_FORMAT.IsMatch(Path.GetFileName(directoryPath));
	}

	public static bool operator ==(RoarClientRootDir left, RoarClientRootDir right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(RoarClientRootDir left, RoarClientRootDir right)
	{
		return !(left == right);
	}

	public SearchDir GetSearchDirForArchitecture(SupportedArchitecture architecture)
	{
		return new SearchDir(Path.Combine(FullPath, ClientAssemblySearcher.GetDirectoryName(architecture)));
	}

	public SearchDir GetSearchDirForCurrentArchitecture()
	{
		return GetSearchDirForArchitecture(ClientAssemblySearcher.DefaultToCurrentProcessArch(null));
	}

	public bool Equals(RoarClientRootDir other)
	{
		return FullPath == other.FullPath;
	}

	public override bool Equals(object obj)
	{
		if (obj is RoarClientRootDir other)
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
