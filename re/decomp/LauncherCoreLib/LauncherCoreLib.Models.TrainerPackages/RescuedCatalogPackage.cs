using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class RescuedCatalogPackage : IRescuedCatalogPackage
{
	[field: CompilerGenerated]
	public string GtiName
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public Version GtiVersion
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ImmutableArray<Version> CatalogVersions
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public PotentialPackageDirectory RescuedDirectory
	{
		[CompilerGenerated]
		get;
	}

	public RescuedCatalogPackage(string gtiName, Version gtiVersion, System.Collections.Generic.IEnumerable<Version> catalogVersions, PotentialPackageDirectory packageDirectory)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(gtiName))
		{
			throw new ArgumentNullException("gtiName");
		}
		GtiName = gtiName;
		GtiVersion = gtiVersion ?? throw new ArgumentNullException("gtiVersion");
		CatalogVersions = ImmutableArray.ToImmutableArray<Version>((System.Collections.Generic.IEnumerable<Version>)Enumerable.OrderBy<Version, Version>(catalogVersions ?? throw new ArgumentNullException("catalogVersions"), (Func<Version, Version>)((Version v) => v)));
		if (CatalogVersions.Length == 0)
		{
			throw new ArgumentException("Must include at least 1 catalog version.");
		}
		RescuedDirectory = packageDirectory ?? throw new ArgumentNullException("packageDirectory");
		if (!RescuedDirectory.HasPotential)
		{
			throw new ArgumentException("Must have potential.");
		}
	}
}
