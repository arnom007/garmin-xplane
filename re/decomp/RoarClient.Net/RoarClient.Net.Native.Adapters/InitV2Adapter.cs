using System;

namespace RoarClient.Net.Native.Adapters;

public class InitV2Adapter : IClientInitAdapter
{
	private readonly InitV2 _library;

	public InitV2Adapter(InitV2 library)
	{
		_library = library ?? throw new ArgumentNullException("library");
	}

	public void CloseClient()
	{
		_library.ROAR_client_close();
	}

	public ROAR_err_type InitClient(ROAR_data_rx_callback? rxCallback)
	{
		if (rxCallback == null)
		{
			return _library.ROAR_client_init();
		}
		return _library.ROAR_client_init_callback(rxCallback);
	}
}
