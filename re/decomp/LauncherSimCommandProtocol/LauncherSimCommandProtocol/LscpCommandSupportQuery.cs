using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace LauncherSimCommandProtocol;

public class LscpCommandSupportQuery
{
	private const int COMMAND_QUERY_SIZE = 4;

	[field: CompilerGenerated]
	public SimCommandType CommandToBeQueried
	{
		[CompilerGenerated]
		get;
	}

	public LscpCommandSupportQuery(SimCommandType commandType)
	{
		CommandToBeQueried = commandType;
	}

	public static bool TryParse(byte[] bytes, out LscpCommandSupportQuery commandQuerry)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		if (bytes.Length != 4)
		{
			commandQuerry = null;
			return false;
		}
		MemoryStream val = new MemoryStream(bytes);
		try
		{
			return TryParse(val, out commandQuerry);
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
	}

	public static bool TryParse(MemoryStream stream, out LscpCommandSupportQuery commandQuerry)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (((Stream)stream).Length != 4)
		{
			commandQuerry = null;
			return false;
		}
		BinaryReader val = new BinaryReader((Stream)(object)stream);
		try
		{
			commandQuerry = new LscpCommandSupportQuery((SimCommandType)val.ReadUInt16());
			return true;
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
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
				val2.Write((ushort)CommandToBeQueried);
				val2.Write((ushort)0);
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
