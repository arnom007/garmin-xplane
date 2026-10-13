using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal class DefaultFlightSession : IFlightSession
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CPostFlight_003Ed__9 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public DefaultFlightSession _003C_003E4__this;

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
			DefaultFlightSession defaultFlightSession = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<ISessionEnvironment> awaiter;
				if (num != 0)
				{
					awaiter = defaultFlightSession._workflow.PostFlight(defaultFlightSession.Environment).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISessionEnvironment>, _003CPostFlight_003Ed__9>(ref awaiter, ref this);
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
				defaultFlightSession.Environment = result;
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
	private struct _003CPreFlight_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public DefaultFlightSession _003C_003E4__this;

		public CancellationToken token;

		private ConfiguredTaskAwaiter<ISessionEnvironment> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DefaultFlightSession defaultFlightSession = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<ISessionEnvironment> awaiter;
				if (num != 0)
				{
					awaiter = defaultFlightSession._workflow.PreFlight(defaultFlightSession.Environment, token).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISessionEnvironment>, _003CPreFlight_003Ed__7>(ref awaiter, ref this);
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
				defaultFlightSession.Environment = result;
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

	private readonly ISessionWorkflow _workflow;

	private readonly ISimSessionFactory _simSessionFactory;

	[field: CompilerGenerated]
	public ISessionEnvironment Environment
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public DefaultFlightSession(ISessionEnvironment startingEnvironment, ISessionWorkflow workflow, ISimSessionFactory simSessionFactory)
	{
		_workflow = workflow;
		Environment = startingEnvironment;
		_simSessionFactory = simSessionFactory;
	}

	[AsyncStateMachine(typeof(_003CPreFlight_003Ed__7))]
	public System.Threading.Tasks.Task PreFlight(CancellationToken token)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003CPreFlight_003Ed__7 _003CPreFlight_003Ed__ = default(_003CPreFlight_003Ed__7);
		_003CPreFlight_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CPreFlight_003Ed__._003C_003E4__this = this;
		_003CPreFlight_003Ed__.token = token;
		_003CPreFlight_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CPreFlight_003Ed__._003C_003Et__builder)).Start<_003CPreFlight_003Ed__7>(ref _003CPreFlight_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CPreFlight_003Ed__._003C_003Et__builder)).Task;
	}

	public System.Threading.Tasks.Task<SimSession> TakeOff()
	{
		return _simSessionFactory.CreateRouterAndSimulators(Environment);
	}

	[AsyncStateMachine(typeof(_003CPostFlight_003Ed__9))]
	public System.Threading.Tasks.Task PostFlight()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CPostFlight_003Ed__9 _003CPostFlight_003Ed__ = default(_003CPostFlight_003Ed__9);
		_003CPostFlight_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CPostFlight_003Ed__._003C_003E4__this = this;
		_003CPostFlight_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CPostFlight_003Ed__._003C_003Et__builder)).Start<_003CPostFlight_003Ed__9>(ref _003CPostFlight_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CPostFlight_003Ed__._003C_003Et__builder)).Task;
	}
}
