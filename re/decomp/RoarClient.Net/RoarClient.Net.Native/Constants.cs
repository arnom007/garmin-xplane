using System.Runtime.InteropServices;

namespace RoarClient.Net.Native;

public static class Constants
{
	public const CallingConvention ROAR_API_CALL = CallingConvention.Cdecl;

	public const bool ROAR_API_SET_LAST_ERROR = false;

	public const CharSet ROAR_API_CHARSET = CharSet.Ansi;
}
