using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using LOLA.Extensions;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;

[DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
public class PackageReferenceUpgradeInfo
{
	[field: CompilerGenerated]
	public TrainerPackageIdentifier CurrentId
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public PackageInstallationInfo InstallInfo
	{
		[CompilerGenerated]
		get;
	}

	public bool HasNewestPackage => NewestPackage != null;

	public bool HasCurrentPackage => CurrentPackage != null;

	[field: CompilerGenerated]
	public ITrainerPackage? CurrentPackage
	{
		[CompilerGenerated]
		get;
	}

	public ITrainerPackage CurrentPackageChecked => CurrentPackage ?? throw new InvalidOperationException("Does not have a current package.");

	public TrainerPackageIdentifier? UpgradeId => NewestPackage?.TrainerPackageId;

	public ITrainerPackage? NewestPackage => InstallInfo.NewestPackage;

	public ITrainerPackage NewestPackageChecked => InstallInfo.NewestPackageChecked;

	public bool IsCurrentIdInstalled => InstallInfo.InstalledVersion == CurrentId.BinaryVersion;

	public bool VersionsAreEqual
	{
		get
		{
			if (HasNewestPackage)
			{
				return CurrentId.BinaryVersion == UpgradeId.BinaryVersion;
			}
			return false;
		}
	}

	public bool IsUpgrade
	{
		get
		{
			if (HasNewestPackage)
			{
				return UpgradeId?.IsBugFixUpdateOf(CurrentId) ?? false;
			}
			return false;
		}
	}

	public bool IsDowngrade
	{
		get
		{
			if (HasNewestPackage)
			{
				return NewestPackage.IsOlderThan(CurrentId.BinaryVersion);
			}
			return false;
		}
	}

	public bool IsActionable
	{
		get
		{
			if (IsUpgrade)
			{
				return !IsHopeless;
			}
			return false;
		}
	}

	public bool HasBreakingChanges
	{
		get
		{
			if (IsUpgrade)
			{
				return NewestPackage.ContainsBreakingChangesSince(CurrentId.BinaryVersion);
			}
			return false;
		}
	}

	public bool IsRequired
	{
		get
		{
			if (IsUpgrade)
			{
				if (InstallInfo.IsInstalled)
				{
					return !IsCurrentIdInstalled;
				}
				return true;
			}
			return false;
		}
	}

	public bool IsDiscontinuedAndNotInstalled => !HasNewestPackage;

	public bool ExactPackageCannotBeInstalled
	{
		get
		{
			if (!IsCurrentIdInstalled && IsUpgrade)
			{
				return !VersionsAreEqual;
			}
			return false;
		}
	}

	public bool RequiresUserAttention
	{
		get
		{
			if (!IsActionable)
			{
				return IsHopeless;
			}
			return true;
		}
	}

	public bool IsHopeless
	{
		get
		{
			if (!IsDowngrade)
			{
				return IsDiscontinuedAndNotInstalled;
			}
			return true;
		}
	}

	public bool IsDiscontinued => InstallInfo.LatestListingInfo.IsDiscontinued;

	public PackageReferenceUpgradeInfo(TrainerPackageIdentifier trainerPackageIdentifier, PackageInstallationInfo installInfo)
	{
		CurrentId = trainerPackageIdentifier;
		InstallInfo = installInfo;
		CurrentPackage = InstallInfo.GetPrimaryPackageByVersionOrNull(CurrentId.BinaryVersion);
	}

	public PackageUpgradeVersions ToUpgradeVersions()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		TrainerPackageIdentifier upgradeId = UpgradeId;
		if ((object)upgradeId == null)
		{
			throw new InvalidOperationException("This does not have an upgrade version.");
		}
		if (CurrentId.IsHigherBinaryVersionThan(upgradeId))
		{
			throw new InvalidOperationException("Downgrades are invalid.");
		}
		return new PackageUpgradeVersions(CurrentId.EmbeddedReleaseCompositeId, new VersionRange(CurrentId.BinaryVersion, upgradeId.BinaryVersion));
	}

	public virtual string ToString()
	{
		return GetDebuggerDisplay();
	}

	private string GetDebuggerDisplay()
	{
		return $"[{CurrentId.EmbeddedReleaseCompositeId}] {CurrentId.BinaryVersion} -> {UpgradeId?.BinaryVersion}";
	}
}
