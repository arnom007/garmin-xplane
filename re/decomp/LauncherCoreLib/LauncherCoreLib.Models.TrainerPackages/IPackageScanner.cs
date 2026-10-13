using System.Threading.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IPackageScanner
{
	System.Threading.Tasks.Task<bool> CanUsePackage(ITrainerPackage package);
}
