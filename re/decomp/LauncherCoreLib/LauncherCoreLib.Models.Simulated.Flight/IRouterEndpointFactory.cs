using System;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IRouterEndpointFactory
{
	IRoarEndpointInternal OpenEndpoint(string endpointName, System.IDisposable cleanup);
}
