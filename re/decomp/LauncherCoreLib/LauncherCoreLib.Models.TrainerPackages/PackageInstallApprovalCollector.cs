using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using LOLA.Reactive;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class PackageInstallApprovalCollector : IPackageInstallApprovalCollector, IPackageInstallRequester
{
	private sealed class RequestApproval : IPackageInstallApproval
	{
		[field: CompilerGenerated]
		public IPackageInstallRequest Request
		{
			[CompilerGenerated]
			get;
		}

		public RequestApproval(IPackageInstallRequest request)
		{
			Request = request;
		}
	}

	private readonly object _packageLock = new object();

	private readonly ObservableCollection<TrainerIdentifier> _embeddedReleaseApprovals;

	public PackageInstallApprovalCollector()
	{
		_embeddedReleaseApprovals = new ObservableCollection<TrainerIdentifier>();
	}

	public void GrantPermissionToDownloadPackage(TrainerPackageIdentifier trainerPackage)
	{
		lock (_packageLock)
		{
			TrainerEmbeddedReleaseId embeddedReleaseCompositeId = trainerPackage.EmbeddedReleaseCompositeId;
			if (!((Collection<TrainerIdentifier>)(object)_embeddedReleaseApprovals).Contains((TrainerIdentifier)embeddedReleaseCompositeId))
			{
				((Collection<TrainerIdentifier>)(object)_embeddedReleaseApprovals).Add((TrainerIdentifier)embeddedReleaseCompositeId);
			}
		}
	}

	public IObservable<InstallRequestResultRecord> RequestPermissionToDownloadPackages(IPackageInstallRequest request)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		IPackageInstallRequest request2 = request;
		if (request2 == null)
		{
			throw new ArgumentNullException("request");
		}
		return _embeddedReleaseApprovals.SnapshotStream<TrainerIdentifier>().Select<System.Collections.Generic.IList<TrainerIdentifier>, InstallRequestResultRecord>((Func<System.Collections.Generic.IList<TrainerIdentifier>, InstallRequestResultRecord>)delegate(System.Collections.Generic.IList<TrainerIdentifier> approvedPackages)
		{
			if (Enumerable.Any<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)request2.PackagesToDownload, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => !((System.Collections.Generic.ICollection<TrainerIdentifier>)approvedPackages).Contains((TrainerIdentifier)p.TrainerPackageId.EmbeddedReleaseCompositeId))))
			{
				return new InstallRequestResultRecord(this, request2, RequestResult.Pending);
			}
			RequestApproval approval = new RequestApproval(request2);
			return new InstallRequestResultRecord(this, approval);
		});
	}

	public void RevokePermissionToDownloadPackage(TrainerPackageIdentifier trainerPackage)
	{
		lock (_packageLock)
		{
			TrainerEmbeddedReleaseId embeddedReleaseCompositeId = trainerPackage.EmbeddedReleaseCompositeId;
			if (((Collection<TrainerIdentifier>)(object)_embeddedReleaseApprovals).Contains((TrainerIdentifier)embeddedReleaseCompositeId))
			{
				((Collection<TrainerIdentifier>)(object)_embeddedReleaseApprovals).Remove((TrainerIdentifier)embeddedReleaseCompositeId);
			}
		}
	}
}
