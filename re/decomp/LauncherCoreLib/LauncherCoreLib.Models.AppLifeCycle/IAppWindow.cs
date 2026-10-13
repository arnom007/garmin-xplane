using System;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public interface IAppWindow
{
	string WindowTitle { get; set; }

	IObservable<string> WhenWindowTitleChanges { get; }

	IObservable<WindowSizeAndLocation> WhenWindowChangesSizeOrLocation { get; }

	System.Threading.Tasks.Task SetWindowSizeAndLocation(WindowSizeAndLocation window);
}
