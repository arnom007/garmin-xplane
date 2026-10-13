using LauncherCoreLib.Models.Simulated.Flight.Serialized;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal interface ISessionRouteTableFactory
{
	IReadOnlyJsonDataSource<RoarUdpOnlyConfiguration> CreateRouteTable(ISessionEnvironment sessionEnvironment);
}
