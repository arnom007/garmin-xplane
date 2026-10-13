using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RoarClient.Net;
using RoarClient.Net.Native;

namespace LauncherCoreLib.Models.Simulated.Flight;

public sealed class RoarEndpoint : IRoarEndpointInternal, IRoarEndpoint, System.IDisposable
{
	private readonly IRoarIOEndpoint _ioEndpoint;

	private readonly IObservable<RoarMessageEventArgs> _receivePayloadStream;

	private readonly System.IDisposable _cleanup;

	public RoarEndpoint(IRoarIOEndpoint ioEndpoint, System.IDisposable cleanup)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		_ioEndpoint = ioEndpoint ?? throw new ArgumentNullException("ioEndpoint");
		_receivePayloadStream = Observable.FromEventPattern<RoarMessageEventArgs>([CompilerGenerated] (EventHandler<RoarMessageEventArgs> m) =>
		{
			_ioEndpoint.OnReceiveMessage += m;
		}, [CompilerGenerated] (EventHandler<RoarMessageEventArgs> m) =>
		{
			_ioEndpoint.OnReceiveMessage -= m;
		}).Select<EventPattern<RoarMessageEventArgs>, RoarMessageEventArgs>((Func<EventPattern<RoarMessageEventArgs>, RoarMessageEventArgs>)((EventPattern<RoarMessageEventArgs> m) => m.EventArgs)).Publish<RoarMessageEventArgs>()
			.RefCount();
		_cleanup = cleanup ?? throw new ArgumentNullException("cleanup");
	}

	public System.Threading.Tasks.Task Send(ROAR_prtcl_fmt_type format, ROAR_rte_chnl_type channel, byte[] payload)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (payload == null)
		{
			throw new ArgumentNullException("payload");
		}
		if (payload.Length == 0)
		{
			throw new ArgumentException("Payload cannot be 0 bytes.", "payload");
		}
		_ioEndpoint.WriteEndpoint(format, channel, payload);
		return System.Threading.Tasks.Task.CompletedTask;
	}

	public IObservable<RoarMessageEventArgs> Messages()
	{
		return _receivePayloadStream;
	}

	public void Dispose()
	{
		((System.IDisposable)_ioEndpoint).Dispose();
		_cleanup.Dispose();
	}
}
