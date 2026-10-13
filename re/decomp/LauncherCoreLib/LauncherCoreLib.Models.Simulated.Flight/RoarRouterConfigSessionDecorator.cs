using System;
using System.Runtime.CompilerServices;
using LOLA.Processes;

namespace LauncherCoreLib.Models.Simulated.Flight;

public sealed class RoarRouterConfigSessionDecorator : IDataRouterSession, System.IDisposable
{
	private readonly IDataRouterSession _session;

	[field: CompilerGenerated]
	public IRoarRouteCollection RouteCollection
	{
		[CompilerGenerated]
		get;
	}

	public IProcessSession RouterProcess => _session.RouterProcess;

	public RoarRouterConfigSessionDecorator(IDataRouterSession session, IRoarRouteCollection routeCollection)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		_session = session ?? throw new ArgumentNullException("session");
		RouteCollection = routeCollection ?? throw new ArgumentNullException("routeCollection");
	}

	public IRouterCommandConnection CreateCommandConnection()
	{
		return _session.CreateCommandConnection();
	}

	public IRoarEndpoint GetOrOpenEndpoint(string endpointName)
	{
		return _session.GetOrOpenEndpoint(endpointName);
	}

	public void Dispose()
	{
		((System.IDisposable)_session).Dispose();
	}
}
