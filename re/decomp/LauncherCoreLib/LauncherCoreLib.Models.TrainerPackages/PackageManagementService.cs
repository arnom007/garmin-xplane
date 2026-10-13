using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using LOLA.Reactive;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class PackageManagementService : IPackageManagementActions, IPackageErrorProvider
{
	private readonly ITrainerPackageRepoEvolved _repo;

	private readonly IPackageErrorProvider _packageErrorProvider;

	private readonly ITrainerPackageInstaller _packageInstaller;

	private readonly GrantAndStartPackageInstallBehavior _grantAndStartInstallBehavior;

	public PackageManagementService(ITrainerPackageRepoEvolved repo, IPackageErrorProvider packageErrorProvider, ITrainerPackageInstaller installer, IPackageInstallApprovalCollector packageApproval)
	{
		_repo = repo;
		_packageErrorProvider = packageErrorProvider;
		_packageInstaller = installer;
		_grantAndStartInstallBehavior = new GrantAndStartPackageInstallBehavior(installer, packageApproval);
	}

	public IObservable<IDictionary<TrainerEmbeddedReleaseId, PackageInstallationInfo>> GetAllInstallInfoOverTime()
	{
		return _repo.GetAllInstallInfoOverTime();
	}

	public IObservable<TrainerPackageLoadException> GetAllCurrentErrors()
	{
		return _packageErrorProvider.GetAllCurrentErrors();
	}

	public void StartPackageInstall(ITrainerPackage package)
	{
		_grantAndStartInstallBehavior.GrantAndStartPackageDownload(package);
	}

	public void StartPackageUninstall(ITrainerPackage package)
	{
		_packageInstaller.Uninstall(package, forceRetry: true);
	}

	public IObservable<ISet<ITrainerPackageInstallationSession>> InstallationSessionsOverTime()
	{
		return (IObservable<ISet<ITrainerPackageInstallationSession>>)(object)_packageInstaller.CurrentAndFutureSessions().ToUpToDateDictionaries<TrainerEmbeddedReleaseId, ITrainerPackageInstallationSession>((ITrainerPackageInstallationSession s) => s.RequestingPackage.TrainerPackageId.EmbeddedReleaseCompositeId).Select<IReadOnlyDictionary<TrainerEmbeddedReleaseId, ITrainerPackageInstallationSession>, HashSet<ITrainerPackageInstallationSession>>((Func<IReadOnlyDictionary<TrainerEmbeddedReleaseId, ITrainerPackageInstallationSession>, HashSet<ITrainerPackageInstallationSession>>)((IReadOnlyDictionary<TrainerEmbeddedReleaseId, ITrainerPackageInstallationSession> dictionary) => Enumerable.ToHashSet<ITrainerPackageInstallationSession>(dictionary.Values)));
	}

	public void DismissFailedInstall(ITrainerPackageInstallationSession failedSession)
	{
		_packageInstaller.DismissErroredSession(failedSession);
	}
}
