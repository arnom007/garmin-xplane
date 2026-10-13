using System;
using RoarClient.Net.Native;

namespace RoarClient.Net;

public interface IRoarIOEndpoint : IDisposable
{
	event EventHandler<RoarMessageEventArgs> OnReceiveMessage;

	void WriteEndpoint(ROAR_prtcl_fmt_type format, ROAR_rte_chnl_type channel, byte[] payload);
}
