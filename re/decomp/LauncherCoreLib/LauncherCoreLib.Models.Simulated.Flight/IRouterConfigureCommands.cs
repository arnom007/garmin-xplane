using System.Collections.Generic;
using RoarClient.Net;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IRouterConfigureCommands
{
	System.Collections.Generic.IEnumerable<RoarUdpEndpoint> KnownEndpoints { get; }

	bool IsEndpointNameTaken(string endpointName);

	RoarUdpEndpoint GetOrCreateUdpEndpoint(string endpointName);

	bool AddRoute(ROAR_prtcl_fmt_type format, RoutePoint source, RoutePoint destination, bool createReverseRoute = false);

	bool StartRouting();
}
