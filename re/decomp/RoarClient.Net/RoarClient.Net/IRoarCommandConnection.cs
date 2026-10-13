using System;

namespace RoarClient.Net;

public interface IRoarCommandConnection : IDisposable
{
	bool IsOpen { get; }

	bool Open();

	void Close();

	RoarUdpEndpoint GetOrAddUdpEndpoint(string ep_name, ushort client_port = 0, ushort master_port = 0);

	IRoarResult AddRoute(RoarRoute route);

	void StartRouting();

	void StopRouting();
}
