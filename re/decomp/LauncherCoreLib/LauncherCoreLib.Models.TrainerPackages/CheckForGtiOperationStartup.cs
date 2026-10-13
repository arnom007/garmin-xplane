using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using GTI.Net;
using LauncherCoreLib.Models.AppLifeCycle;

namespace LauncherCoreLib.Models.TrainerPackages;

public class CheckForGtiOperationStartup : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		public IGtiRunner gtiRunner;
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnStartup_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public CheckForGtiOperationStartup _003C_003E4__this;

		public CancellationToken shutdown;

		private _003C_003Ec__DisplayClass3_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private ConfiguredTaskAwaiter<bool> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CheckForGtiOperationStartup checkForGtiOperationStartup = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<bool> awaiter;
				if (num != 0)
				{
					if (num != 1)
					{
						_003C_003E8__1 = new _003C_003Ec__DisplayClass3_0();
						_003C_003E8__1.gtiRunner = checkForGtiOperationStartup._gtiProvider.UnprotectedGtiRunner;
						goto IL_00d7;
					}
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<bool>);
					num = (_003C_003E1__state = -1);
					goto IL_013a;
				}
				ConfiguredTaskAwaiter awaiter2 = _003C_003Eu__1;
				_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
				num = (_003C_003E1__state = -1);
				goto IL_00c5;
				IL_00d7:
				awaiter = CheckIfGtiCanRun().ConfigureAwait(false).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<bool>, _003COnStartup_003Ed__3>(ref awaiter, ref this);
					return;
				}
				goto IL_013a;
				IL_013a:
				if (!awaiter.GetResult())
				{
					((CancellationToken)(ref shutdown)).ThrowIfCancellationRequested();
					ConfiguredTaskAwaitable val = checkForGtiOperationStartup._presenter(CheckIfGtiCanRun, shutdown).ConfigureAwait(false);
					awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003COnStartup_003Ed__3>(ref awaiter2, ref this);
						return;
					}
					goto IL_00c5;
				}
				goto end_IL_000e;
				IL_00c5:
				((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
				((CancellationToken)(ref shutdown)).ThrowIfCancellationRequested();
				goto IL_00d7;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_0._003C_003COnStartup_003Eg__CheckIfGtiCanRun_007C0_003Ed))]
			async System.Threading.Tasks.Task<bool> CheckIfGtiCanRun()
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				try
				{
					await ((_003C_003Ec__DisplayClass3_0)(object)this).gtiRunner.Execute("--help").IgnoreStandardOutput().ProcessResult.ConfigureAwait(false);
					return true;
				}
				catch (object obj) when (((Func<bool>)delegate
				{
					// Could not convert BlockContainer to single expression
					object obj2 = ((obj is Win32Exception) ? obj : null);
					return obj2 != null && IsAccessDenied((Win32Exception)obj2);
				}).Invoke())
				{
					return false;
				}
			}
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	private readonly GtiUnavailableUiPresenter _presenter;

	private readonly IBaseGtiRunnerProvider _gtiProvider;

	public CheckForGtiOperationStartup(GtiUnavailableUiPresenter presenter, IBaseGtiRunnerProvider gtiProvider)
	{
		_presenter = presenter;
		_gtiProvider = gtiProvider;
	}

	[AsyncStateMachine(typeof(_003COnStartup_003Ed__3))]
	public System.Threading.Tasks.Task OnStartup(CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003COnStartup_003Ed__3 _003COnStartup_003Ed__ = default(_003COnStartup_003Ed__3);
		_003COnStartup_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnStartup_003Ed__._003C_003E4__this = this;
		_003COnStartup_003Ed__.shutdown = shutdown;
		_003COnStartup_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Start<_003COnStartup_003Ed__3>(ref _003COnStartup_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Task;
	}

	private static bool IsAccessDenied(Win32Exception ex)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (ex == null)
		{
			throw new ArgumentNullException("ex");
		}
		return ((System.Exception)(object)ex).HResult == -2147467259;
	}
}
