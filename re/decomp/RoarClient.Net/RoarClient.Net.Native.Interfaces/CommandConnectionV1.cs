using System.Runtime.InteropServices;

namespace RoarClient.Net.Native.Interfaces;

public static class CommandConnectionV1
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_hndl ROAR_adv_open_cmd_conn();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_adv_resolve_endpt_ports(ROAR_hndl cmd_hndl, [MarshalAs(UnmanagedType.LPStr)] string prj_name, int str_sz, bool request_new_ports, ref ROAR_port_type bind_port, ref ROAR_port_type connect_port, ref ROAR_sock_type protocol);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_adv_add_udpendpoint(ROAR_hndl cmd_hndl, [MarshalAs(UnmanagedType.LPStr)] string ep_name, int str_sz, ROAR_port_type client_port, ROAR_port_type master_port);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_adv_get_cmd_conn_last_router_error(ROAR_hndl cmd_hndl);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_adv_get_endpoint_id(ROAR_hndl cmd_hndl, [MarshalAs(UnmanagedType.LPStr)] string ep_name, int ep_name_len);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_adv_add_route(ROAR_hndl cmd_hndl, ROAR_route_args.ROAR_route_args_struct src, ROAR_route_args.ROAR_route_args_struct dest, ROAR_prtcl_fmt_type fmt, byte two_way);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
	public delegate ROAR_err_type ROAR_adv_start_routing(ROAR_hndl cmd_hndl);
}
