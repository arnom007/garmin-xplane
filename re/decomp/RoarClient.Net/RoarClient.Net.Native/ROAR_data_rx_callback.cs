using System.Runtime.InteropServices;

namespace RoarClient.Net.Native;

[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = false)]
public delegate void ROAR_data_rx_callback(ROAR_hndl endpoint, ref ROAR_client_io_type pkt);
