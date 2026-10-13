using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace LauncherCoreLib.Utility;

public class RetrieveStringFromExe : IRetrieveStringFromExe
{
	private const int ERROR_SUCCESS = 0;

	private const int ERROR_FILE_NOT_FOUND = 2;

	private const int ERROR_RESOURCE_TYPE_NOT_FOUND = 1813;

	private const int ERROR_RESOURCE_NAME_NOT_FOUND = 1814;

	private const int GET_STRING_LENGTH_SIZE_INDICATOR = 0;

	private const uint LOAD_LIBRARY_AS_DATAFILE = 2u;

	public unsafe string? GetString(string filePath, int id)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		System.IntPtr intPtr = LoadLibraryEx(filePath, System.IntPtr.Zero, 2u);
		if (intPtr == System.IntPtr.Zero)
		{
			return null;
		}
		try
		{
			System.IntPtr buffer;
			int num = LoadStringWViaReadOnlyPointer(intPtr, (uint)id, out buffer, 0);
			if (num == 0)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				return lastWin32Error switch
				{
					0 => string.Empty, 
					2 => null, 
					1813 => null, 
					1814 => null, 
					_ => throw new Win32Exception(lastWin32Error), 
				};
			}
			return ((object)new System.ReadOnlySpan<char>(buffer.ToPointer(), num)).ToString();
		}
		finally
		{
			FreeLibrary(intPtr);
		}
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	[DefaultDllImportSearchPaths(/*Could not decode attribute arguments.*/)]
	[return: MarshalAs(2)]
	private static extern bool FreeLibrary(System.IntPtr hModule);

	[DllImport("Kernel32.dll", CharSet = 3, SetLastError = true)]
	[DefaultDllImportSearchPaths(/*Could not decode attribute arguments.*/)]
	private static extern System.IntPtr LoadLibraryEx(string lpFileName, System.IntPtr hReservedNull, uint dwFlags);

	[DllImport("user32.dll", EntryPoint = "LoadStringW", SetLastError = true)]
	[DefaultDllImportSearchPaths(/*Could not decode attribute arguments.*/)]
	private static extern int LoadStringWViaReadOnlyPointer(System.IntPtr hInstance, uint uID, out System.IntPtr buffer, int nBufferMax);
}
