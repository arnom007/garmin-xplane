using System;
using System.Collections;
using System.Collections.Generic;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public static class PackageInstallRequestCollectorExtensions
{
	public static void ApproveRequest(this IPackageInstallApprovalCollector approvalCollector, IPackageInstallRequest request)
	{
		System.Collections.Generic.IEnumerator<ITrainerPackage> enumerator = ((System.Collections.Generic.IEnumerable<ITrainerPackage>)request.PackagesToDownload).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				ITrainerPackage current = enumerator.Current;
				approvalCollector.GrantPermissionToDownloadPackage(current.TrainerPackageId);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}
}
