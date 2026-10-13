using System;

namespace RoarClient.Net;

public interface IRoarClient : IDisposable
{
	Version NativeClientVersion { get; }

	IRoarCommandConnection CreateCommandConn();

	IRoarIOEndpoint OpenEndpoint(string endpointName);

	IDisposable RegisterLogger(IRoarClientLogger logger);
}
