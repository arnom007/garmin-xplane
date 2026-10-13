using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LOLA.Extensions;

namespace LauncherCoreLib.Models.Winfs;

public sealed class WinfsDatabaseLinkInstruction : IEquatable<WinfsDatabaseLinkInstruction>
{
	[field: CompilerGenerated]
	public SupportedWinfsDatabase Database
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public WinfsLinkInstruction LinkInstruction
	{
		[CompilerGenerated]
		get;
	}

	public WinfsDatabaseLinkInstruction(SupportedWinfsDatabase supportedWinfsDatabase, WinfsLinkInstruction linkInstruction)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Database = supportedWinfsDatabase;
		LinkInstruction = linkInstruction ?? throw new ArgumentNullException("linkInstruction");
	}

	public SourcedBatchTransform ToSourcedTransform()
	{
		return new SourcedBatchTransform(BatchTransformation.LinksOnly(LinkInstruction.Yield()), BatchTransformSource.Databases);
	}

	public bool Equals(WinfsDatabaseLinkInstruction? other)
	{
		if (this != other)
		{
			if (other != null && Database == other.Database)
			{
				return EqualityComparer<WinfsLinkInstruction>.Default.Equals(LinkInstruction, other.LinkInstruction);
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is WinfsDatabaseLinkInstruction other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (527796611 * -1521134295 + ((object)Database).GetHashCode()) * -1521134295 + EqualityComparer<WinfsLinkInstruction>.Default.GetHashCode(LinkInstruction);
	}
}
