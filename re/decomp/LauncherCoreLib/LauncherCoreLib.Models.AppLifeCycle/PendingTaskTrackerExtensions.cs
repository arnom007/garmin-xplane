using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public static class PendingTaskTrackerExtensions
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass1_0<TResult> where TResult : notnull
	{
		public System.Threading.Tasks.Task<TResult> pendingTask;

		internal System.Threading.Tasks.Task<TResult> _003CTrackTask_003Eb__0()
		{
			return pendingTask;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass2_0
	{
		public System.Threading.Tasks.Task pendingTask;

		internal System.Threading.Tasks.Task _003CTrackTask_003Eb__0()
		{
			return pendingTask;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CTrackTask_003Ed__1<TResult> : IAsyncStateMachine where TResult : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TResult> _003C_003Et__builder;

		public System.Threading.Tasks.Task<TResult> pendingTask;

		public IPendingTaskTracker tracker;

		private _003C_003Ec__DisplayClass1_0<TResult> _003C_003E8__1;

		private System.Exception _003CtrackerError_003E5__2;

		private ConfiguredTaskAwaiter<TResult> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00be: Expected O, but got Unknown
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			TResult result;
			try
			{
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_00da;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass1_0<TResult>();
					_003C_003E8__1.pendingTask = pendingTask;
					_003CtrackerError_003E5__2 = null;
				}
				try
				{
					ConfiguredTaskAwaiter<TResult> awaiter;
					if (num != 0)
					{
						awaiter = tracker.TrackTask<TResult>((Func<System.Threading.Tasks.Task<TResult>>)(object)(Func<System.Threading.Tasks.Task<System.Threading.Tasks.Task<TResult>>>)(() => _003C_003E8__1.pendingTask), "TrackTask").DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TResult>, _003CTrackTask_003Ed__1<TResult>>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TResult>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter.GetResult();
				}
				catch (InvalidOperationException val)
				{
					InvalidOperationException val2 = val;
					if (!tracker.IsAcceptingNewTasks)
					{
						_003CtrackerError_003E5__2 = (System.Exception)(object)val2;
						goto IL_00da;
					}
					throw;
				}
				goto end_IL_0007;
				IL_00da:
				try
				{
					ConfiguredTaskAwaiter<TResult> awaiter;
					if (num != 1)
					{
						awaiter = _003C_003E8__1.pendingTask.DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TResult>, _003CTrackTask_003Ed__1<TResult>>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TResult>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter.GetResult();
				}
				catch (System.Exception ex)
				{
					if (_003CtrackerError_003E5__2 != null)
					{
						throw new AggregateException(new System.Exception[2] { ex, _003CtrackerError_003E5__2 });
					}
					throw;
				}
				end_IL_0007:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CtrackerError_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CtrackerError_003E5__2 = null;
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
	private struct _003CTrackTask_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public System.Threading.Tasks.Task pendingTask;

		public IPendingTaskTracker tracker;

		private _003C_003Ec__DisplayClass2_0 _003C_003E8__1;

		private System.Exception _003CtrackerError_003E5__2;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00b9: Expected O, but got Unknown
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_00d4;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass2_0();
					_003C_003E8__1.pendingTask = pendingTask;
					_003CtrackerError_003E5__2 = null;
				}
				ConfiguredTaskAwaitable val;
				ConfiguredTaskAwaiter awaiter;
				try
				{
					if (num != 0)
					{
						val = tracker.TrackTask(() => _003C_003E8__1.pendingTask, "TrackTask").DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CTrackTask_003Ed__2>(ref awaiter, ref this);
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
				catch (InvalidOperationException val2)
				{
					InvalidOperationException val3 = val2;
					if (tracker.IsAcceptingNewTasks)
					{
						throw;
					}
					_003CtrackerError_003E5__2 = (System.Exception)(object)val3;
				}
				goto IL_00d4;
				IL_00d4:
				try
				{
					if (num != 1)
					{
						val = _003C_003E8__1.pendingTask.DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CTrackTask_003Ed__2>(ref awaiter, ref this);
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
				catch (System.Exception ex)
				{
					if (_003CtrackerError_003E5__2 != null)
					{
						throw new AggregateException(new System.Exception[2] { ex, _003CtrackerError_003E5__2 });
					}
					throw;
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CtrackerError_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CtrackerError_003E5__2 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	public static System.Threading.Tasks.Task TrackTask(this IPendingTaskTracker tracker, Func<System.Threading.Tasks.Task> startTask, [CallerMemberName] string? taskName = null)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Func<System.Threading.Tasks.Task> startTask2 = startTask;
		return tracker.TrackTask<int>([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass0_0._003C_003CTrackTask_003Eb__0_003Ed))] async () =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			await startTask2.Invoke().DetachContext();
			return 0;
		}, taskName ?? NamedTask.GetTaskMethodName(startTask2).Item2);
	}

	[AsyncStateMachine(typeof(_003CTrackTask_003Ed__1<>))]
	public static async System.Threading.Tasks.Task<TResult> TrackTask<TResult>(this IPendingTaskTracker tracker, System.Threading.Tasks.Task<TResult> pendingTask)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		System.Threading.Tasks.Task<TResult> pendingTask2 = pendingTask;
		System.Exception trackerError;
		try
		{
			return await tracker.TrackTask<TResult>(() => pendingTask2, "TrackTask").DetachContext();
		}
		catch (InvalidOperationException val)
		{
			InvalidOperationException val2 = val;
			if (tracker.IsAcceptingNewTasks)
			{
				throw;
			}
			trackerError = (System.Exception)(object)val2;
		}
		try
		{
			return await pendingTask2.DetachContext();
		}
		catch (System.Exception ex)
		{
			if (trackerError != null)
			{
				throw new AggregateException(new System.Exception[2] { ex, trackerError });
			}
			throw;
		}
	}

	[AsyncStateMachine(typeof(_003CTrackTask_003Ed__2))]
	public static System.Threading.Tasks.Task TrackTask(this IPendingTaskTracker tracker, System.Threading.Tasks.Task pendingTask)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CTrackTask_003Ed__2 _003CTrackTask_003Ed__ = default(_003CTrackTask_003Ed__2);
		_003CTrackTask_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CTrackTask_003Ed__.tracker = tracker;
		_003CTrackTask_003Ed__.pendingTask = pendingTask;
		_003CTrackTask_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CTrackTask_003Ed__._003C_003Et__builder)).Start<_003CTrackTask_003Ed__2>(ref _003CTrackTask_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CTrackTask_003Ed__._003C_003Et__builder)).Task;
	}
}
