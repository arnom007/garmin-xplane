using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LauncherCoreLib.Utility.Ascii;

public static class AsciiStringData
{
	public static string ToAsciiString(this byte[] data)
	{
		return Encoding.UTF8.GetString(data, 0, data.Length).TrimEnd('\0');
	}

	public static byte[] PadToLength(this System.Collections.Generic.IEnumerable<byte> data, int length)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		byte[] array = Enumerable.ToArray<byte>(data);
		if (array.Length > length)
		{
			throw new InvalidOperationException("cannot pad array of length " + array.Length + " to " + length);
		}
		return Enumerable.ToArray<byte>(Enumerable.Concat<byte>(data, (System.Collections.Generic.IEnumerable<byte>)new byte[length - array.Length]));
	}

	public static byte[] ToAsciiArray(this string str)
	{
		return Encoding.UTF8.GetBytes(str);
	}
}
