using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.Logging;
using LOLA.Processes;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

public sealed class SimSession : ITrainingSession, System.IDisposable
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CSimulatorExited_003Ed__23 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public IProcessSession process;

		private ConfiguredTaskAwaiter<ProcessResult> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter<ProcessResult> awaiter;
				if (num != 0)
				{
					awaiter = process.Exited.DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ProcessResult>, _003CSimulatorExited_003Ed__23>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ProcessResult>);
					num = (_003C_003E1__state = -1);
				}
				ProcessResult result = awaiter.GetResult();
				Logger.Log(LogLevel.Info, ProcessLogFormatting.LogExitWithSquareBracketNotation(result));
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
	private struct _003CWaitForAllSimsToClose_003Ed__22 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SimSession _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SimSession simSession = _003C_003E4__this;
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
						goto IL_00e9;
					}
					val = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<IProcessSession, System.Threading.Tasks.Task>(simSession.SimProcesses, (Func<IProcessSession, System.Threading.Tasks.Task>)simSession.SimulatorExited)).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CWaitForAllSimsToClose_003Ed__22>(ref awaiter, ref this);
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
				val = WaitForCrashReportersToFinish().DetachContext();
				awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__1 = awaiter;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CWaitForAllSimsToClose_003Ed__22>(ref awaiter, ref this);
					return;
				}
				goto IL_00e9;
				IL_00e9:
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CWaitForCrashReportersToFinish_003Ed__21 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		private ConfiguredTaskAwaiter<ProcessResult[]> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter<ProcessResult[]> awaiter;
				if (num != 0)
				{
					awaiter = System.Threading.Tasks.Task.WhenAll<ProcessResult>((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<ProcessResult>>)Enumerable.ToList<System.Threading.Tasks.Task<ProcessResult>>(Enumerable.Select<ProcessSession, System.Threading.Tasks.Task<ProcessResult>>(Enumerable.Select<Process, ProcessSession>(Enumerable.Where<Process>((System.Collections.Generic.IEnumerable<Process>)Process.GetProcesses(), (Func<Process, bool>)((Process p) => p.ProcessName.StartsWith("CrashSender", (StringComparison)4))), (Func<Process, ProcessSession>)ProcessSession.FromRunningProcess), (Func<ProcessSession, System.Threading.Tasks.Task<ProcessResult>>)((ProcessSession p) => p.Exited)))).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ProcessResult[]>, _003CWaitForCrashReportersToFinish_003Ed__21>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ProcessResult[]>);
					num = (_003C_003E1__state = -1);
				}
				awaiter.GetResult();
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

	[field: CompilerGenerated]
	public IDataRouterSession Router
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public System.Collections.Generic.IReadOnlyList<SimProcessSession> Simulators
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public int SessionId
	{
		[CompilerGenerated]
		get;
	}

	public System.Collections.Generic.IEnumerable<IProcessSession> AllProcesses => Enumerable.Concat<IProcessSession>(SimProcesses, Router.RouterProcess.Yield());

	[field: CompilerGenerated]
	public System.Threading.Tasks.Task AllSimsExited
	{
		[CompilerGenerated]
		get;
	}

	private System.Collections.Generic.IEnumerable<IProcessSession> SimProcesses => Enumerable.Select<SimProcessSession, IProcessSession>((System.Collections.Generic.IEnumerable<SimProcessSession>)Simulators, (Func<SimProcessSession, IProcessSession>)((SimProcessSession s) => s.Process));

	public SimSession(IDataRouterSession router, System.Collections.Generic.IReadOnlyList<SimProcessSession> simulators, int sessionId)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Router = router ?? throw new ArgumentNullException("router");
		Simulators = simulators ?? throw new ArgumentNullException("simulators");
		SessionId = sessionId;
		AllSimsExited = WaitForAllSimsToClose();
	}

	public void CloseRouter()
	{
		IProcessSession routerProcess = Router.RouterProcess;
		if (!routerProcess.HasExited() && !routerProcess.RequestClose())
		{
			routerProcess.ForceClose();
		}
	}

	public void CloseAllSims()
	{
		System.Collections.Generic.IEnumerator<IProcessSession> enumerator = Enumerable.Where<IProcessSession>(SimProcesses, (Func<IProcessSession, bool>)((IProcessSession p) => !p.HasExited())).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				IProcessSession current = enumerator.Current;
				if (!current.RequestClose())
				{
					current.ForceClose();
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	public void ForceCloseAll()
	{
		System.Collections.Generic.IEnumerator<IProcessSession> enumerator = Enumerable.Where<IProcessSession>(AllProcesses, (Func<IProcessSession, bool>)((IProcessSession p) => !p.HasExited())).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				enumerator.Current.ForceClose();
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	public void Dispose()
	{
		((System.IDisposable)Router).Dispose();
		System.Collections.Generic.IEnumerator<IProcessSession> enumerator = AllProcesses.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				((System.IDisposable)enumerator.Current).Dispose();
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		GC.SuppressFinalize((object)this);
	}

	[AsyncStateMachine(typeof(_003CWaitForCrashReportersToFinish_003Ed__21))]
	private static System.Threading.Tasks.Task WaitForCrashReportersToFinish()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CWaitForCrashReportersToFinish_003Ed__21 _003CWaitForCrashReportersToFinish_003Ed__ = default(_003CWaitForCrashReportersToFinish_003Ed__21);
		_003CWaitForCrashReportersToFinish_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CWaitForCrashReportersToFinish_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CWaitForCrashReportersToFinish_003Ed__._003C_003Et__builder)).Start<_003CWaitForCrashReportersToFinish_003Ed__21>(ref _003CWaitForCrashReportersToFinish_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CWaitForCrashReportersToFinish_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CWaitForAllSimsToClose_003Ed__22))]
	private System.Threading.Tasks.Task WaitForAllSimsToClose()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CWaitForAllSimsToClose_003Ed__22 _003CWaitForAllSimsToClose_003Ed__ = default(_003CWaitForAllSimsToClose_003Ed__22);
		_003CWaitForAllSimsToClose_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CWaitForAllSimsToClose_003Ed__._003C_003E4__this = this;
		_003CWaitForAllSimsToClose_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CWaitForAllSimsToClose_003Ed__._003C_003Et__builder)).Start<_003CWaitForAllSimsToClose_003Ed__22>(ref _003CWaitForAllSimsToClose_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CWaitForAllSimsToClose_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CSimulatorExited_003Ed__23))]
	private System.Threading.Tasks.Task SimulatorExited(IProcessSession process)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CSimulatorExited_003Ed__23 _003CSimulatorExited_003Ed__ = default(_003CSimulatorExited_003Ed__23);
		_003CSimulatorExited_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CSimulatorExited_003Ed__.process = process;
		_003CSimulatorExited_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CSimulatorExited_003Ed__._003C_003Et__builder)).Start<_003CSimulatorExited_003Ed__23>(ref _003CSimulatorExited_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CSimulatorExited_003Ed__._003C_003Et__builder)).Task;
	}
}
