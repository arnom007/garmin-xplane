using System;
using System.Threading.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface ITrainerPackageInstallationSession
{
	Guid SessionId { get; }

	ITrainerPackage RequestingPackage { get; }

	TrainerPackageOperation RequestedOperation { get; }

	System.Threading.Tasks.Task<DateTimeOffset> SessionCompletion { get; }

	IObservable<TrainerInstallationStatus> StatusUpdates { get; }

	void AttemptCancel();
}
