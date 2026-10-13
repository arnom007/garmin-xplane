using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AdvancedProcessCreation;
using IO.Win32.Events;
using IO.Win32.Processes;
using LOLA.Logging;
using LOLA.Processes;
using LOLA.Tasks;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class TrainerProcess
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass2_1
	{
		public TrainerForkParameters forkArgs;

		public IProcessSession p;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		[StructLayout(3)]
		private struct _003C_003CMoveMainWindowToSpecifiedLocation_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<IProcessSession> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass3_0 _003C_003E4__this;

			public TrainerForkParameters startup;

			private _003C_003Ec__DisplayClass3_1 _003C_003E8__1;

			private ConfiguredTaskAwaiter<IProcessSession> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass3_0 _003C_003Ec__DisplayClass3_ = _003C_003E4__this;
				IProcessSession p;
				try
				{
					ConfiguredTaskAwaiter<IProcessSession> awaiter;
					if (num != 0)
					{
						_003C_003E8__1 = new _003C_003Ec__DisplayClass3_1();
						_003C_003E8__1.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass3_;
						awaiter = _003C_003Ec__DisplayClass3_.inner(startup).ConfigureAwait(false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IProcessSession>, _003C_003CMoveMainWindowToSpecifiedLocation_003Eb__0_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<IProcessSession>);
						num = (_003C_003E1__state = -1);
					}
					IProcessSession result = awaiter.GetResult();
					_003C_003E8__1.p = result;
					try
					{
						if ((object)startup.StartingLocation != null)
						{
							MoveWindowWhenItsReady(startup.StartingLocation);
						}
						p = _003C_003E8__1.p;
					}
					catch
					{
						((System.IDisposable)_003C_003E8__1.p).Dispose();
						throw;
					}
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003C_003Et__builder.SetResult(p);
				[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_1._003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed))]
				System.Threading.Tasks.Task MoveWindowWhenItsReady(TrainerWindowScreenCoordinates coordinates)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass3_1._003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed _003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed = default(_003C_003Ec__DisplayClass3_1._003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed);
					_003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed._003C_003E4__this = this;
					_003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed.coordinates = coordinates;
					_003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass3_1._003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed>(ref _003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed._003C_003Et__builder)).Task;
				}
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public ForkTrainer inner;

		public IOsWindowOperations windowing;

		public IMessagePumpThreadDispatcher messagePumpThread;

		[AsyncStateMachine(typeof(_003C_003CMoveMainWindowToSpecifiedLocation_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<IProcessSession> _003CMoveMainWindowToSpecifiedLocation_003Eb__0(TrainerForkParameters startup)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass3_1 _003C_003Ec__DisplayClass3_ = new _003C_003Ec__DisplayClass3_1
			{
				CS_0024_003C_003E8__locals1 = this,
				p = await inner(startup).ConfigureAwait(false)
			};
			try
			{
				if ((object)startup.StartingLocation != null)
				{
					_003C_003Ec__DisplayClass3_._003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1(startup.StartingLocation);
				}
				return _003C_003Ec__DisplayClass3_.p;
			}
			catch
			{
				((System.IDisposable)_003C_003Ec__DisplayClass3_.p).Dispose();
				throw;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_1
	{
		[StructLayout(3)]
		private struct _003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public IObserver<System.IntPtr> o;

			public _003C_003Ec__DisplayClass3_1 _003C_003E4__this;

			public CancellationToken c;

			private System.IDisposable _003CeventSubscription_003E5__2;

			private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

			private ConfiguredTaskAwaiter<System.Threading.Tasks.Task> _003C_003Eu__2;

			private ConfiguredTaskAwaiter _003C_003Eu__3;

			private void MoveNext()
			{
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_014c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0151: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_0170: Unknown result type (might be due to invalid IL or missing references)
				//IL_0175: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				//IL_017e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0113: Unknown result type (might be due to invalid IL or missing references)
				//IL_0117: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0193: Unknown result type (might be due to invalid IL or missing references)
				//IL_0195: Unknown result type (might be due to invalid IL or missing references)
				//IL_0131: Unknown result type (might be due to invalid IL or missing references)
				//IL_0133: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass3_1 _003C_003Ec__DisplayClass3_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<System.IDisposable> awaiter;
					if (num != 0)
					{
						if ((uint)(num - 1) <= 1u)
						{
							goto IL_00e7;
						}
						_003C_003Ec__DisplayClass3_2 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_2
						{
							o = o
						};
						awaiter = Win32Events.SubscribeToOutOfContextEvents(new OutOfContextEventSubscriptionArgs(EventId.EVENT_OBJECT_CREATE, EventId.EVENT_OBJECT_CREATE, delegate(EventCallbackArgs ea)
						{
							if (ea.EventId == EventId.EVENT_OBJECT_CREATE && ea.Hwnd != System.IntPtr.Zero)
							{
								CS_0024_003C_003E8__locals0.o.OnNext(ea.Hwnd);
							}
						}, EventProcessIdFilter.FromProcessId(_003C_003Ec__DisplayClass3_.p.ProcessId), EventThreadIdFilter.ALL_THREADS, OutOfContextEventOptions.SkipOwnProcess, delegate(System.Exception? earlyError)
						{
							CS_0024_003C_003E8__locals0.o.OnError((System.Exception)new GenericErrorException("The event processing thread shutdown before the process exited.", earlyError));
						}, _003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals1.messagePumpThread)).ConfigureAwait(false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed>(ref awaiter, ref this);
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
					goto IL_00e7;
					IL_00e7:
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
								goto IL_01c7;
							}
							awaiter3 = System.Threading.Tasks.Task.WhenAny((System.Threading.Tasks.Task)_003C_003Ec__DisplayClass3_.p.Exited, c.UntilCancelled()).ConfigureAwait(false).GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter3;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Threading.Tasks.Task>, _003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed>(ref awaiter3, ref this);
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
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed>(ref awaiter2, ref this);
							return;
						}
						goto IL_01c7;
						IL_01c7:
						((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
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
					_003CeventSubscription_003E5__2 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003CeventSubscription_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		public IProcessSession p;

		public _003C_003Ec__DisplayClass3_0 CS_0024_003C_003E8__locals1;

		internal void _003CMoveMainWindowToSpecifiedLocation_003Eb__4(System.Exception e)
		{
			Logger.LogFormattedException(e, $"Waiting for main window of process: {p.ProcessId} ({p.Name})");
		}

		internal bool _003CMoveMainWindowToSpecifiedLocation_003Eb__5(System.IntPtr ip)
		{
			return ip == p.MainWindowHandle;
		}

		[AsyncStateMachine(typeof(_003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed))]
		internal System.Threading.Tasks.Task _003CMoveMainWindowToSpecifiedLocation_003Eb__7(IObserver<System.IntPtr> o, CancellationToken c)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed _003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed = default(_003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed);
			_003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed._003C_003E4__this = this;
			_003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed.o = o;
			_003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed.c = c;
			_003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed._003C_003Et__builder)).Start<_003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed>(ref _003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CMoveMainWindowToSpecifiedLocation_003Eb__7_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_2
	{
		public IObserver<System.IntPtr> o;

		internal void _003CMoveMainWindowToSpecifiedLocation_003Eb__8(EventCallbackArgs ea)
		{
			if (ea.EventId == EventId.EVENT_OBJECT_CREATE && ea.Hwnd != System.IntPtr.Zero)
			{
				o.OnNext(ea.Hwnd);
			}
		}

		internal void _003CMoveMainWindowToSpecifiedLocation_003Eb__9(System.Exception? earlyError)
		{
			o.OnError((System.Exception)new GenericErrorException("The event processing thread shutdown before the process exited.", earlyError));
		}
	}

	public static ForkTrainer CreateDirectForkStrategy(IOsProcessOperations processes)
	{
		IOsProcessOperations processes2 = processes;
		return (TrainerForkParameters forkArgs) => System.Threading.Tasks.Task.FromResult<IProcessSession>(processes2.Fork(forkArgs.WorkingDirectory.RawPath, forkArgs.ExePath.RawPath, Enumerable.ToArray<string>((System.Collections.Generic.IEnumerable<string>)forkArgs.Arguments)));
	}

	public static ForkTrainer CreateDirectForkStrategy(ForkProcess<IAdvancedProcessStartupInfo> fork)
	{
		ForkProcess<IAdvancedProcessStartupInfo> fork2 = fork;
		return [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass1_0._003C_003CCreateDirectForkStrategy_003Eb__0_003Ed))] async (TrainerForkParameters forkArgs) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			IAdvancedProcessStartupInfo startup;
			if (!OperatingSystem.IsWindowsVersionAtLeast(8, 0, 0, 0))
			{
				startup = new ProcessStartupInfoAdapter(forkArgs.WorkingDirectory.RawPath, forkArgs.ExePath.RawPath, string.Join(" ", Enumerable.ToArray<string>((System.Collections.Generic.IEnumerable<string>)forkArgs.Arguments)));
			}
			else
			{
				Rectangle? windowStartLocation = (((object)forkArgs.StartingLocation != null) ? new Rectangle?(new Rectangle(forkArgs.StartingLocation.Left, forkArgs.StartingLocation.Top, forkArgs.StartingLocation.Width, forkArgs.StartingLocation.Height)) : null);
				startup = new Win32ProcessStartInfoAdapter(new ProcessStartUpInfo(forkArgs.WorkingDirectory.RawPath, forkArgs.ExePath.RawPath, string.Join(" ", Enumerable.ToArray<string>((System.Collections.Generic.IEnumerable<string>)forkArgs.Arguments)), windowStartLocation));
			}
			return await fork2(startup).ConfigureAwait(false);
		};
	}

	public static ForkTrainer LogOutputs(this ForkTrainer inner)
	{
		ForkTrainer inner2 = inner;
		return [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass2_0._003C_003CLogOutputs_003Eb__0_003Ed))] async (TrainerForkParameters forkArgs) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass2_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass2_1();
			CS_0024_003C_003E8__locals0.forkArgs = forkArgs;
			CS_0024_003C_003E8__locals0.p = await inner2(CS_0024_003C_003E8__locals0.forkArgs).ConfigureAwait(false);
			LogProcessOutput(CS_0024_003C_003E8__locals0.p.StdOut, "STDOUT");
			LogProcessOutput(CS_0024_003C_003E8__locals0.p.StdError, "STDERR");
			return CS_0024_003C_003E8__locals0.p;
			[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass2_1._003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed))]
			System.Threading.Tasks.Task LogProcessOutput(StreamReader reader, string prefix)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				_003C_003Ec__DisplayClass2_1._003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed _003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed = default(_003C_003Ec__DisplayClass2_1._003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed);
				_003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
				_003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed.reader = reader;
				_003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed.prefix = prefix;
				_003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass2_1._003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed>(ref _003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003CLogOutputs_003Eg__LogProcessOutput_007C1_003Ed._003C_003Et__builder)).Task;
			}
		};
	}

	public static ForkTrainer MoveMainWindowToSpecifiedLocation(this ForkTrainer inner, IOsWindowOperations windowing, IMessagePumpThreadDispatcher messagePumpThread)
	{
		ForkTrainer inner2 = inner;
		IOsWindowOperations windowing2 = windowing;
		IMessagePumpThreadDispatcher messagePumpThread2 = messagePumpThread;
		object cS_0024_003C_003E8__locals;
		return [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_0._003C_003CMoveMainWindowToSpecifiedLocation_003Eb__0_003Ed))] async (TrainerForkParameters startup) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass3_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_1();
			CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1 = (_003C_003Ec__DisplayClass3_0)cS_0024_003C_003E8__locals;
			CS_0024_003C_003E8__locals0.p = await inner2(startup).ConfigureAwait(false);
			try
			{
				if ((object)startup.StartingLocation != null)
				{
					MoveWindowWhenItsReady(startup.StartingLocation);
				}
				return CS_0024_003C_003E8__locals0.p;
			}
			catch
			{
				((System.IDisposable)CS_0024_003C_003E8__locals0.p).Dispose();
				throw;
			}
			[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_1._003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed))]
			System.Threading.Tasks.Task MoveWindowWhenItsReady(TrainerWindowScreenCoordinates coordinates)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				_003C_003Ec__DisplayClass3_1._003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed _003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed = default(_003C_003Ec__DisplayClass3_1._003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed);
				_003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
				_003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed.coordinates = coordinates;
				_003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass3_1._003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed>(ref _003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003CMoveMainWindowToSpecifiedLocation_003Eg__MoveWindowWhenItsReady_007C1_003Ed._003C_003Et__builder)).Task;
			}
		};
	}
}
