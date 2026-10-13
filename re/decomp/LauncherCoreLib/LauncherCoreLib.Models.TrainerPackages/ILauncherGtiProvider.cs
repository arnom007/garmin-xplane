using System.Threading.Tasks;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface ILauncherGtiProvider
{
	System.Threading.Tasks.Task<ILauncherGtiRunner> GtiExe { get; }
}
