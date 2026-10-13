using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace RoarClient.Net.Native;

public sealed class DynamicNativeDllLoader : IDynamicNativeLibraryLoader, IDisposable
{
	private IntPtr _libHandle;

	private bool _disposed;

	public string LibraryName { get; }

	public DynamicNativeDllLoader(string libraryName)
	{
		if (Environment.OSVersion.Platform != PlatformID.Win32NT)
		{
			throw new NotSupportedException("This class is only supported on Windows.");
		}
		LibraryName = libraryName;
		_libHandle = NativeMethods.LoadLibrary(libraryName);
		if (_libHandle == IntPtr.Zero)
		{
			throw new Win32Exception(Marshal.GetLastWin32Error());
		}
	}

	~DynamicNativeDllLoader()
	{
		FreeLibrary();
	}

	public bool HasFunctionName(string functionName)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException(LibraryName);
		}
		if (NativeMethods.GetProcAddress(_libHandle, functionName) == IntPtr.Zero)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == 127)
			{
				return false;
			}
			throw new Win32Exception(lastWin32Error);
		}
		return true;
	}

	public Delegate GetFunctionDelegate(string functionName, Type delegateType)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException(LibraryName);
		}
		IntPtr procAddress = NativeMethods.GetProcAddress(_libHandle, functionName);
		if (procAddress == IntPtr.Zero)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == 127)
			{
				throw new EntryPointNotFoundException("'" + functionName + "' was not found in '" + LibraryName + "'.");
			}
			throw new Win32Exception(lastWin32Error);
		}
		return Marshal.GetDelegateForFunctionPointer(procAddress, delegateType);
	}

	public void Dispose()
	{
		FreeLibrary();
		GC.SuppressFinalize(this);
	}

	private void FreeLibrary()
	{
		if (!_disposed)
		{
			_disposed = true;
			if (_libHandle != IntPtr.Zero)
			{
				NativeMethods.FreeLibrary(_libHandle);
				_libHandle = IntPtr.Zero;
			}
		}
	}
}
