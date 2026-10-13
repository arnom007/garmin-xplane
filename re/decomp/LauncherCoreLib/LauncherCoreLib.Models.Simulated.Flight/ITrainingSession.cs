using System.Collections.Generic;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ITrainingSession
{
	IDataRouterSession Router { get; }

	System.Collections.Generic.IReadOnlyList<SimProcessSession> Simulators { get; }

	System.Threading.Tasks.Task AllSimsExited { get; }
}
