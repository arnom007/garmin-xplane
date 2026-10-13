using System;
using System.Runtime.InteropServices;

namespace RoarClient.Net.Native.Interfaces;

public static class EndpointIoV1
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_hndl ROAR_endpt_open([MarshalAs(UnmanagedType.LPStr)] string prj_name, int str_sz, ROAR_port_type growl_port);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_endpt_read(ROAR_hndl endpt_hndl, IntPtr buffer, int buff_sz, int tm_out_ms, ref ROAR_client_io_type pkt);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_endpt_write(ROAR_hndl endpt_hndl, ROAR_client_io_type pkt);
}
