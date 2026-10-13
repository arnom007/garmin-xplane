using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ReliableDirectMessageProtocol;

namespace LauncherSimCommandProtocol;

public static class LscpBufferParser
{
	public static System.Collections.Generic.IEnumerable<LauncherSimCommandPacket> GetPackets(RdmpPayload rdmpPayload)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetPackets(Enumerable.ToArray<byte>((System.Collections.Generic.IEnumerable<byte>)rdmpPayload.Data), rdmpPayload.RxAt);
	}

	public static System.Collections.Generic.IEnumerable<LauncherSimCommandPacket> GetPackets(byte[] bytes, DateTimeOffset receivedAt)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		byte[] bytes2 = bytes;
		if (bytes2 == null)
		{
			throw new ArgumentNullException("bytes");
		}
		return Generator();
		[IteratorStateMachine(typeof(_003C_003Ec__DisplayClass1_0._003C_003CGetPackets_003Eg__Generator_007C0_003Ed))]
		System.Collections.Generic.IEnumerable<LauncherSimCommandPacket> Generator()
		{
			MemoryStream stream = new MemoryStream(bytes2);
			try
			{
				BinaryReader reader = new BinaryReader((Stream)(object)stream);
				try
				{
					LauncherSimCommandPacket packet;
					while (LauncherSimCommandPacket.TryParsePacket(reader, receivedAt, out packet))
					{
						yield return packet;
					}
				}
				finally
				{
					((System.IDisposable)reader)?.Dispose();
				}
			}
			finally
			{
				((System.IDisposable)stream)?.Dispose();
			}
		}
	}
}
