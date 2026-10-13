using System.Threading.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class NoopScanner : IPackageScanner
{
	public System.Threading.Tasks.Task<bool> CanUsePackage(ITrainerPackage package)
	{
		return System.Threading.Tasks.Task.FromResult<bool>(true);
	}
}
