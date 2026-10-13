using System;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ITrainingSessionServiceProvider
{
	object GetSessionService(System.Type requestedType, ITrainingSession trainingSession);
}
