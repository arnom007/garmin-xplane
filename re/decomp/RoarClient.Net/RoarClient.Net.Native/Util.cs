using System;

namespace RoarClient.Net.Native;

internal static class Util
{
	public static void CatchAll(Action nativeCallBackAction)
	{
		try
		{
			nativeCallBackAction();
		}
		catch
		{
		}
	}
}
