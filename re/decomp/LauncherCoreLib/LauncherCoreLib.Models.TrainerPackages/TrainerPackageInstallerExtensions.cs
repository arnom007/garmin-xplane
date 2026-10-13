using System;
using System.Reactive.Linq;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public static class TrainerPackageInstallerExtensions
{
	public static IObservable<ITrainerPackageInstallationSession> CurrentAndFutureSessions(this ITrainerPackageInstaller installer)
	{
		return installer.CurrentSessions.ToObservable().Merge<ITrainerPackageInstallationSession>(installer.FutureSessions);
	}

	public static bool IsSafeToStartNewOperation(this ITrainerPackageInstaller installer, ITrainerPackage package)
	{
		return installer.GetCurrentSessionForPackage(package)?.HasCompleted() ?? true;
	}
}
