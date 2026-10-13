namespace RoarClient.Net.Native.Adapters;

public interface IEndpointIoAdapter : INativeHandleAdapter
{
	ROAR_hndl OpenEndpoint(string endpointName);

	ROAR_err_type WriteToEndpoint(RoarMessageEventArgs message);

	ROAR_err_type ReadFromEndpoint(ROAR_hndl hndl, out RoarMessageEventArgs? message);
}
