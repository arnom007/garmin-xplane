using System;
using System.Runtime.InteropServices;

namespace RoarClient.Net.Native.Adapters;

public class EndpointIoV1Adapter : HandleV1Adapter, IEndpointIoAdapter, INativeHandleAdapter
{
	private readonly EndpointIoV1 _endpoints;

	public EndpointIoV1Adapter(EndpointIoV1 endpoints, HandlesV1 handles)
		: base(handles)
	{
		_endpoints = endpoints ?? throw new ArgumentNullException("endpoints");
	}

	public ROAR_hndl OpenEndpoint(string endpointName)
	{
		return _endpoints.ROAR_endpt_open(endpointName, (endpointName ?? throw new ArgumentNullException("endpointName")).Length, (ushort)0);
	}

	public ROAR_err_type ReadFromEndpoint(ROAR_hndl hndl, out RoarMessageEventArgs? message)
	{
		byte[] array = new byte[35000];
		GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
		try
		{
			IntPtr buffer = gCHandle.AddrOfPinnedObject();
			ushort buff_sz = (ushort)array.Length;
			ROAR_client_io_type pkt = default(ROAR_client_io_type);
			ROAR_err_type num = _endpoints.ROAR_endpt_read(hndl, buffer, buff_sz, 0, ref pkt);
			if (num > ROAR_err_type.ROAR_ERR_NO_ERR)
			{
				message = new RoarMessageEventArgs(hndl, pkt);
			}
			else
			{
				message = null;
			}
			return num;
		}
		finally
		{
			gCHandle.Free();
		}
	}

	public ROAR_err_type WriteToEndpoint(RoarMessageEventArgs message)
	{
		if (message == null)
		{
			throw new ArgumentNullException("message");
		}
		ROAR_client_io_type pkt = default(ROAR_client_io_type);
		using (pkt.PinArray(message.GetPayloadForPinning()))
		{
			pkt.Channel = message.Channel;
			pkt.Format = message.Format;
			return _endpoints.ROAR_endpt_write(message.Handle, pkt);
		}
	}
}
