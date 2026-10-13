using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Tasks;
using Nito.AsyncEx;

namespace LauncherCoreLib.Models.AppLifeCycle;

public class FinishPendingTasksShutdownAction : ShutdownActionBase, IPendingTaskTracker
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnShutdown_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public FinishPendingTasksShutdownAction _003C_003E4__this;

		private List<NamedTask> _003CpendingTasks_003E5__2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<System.Threading.Tasks.Task> _003C_003Eu__2;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			FinishPendingTasksShutdownAction finishPendingTasksShutdownAction = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter3;
				ConfiguredTaskAwaiter<System.Threading.Tasks.Task> awaiter2;
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaitable val;
				System.IDisposable result;
				System.Collections.Generic.IEnumerable<NamedTask> enumerable;
				switch (num)
				{
				default:
					awaiter3 = finishPendingTasksShutdownAction._taskLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter3;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003COnShutdown_003Ed__7>(ref awaiter3, ref this);
						return;
					}
					goto IL_008e;
				case 0:
					awaiter3 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_008e;
				case 1:
					awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<System.Threading.Tasks.Task>);
					num = (_003C_003E1__state = -1);
					goto IL_0184;
				case 2:
					awaiter = _003C_003Eu__3;
					_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_01e5;
				case 3:
					{
						awaiter = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_02ea;
					}
					IL_0184:
					val = awaiter2.GetResult().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__3 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003COnShutdown_003Ed__7>(ref awaiter, ref this);
						return;
					}
					goto IL_01e5;
					IL_008e:
					result = awaiter3.GetResult();
					try
					{
						finishPendingTasksShutdownAction.IsAcceptingNewTasks = true;
						_003CpendingTasks_003E5__2 = Enumerable.ToList<NamedTask>((System.Collections.Generic.IEnumerable<NamedTask>)finishPendingTasksShutdownAction._pendingTasks);
					}
					finally
					{
						if (num < 0)
						{
							result?.Dispose();
						}
					}
					finishPendingTasksShutdownAction.Log(LogLevel.Debug, "Waiting for {0} pending tasks to complete.", _003CpendingTasks_003E5__2.Count);
					awaiter2 = System.Threading.Tasks.Task.WhenAny(System.Threading.Tasks.Task.WhenAll(Enumerable.Select<NamedTask, System.Threading.Tasks.Task>((System.Collections.Generic.IEnumerable<NamedTask>)_003CpendingTasks_003E5__2, (Func<NamedTask, System.Threading.Tasks.Task>)((NamedTask nt) => nt.Task))), System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2.0))).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Threading.Tasks.Task>, _003COnShutdown_003Ed__7>(ref awaiter2, ref this);
						return;
					}
					goto IL_0184;
					IL_01e5:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					enumerable = Enumerable.Where<NamedTask>((System.Collections.Generic.IEnumerable<NamedTask>)_003CpendingTasks_003E5__2, (Func<NamedTask, bool>)((NamedTask t) => !t.Task.IsCompleted));
					if (!Enumerable.Any<NamedTask>(enumerable))
					{
						break;
					}
					finishPendingTasksShutdownAction.Log(LogLevel.Info, "Still waiting for pending tasks with names: [{0}]", string.Join(", ", Enumerable.Select<NamedTask, string>(enumerable, (Func<NamedTask, string>)((NamedTask nt) => nt.Name))));
					val = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<NamedTask, System.Threading.Tasks.Task>(enumerable, (Func<NamedTask, System.Threading.Tasks.Task>)((NamedTask nt) => nt.Task))).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 3);
						_003C_003Eu__3 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003COnShutdown_003Ed__7>(ref awaiter, ref this);
						return;
					}
					goto IL_02ea;
					IL_02ea:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					break;
				}
				finishPendingTasksShutdownAction.Log(LogLevel.Debug, "Pending tasks done.", _003CpendingTasks_003E5__2.Count);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CpendingTasks_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CpendingTasks_003E5__2 = null;
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
	private struct _003CTrackTask_003Ed__8<TResult> : IAsyncStateMachine where TResult : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TResult> _003C_003Et__builder;

		public FinishPendingTasksShutdownAction _003C_003E4__this;

		public Func<System.Threading.Tasks.Task<TResult>> startTask;

		public string taskName;

		private NamedTask _003Cinstance_003E5__2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<TResult> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			FinishPendingTasksShutdownAction finishPendingTasksShutdownAction = _003C_003E4__this;
			TResult result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_0106;
					}
					if (finishPendingTasksShutdownAction.IsAcceptingNewTasks)
					{
						throw new InvalidOperationException("Pending Task Tracker is shutdown");
					}
					awaiter = finishPendingTasksShutdownAction._taskLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CTrackTask_003Ed__8<TResult>>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
				}
				System.IDisposable result = awaiter.GetResult();
				System.Threading.Tasks.Task<TResult> task = default(System.Threading.Tasks.Task<TResult>);
				try
				{
					if (finishPendingTasksShutdownAction.IsAcceptingNewTasks)
					{
						throw new InvalidOperationException("Pending Task Tracker is shutdown");
					}
					task = ((Func<System.Threading.Tasks.Task<System.Threading.Tasks.Task<TResult>>>)(object)startTask).Invoke();
					string name = taskName ?? NamedTask.GetTaskMethodName((System.Threading.Tasks.Task)task).Item2;
					_003Cinstance_003E5__2 = new NamedTask(task, name);
					finishPendingTasksShutdownAction._pendingTasks.Add(_003Cinstance_003E5__2);
				}
				finally
				{
					if (num < 0)
					{
						result?.Dispose();
					}
				}
				goto IL_0106;
				IL_0106:
				try
				{
					ConfiguredTaskAwaiter<TResult> awaiter2;
					if (num != 1)
					{
						awaiter2 = task.DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TResult>, _003CTrackTask_003Ed__8<TResult>>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<TResult>);
						num = (_003C_003E1__state = -1);
					}
					result2 = awaiter2.GetResult();
				}
				finally
				{
					if (num < 0)
					{
						finishPendingTasksShutdownAction.UntrackTaskUnlocked(_003Cinstance_003E5__2);
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Cinstance_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Cinstance_003E5__2 = null;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly List<NamedTask> _pendingTasks = new List<NamedTask>();

	private readonly AsyncLock _taskLock = new AsyncLock();

	[field: CompilerGenerated]
	public bool IsAcceptingNewTasks
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public FinishPendingTasksShutdownAction()
	{
		base.ActionName = "Pending Tasks";
	}

	[AsyncStateMachine(typeof(_003COnShutdown_003Ed__7))]
	public override System.Threading.Tasks.Task OnShutdown()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003COnShutdown_003Ed__7 _003COnShutdown_003Ed__ = default(_003COnShutdown_003Ed__7);
		_003COnShutdown_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnShutdown_003Ed__._003C_003E4__this = this;
		_003COnShutdown_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnShutdown_003Ed__._003C_003Et__builder)).Start<_003COnShutdown_003Ed__7>(ref _003COnShutdown_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnShutdown_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CTrackTask_003Ed__8<>))]
	public async System.Threading.Tasks.Task<TResult> TrackTask<TResult>(Func<System.Threading.Tasks.Task<TResult>> startTask, [CallerMemberName] string? taskName = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (IsAcceptingNewTasks)
		{
			throw new InvalidOperationException("Pending Task Tracker is shutdown");
		}
		System.Threading.Tasks.Task<TResult> task;
		NamedTask instance;
		using (await _taskLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			if (IsAcceptingNewTasks)
			{
				throw new InvalidOperationException("Pending Task Tracker is shutdown");
			}
			task = startTask.Invoke();
			string name = taskName ?? NamedTask.GetTaskMethodName((System.Threading.Tasks.Task)task).Item2;
			instance = new NamedTask(task, name);
			_pendingTasks.Add(instance);
		}
		try
		{
			return await task.DetachContext();
		}
		finally
		{
			UntrackTaskUnlocked(instance);
		}
	}

	public IObservable<TResult> TrackStream<TResult>(IObservable<TResult> stream, [CallerMemberName] string? streamName = null)
	{
		string streamName2 = streamName;
		IObservable<TResult> stream2 = stream;
		return Observable.Create<TResult>((Func<IObserver<TResult>, System.Threading.Tasks.Task<System.IDisposable>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass9_0<>._003C_003CTrackStream_003Eb__0_003Ed))] async (IObserver<TResult> o) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			IObserver<TResult> o2 = o;
			if (IsAcceptingNewTasks)
			{
				o2.OnError((System.Exception)new InvalidOperationException("Hangar is shutdown"));
				return Disposable.Empty;
			}
			TaskCompletionSource<object?> tcs = new TaskCompletionSource<object>();
			NamedTask instance = new NamedTask(tcs.Task, streamName2 ?? GetStreamName<TResult>(stream2));
			using (await _taskLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false))
			{
				if (IsAcceptingNewTasks)
				{
					o2.OnError((System.Exception)new InvalidOperationException("Hangar is shutdown"));
					return Disposable.Empty;
				}
				_pendingTasks.Add(instance);
			}
			return new CompositeDisposable(ObservableExtensions.Subscribe<TResult>(stream2, (Action<TResult>)delegate(TResult e)
			{
				o2.OnNext(e);
			}, (Action<System.Exception>)delegate(System.Exception ex)
			{
				tcs.SetException(ex);
				UntrackTaskUnlocked(instance);
				o2.OnError(ex);
			}, (Action)delegate
			{
				tcs.SetResult((object)null);
				UntrackTaskUnlocked(instance);
				o2.OnCompleted();
			}), Disposable.Create((Action)delegate
			{
				UntrackTaskUnlocked(instance);
			}));
		}));
	}

	private static string GetStreamName<TResult>(IObservable<TResult> witness)
	{
		return ((object)witness).GetType().FullName ?? "Unknown Type";
	}

	private void UntrackTaskUnlocked(NamedTask theTask)
	{
		using (_taskLock.Lock())
		{
			while (_pendingTasks.Remove(theTask))
			{
			}
		}
	}
}
