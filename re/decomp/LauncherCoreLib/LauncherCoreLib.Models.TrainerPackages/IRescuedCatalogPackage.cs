using System;
using System.Collections.Immutable;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IRescuedCatalogPackage
{
	string GtiName { get; }

	Version GtiVersion { get; }

	ImmutableArray<Version> CatalogVersions { get; }

	PotentialPackageDirectory RescuedDirectory { get; }
}
