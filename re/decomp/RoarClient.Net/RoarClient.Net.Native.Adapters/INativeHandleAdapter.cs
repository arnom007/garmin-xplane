namespace RoarClient.Net.Native.Adapters;

public interface INativeHandleAdapter
{
	ROAR_err_type ConvertHandleToError(ROAR_hndl hndl);

	ROAR_err_type CloseHandle(ROAR_hndl hndl);
}
