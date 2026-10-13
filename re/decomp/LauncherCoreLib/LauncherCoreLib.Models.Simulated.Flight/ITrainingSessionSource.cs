using System;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ITrainingSessionSource
{
	IObservable<ITrainingSession?> TrainingSessions();
}
