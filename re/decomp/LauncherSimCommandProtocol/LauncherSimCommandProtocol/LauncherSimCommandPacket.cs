using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ReliableDirectMessageProtocol;

namespace LauncherSimCommandProtocol;

public class LauncherSimCommandPacket
{
	private const int MIN_PACKET_SIZE = 4;

	private readonly byte[] _payload;

	[field: CompilerGenerated]
	public SimCommandType CommandType
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ushort PayloadSize
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public DateTimeOffset ReceivedAt
	{
		[CompilerGenerated]
		get;
	}

	private LauncherSimCommandPacket(SimCommandType requestType, byte[] payload, DateTimeOffset receivedAt)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		CommandType = requestType;
		_payload = payload;
		PayloadSize = Convert.ToUInt16(_payload.Length);
		ReceivedAt = receivedAt;
	}

	public static bool TryParsePacket(RdmpPayload rdmpPayload, out LauncherSimCommandPacket packet)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (rdmpPayload == null)
		{
			throw new ArgumentNullException("rdmpPayload");
		}
		if (TryParsePacket(Enumerable.ToArray<byte>((System.Collections.Generic.IEnumerable<byte>)rdmpPayload.Data), rdmpPayload.RxAt, out packet))
		{
			return true;
		}
		return false;
	}

	public static bool TryParsePacket(byte[] packetBytes, DateTimeOffset receivedAt, out LauncherSimCommandPacket packet)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (packetBytes == null)
		{
			throw new ArgumentNullException("packetBytes");
		}
		if (packetBytes.Length < 4)
		{
			packet = null;
			return false;
		}
		MemoryStream val = new MemoryStream(packetBytes);
		try
		{
			BinaryReader val2 = new BinaryReader((Stream)(object)val);
			try
			{
				return TryParsePacket(val2, receivedAt, out packet);
			}
			finally
			{
				((System.IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
	}

	public static bool TryParsePacket(BinaryReader reader, DateTimeOffset receivedAt, out LauncherSimCommandPacket packet)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		if (reader.BaseStream.Length - reader.BaseStream.Position < 4)
		{
			packet = null;
			return false;
		}
		try
		{
			ushort num = reader.ReadUInt16();
			if (!System.Enum.IsDefined(typeof(SimCommandType), (object)num))
			{
				packet = null;
				return false;
			}
			SimCommandType requestType = (SimCommandType)num;
			ushort num2 = reader.ReadUInt16();
			byte[] array = System.Array.Empty<byte>();
			if (num2 > 0)
			{
				if (reader.BaseStream.Length - reader.BaseStream.Position < num2)
				{
					packet = null;
					return false;
				}
				array = new byte[num2];
				if (reader.Read(array, 0, (int)num2) != num2)
				{
					packet = null;
					return false;
				}
			}
			else if (reader.BaseStream.Position < reader.BaseStream.Length)
			{
				packet = null;
				return false;
			}
			packet = new LauncherSimCommandPacket(requestType, array, receivedAt);
			return true;
		}
		catch (EndOfStreamException)
		{
		}
		packet = null;
		return false;
	}

	public static LauncherSimCommandPacket CreatePacket(SimCommandType requestType, byte[] payload)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (payload != null && payload.Length > 65535)
		{
			throw new ArgumentException("payload is too big. Size must fit in a ushort", "payload");
		}
		return new LauncherSimCommandPacket(requestType, payload ?? System.Array.Empty<byte>(), default(DateTimeOffset));
	}

	public MemoryStream OpenPayload()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		return new MemoryStream(_payload, false);
	}

	public byte[] ToBinary()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		MemoryStream val = new MemoryStream();
		try
		{
			BinaryWriter val2 = new BinaryWriter((Stream)(object)val);
			try
			{
				val2.Write((ushort)CommandType);
				if (_payload != null)
				{
					val2.Write(Convert.ToUInt16(_payload.Length));
					val2.Write(_payload);
				}
				else
				{
					val2.Write((ushort)0);
				}
				return val.ToArray();
			}
			finally
			{
				((System.IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
	}
}
