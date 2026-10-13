using System;
using DataRouterMatrix;
using LOLA.Processes;
using RoarClient.Net;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class DefaultDataRouterSessionFactory : IDataRouterSessionFactory
{
	private readonly ILauncherRoarClientFactory _clientFactory;

	public DefaultDataRouterSessionFactory(ILauncherRoarClientFactory launcherRoarClientFactory)
	{
		_clientFactory = launcherRoarClientFactory;
	}

	public IDataRouterSession CreateSession(IProcessSession routerProcess, DataRouterAssemblyInfo clientInfo)
	{
		IRoarClient roarClient = _clientFactory.Create(clientInfo);
		try
		{
			RoarRouterCommandConnectionFactory commandConnectionFactory = new RoarRouterCommandConnectionFactory(new RoarClientProvider(roarClient));
			RoarEndpointFactory endpointFactory = new RoarEndpointFactory(new RoarClientProvider(roarClient));
			return new RoarRouterSession(routerProcess, roarClient, commandConnectionFactory, endpointFactory);
		}
		catch
		{
			((System.IDisposable)roarClient).Dispose();
			throw;
		}
	}
}
