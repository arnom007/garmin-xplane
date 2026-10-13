namespace RoarClient.Net;

public class RoarUdpEndpoint : RoarEndpoint
{
	public ushort ClientPort { get; }

	public ushort MasterPort { get; }

	public RoarUdpEndpoint(int id, string name, ushort client = 0, ushort master = 0)
		: base(id, name)
	{
		ClientPort = client;
		MasterPort = master;
	}
}
