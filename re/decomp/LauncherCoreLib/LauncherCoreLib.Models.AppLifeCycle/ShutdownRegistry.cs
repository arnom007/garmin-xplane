using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.Logging;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public class ShutdownRegistry : IShutdownRegistry, IRootShutdown
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnShutdown_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ShutdownRegistry _003C_003E4__this;

		private IShutdownAction[] _003Cshutdowns_003E5__2;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ShutdownRegistry shutdownRegistry = _003C_003E4__this;
			try
			{
				if (num != 0)
				{
					shutdownRegistry.IsShuttingDown = true;
					_003Cshutdowns_003E5__2 = Enumerable.ToArray<IShutdownAction>((System.Collections.Generic.IEnumerable<IShutdownAction>)shutdownRegistry._shutdowns);
				}
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<IShutdownAction, System.Threading.Tasks.Task>((System.Collections.Generic.IEnumerable<IShutdownAction>)_003Cshutdowns_003E5__2, (Func<IShutdownAction, System.Threading.Tasks.Task>)shutdownRegistry.PerformShutdownAction)).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003COnShutdown_003Ed__7>(ref awaiter, ref this);
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
				finally
				{
					if (num < 0)
					{
						IShutdownAction[] array = _003Cshutdowns_003E5__2;
						foreach (IShutdownAction item in array)
						{
							shutdownRegistry._shutdowns.Remove(item);
						}
						shutdownRegistry.IsShuttingDown = false;
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Cshutdowns_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Cshutdowns_003E5__2 = null;
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
	private struct _003CPerformShutdownAction_003Ed__9 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public IShutdownAction action;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				if (num != 0)
				{
					Logger.Log(LogLevel.Info, "[Shutdown action][{0}] Started", action.ActionName);
				}
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = action.OnShutdown().DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CPerformShutdownAction_003Ed__9>(ref awaiter, ref this);
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
					Logger.Log(LogLevel.Info, "[Shutdown action][{0}] Completed", action.ActionName);
				}
				catch (System.Exception e)
				{
					Logger.LogFormattedException(e, "[Shutdown action][{0}] failed", action.ActionName);
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

	private readonly ConcurrentHashSet<IShutdownAction> _shutdowns = new ConcurrentHashSet<IShutdownAction>();

	public System.Collections.Generic.IEnumerable<string> RegisteredActions => Enumerable.Select<IShutdownAction, string>((System.Collections.Generic.IEnumerable<IShutdownAction>)_shutdowns, (Func<IShutdownAction, string>)((IShutdownAction s) => s.ActionName));

	[field: CompilerGenerated]
	public bool IsShuttingDown
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[AsyncStateMachine(typeof(_003COnShutdown_003Ed__7))]
	public System.Threading.Tasks.Task OnShutdown()
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

	public System.IDisposable? RegisterShutdownAction(IShutdownAction shutdownAction)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		IShutdownAction shutdownAction2 = shutdownAction;
		if (shutdownAction2 == null)
		{
			throw new ArgumentNullException("shutdownAction");
		}
		if (string.IsNullOrEmpty(shutdownAction2.ActionName))
		{
			throw new ArgumentException("ActionName cannot be null or empty.");
		}
		if (IsShuttingDown)
		{
			Logger.Log(LogLevel.Info, "Could not register shutdown action [{0}] because the app is already shutting down.", shutdownAction2.ActionName);
			return null;
		}
		_shutdowns.Add(shutdownAction2);
		return Disposable.Create((Action)delegate
		{
			_shutdowns.Remove(shutdownAction2);
		});
	}

	[AsyncStateMachine(typeof(_003CPerformShutdownAction_003Ed__9))]
	private System.Threading.Tasks.Task PerformShutdownAction(IShutdownAction action)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CPerformShutdownAction_003Ed__9 _003CPerformShutdownAction_003Ed__ = default(_003CPerformShutdownAction_003Ed__9);
		_003CPerformShutdownAction_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CPerformShutdownAction_003Ed__.action = action;
		_003CPerformShutdownAction_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CPerformShutdownAction_003Ed__._003C_003Et__builder)).Start<_003CPerformShutdownAction_003Ed__9>(ref _003CPerformShutdownAction_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CPerformShutdownAction_003Ed__._003C_003Et__builder)).Task;
	}
}
