using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherSimCommandProtocol;

public interface IPingSimReadyForLscp
{
	System.Threading.Tasks.Task<LscpPingResult> PingSim(ITrainingSession trainingSession, SimInstallKey installKey, IScheduler scheduler, TimeSpan timeout, CancellationToken token);
}
