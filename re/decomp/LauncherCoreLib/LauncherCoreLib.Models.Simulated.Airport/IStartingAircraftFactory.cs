using System.Collections.Generic;

namespace LauncherCoreLib.Models.Simulated.Airport;

public interface IStartingAircraftFactory
{
	System.Collections.Generic.IList<AircraftListData> CreateStartingAircraftInfos();
}
