using System;

namespace LauncherCoreLib.Models.DeployedEnv;

public interface IRealityXpInstallationDetector
{
	IObservable<string> RealityXpInstallationSigns();
}
