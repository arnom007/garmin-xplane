using System.Collections.Generic;
using System.Threading.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;

public interface IPackageUpgradeDecisionRepo
{
	System.Threading.Tasks.Task<System.Collections.Generic.IList<UpgradeDecisionLog>> GetPriorUserUpgradeDecisions(System.Collections.Generic.IEnumerable<TrainerEmbeddedReleaseId> releases);

	System.Threading.Tasks.Task StoreUserUpgradeDecision(System.Collections.Generic.IEnumerable<PackageUpgradeVersions> versionsInDecision, UpgradeUserDecision selectedOption);
}
