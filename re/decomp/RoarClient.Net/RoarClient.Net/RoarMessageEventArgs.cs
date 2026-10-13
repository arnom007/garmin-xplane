using System;
using System.IO;
using System.Linq;
using RoarClient.Net.Native;

namespace RoarClient.Net;

public class RoarMessageEventArgs : EventArgs, IEquatable<RoarMessageEventArgs>
{
	private readonly byte[] _payload;

	public ROAR_hndl Handle { get; }

	public ROAR_rte_chnl_type Channel { get; }

	public ROAR_prtcl_fmt_type Format { get; }

	[Obsolete("Use OpenPayload instead.")]
	public byte[] Payload => _payload;

	public RoarMessageEventArgs(ROAR_hndl hndl, ROAR_client_io_type pkt)
	{
		Handle = hndl;
		Channel = pkt.Channel;
		Format = pkt.Format;
		_payload = new byte[pkt.Size];
		pkt.CopyToArray(_payload, 0);
	}

	public RoarMessageEventArgs(ROAR_hndl hndl, ROAR_prtcl_fmt_type format, ROAR_rte_chnl_type channel, byte[] payload)
	{
		Handle = hndl;
		Channel = channel;
		Format = format;
		_payload = payload ?? Array.Empty<byte>();
	}

	public MemoryStream OpenPayload()
	{
		return new MemoryStream(_payload, writable: false);
	}

	public bool Equals(RoarMessageEventArgs other)
	{
		if (this != other)
		{
			if (other != null && Handle == other.Handle && Format == other.Format && Channel == other.Channel)
			{
				if (_payload != other._payload)
				{
					return _payload.SequenceEqual(other._payload);
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		if (obj is RoarMessageEventArgs other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Handle.GetHashCode() ^ Channel.GetHashCode() ^ Format.GetHashCode();
	}

	internal byte[] GetPayloadForPinning()
	{
		return _payload;
	}
}
