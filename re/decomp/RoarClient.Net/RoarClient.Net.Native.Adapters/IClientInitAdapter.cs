namespace RoarClient.Net.Native.Adapters;

public interface IClientInitAdapter
{
	ROAR_err_type InitClient(ROAR_data_rx_callback? rxCallback);

	void CloseClient();
}
