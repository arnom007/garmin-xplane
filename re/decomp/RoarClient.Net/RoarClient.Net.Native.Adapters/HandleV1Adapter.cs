using System;

namespace RoarClient.Net.Native.Adapters;

public class HandleV1Adapter : INativeHandleAdapter
{
	private readonly HandlesV1 _handles;

	public HandleV1Adapter(HandlesV1 handles)
	{
		_handles = handles ?? throw new ArgumentNullException("handles");
	}

	public ROAR_err_type CloseHandle(ROAR_hndl hndl)
	{
		return _handles.ROAR_hndl_close(hndl);
	}

	public ROAR_err_type ConvertHandleToError(ROAR_hndl hndl)
	{
		return _handles.ROAR_hndl_is_err(hndl);
	}
}
