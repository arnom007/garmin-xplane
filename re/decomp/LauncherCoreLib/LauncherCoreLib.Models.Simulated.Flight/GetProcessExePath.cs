using System.Threading;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

public delegate System.Threading.Tasks.Task<string?> GetProcessExePath(int processId, CancellationToken token);
