using System;
using System.Threading.Tasks;
using LauncherCoreLib.Models.Simulated.Airport;

namespace LauncherCoreLib.Models.Simulated.Panel;

public interface IPowerStatsRepo
{
	IObservable<SimPowerStats> WhenSimulatorPowerStatsChange(SimAircraftKey simulator);

	System.Threading.Tasks.Task<SimPowerStats> GetPowerStats(SimAircraftKey simulator);

	System.Threading.Tasks.Task IncrementPowerUpsForPanel(SimPanel panel);

	System.Threading.Tasks.Task ResetPowerUps(SimAircraftKey simulator);

	System.Threading.Tasks.Task ClearAllStatsForAircraft(AircraftIdentifier aircraft);
}
