using System;

namespace RoarClient.Net.Native.Adapters;

public static class HandleExtensions
{
	public static ROAR_err_type CloseHandleSafe(this INativeHandleAdapter adapter, ROAR_hndl hndl, bool canThrow)
	{
		try
		{
			return adapter.ThrowIfNull().CloseHandle(hndl);
		}
		catch (ObjectDisposedException) when (!canThrow)
		{
			return ROAR_err_type.ROAR_ERR_INVLD_CALL;
		}
	}

	private static INativeHandleAdapter ThrowIfNull(this INativeHandleAdapter adapter)
	{
		return adapter ?? throw new ArgumentNullException("adapter");
	}
}
