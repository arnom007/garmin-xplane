using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Tasks;
using LauncherCoreLib.Models.AppLifeCycle;
using LauncherCoreLib.Models.DAL;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class PersistHangarOnShutdown : ShutdownActionBase
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnShutdown_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public PersistHangarOnShutdown _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PersistHangarOnShutdown persistHangarOnShutdown = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = persistHangarOnShutdown._unitofWork.FlushEverything().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003COnShutdown_003Ed__3>(ref awaiter, ref this);
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
				persistHangarOnShutdown.Log(LogLevel.Info, "Flushed all data successfully.");
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

	private const string ACTION_NAME = "Store Hangar";

	private readonly IUnitOfWork _unitofWork;

	public PersistHangarOnShutdown(IUnitOfWork unitOfWork)
	{
		_unitofWork = unitOfWork;
		base.ActionName = "Store Hangar";
	}

	[AsyncStateMachine(typeof(_003COnShutdown_003Ed__3))]
	public override System.Threading.Tasks.Task OnShutdown()
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
