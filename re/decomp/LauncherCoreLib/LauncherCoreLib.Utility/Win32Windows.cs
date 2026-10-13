using System;
using System.Drawing;
using LOLA.Logging;

namespace LauncherCoreLib.Utility;

public class Win32Windows : IOsWindowOperations
{
	public static readonly Win32Windows Instance = new Win32Windows();

	public OsWindow GetCurrentWindow(System.IntPtr windowHandle)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		NativeMethods.WINDOWPLACEMENT lpwndpl = NativeMethods.WINDOWPLACEMENT.Default;
		if (NativeMethods.GetWindowPlacement(windowHandle, ref lpwndpl))
		{
			return new OsWindow(windowHandle, GetState(ref lpwndpl), GetRectangle(ref lpwndpl));
		}
		return OsWindow.DEFAULT;
	}

	public OsWindow UpdateWindow(OsWindow desiredWindowState)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		NativeMethods.WINDOWPLACEMENT lpwndpl = NativeMethods.WINDOWPLACEMENT.Default;
		if (NativeMethods.GetWindowPlacement(desiredWindowState.WindowHandle, ref lpwndpl))
		{
			NativeMethods.WINDOWPLACEMENT lpwndpl2 = lpwndpl;
			lpwndpl2.ShowCmd = GetShowCommand(desiredWindowState.State);
			Rectangle rectangle = desiredWindowState.Rectangle;
			lpwndpl2.NormalPosition = new NativeMethods.RECT
			{
				Left = ((Rectangle)(ref rectangle)).Left,
				Top = ((Rectangle)(ref rectangle)).Top,
				Right = ((Rectangle)(ref rectangle)).Right,
				Bottom = ((Rectangle)(ref rectangle)).Bottom
			};
			flag = NativeMethods.SetWindowPlacement(desiredWindowState.WindowHandle, ref lpwndpl2);
		}
		if (!flag)
		{
			Logger.Log(LogLevel.Debug, "[Warning] Could not set window ({0}) placement.", desiredWindowState.WindowHandle);
		}
		return GetCurrentWindow(desiredWindowState.WindowHandle);
	}

	public bool SetActiveWindow(System.IntPtr windowHandle)
	{
		return NativeMethods.SetForegroundWindow(windowHandle);
	}

	private static Rectangle GetRectangle(ref NativeMethods.WINDOWPLACEMENT windowPlacement)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		NativeMethods.RECT normalPosition = windowPlacement.NormalPosition;
		return new Rectangle(normalPosition.Left, normalPosition.Top, normalPosition.Right - normalPosition.Left, normalPosition.Bottom - normalPosition.Top);
	}

	private static NativeMethods.ShowWindowCommands GetShowCommand(OsWindowState state)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return state switch
		{
			OsWindowState.Normal => NativeMethods.ShowWindowCommands.Normal, 
			OsWindowState.Minimized => NativeMethods.ShowWindowCommands.Minimize, 
			OsWindowState.Maximized => NativeMethods.ShowWindowCommands.Maximize, 
			_ => throw new NotSupportedException(), 
		};
	}

	private static OsWindowState GetState(ref NativeMethods.WINDOWPLACEMENT windowPlacement)
	{
		if (windowPlacement.ShowCmd == NativeMethods.ShowWindowCommands.ShowMinimized)
		{
			return OsWindowState.Minimized;
		}
		if (windowPlacement.ShowCmd == NativeMethods.ShowWindowCommands.Maximize)
		{
			return OsWindowState.Maximized;
		}
		return OsWindowState.Normal;
	}
}
