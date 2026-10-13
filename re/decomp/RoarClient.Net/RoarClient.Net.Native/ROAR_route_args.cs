using System;
using System.Runtime.InteropServices;

namespace RoarClient.Net.Native;

[StructLayout(LayoutKind.Sequential)]
public sealed class ROAR_route_args : IDisposable, IEquatable<ROAR_route_args>
{
	public struct ROAR_route_args_struct
	{
		public IntPtr ep_name;

		public int len;

		public ROAR_rte_chnl_type chnl;
	}

	public const int MAX_ENDPOINT_NAME_LENGTH = 30;

	private readonly string _ep_name;

	private readonly ROAR_rte_chnl_type _chnl;

	private IntPtr _ep_name_ptr;

	private readonly int _len;

	public string Endpoint => _ep_name;

	public ROAR_rte_chnl_type Channel => _chnl;

	public ROAR_route_args_struct ConvertRouteArgsForInterop()
	{
		if (_ep_name_ptr == IntPtr.Zero)
		{
			throw new ObjectDisposedException("ROAR_route_args");
		}
		ROAR_route_args_struct result = default(ROAR_route_args_struct);
		result.ep_name = _ep_name_ptr;
		result.len = _len;
		result.chnl = _chnl;
		return result;
	}

	public ROAR_route_args(string endpointName, int channel)
	{
		_ep_name = endpointName ?? string.Empty;
		_ep_name_ptr = Marshal.StringToHGlobalAnsi(_ep_name);
		_len = _ep_name.Length;
		_chnl = (ushort)channel;
	}

	public void Dispose()
	{
		FreeCurrent();
		GC.SuppressFinalize(this);
	}

	public bool Equals(ROAR_route_args other)
	{
		if (this != other)
		{
			if (other != null && _ep_name == other._ep_name)
			{
				return Channel == other.Channel;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		if (obj is ROAR_route_args other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _ep_name.GetHashCode() ^ _chnl.GetHashCode();
	}

	public override string ToString()
	{
		return $"{Endpoint}: {Channel}";
	}

	~ROAR_route_args()
	{
		FreeCurrent();
	}

	private void FreeCurrent()
	{
		if (_ep_name_ptr != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(_ep_name_ptr);
			_ep_name_ptr = IntPtr.Zero;
		}
	}
}
