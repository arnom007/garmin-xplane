using System;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.Simulated.Panel;

public static class PowerUpStatsRepoExtensions
{
	public static IObservable<SimPowerStats> SimulatorPowerStatsOverTime(this IPowerStatsRepo repo, SimAircraftKey simulator)
	{
		IPowerStatsRepo repo2 = repo;
		SimAircraftKey simulator2 = simulator;
		return Observable.FromAsync<SimPowerStats>((Func<System.Threading.Tasks.Task<SimPowerStats>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass0_0._003C_003CSimulatorPowerStatsOverTime_003Eb__0_003Ed))] async () => await repo2.GetPowerStats(simulator2).DetachContext())).Concat<SimPowerStats>(repo2.WhenSimulatorPowerStatsChange(simulator2));
	}
}
