using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.DAL;

public static class FlushHelpers
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CFlushAll_003Ed__0 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public IFlushable[] flushables;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<IFlushable, System.Threading.Tasks.Task>(Enumerable.Distinct<IFlushable>((System.Collections.Generic.IEnumerable<IFlushable>)flushables), (Func<IFlushable, System.Threading.Tasks.Task>)((IFlushable f) => f.Flush()))).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CFlushAll_003Ed__0>(ref awaiter, ref this);
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

	[AsyncStateMachine(typeof(_003CFlushAll_003Ed__0))]
	public static System.Threading.Tasks.Task FlushAll(params IFlushable[] flushables)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CFlushAll_003Ed__0 _003CFlushAll_003Ed__ = default(_003CFlushAll_003Ed__0);
		_003CFlushAll_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CFlushAll_003Ed__.flushables = flushables;
		_003CFlushAll_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CFlushAll_003Ed__._003C_003Et__builder)).Start<_003CFlushAll_003Ed__0>(ref _003CFlushAll_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CFlushAll_003Ed__._003C_003Et__builder)).Task;
	}
}
