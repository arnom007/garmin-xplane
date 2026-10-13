using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.TrainerPackages;

public static class TrainerPackageInstallationSessionExtensions
{
	public static TrainerInstallationStatus MostRecentUpdate(this ITrainerPackageInstallationSession session)
	{
		return Enumerable.First<TrainerInstallationStatus>(session.StatusUpdates.ToEnumerable<TrainerInstallationStatus>());
	}

	public static bool HasCompleted(this ITrainerPackageInstallationSession session)
	{
		return ((System.Threading.Tasks.Task)session.SessionCompletion).IsCompleted;
	}

	public static DateTimeOffset? CompletedAt(this ITrainerPackageInstallationSession session)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!session.HasCompleted())
		{
			return null;
		}
		return session.SessionCompletion.Result;
	}

	public static bool IsFailure(this ITrainerPackageInstallationSession session)
	{
		if (session.HasCompleted())
		{
			return session.MostRecentUpdate().OnDemandState == TrainerPackageOperationState.Error;
		}
		return false;
	}

	public static bool WasCancelled(this ITrainerPackageInstallationSession session)
	{
		if (session.HasCompleted())
		{
			return session.MostRecentUpdate().OnDemandState == TrainerPackageOperationState.Canceled;
		}
		return false;
	}

	public static ITrainerPackageInstallationSession RetryInstall(this ITrainerPackageInstaller installer, ITrainerPackageInstallationSession session)
	{
		return installer.Install(session.RequestingPackage, forceRetry: true);
	}
}
