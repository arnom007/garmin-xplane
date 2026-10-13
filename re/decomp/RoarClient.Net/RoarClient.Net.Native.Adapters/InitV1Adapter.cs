using System;

namespace RoarClient.Net.Native.Adapters;

public class InitV1Adapter : IClientInitAdapter
{
	private readonly InitV1 _library;

	public InitV1Adapter(InitV1 library)
	{
		_library = library ?? throw new ArgumentNullException("library");
	}

	public void CloseClient()
	{
		_library.ROAR_client_close();
	}

	public ROAR_err_type InitClient(ROAR_data_rx_callback? rxCallback)
	{
		return _library.ROAR_client_init(rxCallback);
	}
}
