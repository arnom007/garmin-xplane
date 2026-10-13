using System;
using System.CodeDom.Compiler;
using RoarClient.Net.Native.Interfaces;

namespace RoarClient.Net.Native;

[GeneratedCode("ClientInterfaces.tt", "0.1.0")]
public sealed class CommandConnectionV1 : INativeLibraryInterface
{
	public RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_open_cmd_conn ROAR_adv_open_cmd_conn { get; }

	public RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_resolve_endpt_ports ROAR_adv_resolve_endpt_ports { get; }

	public RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_add_udpendpoint ROAR_adv_add_udpendpoint { get; }

	public RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_get_cmd_conn_last_router_error ROAR_adv_get_cmd_conn_last_router_error { get; }

	public RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_get_endpoint_id ROAR_adv_get_endpoint_id { get; }

	public RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_add_route ROAR_adv_add_route { get; }

	public RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_start_routing ROAR_adv_start_routing { get; }

	public CommandConnectionV1(RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_open_cmd_conn ROAR_adv_open_cmd_conn, RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_resolve_endpt_ports ROAR_adv_resolve_endpt_ports, RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_add_udpendpoint ROAR_adv_add_udpendpoint, RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_get_cmd_conn_last_router_error ROAR_adv_get_cmd_conn_last_router_error, RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_get_endpoint_id ROAR_adv_get_endpoint_id, RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_add_route ROAR_adv_add_route, RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_start_routing ROAR_adv_start_routing)
	{
		this.ROAR_adv_open_cmd_conn = ROAR_adv_open_cmd_conn ?? throw new ArgumentNullException("ROAR_adv_open_cmd_conn");
		this.ROAR_adv_resolve_endpt_ports = ROAR_adv_resolve_endpt_ports ?? throw new ArgumentNullException("ROAR_adv_resolve_endpt_ports");
		this.ROAR_adv_add_udpendpoint = ROAR_adv_add_udpendpoint ?? throw new ArgumentNullException("ROAR_adv_add_udpendpoint");
		this.ROAR_adv_get_cmd_conn_last_router_error = ROAR_adv_get_cmd_conn_last_router_error ?? throw new ArgumentNullException("ROAR_adv_get_cmd_conn_last_router_error");
		this.ROAR_adv_get_endpoint_id = ROAR_adv_get_endpoint_id ?? throw new ArgumentNullException("ROAR_adv_get_endpoint_id");
		this.ROAR_adv_add_route = ROAR_adv_add_route ?? throw new ArgumentNullException("ROAR_adv_add_route");
		this.ROAR_adv_start_routing = ROAR_adv_start_routing ?? throw new ArgumentNullException("ROAR_adv_start_routing");
	}

	public static bool IsSupportedByLibrary(IDynamicNativeLibraryLoader library)
	{
		if (library.SupportsDelegate<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_open_cmd_conn>() && library.SupportsDelegate<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_resolve_endpt_ports>() && library.SupportsDelegate<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_add_udpendpoint>() && library.SupportsDelegate<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_get_cmd_conn_last_router_error>() && library.SupportsDelegate<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_get_endpoint_id>() && library.SupportsDelegate<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_add_route>())
		{
			return library.SupportsDelegate<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_start_routing>();
		}
		return false;
	}

	public static CommandConnectionV1 BindToLibrary(IDynamicNativeLibraryLoader library)
	{
		try
		{
			return new CommandConnectionV1(library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_open_cmd_conn>(), library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_resolve_endpt_ports>(), library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_add_udpendpoint>(), library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_get_cmd_conn_last_router_error>(), library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_get_endpoint_id>(), library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_add_route>(), library.GetFunctionDelegateByTypeName<RoarClient.Net.Native.Interfaces.CommandConnectionV1.ROAR_adv_start_routing>());
		}
		catch (EntryPointNotFoundException innerException)
		{
			throw new NotSupportedException("The native library does not have all of the functions for this interface.", innerException);
		}
	}
}
