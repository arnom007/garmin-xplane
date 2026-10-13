using DataRouterMatrix;
using RoarClient.Net;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ILauncherRoarClientFactory
{
	IRoarClient Create(DataRouterAssemblyInfo clientAssemblyInfo);
}
