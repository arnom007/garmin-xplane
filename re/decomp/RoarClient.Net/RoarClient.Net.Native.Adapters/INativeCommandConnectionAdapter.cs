namespace RoarClient.Net.Native.Adapters;

public interface INativeCommandConnectionAdapter : INativeHandleAdapter
{
	ROAR_hndl OpenCommandConnection();

	ROAR_err_type GetLastRouterResponseError(ROAR_hndl cmd_hndl);

	ROAR_err_type GetEndpointId(ROAR_hndl cmdHndl, string epName);

	ROAR_err_type AddUdpEndpoint(ROAR_hndl cmdHndl, string epName, ROAR_port_type clientPort, ROAR_port_type masterPort);

	EndpointPortsResult GetEndpointPorts(ROAR_hndl cmdHndl, string epName);

	ROAR_err_type AddRoute(ROAR_hndl cmdHndl, ROAR_route_args src, ROAR_route_args dest, ROAR_prtcl_fmt_type fmt, bool two_way);

	ROAR_err_type StartRouting(ROAR_hndl cmdHndl);

	ROAR_err_type StopRouting(ROAR_hndl cmdHndl);
}
