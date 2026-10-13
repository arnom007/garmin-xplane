using System;

namespace RoarClient.Net;

public interface IEndpointRx
{
	event EventHandler<RoarMessageEventArgs> MessageReceived;
}
