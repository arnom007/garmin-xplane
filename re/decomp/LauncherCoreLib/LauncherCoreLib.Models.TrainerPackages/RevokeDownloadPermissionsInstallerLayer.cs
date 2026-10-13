using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class RevokeDownloadPermissionsInstallerLayer : ITrainerPackageInstaller
{
	private sealed class RevokeOnCancelledOrErroredInstall : ITrainerPackageInstallationSession
	{
		private readonly ITrainerPackageInstallationSession _actualSession;

		private readonly IPackageInstallApprovalCollector _approvalCollector;

		public IObservable<TrainerInstallationStatus> StatusUpdates => _actualSession.StatusUpdates.Do<TrainerInstallationStatus>((Action<TrainerInstallationStatus>)([CompilerGenerated] (TrainerInstallationStatus su) =>
		{
			if (su.OnDemandState == TrainerPackageOperationState.Canceled)
			{
				_approvalCollector.RevokePermissionToDownloadPackage(su.RequestingPackage.TrainerPackageId);
			}
		}));

		public Guid SessionId => _actualSession.SessionId;

		public ITrainerPackage RequestingPackage => _actualSession.RequestingPackage;

		public TrainerPackageOperation RequestedOperation => _actualSession.RequestedOperation;

		public System.Threading.Tasks.Task<DateTimeOffset> SessionCompletion => _actualSession.SessionCompletion;

		public RevokeOnCancelledOrErroredInstall(ITrainerPackageInstallationSession session, IPackageInstallApprovalCollector approvalCollector)
		{
			_actualSession = session;
			_approvalCollector = approvalCollector;
		}

		public void AttemptCancel()
		{
			_actualSession.AttemptCancel();
		}
	}

	private readonly ITrainerPackageInstaller _actualInstaller;

	private readonly IPackageInstallApprovalCollector _approvalCollector;

	public System.Collections.Generic.IEnumerable<ITrainerPackageInstallationSession> CurrentSessions => WrapInstalls(_actualInstaller.CurrentSessions);

	public IObservable<ITrainerPackageInstallationSession> FutureSessions => WrapInstalls(_actualInstaller.FutureSessions);

	public RevokeDownloadPermissionsInstallerLayer(ITrainerPackageInstaller actualInstaller, IPackageInstallApprovalCollector approvalCollector)
	{
		_actualInstaller = actualInstaller;
		_approvalCollector = approvalCollector;
	}

	public System.Collections.Generic.IEnumerable<ITrainerPackageInstallationSession> GetAllSessions()
	{
		return WrapInstalls(_actualInstaller.GetAllSessions());
	}

	public ITrainerPackageInstallationSession Install(ITrainerPackage package, bool forceRetry = false)
	{
		return WrapSession(_actualInstaller.Install(package, forceRetry));
	}

	public ITrainerPackageInstallationSession Uninstall(ITrainerPackage package, bool forceRetry = false)
	{
		_approvalCollector.RevokePermissionToDownloadPackage(package.TrainerPackageId);
		return _actualInstaller.Uninstall(package, forceRetry);
	}

	public ITrainerPackageInstallationSession? GetCurrentSessionForPackage(ITrainerPackage package)
	{
		return WrapSession(_actualInstaller.GetCurrentSessionForPackage(package));
	}

	public void DismissErroredSession(ITrainerPackageInstallationSession failedSession)
	{
		_actualInstaller.DismissErroredSession(failedSession);
	}

	private IObservable<ITrainerPackageInstallationSession?> WrapInstalls(IObservable<ITrainerPackageInstallationSession> innerSessions)
	{
		return innerSessions.Select<ITrainerPackageInstallationSession, ITrainerPackageInstallationSession>((Func<ITrainerPackageInstallationSession, ITrainerPackageInstallationSession>)([CompilerGenerated] (ITrainerPackageInstallationSession session) => WrapSession(session)));
	}

	private System.Collections.Generic.IEnumerable<ITrainerPackageInstallationSession?> WrapInstalls(System.Collections.Generic.IEnumerable<ITrainerPackageInstallationSession> innerSessions)
	{
		return Enumerable.Select<ITrainerPackageInstallationSession, ITrainerPackageInstallationSession>(innerSessions, (Func<ITrainerPackageInstallationSession, ITrainerPackageInstallationSession>)([CompilerGenerated] (ITrainerPackageInstallationSession session) => WrapSession(session)));
	}

	private ITrainerPackageInstallationSession? WrapSession(ITrainerPackageInstallationSession? session)
	{
		if (session == null)
		{
			return null;
		}
		if (session is RevokeDownloadPermissionsInstallerLayer)
		{
			return session;
		}
		if (session.RequestedOperation == TrainerPackageOperation.Install)
		{
			return new RevokeOnCancelledOrErroredInstall(session, _approvalCollector);
		}
		return session;
	}
}
