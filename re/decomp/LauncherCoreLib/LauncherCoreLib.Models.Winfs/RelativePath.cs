using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Winfs;

public sealed class RelativePath : IEquatable<RelativePath>, IEquatable<string>
{
	[field: CompilerGenerated]
	public string RelPath
	{
		[CompilerGenerated]
		get;
	}

	public bool IsRelative => !Path.IsPathRooted(RelPath);

	public RelativePath(string path)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		if (!IsPathValid(path))
		{
			throw new FormatException("Invalid relative path.");
		}
		RelPath = path;
	}

	public static bool IsPathValid(string? path)
	{
		return !string.IsNullOrEmpty(path);
	}

	public static bool operator ==(RelativePath? lhs, RelativePath? rhs)
	{
		return EqualityComparer<RelativePath>.Default.Equals(lhs, rhs);
	}

	public static bool operator !=(RelativePath? lhs, RelativePath? rhs)
	{
		return !EqualityComparer<RelativePath>.Default.Equals(lhs, rhs);
	}

	public static implicit operator string(RelativePath rp)
	{
		return rp.RelPath;
	}

	public static implicit operator RelativePath(string path)
	{
		return FromNonNullString(path);
	}

	public static string NormalizePath(string path)
	{
		return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
	}

	public static RelativePath? FromString(string? path)
	{
		if (path != null)
		{
			return new RelativePath(path);
		}
		return null;
	}

	public static RelativePath FromNonNullString(string path)
	{
		return FromString(path);
	}

	public string AppendTo(string basePath)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (basePath == null)
		{
			throw new ArgumentNullException("basePath");
		}
		basePath = NormalizePath(basePath);
		return Combine(basePath, NormalizePath(RelPath));
	}

	public RelativePath ToAbsolutePath(string basePath)
	{
		if (IsRelative)
		{
			return Path.GetFullPath(AppendTo(basePath));
		}
		return Path.GetFullPath(RelPath);
	}

	public RelativePath ToNormalizedPath()
	{
		return new RelativePath(NormalizePath(RelPath));
	}

	public bool Equals(RelativePath? other)
	{
		if ((object)other != null)
		{
			return RelPath == other.RelPath;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is RelativePath other) || !Equals(other))
		{
			if (obj is string other2)
			{
				return Equals(other2);
			}
			return false;
		}
		return true;
	}

	public bool Equals(string? other)
	{
		if (IsPathValid(other))
		{
			return Equals((RelativePath)other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return RelPath.GetHashCode((StringComparison)4);
	}

	public override string ToString()
	{
		return RelPath;
	}

	private static string Combine(string left, string right)
	{
		left = left.TrimEnd(Path.DirectorySeparatorChar);
		right = right.TrimStart(Path.DirectorySeparatorChar);
		return left + Path.DirectorySeparatorChar + right;
	}
}
