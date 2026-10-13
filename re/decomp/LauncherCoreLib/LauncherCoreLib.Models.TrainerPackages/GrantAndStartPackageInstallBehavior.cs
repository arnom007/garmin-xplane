using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class GrantAndStartPackageInstallBehavior
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass4_0
	{
		public GrantAndStartPackageInstallBehavior _003C_003E4__this;

		public ITrainerPackage package;
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGrantDownloadAndInstallPackage_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TrainerPackageOperationState> _003C_003Et__builder;

		public GrantAndStartPackageInstallBehavior _003C_003E4__this;

		public ITrainerPackage package;

		private _003C_003Ec__DisplayClass4_0 _003C_003E8__1;

		private ITrainerPackageInstallationSession _003Ccurrent_003E5__2;

		private ConfiguredTaskAwaiter<DateTimeOffset> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			GrantAndStartPackageInstallBehavior grantAndStartPackageInstallBehavior = _003C_003E4__this;
			TrainerPackageOperationState result;
			try
			{
				ConfiguredTaskAwaiter<DateTimeOffset> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<DateTimeOffset>);
					num = (_003C_003E1__state = -1);
					goto IL_019c;
				}
				if (num == 1)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<DateTimeOffset>);
					num = (_003C_003E1__state = -1);
					goto IL_0218;
				}
				_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
				_003C_003E8__1.package = package;
				if (_003C_003E8__1.package == null)
				{
					throw new ArgumentNullException("package");
				}
				grantAndStartPackageInstallBehavior._approvalCollector.GrantPermissionToDownloadPackage(_003C_003E8__1.package.TrainerPackageId);
				_003Ccurrent_003E5__2 = grantAndStartPackageInstallBehavior._packageInstaller.GetCurrentSessionForPackage(_003C_003E8__1.package);
				if (_003Ccurrent_003E5__2 == null)
				{
					_003Ccurrent_003E5__2 = grantAndStartPackageInstallBehavior._003CGrantDownloadAndInstallPackage_003Eg__StartNewInstall_007C4_0(ref _003C_003E8__1);
					goto IL_01b6;
				}
				if (_003Ccurrent_003E5__2.RequestedOperation != TrainerPackageOperation.Uninstall)
				{
					if (_003Ccurrent_003E5__2.IsFailure() || _003Ccurrent_003E5__2.WasCancelled())
					{
						_003Ccurrent_003E5__2 = grantAndStartPackageInstallBehavior._003CGrantDownloadAndInstallPackage_003Eg__StartNewInstall_007C4_0(ref _003C_003E8__1);
					}
					else if (_003C_003E8__1.package.IsSuccessorOf(_003Ccurrent_003E5__2.RequestingPackage))
					{
						awaiter = _003Ccurrent_003E5__2.SessionCompletion.Catch().DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<DateTimeOffset>, _003CGrantDownloadAndInstallPackage_003Ed__4>(ref awaiter, ref this);
							return;
						}
						goto IL_019c;
					}
					goto IL_01b6;
				}
				if (_003Ccurrent_003E5__2.HasCompleted())
				{
					_003Ccurrent_003E5__2 = grantAndStartPackageInstallBehavior._003CGrantDownloadAndInstallPackage_003Eg__StartNewInstall_007C4_0(ref _003C_003E8__1);
					goto IL_01b6;
				}
				result = TrainerPackageOperationState.Canceled;
				goto end_IL_000e;
				IL_01b6:
				awaiter = _003Ccurrent_003E5__2.SessionCompletion.DetachContext().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__1 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<DateTimeOffset>, _003CGrantDownloadAndInstallPackage_003Ed__4>(ref awaiter, ref this);
					return;
				}
				goto IL_0218;
				IL_019c:
				awaiter.GetResult();
				_003Ccurrent_003E5__2 = grantAndStartPackageInstallBehavior._003CGrantDownloadAndInstallPackage_003Eg__StartNewInstall_007C4_0(ref _003C_003E8__1);
				goto IL_01b6;
				IL_0218:
				awaiter.GetResult();
				result = _003Ccurrent_003E5__2.MostRecentUpdate().OnDemandState;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = default(_003C_003Ec__DisplayClass4_0);
				_003Ccurrent_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = default(_003C_003Ec__DisplayClass4_0);
			_003Ccurrent_003E5__2 = null;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly ITrainerPackageInstaller _packageInstaller;

	private readonly IPackageInstallApprovalCollector _approvalCollector;

	public GrantAndStartPackageInstallBehavior(ITrainerPackageInstaller packageInstaller, IPackageInstallApprovalCollector approvalCollector)
	{
		_packageInstaller = packageInstaller;
		_approvalCollector = approvalCollector;
	}

	public void GrantAndStartPackageDownload(ITrainerPackage package)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (package == null)
		{
			throw new ArgumentNullException("package");
		}
		_approvalCollector.GrantPermissionToDownloadPackage(package.TrainerPackageId);
		if (_packageInstaller.IsSafeToStartNewOperation(package))
		{
			_packageInstaller.Install(package, forceRetry: true);
		}
	}

	[AsyncStateMachine(typeof(_003CGrantDownloadAndInstallPackage_003Ed__4))]
	public async System.Threading.Tasks.Task<TrainerPackageOperationState> GrantDownloadAndInstallPackage(ITrainerPackage package)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ITrainerPackage package2 = package;
		if (package2 == null)
		{
			throw new ArgumentNullException("package");
		}
		_approvalCollector.GrantPermissionToDownloadPackage(package2.TrainerPackageId);
		ITrainerPackageInstallationSession current = _packageInstaller.GetCurrentSessionForPackage(package2);
		if (current == null)
		{
			current = StartNewInstall();
		}
		else if (current.RequestedOperation == TrainerPackageOperation.Uninstall)
		{
			if (!current.HasCompleted())
			{
				return TrainerPackageOperationState.Canceled;
			}
			current = StartNewInstall();
		}
		else if (current.IsFailure() || current.WasCancelled())
		{
			current = StartNewInstall();
		}
		else if (package2.IsSuccessorOf(current.RequestingPackage))
		{
			await current.SessionCompletion.Catch().DetachContext();
			current = StartNewInstall();
		}
		await current.SessionCompletion.DetachContext();
		return current.MostRecentUpdate().OnDemandState;
		[CompilerGenerated]
		ITrainerPackageInstallationSession StartNewInstall()
		{
			return _packageInstaller.Install(package2, forceRetry: true);
		}
	}

	[CompilerGenerated]
	private ITrainerPackageInstallationSession _003CGrantDownloadAndInstallPackage_003Eg__StartNewInstall_007C4_0(ref _003C_003Ec__DisplayClass4_0 P_0)
	{
		return _packageInstaller.Install(P_0.package, forceRetry: true);
	}
}
