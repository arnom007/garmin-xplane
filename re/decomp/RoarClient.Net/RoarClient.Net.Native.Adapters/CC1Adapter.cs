using System;

namespace RoarClient.Net.Native.Adapters;

internal class CC1Adapter : HandleV1Adapter, INativeCommandConnectionAdapter, INativeHandleAdapter
{
	private readonly CommandConnectionV1 _client;

	public CC1Adapter(CommandConnectionV1 client, HandlesV1 handles)
		: base(handles)
	{
		_client = client ?? throw new ArgumentNullException("client");
	}

	public ROAR_hndl OpenCommandConnection()
	{
		return _client.ROAR_adv_open_cmd_conn();
	}

	public ROAR_err_type GetLastRouterResponseError(ROAR_hndl cmd_hndl)
	{
		return _client.ROAR_adv_get_cmd_conn_last_router_error(cmd_hndl);
	}

	public ROAR_err_type GetEndpointId(ROAR_hndl cmdHndl, string epName)
	{
		return _client.ROAR_adv_get_endpoint_id(cmdHndl, epName, epName.Length);
	}

	public ROAR_err_type AddUdpEndpoint(ROAR_hndl cmdHndl, string epName, ROAR_port_type clientPort, ROAR_port_type masterPort)
	{
		return _client.ROAR_adv_add_udpendpoint(cmdHndl, epName, epName.Length, clientPort, masterPort);
	}

	public EndpointPortsResult GetEndpointPorts(ROAR_hndl cmdHndl, string epName)
	{
		ROAR_port_type bind_port = default(ROAR_port_type);
		ROAR_port_type connect_port = default(ROAR_port_type);
		ROAR_sock_type protocol = ROAR_sock_type.ROAR_SOCK_ERR;
		return new EndpointPortsResult(_client.ROAR_adv_resolve_endpt_ports(cmdHndl, epName, epName.Length, request_new_ports: false, ref bind_port, ref connect_port, ref protocol), bind_port, connect_port, protocol);
	}

	public ROAR_err_type AddRoute(ROAR_hndl cmdHndl, ROAR_route_args src, ROAR_route_args dest, ROAR_prtcl_fmt_type fmt, bool two_way)
	{
		return _client.ROAR_adv_add_route(cmdHndl, ThrowIfNull(src, "src").ConvertRouteArgsForInterop(), ThrowIfNull(dest, "dest").ConvertRouteArgsForInterop(), fmt, (byte)(two_way ? 1u : 0u));
		static ROAR_route_args ThrowIfNull(ROAR_route_args? args, string argName)
		{
			if (args == null)
			{
				throw new ArgumentNullException(argName);
			}
			return args;
		}
	}

	public ROAR_err_type StartRouting(ROAR_hndl cmd_hndl)
	{
		return _client.ROAR_adv_start_routing(cmd_hndl);
	}

	public ROAR_err_type StopRouting(ROAR_hndl cmd_hndl)
	{
		throw new NotSupportedException();
	}
}
