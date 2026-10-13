using System.Threading.Tasks;
using LOLA.Reactive;
using LauncherCoreLib.Models.Simulated.Airport;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ISimSessionEvolved
{
	System.Threading.Tasks.Task<AircraftSnapshotData> SessionSnapshot { get; }

	IReactiveEntity<PowerStage> Stage { get; }

	System.Threading.Tasks.Task<ITrainingSession?> TrainingSession { get; }

	System.Threading.Tasks.Task Completion { get; }

	void InitiateSessionShutdown();
}
