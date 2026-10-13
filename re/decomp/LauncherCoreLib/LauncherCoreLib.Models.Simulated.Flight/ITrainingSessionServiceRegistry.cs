using System;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ITrainingSessionServiceRegistry
{
	System.IDisposable RegisterServiceFactory(System.Type targetType, ITrainingSessionServiceFactory serviceFactory);
}
