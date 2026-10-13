namespace RoarClient.Net.Native;

public class EndpointPortsResult
{
	public ROAR_err_type Error { get; }

	public ROAR_port_type Client { get; }

	public ROAR_port_type Master { get; }

	public ROAR_sock_type Protocol { get; }

	public EndpointPortsResult(ROAR_err_type error, ROAR_port_type client, ROAR_port_type master, ROAR_sock_type protocol)
	{
		Error = error;
		Client = client;
		Master = master;
		Protocol = protocol;
	}
}
