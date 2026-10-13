using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public class FirstRunBackgroundAction : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnStartup_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public FirstRunBackgroundAction _003C_003E4__this;

		public CancellationToken shutdown;

		private IRunOnce _003CappRunInfo_003E5__2;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private ConfiguredTaskAwaiter<IRunOnce> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			FirstRunBackgroundAction firstRunBackgroundAction = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_00c1;
				}
				ConfiguredTaskAwaiter<IRunOnce> awaiter2;
				if (num == 1)
				{
					awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<IRunOnce>);
					num = (_003C_003E1__state = -1);
					goto IL_012a;
				}
				_003CappRunInfo_003E5__2 = firstRunBackgroundAction._runOnce.GetAppFirstRunInfo();
				if (!_003CappRunInfo_003E5__2.HasCompleted())
				{
					firstRunBackgroundAction.Log(LogLevel.Info, "This is the first Launcher run. (at least since first run was added)");
					ConfiguredTaskAwaitable val = shutdown.WaitForCancellation().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003COnStartup_003Ed__2>(ref awaiter, ref this);
						return;
					}
					goto IL_00c1;
				}
				firstRunBackgroundAction.Log(LogLevel.Info, "The launcher has run before.");
				goto end_IL_000e;
				IL_012a:
				awaiter2.GetResult();
				firstRunBackgroundAction.Log(LogLevel.Info, "Recorded the first run of the launcher.");
				goto end_IL_000e;
				IL_00c1:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				awaiter2 = _003CappRunInfo_003E5__2.MarkCompleted().DetachContext().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter2;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IRunOnce>, _003COnStartup_003Ed__2>(ref awaiter2, ref this);
					return;
				}
				goto IL_012a;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CappRunInfo_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CappRunInfo_003E5__2 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	private readonly IRunOnceProvider _runOnce;

	public FirstRunBackgroundAction(IRunOnceProvider runOnce)
	{
		_runOnce = runOnce;
	}

	[AsyncStateMachine(typeof(_003COnStartup_003Ed__2))]
	public System.Threading.Tasks.Task OnStartup(CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003COnStartup_003Ed__2 _003COnStartup_003Ed__ = default(_003COnStartup_003Ed__2);
		_003COnStartup_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnStartup_003Ed__._003C_003E4__this = this;
		_003COnStartup_003Ed__.shutdown = shutdown;
		_003COnStartup_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Start<_003COnStartup_003Ed__2>(ref _003COnStartup_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Task;
	}
}
