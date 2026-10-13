using System;
using System.Runtime.InteropServices;

namespace RoarClient.Net.Native;

public struct ROAR_client_io_type
{
	private class ROAR_pkt_cleanup : IDisposable
	{
		private GCHandle _handle;

		public ROAR_pkt_cleanup(GCHandle handle)
		{
			_handle = handle;
		}

		public void Dispose()
		{
			if (_handle.IsAllocated)
			{
				_handle.Free();
			}
		}
	}

	public const int MAX_PACKET_SIZE = 35000;

	private ROAR_prtcl_fmt_type _format;

	private ROAR_rte_chnl_type _src_chnl;

	private ushort _size;

	private IntPtr _payload;

	public ROAR_prtcl_fmt_type Format
	{
		get
		{
			return _format;
		}
		set
		{
			_format = value;
		}
	}

	public ROAR_rte_chnl_type Channel
	{
		get
		{
			return _src_chnl;
		}
		set
		{
			_src_chnl = value;
		}
	}

	public ushort Size => _size;

	public void CopyToArray(byte[] data, int offset)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset", "Must be positive");
		}
		if (offset > data.Length)
		{
			throw new ArgumentOutOfRangeException("offset", "Offset is outside of array.");
		}
		if (_payload != IntPtr.Zero && _size != 0)
		{
			Marshal.Copy(_payload, data, offset, Math.Min(data.Length - offset, _size));
		}
	}

	public IDisposable PinArray(byte[] payload)
	{
		if (payload == null)
		{
			throw new ArgumentNullException("payload");
		}
		if (payload.Length > 35000)
		{
			throw new InvalidOperationException("Max packet length is " + 35000 + ".");
		}
		GCHandle handle = GCHandle.Alloc(payload, GCHandleType.Pinned);
		_payload = handle.AddrOfPinnedObject();
		_size = (ushort)payload.Length;
		return new ROAR_pkt_cleanup(handle);
	}
}
