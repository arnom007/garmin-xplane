using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal class EnhancedSessionWorkflow : ISessionWorkflow, IEnhanceSession
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CPostFlight_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISessionEnvironment> _003C_003Et__builder;

		public EnhancedSessionWorkflow _003C_003E4__this;

		public ISessionEnvironment sessionEnvironment;

		private ConfiguredTaskAwaiter<ISessionEnvironment> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			EnhancedSessionWorkflow enhancedSessionWorkflow = _003C_003E4__this;
			ISessionEnvironment result;
			try
			{
				ConfiguredTaskAwaiter<ISessionEnvironment> awaiter;
				if (num != 0)
				{
					awaiter = enhancedSessionWorkflow._sessionEnhancements.PostFlight(sessionEnvironment).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISessionEnvironment>, _003CPostFlight_003Ed__3>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISessionEnvironment>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult();
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
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
	private struct _003CPreFlight_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISessionEnvironment> _003C_003Et__builder;

		public CancellationToken token;

		public EnhancedSessionWorkflow _003C_003E4__this;

		public ISessionEnvironment sessionEnvironment;

		private ConfiguredTaskAwaiter<ISessionEnvironment> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			EnhancedSessionWorkflow enhancedSessionWorkflow = _003C_003E4__this;
			ISessionEnvironment result;
			try
			{
				ConfiguredTaskAwaiter<ISessionEnvironment> awaiter;
				if (num != 0)
				{
					((CancellationToken)(ref token)).ThrowIfCancellationRequested();
					awaiter = enhancedSessionWorkflow._sessionEnhancements.PreFlight(sessionEnvironment, token).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISessionEnvironment>, _003CPreFlight_003Ed__2>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISessionEnvironment>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult();
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly StackedWorkflows _sessionEnhancements = new StackedWorkflows();

	public void AddWorkflow(ISessionWorkflow workflow)
	{
		_sessionEnhancements.AddWorkflow(workflow);
	}

	[AsyncStateMachine(typeof(_003CPreFlight_003Ed__2))]
	public async System.Threading.Tasks.Task<ISessionEnvironment> PreFlight(ISessionEnvironment sessionEnvironment, CancellationToken token)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		((CancellationToken)(ref token)).ThrowIfCancellationRequested();
		return await _sessionEnhancements.PreFlight(sessionEnvironment, token).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CPostFlight_003Ed__3))]
	public async System.Threading.Tasks.Task<ISessionEnvironment> PostFlight(ISessionEnvironment sessionEnvironment)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await _sessionEnhancements.PostFlight(sessionEnvironment).DetachContext();
	}
}
