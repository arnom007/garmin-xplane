using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class SimManipulator : IManipulateRunningSims
{
	private readonly IOsWindowOperations _windows;

	public SimManipulator(IOsWindowOperations windows)
	{
		_windows = windows;
	}

	public bool FocusWindow(RunningSim sim)
	{
		return _windows.SetActiveWindow(sim.Window.WindowHandle);
	}

	public OsWindow ManipulateWindow(RunningSim sim, OsWindow desiredWindow)
	{
		return _windows.UpdateWindow(desiredWindow);
	}
}
