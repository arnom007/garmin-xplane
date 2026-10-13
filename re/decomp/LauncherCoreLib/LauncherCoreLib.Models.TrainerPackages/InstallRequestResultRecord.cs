using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.TrainerPackages;

public sealed class InstallRequestResultRecord
{
	private IPackageInstallApproval? _approval;

	[field: CompilerGenerated]
	public IPackageInstallRequest Request
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public RequestResult Result
	{
		[CompilerGenerated]
		get;
	}

	public IPackageInstallApproval? Approval
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (Result != RequestResult.Approved)
			{
				throw new InvalidOperationException("Only approved records can access this property.");
			}
			return _approval;
		}
		private set
		{
			_approval = value;
		}
	}

	[field: CompilerGenerated]
	public IPackageInstallApprovalCollector RequestCollector
	{
		[CompilerGenerated]
		get;
	}

	public InstallRequestResultRecord(IPackageInstallApprovalCollector requestCollector, IPackageInstallRequest request, RequestResult result)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		RequestCollector = requestCollector;
		Request = request ?? throw new ArgumentNullException("request");
		Result = result;
	}

	public InstallRequestResultRecord(IPackageInstallApprovalCollector requestCollector, IPackageInstallApproval approval)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (approval == null)
		{
			throw new ArgumentNullException("approval");
		}
		RequestCollector = requestCollector;
		Request = approval.Request;
		Approval = approval;
		Result = RequestResult.Approved;
	}

	public void ApproveRequest()
	{
		RequestCollector.ApproveRequest(Request);
	}
}
