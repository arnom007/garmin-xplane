using System.Collections.Generic;
using System.Threading.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IInstallerListingInfoProvider
{
	System.Threading.Tasks.Task<ITrainerInstallerListingInfo> GetInstallerListingForRelease(TrainerEmbeddedReleaseId releaseId, System.Collections.Generic.IEnumerable<ITrainerPackage> packageHints);
}
