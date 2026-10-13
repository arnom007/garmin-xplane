using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace LauncherCoreLib.Utility;

internal static class NativeMethods
{
	public enum ShowWindowCommands
	{
		Hide = 0,
		Normal = 1,
		ShowMinimized = 2,
		Maximize = 3,
		ShowMaximized = 3,
		ShowNoActivate = 4,
		Show = 5,
		Minimize = 6,
		ShowMinNoActive = 7,
		ShowNA = 8,
		Restore = 9,
		ShowDefault = 10,
		ForceMinimize = 11
	}

	[Serializable]
	public struct RECT
	{
		public int Left;

		public int Top;

		public int Right;

		public int Bottom;
	}

	[Serializable]
	public struct WINDOWPLACEMENT
	{
		public int Length;

		public int Flags;

		public ShowWindowCommands ShowCmd;

		public Point MinPosition;

		public Point MaxPosition;

		public RECT NormalPosition;

		public static WINDOWPLACEMENT Default
		{
			get
			{
				WINDOWPLACEMENT wINDOWPLACEMENT = default(WINDOWPLACEMENT);
				wINDOWPLACEMENT.Length = Marshal.SizeOf<WINDOWPLACEMENT>(wINDOWPLACEMENT);
				return wINDOWPLACEMENT;
			}
		}
	}

	[DllImport("user32.dll", SetLastError = true)]
	[DefaultDllImportSearchPaths(/*Could not decode attribute arguments.*/)]
	public static extern bool GetWindowRect(System.IntPtr hwnd, out RECT lpRect);

	[DllImport("user32.dll", SetLastError = true)]
	[DefaultDllImportSearchPaths(/*Could not decode attribute arguments.*/)]
	[return: MarshalAs(2)]
	public static extern bool GetWindowPlacement(System.IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

	[DllImport("user32.dll", SetLastError = true)]
	[DefaultDllImportSearchPaths(/*Could not decode attribute arguments.*/)]
	[return: MarshalAs(2)]
	public static extern bool SetWindowPlacement(System.IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

	[DllImport("user32.dll")]
	[DefaultDllImportSearchPaths(/*Could not decode attribute arguments.*/)]
	public static extern bool SetForegroundWindow(System.IntPtr hWnd);
}
