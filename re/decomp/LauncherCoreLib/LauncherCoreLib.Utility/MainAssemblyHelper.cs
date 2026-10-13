using System;
using System.Diagnostics;
using System.Reflection;

namespace LauncherCoreLib.Utility;

public static class MainAssemblyHelper
{
	public static Version GetMainAssemblyVersion()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		Assembly entryAssembly = Assembly.GetEntryAssembly();
		Version version = default(Version);
		if (entryAssembly == null)
		{
			ProcessModule mainModule = Process.GetCurrentProcess().MainModule;
			Version.TryParse(((mainModule != null) ? mainModule.FileVersionInfo.FileVersion : null) ?? string.Empty, ref version);
		}
		else
		{
			version = entryAssembly.GetName().Version;
		}
		return (Version)(((object)version) ?? ((object)new Version(0, 0, 0, 0)));
	}
}
