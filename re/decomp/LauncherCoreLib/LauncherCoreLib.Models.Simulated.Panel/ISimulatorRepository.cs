using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Panel;

public interface ISimulatorRepository : IFlushable
{
	IObservable<DataChangeNotification<SimInstallKey>> WhenSimulatorIsChangedOnAircraft(AircraftIdentifier aircraftIdentifier);

	System.Threading.Tasks.Task<SimPanel> GetSimulatorPanel(AircraftIdentifier aircraftIdentifier);

	System.Threading.Tasks.Task UpdateSimulatorPanel(AircraftIdentifier aircraftIdentifier, SimPanel simulatorPanel);

	System.Threading.Tasks.Task<SimInstallData> GetSimulator(AircraftIdentifier aircraftIdentifier, SimInstallKey simulatorKey);

	System.Threading.Tasks.Task CreateOrUpdateSimulator(AircraftIdentifier aircraftIdentifier, SimInstallData simulator);

	System.Threading.Tasks.Task<System.Collections.Generic.IList<SimInstallKey>> GetAircraftWinfsArchives(AircraftIdentifier aircraftIdentifier, Dictionary<SimInstallKey, string> simWinfsArchiveOutputMap);

	System.Threading.Tasks.Task UpdateAircraftWinfsArchives(AircraftIdentifier aircraftIdentifier, Dictionary<SimInstallKey, string> simWinfsArchiveInputMap);

	System.Threading.Tasks.Task DeleteAircraftWinfsArchives(AircraftIdentifier aircraftIdentifier, System.Collections.Generic.IEnumerable<SimInstallKey> simsToClear);
}
