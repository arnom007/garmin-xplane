using LauncherCoreLib.Models.TrainerPackages;
using LauncherCoreLib.Utility;
using TrainerPackageLib;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models;

public class StartDownloadIfNetworkAvailableBehavior
{
	private readonly INetworkDetector _networkDetector;

	private readonly GrantAndStartPackageInstallBehavior _grantAndStartPackageInstallBehavior;

	public StartDownloadIfNetworkAvailableBehavior(INetworkDetector networkDetector, GrantAndStartPackageInstallBehavior grantAndStartPackageInstallBehavior)
	{
		_networkDetector = networkDetector;
		_grantAndStartPackageInstallBehavior = grantAndStartPackageInstallBehavior;
	}

	public void StartInstallIfOnline(ITrainerPackage package)
	{
		if (_networkDetector.IsNetworkAvailable)
		{
			_grantAndStartPackageInstallBehavior.GrantAndStartPackageDownload(package);
		}
	}
}
