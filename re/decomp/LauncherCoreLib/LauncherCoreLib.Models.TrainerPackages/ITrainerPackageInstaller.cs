using System;
using System.Collections.Generic;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface ITrainerPackageInstaller
{
	System.Collections.Generic.IEnumerable<ITrainerPackageInstallationSession> CurrentSessions { get; }

	IObservable<ITrainerPackageInstallationSession> FutureSessions { get; }

	System.Collections.Generic.IEnumerable<ITrainerPackageInstallationSession> GetAllSessions();

	ITrainerPackageInstallationSession? GetCurrentSessionForPackage(ITrainerPackage package);

	ITrainerPackageInstallationSession Install(ITrainerPackage package, bool forceRetry = false);

	ITrainerPackageInstallationSession Uninstall(ITrainerPackage package, bool forceRetry = false);

	void DismissErroredSession(ITrainerPackageInstallationSession failedSession);
}
