using System.Threading;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.TrainerPackages;

public delegate System.Threading.Tasks.Task GtiUnavailableUiPresenter(CheckIfGtiIsAvailable checker, CancellationToken shutdown);
