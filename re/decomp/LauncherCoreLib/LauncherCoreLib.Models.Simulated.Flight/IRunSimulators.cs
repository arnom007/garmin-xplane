using System;
using LauncherCoreLib.Models.AbstractUi;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IRunSimulators
{
	bool CanRun { get; }

	bool IsRunning { get; }

	IObservable<bool> WhenCanRunChanges { get; }

	IObservable<bool> WhenIsRunningChanges { get; }

	IObservable<IUserAction> WhenActionChanges { get; }

	void Launch();

	void Abort();
}
