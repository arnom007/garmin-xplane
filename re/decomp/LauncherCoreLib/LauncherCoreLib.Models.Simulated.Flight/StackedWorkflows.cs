using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal class StackedWorkflows : ISessionWorkflow
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CPostFlight_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISessionEnvironment> _003C_003Et__builder;

		public ISessionEnvironment sessionEnvironment;

		public StackedWorkflows _003C_003E4__this;

		private Stack<ISessionWorkflow> _003CappliedWorkflows_003E5__2;

		private ConfiguredTaskAwaiter<ISessionEnvironment> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			StackedWorkflows stackedWorkflows = _003C_003E4__this;
			ISessionEnvironment result;
			try
			{
				ISessionEnvironment sessionEnvironment;
				if (num != 0)
				{
					_003CappliedWorkflows_003E5__2 = this.sessionEnvironment.GetWorkflowState<Stack<ISessionWorkflow>>(stackedWorkflows._attachedWorkflowsKey);
					sessionEnvironment = this.sessionEnvironment.DetachWorkflowState(stackedWorkflows._attachedWorkflowsKey);
					goto IL_00ac;
				}
				ConfiguredTaskAwaiter<ISessionEnvironment> awaiter = _003C_003Eu__1;
				_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISessionEnvironment>);
				num = (_003C_003E1__state = -1);
				goto IL_00a4;
				IL_00a4:
				sessionEnvironment = awaiter.GetResult();
				goto IL_00ac;
				IL_00ac:
				if (_003CappliedWorkflows_003E5__2 != null && _003CappliedWorkflows_003E5__2.Count > 0)
				{
					awaiter = _003CappliedWorkflows_003E5__2.Pop().PostFlight(sessionEnvironment).DetachContext()
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISessionEnvironment>, _003CPostFlight_003Ed__4>(ref awaiter, ref this);
						return;
					}
					goto IL_00a4;
				}
				result = sessionEnvironment;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CappliedWorkflows_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CappliedWorkflows_003E5__2 = null;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CPreFlight_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISessionEnvironment> _003C_003Et__builder;

		public ISessionEnvironment sessionEnvironment;

		public StackedWorkflows _003C_003E4__this;

		public CancellationToken token;

		private ISessionEnvironment _003Cenv_003E5__2;

		private Stack<ISessionWorkflow> _003CappliedWorkflows_003E5__3;

		private ExceptionDispatchInfo _003CerrorInPreflight_003E5__4;

		private Enumerator<ISessionWorkflow> _003C_003E7__wrap4;

		private ISessionWorkflow _003Csession_003E5__6;

		private ConfiguredTaskAwaiter<ISessionEnvironment> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			StackedWorkflows stackedWorkflows = _003C_003E4__this;
			ISessionEnvironment result2;
			try
			{
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_017a;
					}
					_003Cenv_003E5__2 = sessionEnvironment;
					_003CappliedWorkflows_003E5__3 = new Stack<ISessionWorkflow>();
					_003CerrorInPreflight_003E5__4 = null;
					_003C_003E7__wrap4 = stackedWorkflows._sessions.GetEnumerator();
				}
				try
				{
					if (num == 0)
					{
						goto IL_0062;
					}
					goto IL_011d;
					IL_011d:
					if (_003C_003E7__wrap4.MoveNext())
					{
						_003Csession_003E5__6 = _003C_003E7__wrap4.Current;
						goto IL_0062;
					}
					goto end_IL_0048;
					IL_0062:
					try
					{
						ConfiguredTaskAwaiter<ISessionEnvironment> awaiter;
						if (num != 0)
						{
							awaiter = _003Csession_003E5__6.PreFlight(_003Cenv_003E5__2, token).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISessionEnvironment>, _003CPreFlight_003Ed__3>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISessionEnvironment>);
							num = (_003C_003E1__state = -1);
						}
						ISessionEnvironment result = awaiter.GetResult();
						_003Cenv_003E5__2 = result;
					}
					catch (System.Exception ex)
					{
						_003CerrorInPreflight_003E5__4 = ExceptionDispatchInfo.Capture(ex);
						goto end_IL_0048;
					}
					_003CappliedWorkflows_003E5__3.Push(_003Csession_003E5__6);
					if (!((CancellationToken)(ref token)).IsCancellationRequested)
					{
						_003Csession_003E5__6 = null;
						goto IL_011d;
					}
					end_IL_0048:;
				}
				finally
				{
					if (num < 0)
					{
						((System.IDisposable)_003C_003E7__wrap4).Dispose();
					}
				}
				_003C_003E7__wrap4 = default(Enumerator<ISessionWorkflow>);
				_003Cenv_003E5__2 = _003Cenv_003E5__2.AttachWorkflowState(stackedWorkflows._attachedWorkflowsKey, _003CappliedWorkflows_003E5__3);
				if (_003CerrorInPreflight_003E5__4 != null)
				{
					goto IL_017a;
				}
				goto IL_0221;
				IL_0221:
				result2 = _003Cenv_003E5__2;
				goto end_IL_000e;
				IL_017a:
				try
				{
					ConfiguredTaskAwaiter<ISessionEnvironment> awaiter;
					if (num != 1)
					{
						awaiter = stackedWorkflows.PostFlight(_003Cenv_003E5__2).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISessionEnvironment>, _003CPreFlight_003Ed__3>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISessionEnvironment>);
						num = (_003C_003E1__state = -1);
					}
					ISessionEnvironment result = awaiter.GetResult();
					_003Cenv_003E5__2 = result;
				}
				catch (System.Exception ex2)
				{
					throw new AggregateException(new System.Exception[2] { _003CerrorInPreflight_003E5__4.SourceException, ex2 });
				}
				_003CerrorInPreflight_003E5__4.Throw();
				goto IL_0221;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Cenv_003E5__2 = null;
				_003CappliedWorkflows_003E5__3 = null;
				_003CerrorInPreflight_003E5__4 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Cenv_003E5__2 = null;
			_003CappliedWorkflows_003E5__3 = null;
			_003CerrorInPreflight_003E5__4 = null;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly List<ISessionWorkflow> _sessions = new List<ISessionWorkflow>();

	private readonly object _attachedWorkflowsKey = new object();

	public void AddWorkflow(ISessionWorkflow workflow)
	{
		_sessions.Add(workflow);
	}

	[AsyncStateMachine(typeof(_003CPreFlight_003Ed__3))]
	public async System.Threading.Tasks.Task<ISessionEnvironment> PreFlight(ISessionEnvironment sessionEnvironment, CancellationToken token)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		ISessionEnvironment env = sessionEnvironment;
		Stack<ISessionWorkflow> appliedWorkflows = new Stack<ISessionWorkflow>();
		ExceptionDispatchInfo errorInPreflight = null;
		Enumerator<ISessionWorkflow> enumerator = _sessions.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ISessionWorkflow session = enumerator.Current;
				try
				{
					env = await session.PreFlight(env, token).DetachContext();
				}
				catch (System.Exception ex)
				{
					errorInPreflight = ExceptionDispatchInfo.Capture(ex);
					break;
				}
				appliedWorkflows.Push(session);
				if (((CancellationToken)(ref token)).IsCancellationRequested)
				{
					break;
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator).Dispose();
		}
		env = env.AttachWorkflowState(_attachedWorkflowsKey, appliedWorkflows);
		if (errorInPreflight != null)
		{
			try
			{
				env = await PostFlight(env).DetachContext();
			}
			catch (System.Exception ex2)
			{
				throw new AggregateException(new System.Exception[2] { errorInPreflight.SourceException, ex2 });
			}
			errorInPreflight.Throw();
		}
		return env;
	}

	[AsyncStateMachine(typeof(_003CPostFlight_003Ed__4))]
	public async System.Threading.Tasks.Task<ISessionEnvironment> PostFlight(ISessionEnvironment sessionEnvironment)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Stack<ISessionWorkflow> appliedWorkflows = sessionEnvironment.GetWorkflowState<Stack<ISessionWorkflow>>(_attachedWorkflowsKey);
		ISessionEnvironment sessionEnvironment2 = sessionEnvironment.DetachWorkflowState(_attachedWorkflowsKey);
		while (appliedWorkflows != null && appliedWorkflows.Count > 0)
		{
			sessionEnvironment2 = await appliedWorkflows.Pop().PostFlight(sessionEnvironment2).DetachContext();
		}
		return sessionEnvironment2;
	}
}
