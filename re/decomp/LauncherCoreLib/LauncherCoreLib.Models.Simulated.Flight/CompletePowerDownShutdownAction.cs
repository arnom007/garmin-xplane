using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Tasks;
using LauncherCoreLib.Models.AppLifeCycle;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class CompletePowerDownShutdownAction : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnShutdown_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public CompletePowerDownShutdownAction _003C_003E4__this;

		private ConfiguredTaskAwaiter<ISimRunnerEvolved> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<ISimSessionEvolved?> _003C_003Eu__2;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CompletePowerDownShutdownAction completePowerDownShutdownAction = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<ISimRunnerEvolved> awaiter3;
				ConfiguredTaskAwaiter<ISimSessionEvolved> awaiter2;
				ConfiguredTaskAwaiter awaiter;
				ISimSessionEvolved result;
				switch (num)
				{
				default:
					awaiter3 = completePowerDownShutdownAction._sessionCoordinator.CurrentlyRunning.GetUpdatedValue().ConfigureAwait(false).GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter3;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISimRunnerEvolved>, _003COnShutdown_003Ed__3>(ref awaiter3, ref this);
						return;
					}
					goto IL_0088;
				case 0:
					awaiter3 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISimRunnerEvolved>);
					num = (_003C_003E1__state = -1);
					goto IL_0088;
				case 1:
					awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ISimSessionEvolved>);
					num = (_003C_003E1__state = -1);
					goto IL_00f4;
				case 2:
					{
						awaiter = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_018d;
					}
					IL_00f4:
					result = awaiter2.GetResult();
					if (result != null)
					{
						completePowerDownShutdownAction.Log(LogLevel.Info, "Aborting sim session.");
						result.InitiateSessionShutdown();
						completePowerDownShutdownAction.Log(LogLevel.Info, "Waiting for simulators to power down.");
						ConfiguredTaskAwaitable val = result.Completion.Catch().ConfigureAwait(false);
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__3 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003COnShutdown_003Ed__3>(ref awaiter, ref this);
							return;
						}
						goto IL_018d;
					}
					completePowerDownShutdownAction.Log(LogLevel.Info, "No simulators are running.");
					break;
					IL_0088:
					awaiter2 = awaiter3.GetResult().Sessions.GetUpdatedValue().ConfigureAwait(false).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISimSessionEvolved>, _003COnShutdown_003Ed__3>(ref awaiter2, ref this);
						return;
					}
					goto IL_00f4;
					IL_018d:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					completePowerDownShutdownAction.Log(LogLevel.Info, "Simulators powered down.");
					break;
				}
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnStartup_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public CancellationToken shutdown;

		public CompletePowerDownShutdownAction _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CompletePowerDownShutdownAction completePowerDownShutdownAction = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaitable val;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_00db;
					}
					val = shutdown.WaitForCancellation().ConfigureAwait(false);
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003COnStartup_003Ed__2>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
				}
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				val = completePowerDownShutdownAction.OnShutdown().ConfigureAwait(false);
				awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__1 = awaiter;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003COnStartup_003Ed__2>(ref awaiter, ref this);
					return;
				}
				goto IL_00db;
				IL_00db:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

	private readonly ISimSessionCoordinator _sessionCoordinator;

	public CompletePowerDownShutdownAction(ISimSessionCoordinator simRunner)
	{
		base.ActionName = "Power down on shutdown";
		_sessionCoordinator = simRunner;
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

	[AsyncStateMachine(typeof(_003COnShutdown_003Ed__3))]
	private System.Threading.Tasks.Task OnShutdown()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003COnShutdown_003Ed__3 _003COnShutdown_003Ed__ = default(_003COnShutdown_003Ed__3);
		_003COnShutdown_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnShutdown_003Ed__._003C_003E4__this = this;
		_003COnShutdown_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnShutdown_003Ed__._003C_003Et__builder)).Start<_003COnShutdown_003Ed__3>(ref _003COnShutdown_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnShutdown_003Ed__._003C_003Et__builder)).Task;
	}
}
