using GTI.Net;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IBaseGtiRunnerProvider
{
	IGtiRunner UnprotectedGtiRunner { get; }
}
