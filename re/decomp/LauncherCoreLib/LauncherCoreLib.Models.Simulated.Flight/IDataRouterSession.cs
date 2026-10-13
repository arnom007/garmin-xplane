using System;
using LOLA.Processes;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IDataRouterSession : System.IDisposable
{
	IProcessSession RouterProcess { get; }

	IRoarRouteCollection RouteCollection { get; }

	IRouterCommandConnection CreateCommandConnection();

	IRoarEndpoint GetOrOpenEndpoint(string endpointName);
}
