using System.Threading.Tasks;

namespace LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;

public interface IPackageReferenceFilter
{
	System.Threading.Tasks.Task<PackageReferenceUpgradeInfo?> RemoveUnsupportedPackages(PackageReferenceUpgradeInfo packageReference);
}
