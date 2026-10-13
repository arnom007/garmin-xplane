using System;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Panel;

public interface IResetSimulators
{
	System.Threading.Tasks.Task ResetSimulator(ReactiveSimulator simulator);

	IObservable<bool> CanBeReset(ReactiveSimulator simulator);
}
