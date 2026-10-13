using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using IO.Win32.Events;
using LOLA.Processes;
using LOLA.Reactive;
using LOLA.Tasks;
using LanguageExt;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class Win32EventsAdapter
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass0_0
	{
		public IOsWindowOperations windowing;

		public IMessagePumpThreadDispatcher messagePumpThread;

		internal IObservable<TrainerWindowScreenCoordinates> _003CCreateWindowWatcher_003Eb__0(SimProcessSession session)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return TimingEvent.StartStreamAndConcat<TimingEvent>(WhenProcessMainWindowMoves(session.Process).Throttle<TimingEvent>(TimeSpan.FromMilliseconds(500.0))).SelectCurrentWindow<TimingEvent>(windowing, session);
			IObservable<TimingEvent> WhenProcessMainWindowMoves(IProcessSession process)
			{
				_003C_003Ec__DisplayClass0_1 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass0_1
				{
					process = process
				};
				return WindowHandleEvents(CS_0024_003C_003E8__locals1.process, EventId.EVENT_OBJECT_SHOW, new EventId[1] { EventId.EVENT_OBJECT_LOCATIONCHANGE }).Where<System.IntPtr>((Func<System.IntPtr, bool>)((System.IntPtr ip) => ip == CS_0024_003C_003E8__locals1.process.MainWindowHandle)).ToTimingEventStream<System.IntPtr>();
			}
			IObservable<System.IntPtr> WindowHandleEvents(IProcessSession process, EventId eventId, EventId[] extras)
			{
				_003C_003Ec__DisplayClass0_2 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass0_2
				{
					CS_0024_003C_003E8__locals1 = this,
					process = process
				};
				return (from eventId in Prelude.Seq1(eventId).Concat(extras)
					select WindowHandleEventsRange(CS_0024_003C_003E8__locals0.process, eventId, eventId)).Merge();
			}
			IObservable<System.IntPtr> WindowHandleEventsRange(IProcessSession process, EventId eventIdMin, EventId eventIdMax)
			{
				return Observable.Create<System.IntPtr>((Func<IObserver<System.IntPtr>, CancellationToken, System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass0_3
				{
					CS_0024_003C_003E8__locals2 = this,
					eventIdMin = eventIdMin,
					eventIdMax = eventIdMax,
					process = process
				}._003CCreateWindowWatcher_003Eb__6);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass0_1
	{
		public IProcessSession process;

		internal bool _003CCreateWindowWatcher_003Eb__4(System.IntPtr ip)
		{
			return ip == process.MainWindowHandle;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass0_2
	{
		public IProcessSession process;

		public _003C_003Ec__DisplayClass0_0 CS_0024_003C_003E8__locals1;

		internal IObservable<System.IntPtr> _003CCreateWindowWatcher_003Eb__5(EventId eventId)
		{
			return WindowHandleEventsRange(process, eventId, eventId);
			IObservable<System.IntPtr> WindowHandleEventsRange(IProcessSession process, EventId eventIdMin, EventId eventIdMax)
			{
				return Observable.Create<System.IntPtr>((Func<IObserver<System.IntPtr>, CancellationToken, System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass0_3
				{
					CS_0024_003C_003E8__locals2 = this,
					eventIdMin = eventIdMin,
					eventIdMax = eventIdMax,
					process = process
				}._003CCreateWindowWatcher_003Eb__6);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass0_3
	{
		[StructLayout(3)]
		private struct _003C_003CCreateWindowWatcher_003Eb__6_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public IObserver<System.IntPtr> o;

			public _003C_003Ec__DisplayClass0_3 _003C_003E4__this;

			public CancellationToken c;

			private _003C_003Ec__DisplayClass0_4 _003C_003E8__1;

			private System.IDisposable _003CeventSubscription_003E5__2;

			private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

			private ConfiguredTaskAwaiter<System.Threading.Tasks.Task> _003C_003Eu__2;

			private ConfiguredTaskAwaiter _003C_003Eu__3;

			private void MoveNext()
			{
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Expected O, but got Unknown
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_018e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0193: Unknown result type (might be due to invalid IL or missing references)
				//IL_019b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0150: Unknown result type (might be due to invalid IL or missing references)
				//IL_0155: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0173: Unknown result type (might be due to invalid IL or missing references)
				//IL_0175: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass0_3 _003C_003Ec__DisplayClass0_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<System.IDisposable> awaiter;
					if (num != 0)
					{
						if ((uint)(num - 1) <= 1u)
						{
							goto IL_010a;
						}
						_003C_003E8__1 = new _003C_003Ec__DisplayClass0_4();
						_003C_003E8__1.o = o;
						_003C_003E8__1.tcs = new TaskCompletionSource();
						awaiter = Win32Events.SubscribeToOutOfContextEvents(new OutOfContextEventSubscriptionArgs(_003C_003Ec__DisplayClass0_.eventIdMin, _003C_003Ec__DisplayClass0_.eventIdMax, delegate(EventCallbackArgs ea)
						{
							if (ea.Hwnd != System.IntPtr.Zero)
							{
								_003C_003E8__1.o.OnNext(ea.Hwnd);
							}
						}, EventProcessIdFilter.FromProcessId(_003C_003Ec__DisplayClass0_.process.ProcessId), EventThreadIdFilter.ALL_THREADS, OutOfContextEventOptions.SkipOwnProcess, delegate(System.Exception? earlyError)
						{
							_003C_003E8__1.tcs.SetException((System.Exception)new GenericErrorException("The event processing thread shutdown before the process exited.", earlyError));
						}, _003C_003Ec__DisplayClass0_.CS_0024_003C_003E8__locals2.messagePumpThread)).ConfigureAwait(false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003C_003CCreateWindowWatcher_003Eb__6_003Ed>(ref awaiter, ref this);
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
					_003CeventSubscription_003E5__2 = result;
					goto IL_010a;
					IL_010a:
					try
					{
						ConfiguredTaskAwaiter awaiter2;
						ConfiguredTaskAwaiter<System.Threading.Tasks.Task> awaiter3;
						if (num != 1)
						{
							if (num == 2)
							{
								awaiter2 = _003C_003Eu__3;
								_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
								num = (_003C_003E1__state = -1);
								goto IL_020c;
							}
							awaiter3 = System.Threading.Tasks.Task.WhenAny(new System.Threading.Tasks.Task[3]
							{
								_003C_003Ec__DisplayClass0_.process.Exited,
								c.UntilCancelled(),
								_003C_003E8__1.tcs.Task
							}).ConfigureAwait(false).GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter3;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Threading.Tasks.Task>, _003C_003CCreateWindowWatcher_003Eb__6_003Ed>(ref awaiter3, ref this);
								return;
							}
						}
						else
						{
							awaiter3 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<System.Threading.Tasks.Task>);
							num = (_003C_003E1__state = -1);
						}
						ConfiguredTaskAwaitable val = awaiter3.GetResult().ConfigureAwait(false);
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__3 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CCreateWindowWatcher_003Eb__6_003Ed>(ref awaiter2, ref this);
							return;
						}
						goto IL_020c;
						IL_020c:
						((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
						if (_003C_003E8__1.tcs.Task.IsFaulted)
						{
							_003C_003E8__1.o.OnError(((System.Exception)(object)_003C_003E8__1.tcs.Task.Exception).InnerException);
						}
						else
						{
							_003C_003E8__1.o.OnCompleted();
						}
					}
					finally
					{
						if (num < 0 && _003CeventSubscription_003E5__2 != null)
						{
							_003CeventSubscription_003E5__2.Dispose();
						}
					}
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = null;
					_003CeventSubscription_003E5__2 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CeventSubscription_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		public EventId eventIdMin;

		public EventId eventIdMax;

		public IProcessSession process;

		public _003C_003Ec__DisplayClass0_0 CS_0024_003C_003E8__locals2;

		[AsyncStateMachine(typeof(_003C_003CCreateWindowWatcher_003Eb__6_003Ed))]
		internal System.Threading.Tasks.Task _003CCreateWindowWatcher_003Eb__6(IObserver<System.IntPtr> o, CancellationToken c)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CCreateWindowWatcher_003Eb__6_003Ed _003C_003CCreateWindowWatcher_003Eb__6_003Ed = default(_003C_003CCreateWindowWatcher_003Eb__6_003Ed);
			_003C_003CCreateWindowWatcher_003Eb__6_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CCreateWindowWatcher_003Eb__6_003Ed._003C_003E4__this = this;
			_003C_003CCreateWindowWatcher_003Eb__6_003Ed.o = o;
			_003C_003CCreateWindowWatcher_003Eb__6_003Ed.c = c;
			_003C_003CCreateWindowWatcher_003Eb__6_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CCreateWindowWatcher_003Eb__6_003Ed._003C_003Et__builder)).Start<_003C_003CCreateWindowWatcher_003Eb__6_003Ed>(ref _003C_003CCreateWindowWatcher_003Eb__6_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateWindowWatcher_003Eb__6_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass0_4
	{
		public IObserver<System.IntPtr> o;

		public TaskCompletionSource tcs;

		internal void _003CCreateWindowWatcher_003Eb__7(EventCallbackArgs ea)
		{
			if (ea.Hwnd != System.IntPtr.Zero)
			{
				o.OnNext(ea.Hwnd);
			}
		}

		internal void _003CCreateWindowWatcher_003Eb__8(System.Exception? earlyError)
		{
			tcs.SetException((System.Exception)new GenericErrorException("The event processing thread shutdown before the process exited.", earlyError));
		}
	}

	public static WatchTrainerWindowRectangle CreateWindowWatcher(IMessagePumpThreadDispatcher messagePumpThread, IOsWindowOperations windowing)
	{
		IOsWindowOperations windowing2 = windowing;
		IMessagePumpThreadDispatcher messagePumpThread2 = messagePumpThread;
		object cS_0024_003C_003E8__locals;
		return (SimProcessSession session) => TimingEvent.StartStreamAndConcat<TimingEvent>(WhenProcessMainWindowMoves(session.Process).Throttle<TimingEvent>(TimeSpan.FromMilliseconds(500.0))).SelectCurrentWindow<TimingEvent>(windowing2, session);
		IObservable<TimingEvent> WhenProcessMainWindowMoves(IProcessSession process)
		{
			IProcessSession process3 = process;
			return WindowHandleEvents(process3, EventId.EVENT_OBJECT_SHOW, new EventId[1] { EventId.EVENT_OBJECT_LOCATIONCHANGE }).Where<System.IntPtr>((Func<System.IntPtr, bool>)((System.IntPtr ip) => ip == process3.MainWindowHandle)).ToTimingEventStream<System.IntPtr>();
		}
		IObservable<System.IntPtr> WindowHandleEvents(IProcessSession process, EventId eventId, EventId[] extras)
		{
			IProcessSession process2 = process;
			return (from eventId in Prelude.Seq1(eventId).Concat(extras)
				select WindowHandleEventsRange(process2, eventId, eventId)).Merge();
		}
		IObservable<System.IntPtr> WindowHandleEventsRange(IProcessSession process, EventId eventIdMin, EventId eventIdMax)
		{
			_003C_003Ec__DisplayClass0_3 _003C_003Ec__DisplayClass0_ = new _003C_003Ec__DisplayClass0_3();
			_003C_003Ec__DisplayClass0_.CS_0024_003C_003E8__locals2 = (_003C_003Ec__DisplayClass0_0)cS_0024_003C_003E8__locals;
			_003C_003Ec__DisplayClass0_.eventIdMin = eventIdMin;
			_003C_003Ec__DisplayClass0_.eventIdMax = eventIdMax;
			_003C_003Ec__DisplayClass0_.process = process;
			return Observable.Create<System.IntPtr>((Func<IObserver<System.IntPtr>, CancellationToken, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass0_3._003C_003CCreateWindowWatcher_003Eb__6_003Ed))] (IObserver<System.IntPtr> o, CancellationToken c) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				_003C_003Ec__DisplayClass0_3._003C_003CCreateWindowWatcher_003Eb__6_003Ed _003C_003CCreateWindowWatcher_003Eb__6_003Ed = default(_003C_003Ec__DisplayClass0_3._003C_003CCreateWindowWatcher_003Eb__6_003Ed);
				_003C_003CCreateWindowWatcher_003Eb__6_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003CCreateWindowWatcher_003Eb__6_003Ed._003C_003E4__this = _003C_003Ec__DisplayClass0_;
				_003C_003CCreateWindowWatcher_003Eb__6_003Ed.o = o;
				_003C_003CCreateWindowWatcher_003Eb__6_003Ed.c = c;
				_003C_003CCreateWindowWatcher_003Eb__6_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003CCreateWindowWatcher_003Eb__6_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass0_3._003C_003CCreateWindowWatcher_003Eb__6_003Ed>(ref _003C_003CCreateWindowWatcher_003Eb__6_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateWindowWatcher_003Eb__6_003Ed._003C_003Et__builder)).Task;
			}));
		}
	}
}
