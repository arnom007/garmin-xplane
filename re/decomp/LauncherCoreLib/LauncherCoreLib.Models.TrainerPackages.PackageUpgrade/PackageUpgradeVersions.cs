using System;
using System.Runtime.CompilerServices;
using LOLA.Extensions;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;

public sealed class PackageUpgradeVersions : IEquatable<PackageUpgradeVersions>
{
	[field: CompilerGenerated]
	public TrainerEmbeddedReleaseId EmbeddedReleaseId
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public VersionRange VersionRange
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public Version LocalVersion => VersionRange.Start;

	public Version UpgradeVersion => VersionRange.End;

	public PackageUpgradeVersions(TrainerEmbeddedReleaseId trainerEmbeddedReleaseId, VersionRange versionRange)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		EmbeddedReleaseId = trainerEmbeddedReleaseId ?? throw new ArgumentNullException("trainerEmbeddedReleaseId");
		VersionRange = versionRange;
	}

	public bool CanUpgradePackage(TrainerPackageIdentifier trainerPackageIdentifier)
	{
		if (trainerPackageIdentifier.EmbeddedReleaseCompositeId == EmbeddedReleaseId && VersionRange.RangeContainsVersion(trainerPackageIdentifier.BinaryVersion))
		{
			return UpgradeVersion != trainerPackageIdentifier.BinaryVersion;
		}
		return false;
	}

	public bool Equals(PackageUpgradeVersions? other)
	{
		if (this != other)
		{
			if (other != null && EmbeddedReleaseId == other.EmbeddedReleaseId && LocalVersion == other.LocalVersion)
			{
				return UpgradeVersion == other.UpgradeVersion;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is PackageUpgradeVersions other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine<TrainerEmbeddedReleaseId, VersionRange>(EmbeddedReleaseId, VersionRange);
	}
}
