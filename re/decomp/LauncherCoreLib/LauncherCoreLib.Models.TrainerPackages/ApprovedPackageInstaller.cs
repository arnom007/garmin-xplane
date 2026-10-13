using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class ApprovedPackageInstaller : IApprovedPackageInstaller
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass2_0
	{
		[StructLayout(3)]
		private struct _003C_003CDownloadAndInstall_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass2_0 _003C_003E4__this;

			public IObserver<ITrainerPackageInstallationSession> o;

			public CancellationToken token;

			private ConfiguredTaskAwaiter<DateTimeOffset> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				//IL_012b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0133: Unknown result type (might be due to invalid IL or missing references)
				//IL_0144: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass2_0 _003C_003Ec__DisplayClass2_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<DateTimeOffset> awaiter;
					if (num != 0)
					{
						if (num != 1)
						{
							ITrainerPackageInstallationSession currentSessionForPackage;
							try
							{
								currentSessionForPackage = _003C_003Ec__DisplayClass2_._003C_003E4__this._installer.GetCurrentSessionForPackage(_003C_003Ec__DisplayClass2_.package);
							}
							catch (System.Exception ex)
							{
								o.OnError(ex);
								goto end_IL_000e;
							}
							if (currentSessionForPackage != null)
							{
								if (currentSessionForPackage.RequestedOperation != TrainerPackageOperation.Install)
								{
									awaiter = currentSessionForPackage.SessionCompletion.DetachContext().GetAwaiter();
									if (!awaiter.IsCompleted)
									{
										num = (_003C_003E1__state = 0);
										_003C_003Eu__1 = awaiter;
										((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<DateTimeOffset>, _003C_003CDownloadAndInstall_003Eb__1_003Ed>(ref awaiter, ref this);
										return;
									}
									goto IL_00b6;
								}
								if (_003C_003Ec__DisplayClass2_.package.IsBugFixUpdateOf(currentSessionForPackage.RequestingPackage))
								{
									o.OnNext(currentSessionForPackage);
									awaiter = currentSessionForPackage.SessionCompletion.DetachContext().GetAwaiter();
									if (!awaiter.IsCompleted)
									{
										num = (_003C_003E1__state = 1);
										_003C_003Eu__1 = awaiter;
										((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<DateTimeOffset>, _003C_003CDownloadAndInstall_003Eb__1_003Ed>(ref awaiter, ref this);
										return;
									}
									goto IL_0142;
								}
							}
							goto IL_014a;
						}
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<DateTimeOffset>);
						num = (_003C_003E1__state = -1);
						goto IL_0142;
					}
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<DateTimeOffset>);
					num = (_003C_003E1__state = -1);
					goto IL_00b6;
					IL_014a:
					if (!((CancellationToken)(ref token)).IsCancellationRequested)
					{
						o.OnNext(_003C_003Ec__DisplayClass2_._003C_003E4__this._installer.Install(_003C_003Ec__DisplayClass2_.package, forceRetry: true));
					}
					o.OnCompleted();
					goto end_IL_000e;
					IL_0142:
					awaiter.GetResult();
					goto IL_014a;
					IL_00b6:
					awaiter.GetResult();
					goto IL_014a;
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		public ITrainerPackage package;

		public ApprovedPackageInstaller _003C_003E4__this;

		[AsyncStateMachine(typeof(_003C_003CDownloadAndInstall_003Eb__1_003Ed))]
		internal System.Threading.Tasks.Task _003CDownloadAndInstall_003Eb__1(IObserver<ITrainerPackageInstallationSession> o, CancellationToken token)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CDownloadAndInstall_003Eb__1_003Ed _003C_003CDownloadAndInstall_003Eb__1_003Ed = default(_003C_003CDownloadAndInstall_003Eb__1_003Ed);
			_003C_003CDownloadAndInstall_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CDownloadAndInstall_003Eb__1_003Ed._003C_003E4__this = this;
			_003C_003CDownloadAndInstall_003Eb__1_003Ed.o = o;
			_003C_003CDownloadAndInstall_003Eb__1_003Ed.token = token;
			_003C_003CDownloadAndInstall_003Eb__1_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CDownloadAndInstall_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003CDownloadAndInstall_003Eb__1_003Ed>(ref _003C_003CDownloadAndInstall_003Eb__1_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CDownloadAndInstall_003Eb__1_003Ed._003C_003Et__builder)).Task;
		}
	}

	private readonly ITrainerPackageInstaller _installer;

	public ApprovedPackageInstaller(ITrainerPackageInstaller installer)
	{
		_installer = installer;
	}

	public IObservable<TrainerInstallationStatus> DownloadAndInstall(IPackageInstallApproval approval)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (approval == null)
		{
			throw new ArgumentNullException("approval");
		}
		return ((System.Collections.Generic.IEnumerable<ITrainerPackage>)approval.Request.PackagesToDownload).ToObservable().Select<ITrainerPackage, IObservable<TrainerInstallationStatus>>((Func<ITrainerPackage, IObservable<TrainerInstallationStatus>>)([CompilerGenerated] (ITrainerPackage package) =>
		{
			_003C_003Ec__DisplayClass2_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass2_0();
			CS_0024_003C_003E8__locals0._003C_003E4__this = this;
			CS_0024_003C_003E8__locals0.package = package;
			return Observable.Return(TrainerInstallationStatus.CreateProgress(CS_0024_003C_003E8__locals0.package, 0.0)).Concat<TrainerInstallationStatus>(Observable.Create<ITrainerPackageInstallationSession>((Func<IObserver<ITrainerPackageInstallationSession>, CancellationToken, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass2_0._003C_003CDownloadAndInstall_003Eb__1_003Ed))] (IObserver<ITrainerPackageInstallationSession> o, CancellationToken token) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				_003C_003Ec__DisplayClass2_0._003C_003CDownloadAndInstall_003Eb__1_003Ed _003C_003CDownloadAndInstall_003Eb__1_003Ed = default(_003C_003Ec__DisplayClass2_0._003C_003CDownloadAndInstall_003Eb__1_003Ed);
				_003C_003CDownloadAndInstall_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003CDownloadAndInstall_003Eb__1_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
				_003C_003CDownloadAndInstall_003Eb__1_003Ed.o = o;
				_003C_003CDownloadAndInstall_003Eb__1_003Ed.token = token;
				_003C_003CDownloadAndInstall_003Eb__1_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003CDownloadAndInstall_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass2_0._003C_003CDownloadAndInstall_003Eb__1_003Ed>(ref _003C_003CDownloadAndInstall_003Eb__1_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003CDownloadAndInstall_003Eb__1_003Ed._003C_003Et__builder)).Task;
			})).Select<ITrainerPackageInstallationSession, IObservable<TrainerInstallationStatus>>((Func<ITrainerPackageInstallationSession, IObservable<TrainerInstallationStatus>>)((ITrainerPackageInstallationSession session) => session.StatusUpdates)).Switch<TrainerInstallationStatus>());
		})).Merge<TrainerInstallationStatus>();
	}
}
