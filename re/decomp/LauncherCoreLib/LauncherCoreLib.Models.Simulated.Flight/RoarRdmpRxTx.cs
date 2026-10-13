using System;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ReliableDirectMessageProtocol;
using RoarClient.Net;
using RoarClient.Net.Native;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class RoarRdmpRxTx : IRdmpRxTx
{
	private readonly IRoarEndpoint _endpoint;

	private readonly ROAR_rte_chnl_type _channel;

	public RoarRdmpRxTx(IRoarEndpoint endpoint, ROAR_rte_chnl_type channel)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_endpoint = endpoint ?? throw new ArgumentNullException("endpoint");
		_channel = channel;
	}

	public System.Threading.Tasks.Task Tx(byte[] data)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		return _endpoint.Send(ROAR_prtcl_fmt_type.ROAR_SIM_GENERIC_FMT, _channel, data);
	}

	public System.Threading.Tasks.Task Tx(byte[] data, CancellationToken token)
	{
		return Tx(data);
	}

	public IObservable<MemoryStream> Rx()
	{
		return _endpoint.Messages().Where<RoarMessageEventArgs>((Func<RoarMessageEventArgs, bool>)([CompilerGenerated] (RoarMessageEventArgs eventArgs) => eventArgs.Channel == _channel && eventArgs.Format == ROAR_prtcl_fmt_type.ROAR_SIM_GENERIC_FMT)).Select<RoarMessageEventArgs, MemoryStream>((Func<RoarMessageEventArgs, MemoryStream>)((RoarMessageEventArgs eventArgs) => eventArgs.OpenPayload()));
	}
}
