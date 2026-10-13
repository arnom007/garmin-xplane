using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Reactive;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public static class StartupRegistryExtensions
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass0_0
	{
		public IStartupRegistry startupRegistry;

		public System.Type actionType;

		internal void _003CWaitForStartupActionByType_003Eb__0(EventHandler h)
		{
			startupRegistry.StartupActionAdded += h;
		}

		internal void _003CWaitForStartupActionByType_003Eb__1(EventHandler h)
		{
			startupRegistry.StartupActionAdded -= h;
		}

		internal System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord> _003CWaitForStartupActionByType_003Eb__2(TimingEvent _)
		{
			return (System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)startupRegistry.GetStartupActions(null, actionType);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CWaitForStartupActionByType_003Ed__0 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AppLifeCycleActionRecord> _003C_003Et__builder;

		public IStartupRegistry startupRegistry;

		public System.Type actionType;

		private object _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			AppLifeCycleActionRecord result;
			try
			{
				AsyncSubject<AppLifeCycleActionRecord> asyncSubject;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass0_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass0_0
					{
						startupRegistry = startupRegistry,
						actionType = actionType
					};
					if (CS_0024_003C_003E8__locals0.actionType == (System.Type)null)
					{
						throw new ArgumentNullException("actionType");
					}
					asyncSubject = TimingEvent.StartStreamAndConcat<EventPattern<object>>(Observable.FromEventPattern(delegate(EventHandler h)
					{
						CS_0024_003C_003E8__locals0.startupRegistry.StartupActionAdded += h;
					}, delegate(EventHandler h)
					{
						CS_0024_003C_003E8__locals0.startupRegistry.StartupActionAdded -= h;
					}), "Startup action check trigger").SelectMany<TimingEvent, AppLifeCycleActionRecord>((Func<TimingEvent, System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>>)((TimingEvent _) => (System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)CS_0024_003C_003E8__locals0.startupRegistry.GetStartupActions(null, CS_0024_003C_003E8__locals0.actionType))).Where<AppLifeCycleActionRecord>((Func<AppLifeCycleActionRecord, bool>)((AppLifeCycleActionRecord action) => action != null))
						.FirstOrDefaultAsync<AppLifeCycleActionRecord>()
						.GetAwaiter<AppLifeCycleActionRecord>();
					if (!asyncSubject.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = asyncSubject;
						_003C_003Et__builder.AwaitOnCompleted<AsyncSubject<AppLifeCycleActionRecord>, _003CWaitForStartupActionByType_003Ed__0>(ref asyncSubject, ref this);
						return;
					}
				}
				else
				{
					asyncSubject = (AsyncSubject<AppLifeCycleActionRecord>)_003C_003Eu__1;
					_003C_003Eu__1 = null;
					num = (_003C_003E1__state = -1);
				}
				result = asyncSubject.GetResult();
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
	private struct _003CWaitForStartupActionToComplete_003Ed__2<TActionType> : IAsyncStateMachine where TActionType : notnull, IStartupAction
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AppLifeCycleActionRecord> _003C_003Et__builder;

		public IStartupRegistry startupRegistry;

		private AppLifeCycleActionRecord _003Crecord_003E5__2;

		private ConfiguredTaskAwaiter<AppLifeCycleActionRecord> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			AppLifeCycleActionRecord result2;
			try
			{
				ConfiguredTaskAwaiter<AppLifeCycleActionRecord> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_0080;
					}
					awaiter = startupRegistry.WaitForStartupActionByType<TActionType>().DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AppLifeCycleActionRecord>, _003CWaitForStartupActionToComplete_003Ed__2<TActionType>>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AppLifeCycleActionRecord>);
					num = (_003C_003E1__state = -1);
				}
				AppLifeCycleActionRecord result = awaiter.GetResult();
				_003Crecord_003E5__2 = result;
				goto IL_0080;
				IL_0080:
				try
				{
					ConfiguredTaskAwaiter awaiter2;
					if (num != 1)
					{
						ConfiguredTaskAwaitable val = _003Crecord_003E5__2.Completion.DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CWaitForStartupActionToComplete_003Ed__2<TActionType>>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
				}
				catch (System.Exception ex) when (!(ex is OperationCanceledException))
				{
					throw new AppLifeCycleActionFailedException(_003Crecord_003E5__2, ex);
				}
				result2 = _003Crecord_003E5__2;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Crecord_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Crecord_003E5__2 = null;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[AsyncStateMachine(typeof(_003CWaitForStartupActionByType_003Ed__0))]
	public static async System.Threading.Tasks.Task<AppLifeCycleActionRecord> WaitForStartupActionByType(this IStartupRegistry startupRegistry, System.Type actionType)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		IStartupRegistry startupRegistry2 = startupRegistry;
		System.Type actionType2 = actionType;
		if (actionType2 == (System.Type)null)
		{
			throw new ArgumentNullException("actionType");
		}
		return await TimingEvent.StartStreamAndConcat<EventPattern<object>>(Observable.FromEventPattern(delegate(EventHandler h)
		{
			startupRegistry2.StartupActionAdded += h;
		}, delegate(EventHandler h)
		{
			startupRegistry2.StartupActionAdded -= h;
		}), "Startup action check trigger").SelectMany<TimingEvent, AppLifeCycleActionRecord>((Func<TimingEvent, System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>>)((TimingEvent _) => (System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)startupRegistry2.GetStartupActions(null, actionType2))).Where<AppLifeCycleActionRecord>((Func<AppLifeCycleActionRecord, bool>)((AppLifeCycleActionRecord action) => action != null))
			.FirstOrDefaultAsync<AppLifeCycleActionRecord>();
	}

	public static System.Threading.Tasks.Task<AppLifeCycleActionRecord> WaitForStartupActionByType<TActionType>(this IStartupRegistry startupRegistry) where TActionType : IStartupAction
	{
		return startupRegistry.WaitForStartupActionByType(typeof(TActionType));
	}

	[AsyncStateMachine(typeof(_003CWaitForStartupActionToComplete_003Ed__2<>))]
	public static async System.Threading.Tasks.Task<AppLifeCycleActionRecord> WaitForStartupActionToComplete<TActionType>(this IStartupRegistry startupRegistry) where TActionType : IStartupAction
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AppLifeCycleActionRecord record = await startupRegistry.WaitForStartupActionByType<TActionType>().DetachContext();
		try
		{
			await record.Completion.DetachContext();
			return record;
		}
		catch (System.Exception ex) when (!(ex is OperationCanceledException))
		{
			throw new AppLifeCycleActionFailedException(record, ex);
		}
	}
}
