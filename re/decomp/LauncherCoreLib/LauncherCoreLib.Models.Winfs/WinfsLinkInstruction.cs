using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Winfs;

public sealed class WinfsLinkInstruction : IEquatable<WinfsLinkInstruction>
{
	[field: CompilerGenerated]
	public RelativePath PackagePath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public RelativePath WinfsPath
	{
		[CompilerGenerated]
		get;
	}

	public WinfsLinkInstruction(RelativePath installed, RelativePath winfs)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		PackagePath = installed ?? throw new ArgumentNullException("installed");
		WinfsPath = winfs ?? throw new ArgumentNullException("winfs");
	}

	public bool Equals(WinfsLinkInstruction? other)
	{
		if (this != other)
		{
			if (other != null && PackagePath == other.PackagePath)
			{
				return WinfsPath == other.WinfsPath;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is WinfsLinkInstruction other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine<RelativePath, RelativePath>(PackagePath, WinfsPath);
	}
}
