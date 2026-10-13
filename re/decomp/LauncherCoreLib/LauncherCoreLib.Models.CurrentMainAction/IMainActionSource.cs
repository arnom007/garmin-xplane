using System;

namespace LauncherCoreLib.Models.CurrentMainAction;

public interface IMainActionSource
{
	IObservable<IMainAction?> MainActions { get; }

	System.IDisposable Push(IMainAction action);

	System.IDisposable Push(IObservable<IMainAction?> actions);

	void Pop();
}
