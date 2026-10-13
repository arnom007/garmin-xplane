using System;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IRouterCommandConnection : System.IDisposable
{
	bool IsOpen { get; }

	IRouterConfigureCommands Configuration { get; }

	bool Open();

	void Close();
}
