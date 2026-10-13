using System;
using System.Reactive.Linq;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class AllTrainerDirectoryProviders : ITrainerPackageDirectoryProvider
{
	private readonly LegacyPackageDirectoryProvider _legacy;

	private readonly SuppressedGtiDirectoryProvider _gti;

	private readonly CatalogRefugeDirectoryProvider _refuge;

	public AllTrainerDirectoryProviders(LegacyPackageDirectoryProvider legacy, SuppressedGtiDirectoryProvider gti, CatalogRefugeDirectoryProvider refuge)
	{
		_legacy = legacy;
		_gti = gti;
		_refuge = refuge;
	}

	public IObservable<PotentialPackageDirectory> SearchDirectories()
	{
		return _legacy.SearchDirectories().Merge<PotentialPackageDirectory>(_refuge.SearchDirectories()).Merge<PotentialPackageDirectory>(_gti.SearchDirectories());
	}
}
