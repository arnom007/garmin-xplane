using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using LOLA.Extensions;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public sealed class PackageInstallationInfo : IEquatable<PackageInstallationInfo>
{
	private readonly ITrainerPackage[] _packagesInPriorityOrder;

	[field: CompilerGenerated]
	public TrainerEmbeddedReleaseId EmbeddedReleaseId
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ITrainerPackage? CatalogedPackage
	{
		[CompilerGenerated]
		get;
	}

	public ITrainerPackage CatalogedPackageChecked => CatalogedPackage ?? CreateNullException("CatalogedPackage");

	[field: CompilerGenerated]
	public ITrainerPackage? InstalledPackage
	{
		[CompilerGenerated]
		get;
	}

	public ITrainerPackage InstalledPackageChecked => InstalledPackage ?? CreateNullException("InstalledPackage");

	[field: CompilerGenerated]
	public ITrainerInstallerListingInfo LatestListingInfo
	{
		[CompilerGenerated]
		get;
	}

	public bool IsCataloged => CatalogedPackage != null;

	[field: CompilerGenerated]
	public MissingCatalogReason CatalogMissingReason
	{
		[CompilerGenerated]
		get;
	}

	public bool IsInstalled => InstalledPackage != null;

	public Version? CatalogVersion
	{
		get
		{
			if (!IsCataloged)
			{
				return null;
			}
			return CatalogedPackage.TrainerPackageId.BinaryVersion;
		}
	}

	public Version? InstalledVersion
	{
		get
		{
			if (!IsInstalled)
			{
				return null;
			}
			return InstalledPackage.TrainerPackageId.BinaryVersion;
		}
	}

	public bool IsUpdateAvailable
	{
		get
		{
			if (IsCataloged && IsInstalled)
			{
				return CatalogedPackage.TrainerPackageId.IsHigherBinaryVersionThan(InstalledPackage.TrainerPackageId);
			}
			return false;
		}
	}

	public bool IsRelevantToUser
	{
		get
		{
			if (!IsInstalled)
			{
				if (IsCataloged)
				{
					return !LatestListingInfo.IsDiscontinued;
				}
				return false;
			}
			return true;
		}
	}

	[field: CompilerGenerated]
	public ITrainerPackage? PrimaryPackage
	{
		[CompilerGenerated]
		get;
	}

	public ITrainerPackage PrimaryPackageChecked => PrimaryPackage ?? CreateNullException("PrimaryPackage");

	[field: CompilerGenerated]
	public ITrainerPackage? NewestPackage
	{
		[CompilerGenerated]
		get;
	}

	public ITrainerPackage NewestPackageChecked => NewestPackage ?? CreateNullException("NewestPackage");

	public System.Collections.Generic.IReadOnlyCollection<ITrainerPackage> Packages => (System.Collections.Generic.IReadOnlyCollection<ITrainerPackage>)(object)_packagesInPriorityOrder;

	public PackageInstallationInfo(TrainerEmbeddedReleaseId embeddedReleaseId, MissingCatalogReason missingCatalogReason, ITrainerInstallerListingInfo latestListingInfo)
		: this(embeddedReleaseId, System.Array.Empty<ITrainerPackage>(), missingCatalogReason, latestListingInfo)
	{
	}

	public PackageInstallationInfo(ITrainerPackage installed, MissingCatalogReason missingCatalogReason, ITrainerInstallerListingInfo latestListingInfo)
		: this(null, installed, missingCatalogReason, latestListingInfo)
	{
	}

	public PackageInstallationInfo(System.Collections.Generic.IReadOnlyCollection<ITrainerPackage> packages, MissingCatalogReason missingCatalogReason)
		: this(packages, missingCatalogReason, new NotListedInfo())
	{
	}

	public PackageInstallationInfo(System.Collections.Generic.IReadOnlyCollection<ITrainerPackage> packages, MissingCatalogReason missingCatalogReason, ITrainerInstallerListingInfo latestListingInfo)
		: this(GetEmbeddedReleaseFrom((System.Collections.Generic.IEnumerable<ITrainerPackage?>)packages) ?? throw new ArgumentException("Must provide at least one package."), (System.Collections.Generic.IEnumerable<ITrainerPackage?>)packages, missingCatalogReason, latestListingInfo)
	{
	}//IL_0010: Unknown result type (might be due to invalid IL or missing references)


	public PackageInstallationInfo(ITrainerPackage? catalog, ITrainerPackage? installed, MissingCatalogReason missingCatalogReason = MissingCatalogReason.NotMissing, ITrainerInstallerListingInfo? latestInstallerInfo = null)
		: this(GetEmbeddedReleaseFrom(new ITrainerPackage[2] { catalog, installed }) ?? throw new ArgumentException("Must provide at least one package."), new ITrainerPackage[2] { catalog, installed }, missingCatalogReason, latestInstallerInfo ?? new NotListedInfo())
	{
	}//IL_001d: Unknown result type (might be due to invalid IL or missing references)


	private PackageInstallationInfo(TrainerEmbeddedReleaseId embeddedReleaseId, System.Collections.Generic.IEnumerable<ITrainerPackage?> packages, MissingCatalogReason missingCatalogReason, ITrainerInstallerListingInfo latestListingInfo)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		if ((object)embeddedReleaseId == null)
		{
			throw new ArgumentNullException("embeddedReleaseId");
		}
		List<ITrainerPackage> val = Enumerable.ToList<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)Enumerable.OrderByDescending<ITrainerPackage, Version>(packages.WhereClassNotNull(), (Func<ITrainerPackage, Version>)((ITrainerPackage p) => p.BaseInfo.BinaryVersion)));
		if (Enumerable.Count<TrainerEmbeddedReleaseId>(Enumerable.Distinct<TrainerEmbeddedReleaseId>(Enumerable.Concat<TrainerEmbeddedReleaseId>((System.Collections.Generic.IEnumerable<TrainerEmbeddedReleaseId>)new TrainerEmbeddedReleaseId[1] { embeddedReleaseId }, Enumerable.Select<ITrainerPackage, TrainerEmbeddedReleaseId>((System.Collections.Generic.IEnumerable<ITrainerPackage>)val, (Func<ITrainerPackage, TrainerEmbeddedReleaseId>)((ITrainerPackage p) => p.TrainerPackageId.EmbeddedReleaseCompositeId))).WhereNotNull())) != 1)
		{
			throw new ArgumentException("Must be the same embedded release lines.");
		}
		EmbeddedReleaseId = embeddedReleaseId;
		List<ITrainerPackage> val2 = Enumerable.ToList<ITrainerPackage>(Enumerable.Where<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)val, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => p.IsACatalogPackage())));
		ITrainerPackage trainerPackage = (CatalogedPackage = Enumerable.FirstOrDefault<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)val2));
		InstalledPackage = Enumerable.SingleOrDefault<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)val, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => !p.IsACatalogPackage()));
		_packagesInPriorityOrder = Enumerable.ToArray<ITrainerPackage>(Enumerable.Concat<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)new ITrainerPackage[1] { InstalledPackage }, (System.Collections.Generic.IEnumerable<ITrainerPackage>)val2).WhereClassNotNull());
		PrimaryPackage = Enumerable.FirstOrDefault<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)_packagesInPriorityOrder);
		NewestPackage = Enumerable.MaxBy<ITrainerPackage, Version>(new ITrainerPackage[2] { InstalledPackage, CatalogedPackage }.WhereClassNotNull(), (Func<ITrainerPackage, Version>)((ITrainerPackage p) => p.TrainerPackageId.BinaryVersion));
		if (trainerPackage == null)
		{
			if (missingCatalogReason == MissingCatalogReason.NotMissing)
			{
				throw new ArgumentException("Must provide a reason for the missing catalog package.");
			}
			CatalogMissingReason = missingCatalogReason;
		}
		LatestListingInfo = latestListingInfo;
	}

	public ITrainerPackage? GetCatalogedPackageByVersionOrNull(Version binaryVersion)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		System.Collections.Generic.IList<ITrainerPackage> list = _packagesInPriorityOrder;
		if (IsInstalled)
		{
			list = (System.Collections.Generic.IList<ITrainerPackage>)(object)new ArraySegment<ITrainerPackage>(_packagesInPriorityOrder, 1, _packagesInPriorityOrder.Length - 1);
		}
		int num = list.BinarySearchIndexOfBy<ITrainerPackage, Version, Version>(binaryVersion, (ITrainerPackage p) => p.TrainerPackageId.BinaryVersion, (ListSortDirection)1, (Version p) => p);
		if (num < 0)
		{
			return null;
		}
		return list[num];
	}

	public ITrainerPackage? GetPrimaryPackageByVersionOrNull(Version binaryVersion)
	{
		if (InstalledPackage?.TrainerPackageId.BinaryVersion == binaryVersion)
		{
			return InstalledPackage;
		}
		return GetCatalogedPackageByVersionOrNull(binaryVersion);
	}

	public bool Equals(PackageInstallationInfo? other)
	{
		if (this != other)
		{
			if (other != null && EmbeddedReleaseId == other.EmbeddedReleaseId && CatalogMissingReason == other.CatalogMissingReason)
			{
				return Enumerable.SequenceEqual<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)_packagesInPriorityOrder, (System.Collections.Generic.IEnumerable<ITrainerPackage>)other._packagesInPriorityOrder, (IEqualityComparer<ITrainerPackage>)(object)EqualityComparer<ITrainerPackage>.Default);
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is PackageInstallationInfo other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)EmbeddedReleaseId).GetHashCode();
	}

	public override string ToString()
	{
		return ((object)EmbeddedReleaseId).ToString();
	}

	private static TrainerEmbeddedReleaseId? GetEmbeddedReleaseFrom(System.Collections.Generic.IEnumerable<ITrainerPackage?> packages)
	{
		return Enumerable.FirstOrDefault<TrainerEmbeddedReleaseId>(Enumerable.Select<ITrainerPackage, TrainerEmbeddedReleaseId>(packages.WhereClassNotNull(), (Func<ITrainerPackage, TrainerEmbeddedReleaseId>)((ITrainerPackage p) => p.TrainerPackageId.EmbeddedReleaseCompositeId)));
	}

	private static ITrainerPackage CreateNullException(string memberName)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		throw new InvalidOperationException("The " + memberName + " is null.");
	}
}
