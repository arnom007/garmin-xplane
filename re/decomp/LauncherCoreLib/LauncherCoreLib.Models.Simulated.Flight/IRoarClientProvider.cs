using RoarClient.Net;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IRoarClientProvider
{
	IRoarClient Roar { get; }
}
