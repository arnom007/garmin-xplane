using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace LauncherSimCommandProtocol;

public class LscpCommandResponse
{
	private const int MIN_RESPONSE_SIZE = 8;

	private readonly byte[] _payload;

	[field: CompilerGenerated]
	public SimCommandType ResponseTo
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public LscpCommandError Error
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

	public bool IsSuccess => Error == LscpCommandError.sim_LSCP_ERR_NONE;

	private LscpCommandResponse(SimCommandType responseTo, LscpCommandError error, byte[] payload)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		ResponseTo = responseTo;
		Error = error;
		_payload = payload ?? throw new ArgumentNullException("payload");
		PayloadSize = Convert.ToUInt16(_payload.Length);
	}

	public static LscpCommandResponse Create(SimCommandType responseTo, LscpCommandError error, byte[] payload)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (payload != null && payload.Length > 65535)
		{
			throw new ArgumentException("Payload size is too big", "payload");
		}
		return new LscpCommandResponse(responseTo, error, payload ?? System.Array.Empty<byte>());
	}

	public static bool TryParseForCommand(MemoryStream stream, SimCommandType responseFor, out LscpCommandResponse response)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (((Stream)stream).Length < 8)
		{
			throw new ArgumentException("Stream is too short.", "response");
		}
		try
		{
			BinaryReader val = new BinaryReader((Stream)(object)stream);
			try
			{
				SimCommandType simCommandType = (SimCommandType)val.ReadInt16();
				if (simCommandType != responseFor)
				{
					response = null;
					return false;
				}
				LscpCommandError error = (LscpCommandError)val.ReadInt16();
				val.ReadUInt16();
				ushort num = val.ReadUInt16();
				byte[] payload = System.Array.Empty<byte>();
				if (num > 0)
				{
					payload = val.ReadBytes((int)num);
				}
				response = new LscpCommandResponse(simCommandType, error, payload);
				return true;
			}
			finally
			{
				((System.IDisposable)val)?.Dispose();
			}
		}
		catch (EndOfStreamException)
		{
			response = null;
			return false;
		}
	}

	public static bool TryParseForCommand(byte[] bytes, SimCommandType responseFor, out LscpCommandResponse response)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		if (bytes.Length < 8)
		{
			throw new ArgumentException("Not enough bytes.", "bytes");
		}
		MemoryStream val = new MemoryStream(bytes);
		try
		{
			return TryParseForCommand(val, responseFor, out response);
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
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
				val2.Write((ushort)ResponseTo);
				val2.Write((ushort)Error);
				val2.Write((ushort)0);
				val2.Write(PayloadSize);
				if (_payload != null)
				{
					val2.Write(_payload);
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
