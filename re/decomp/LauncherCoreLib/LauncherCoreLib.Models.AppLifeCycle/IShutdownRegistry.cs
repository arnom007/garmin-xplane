using System;

namespace LauncherCoreLib.Models.AppLifeCycle;

public interface IShutdownRegistry
{
	System.IDisposable? RegisterShutdownAction(IShutdownAction shutdownAction);
}
