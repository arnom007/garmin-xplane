using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IManipulateRunningSims
{
	OsWindow ManipulateWindow(RunningSim sim, OsWindow desiredWindow);

	bool FocusWindow(RunningSim sim);
}
