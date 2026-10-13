namespace LauncherCoreLib.Models.Simulated.Flight;

public class RoarRouterCommandConnectionFactory : IRouterCommandConnectionFactory
{
	private readonly IRoarClientProvider _roarProvider;

	public RoarRouterCommandConnectionFactory(IRoarClientProvider clientProvider)
	{
		_roarProvider = clientProvider;
	}

	public IRouterCommandConnection CreateCommandConnection()
	{
		return new RoarCommandConnection(_roarProvider.Roar.CreateCommandConn());
	}
}
