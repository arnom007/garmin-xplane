using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.Logging;
using LOLA.Tasks;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.AppLifeCycle;

public static class StartupActions
{
	private sealed class Impl : IStartupRegistry
	{
		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass6_0
		{
			public System.Collections.Generic.IReadOnlyList<AppLifeCycleActionRecord> startupActionRecords;
		}

		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CRunAction_003Ed__10 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public IStartupAction action;

			public Impl _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00ca: Expected O, but got Unknown
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				Impl impl = _003C_003E4__this;
				try
				{
					if (num != 0)
					{
						Logger.Log(LogLevel.Info, "[Startup action][{0}] Started", action.ActionName);
					}
					try
					{
						ConfiguredTaskAwaiter awaiter;
						if (num != 0)
						{
							ConfiguredTaskAwaitable val = action.OnStartup(impl._shutdownToken.Token).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CRunAction_003Ed__10>(ref awaiter, ref this);
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
						Logger.Log(LogLevel.Info, "[Startup action][{0}] Completed", action.ActionName);
					}
					catch (OperationCanceledException val2)
					{
						OperationCanceledException e = val2;
						if (impl._shutdownToken.IsCancellationRequested)
						{
							Logger.Log(LogLevel.Info, "[Startup action][{0}] cancelled due to shutdown.", action.ActionName);
						}
						else
						{
							Logger.LogFormattedException((System.Exception)(object)e, "[Startup action][{0}] failed", action.ActionName);
						}
						throw;
					}
					catch (System.Exception e2)
					{
						Logger.LogFormattedException(e2, "[Startup action][{0}] failed", action.ActionName);
						throw;
					}
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
		private struct _003CWhenAllStartupActionsComplete_003Ed__6 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public Impl _003C_003E4__this;

			private _003C_003Ec__DisplayClass6_0 _003C_003E8__1;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0104: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b8: Expected O, but got Unknown
				int num = _003C_003E1__state;
				Impl impl = _003C_003E4__this;
				try
				{
					if (num != 0)
					{
						_003C_003E8__1.startupActionRecords = impl.AllRecords();
						goto IL_0025;
					}
					ConfiguredTaskAwaiter awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_0113;
					IL_0113:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					goto IL_011a;
					IL_011a:
					if (Enumerable.Any<AppLifeCycleActionRecord>(_003CWhenAllStartupActionsComplete_003Eg__StillRunning_007C6_2(ref _003C_003E8__1)))
					{
						goto IL_0025;
					}
					List<GenericErrorException> val = Enumerable.ToList<GenericErrorException>(Enumerable.Select<AppLifeCycleActionRecord, GenericErrorException>(Enumerable.Where<AppLifeCycleActionRecord>((System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)_003C_003E8__1.startupActionRecords, (Func<AppLifeCycleActionRecord, bool>)((AppLifeCycleActionRecord sr) => sr.Completion.IsFaulted)), (Func<AppLifeCycleActionRecord, GenericErrorException>)((AppLifeCycleActionRecord sr) => new GenericErrorException("The " + sr.Action.ActionName + " startup action failed.", ((System.Exception)(object)sr.Completion.Exception.SuppressNull<AggregateException>("Checked in Where")).InnerException))));
					System.Exception ex = val.Count switch
					{
						0 => null, 
						1 => val[0], 
						_ => (System.Exception)new AggregateException("Multiple startup errors", (System.Collections.Generic.IEnumerable<System.Exception>)val), 
					};
					if (ex != null)
					{
						Logger.Log(ex, "All startup actions have completed. Some with errors.");
						throw ex;
					}
					Logger.Log(LogLevel.Info, "All startup actions have completed.");
					goto end_IL_000e;
					IL_0025:
					List<AppLifeCycleActionRecord> val2 = Enumerable.ToList<AppLifeCycleActionRecord>(_003CWhenAllStartupActionsComplete_003Eg__StillRunning_007C6_2(ref _003C_003E8__1));
					if (val2.Count > 0)
					{
						Logger.Log(LogLevel.Info, "Waiting for startup actions: [{0}] to complete.", string.Join(", ", Enumerable.Select<AppLifeCycleActionRecord, string>((System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)val2, (Func<AppLifeCycleActionRecord, string>)((AppLifeCycleActionRecord sr) => sr.Action.ActionName))));
						ConfiguredTaskAwaitable val3 = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<AppLifeCycleActionRecord, System.Threading.Tasks.Task>((System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)val2, (Func<AppLifeCycleActionRecord, System.Threading.Tasks.Task>)((AppLifeCycleActionRecord sr) => sr.Completion))).Catch().ConfigureAwait(false);
						awaiter = ((ConfiguredTaskAwaitable)(ref val3)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CWhenAllStartupActionsComplete_003Ed__6>(ref awaiter, ref this);
							return;
						}
						goto IL_0113;
					}
					goto IL_011a;
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = default(_003C_003Ec__DisplayClass6_0);
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = default(_003C_003Ec__DisplayClass6_0);
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		private readonly ICancellation _shutdownToken = CancellationFactory.Create();

		private readonly List<AppLifeCycleActionRecord> _startupTasks = new List<AppLifeCycleActionRecord>();

		[CompilerGenerated]
		private EventHandler? m_StartupActionAdded;

		public event EventHandler StartupActionAdded
		{
			[CompilerGenerated]
			add
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				EventHandler val = this.m_StartupActionAdded;
				EventHandler val2;
				do
				{
					val2 = val;
					EventHandler val3 = (EventHandler)System.Delegate.Combine((System.Delegate)(object)val2, (System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler>(ref this.m_StartupActionAdded, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				EventHandler val = this.m_StartupActionAdded;
				EventHandler val2;
				do
				{
					val2 = val;
					EventHandler val3 = (EventHandler)System.Delegate.Remove((System.Delegate)(object)val2, (System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler>(ref this.m_StartupActionAdded, val3, val2);
				}
				while (val != val2);
			}
		}

		public System.Collections.Generic.IReadOnlyList<AppLifeCycleActionRecord> AllRecords()
		{
			lock (_startupTasks)
			{
				return (System.Collections.Generic.IReadOnlyList<AppLifeCycleActionRecord>)Enumerable.ToList<AppLifeCycleActionRecord>((System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)_startupTasks);
			}
		}

		public System.Collections.Generic.IReadOnlyList<AppLifeCycleActionRecord> RunningActions()
		{
			lock (_startupTasks)
			{
				return (System.Collections.Generic.IReadOnlyList<AppLifeCycleActionRecord>)Enumerable.ToList<AppLifeCycleActionRecord>(Enumerable.Where<AppLifeCycleActionRecord>((System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)_startupTasks, (Func<AppLifeCycleActionRecord, bool>)((AppLifeCycleActionRecord sr) => !sr.Completion.IsCompleted)));
			}
		}

		public System.Threading.Tasks.Task RunStartupAction(params IStartupAction[] otherActions)
		{
			List<AppLifeCycleActionRecord> val = Enumerable.ToList<AppLifeCycleActionRecord>(Enumerable.Select<IStartupAction, AppLifeCycleActionRecord>((System.Collections.Generic.IEnumerable<IStartupAction>)otherActions, (Func<IStartupAction, AppLifeCycleActionRecord>)BuildRecord));
			lock (_startupTasks)
			{
				_startupTasks.AddRange((System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)val);
			}
			EventHandler? startupActionAdded = this.StartupActionAdded;
			if (startupActionAdded != null)
			{
				startupActionAdded.Invoke((object)this, EventArgs.Empty);
			}
			return System.Threading.Tasks.Task.WhenAll(Enumerable.Select<AppLifeCycleActionRecord, System.Threading.Tasks.Task>((System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)val, (Func<AppLifeCycleActionRecord, System.Threading.Tasks.Task>)((AppLifeCycleActionRecord sr) => sr.Completion)));
		}

		public void ShutdownActions()
		{
			_shutdownToken.Cancel();
		}

		[AsyncStateMachine(typeof(_003CWhenAllStartupActionsComplete_003Ed__6))]
		public System.Threading.Tasks.Task WhenAllStartupActionsComplete()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003CWhenAllStartupActionsComplete_003Ed__6 _003CWhenAllStartupActionsComplete_003Ed__ = default(_003CWhenAllStartupActionsComplete_003Ed__6);
			_003CWhenAllStartupActionsComplete_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003CWhenAllStartupActionsComplete_003Ed__._003C_003E4__this = this;
			_003CWhenAllStartupActionsComplete_003Ed__._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003CWhenAllStartupActionsComplete_003Ed__._003C_003Et__builder)).Start<_003CWhenAllStartupActionsComplete_003Ed__6>(ref _003CWhenAllStartupActionsComplete_003Ed__);
			return ((AsyncTaskMethodBuilder)(ref _003CWhenAllStartupActionsComplete_003Ed__._003C_003Et__builder)).Task;
		}

		public System.Collections.Generic.IReadOnlyCollection<AppLifeCycleActionRecord> GetStartupActions(string? actionName = null, System.Type? actionType = null)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			string actionName2 = actionName;
			System.Type actionType2 = actionType;
			if (actionType2 != (System.Type)null && !typeof(IStartupAction).IsAssignableFrom(actionType2))
			{
				throw new ArgumentException("actionType must be a IStartupAction");
			}
			lock (_startupTasks)
			{
				return (System.Collections.Generic.IReadOnlyCollection<AppLifeCycleActionRecord>)Enumerable.ToList<AppLifeCycleActionRecord>(((System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)_startupTasks).WhereIf(actionName2 != null, (AppLifeCycleActionRecord ar) => ar.Action.ActionName == actionName2).WhereIf(actionType2 != (System.Type)null, (AppLifeCycleActionRecord ar) => ((object)ar.Action).GetType().IsAssignableFrom(actionType2)));
			}
		}

		private static System.Exception? BuildUpException(System.Exception? root, System.Exception next)
		{
			return root.BuildUpException(next, "Multiple Startup Tasks failed.");
		}

		private AppLifeCycleActionRecord BuildRecord(IStartupAction arg)
		{
			return new AppLifeCycleActionRecord(arg, RunAction(arg));
		}

		[AsyncStateMachine(typeof(_003CRunAction_003Ed__10))]
		private System.Threading.Tasks.Task RunAction(IStartupAction action)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003CRunAction_003Ed__10 _003CRunAction_003Ed__ = default(_003CRunAction_003Ed__10);
			_003CRunAction_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003CRunAction_003Ed__._003C_003E4__this = this;
			_003CRunAction_003Ed__.action = action;
			_003CRunAction_003Ed__._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003CRunAction_003Ed__._003C_003Et__builder)).Start<_003CRunAction_003Ed__10>(ref _003CRunAction_003Ed__);
			return ((AsyncTaskMethodBuilder)(ref _003CRunAction_003Ed__._003C_003Et__builder)).Task;
		}

		[CompilerGenerated]
		internal static System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord> _003CWhenAllStartupActionsComplete_003Eg__StillRunning_007C6_2(ref _003C_003Ec__DisplayClass6_0 P_0)
		{
			return Enumerable.Where<AppLifeCycleActionRecord>((System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)P_0.startupActionRecords, (Func<AppLifeCycleActionRecord, bool>)((AppLifeCycleActionRecord r) => !r.Completion.IsCompleted));
		}
	}

	private static readonly Impl IMPL = new Impl();

	public static IStartupRegistry Registry => IMPL;

	public static System.Collections.Generic.IList<IAppLifeCycleAction> GetRunningActions()
	{
		return (System.Collections.Generic.IList<IAppLifeCycleAction>)Enumerable.ToList<IAppLifeCycleAction>(Enumerable.Select<AppLifeCycleActionRecord, IAppLifeCycleAction>((System.Collections.Generic.IEnumerable<AppLifeCycleActionRecord>)IMPL.RunningActions(), (Func<AppLifeCycleActionRecord, IAppLifeCycleAction>)((AppLifeCycleActionRecord ra) => ra.Action)));
	}

	public static System.Threading.Tasks.Task Run(params IStartupAction[] otherActions)
	{
		return IMPL.RunStartupAction(otherActions);
	}

	public static void StartAndThrowOnImmediateError(params IStartupAction[] actions)
	{
		System.Threading.Tasks.Task task = Run(actions);
		if (task.IsFaulted)
		{
			throw ((object)((System.Exception)(object)task.Exception).InnerException) ?? ((object)task.Exception);
		}
	}

	public static void Shutdown()
	{
		IMPL.ShutdownActions();
	}

	public static System.Threading.Tasks.Task WhenAllComplete()
	{
		return IMPL.WhenAllStartupActionsComplete();
	}
}
