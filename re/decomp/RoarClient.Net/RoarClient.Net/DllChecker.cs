using System;
using System.IO;

namespace RoarClient.Net;

public static class DllChecker
{
	public enum MachineType : ushort
	{
		IMAGE_FILE_MACHINE_UNKNOWN = 0,
		IMAGE_FILE_MACHINE_AM33 = 467,
		IMAGE_FILE_MACHINE_AMD64 = 34404,
		IMAGE_FILE_MACHINE_ARM = 448,
		IMAGE_FILE_MACHINE_EBC = 3772,
		IMAGE_FILE_MACHINE_I386 = 332,
		IMAGE_FILE_MACHINE_IA64 = 512,
		IMAGE_FILE_MACHINE_M32R = 36929,
		IMAGE_FILE_MACHINE_MIPS16 = 614,
		IMAGE_FILE_MACHINE_MIPSFPU = 870,
		IMAGE_FILE_MACHINE_MIPSFPU16 = 1126,
		IMAGE_FILE_MACHINE_POWERPC = 496,
		IMAGE_FILE_MACHINE_POWERPCFP = 497,
		IMAGE_FILE_MACHINE_R4000 = 358,
		IMAGE_FILE_MACHINE_SH3 = 418,
		IMAGE_FILE_MACHINE_SH3DSP = 419,
		IMAGE_FILE_MACHINE_SH4 = 422,
		IMAGE_FILE_MACHINE_SH5 = 424,
		IMAGE_FILE_MACHINE_THUMB = 450,
		IMAGE_FILE_MACHINE_WCEMIPSV2 = 361
	}

	public static MachineType GetDllMachineType(FileStream dllContents)
	{
		if (dllContents == null)
		{
			throw new ArgumentNullException("dllContents");
		}
		using BinaryReader binaryReader = new BinaryReader(dllContents);
		try
		{
			dllContents.Seek(60L, SeekOrigin.Begin);
			int num = binaryReader.ReadInt32();
			dllContents.Seek(num, SeekOrigin.Begin);
			if (binaryReader.ReadUInt32() != 17744)
			{
				throw new FormatException("Can't find PE header. Is this a native DLL?");
			}
			return (MachineType)binaryReader.ReadUInt16();
		}
		catch (EndOfStreamException innerException)
		{
			throw new FormatException("Can't find PE header. Is this a native DLL?", innerException);
		}
	}
}
