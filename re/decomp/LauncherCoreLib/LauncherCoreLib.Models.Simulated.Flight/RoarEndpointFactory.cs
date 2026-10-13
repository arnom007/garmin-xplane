using System;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class RoarEndpointFactory : IRouterEndpointFactory
{
	private readonly IRoarClientProvider _clientProvider;

	public RoarEndpointFactory(IRoarClientProvider clientProvider)
	{
		_clientProvider = clientProvider;
	}

	public IRoarEndpointInternal OpenEndpoint(string endpointName, System.IDisposable cleanup)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(endpointName))
		{
			throw new ArgumentNullException("endpointName");
		}
		if (cleanup == null)
		{
			throw new ArgumentNullException("cleanup");
		}
		return new RoarEndpoint(_clientProvider.Roar.OpenEndpoint(endpointName), cleanup);
	}
}
