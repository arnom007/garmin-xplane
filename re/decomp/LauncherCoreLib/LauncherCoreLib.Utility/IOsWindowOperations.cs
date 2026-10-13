using System;

namespace LauncherCoreLib.Utility;

public interface IOsWindowOperations
{
	OsWindow UpdateWindow(OsWindow desiredWindowState);

	OsWindow GetCurrentWindow(System.IntPtr windowHandle);

	bool SetActiveWindow(System.IntPtr windowHandle);
}
