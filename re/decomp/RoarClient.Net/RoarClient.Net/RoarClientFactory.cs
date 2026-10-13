using System;
using System.ComponentModel;
using System.Diagnostics;
using RoarClient.Net.Native;

namespace RoarClient.Net;

public static class RoarClientFactory
{
	public static IRoarClient Create(string nativeRoarClientDllPath, Version? nativeVersion = null)
	{
		if (nativeRoarClientDllPath == null)
		{
			throw new ArgumentNullException("nativeRoarClientDllPath");
		}
		if ((object)nativeVersion == null)
		{
			nativeVersion = ParseVersionFromFile(nativeRoarClientDllPath);
		}
		try
		{
			DynamicNativeDllLoader dynamicNativeDllLoader = new DynamicNativeDllLoader(nativeRoarClientDllPath);
			try
			{
				return new ClientContainer(dynamicNativeDllLoader, nativeVersion);
			}
			catch
			{
				dynamicNativeDllLoader.Dispose();
				throw;
			}
		}
		catch (Win32Exception innerException)
		{
			throw new NotSupportedException("'" + nativeRoarClientDllPath + "' could not be loaded properly.", innerException);
		}
		catch (EntryPointNotFoundException innerException2)
		{
			throw new NotSupportedException("'" + nativeRoarClientDllPath + "' is missing required functions.", innerException2);
		}
	}

	public static IRoarClient Create((DllFilePath Path, Version NativeVersion) searchResult)
	{
		return Create(searchResult.Path.FullPath, searchResult.NativeVersion);
	}

	private static Version ParseVersionFromFile(string nativeRoarClientDllPath)
	{
		return Version.Parse(FileVersionInfo.GetVersionInfo(nativeRoarClientDllPath).FileVersion);
	}
}
