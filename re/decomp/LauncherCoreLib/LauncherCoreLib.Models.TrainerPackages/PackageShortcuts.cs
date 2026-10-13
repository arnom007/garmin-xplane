using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public static class PackageShortcuts
{
	public static ITrainerUiAliasProvider GetUiAliasProvider(ITrainerPackage package)
	{
		return package.GetUiAliases();
	}
}
