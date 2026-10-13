using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LauncherCoreLib.Models.CurrentMainAction;

public class MainActionSource : IMainActionSource
{
	private readonly Stack<IObservable<IMainAction?>> _actionStack = new Stack<IObservable<IMainAction>>();

	private readonly object _mutex = new object();

	[CompilerGenerated]
	private EventHandler? m_OnActionChange;

	public IObservable<IMainAction?> MainActions => Observable.Return<IObservable<IMainAction>>(Current).Concat<IObservable<IMainAction>>(Observable.FromEventPattern([CompilerGenerated] (EventHandler h) =>
	{
		OnActionChange += h;
	}, [CompilerGenerated] (EventHandler h) =>
	{
		OnActionChange -= h;
	}).Select<EventPattern<object>, IObservable<IMainAction>>((Func<EventPattern<object>, IObservable<IMainAction>>)([CompilerGenerated] (EventPattern<object> _) => Current))).Select<IObservable<IMainAction>, IObservable<IMainAction>>((Func<IObservable<IMainAction>, IObservable<IMainAction>>)((IObservable<IMainAction> stream) => AllSequencesStartWithNoButton(stream)))
		.Switch<IMainAction>()
		.DistinctUntilChanged<IMainAction>();

	protected IObservable<IMainAction?> Current
	{
		get
		{
			lock (_mutex)
			{
				return (IObservable<IMainAction?>)((_actionStack.Count > 0) ? ((object)_actionStack.Peek()) : ((object)Observable.Return<IMainAction>(null)));
			}
		}
	}

	private event EventHandler OnActionChange
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			EventHandler val = this.m_OnActionChange;
			EventHandler val2;
			do
			{
				val2 = val;
				EventHandler val3 = (EventHandler)System.Delegate.Combine((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<EventHandler>(ref this.m_OnActionChange, val3, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			EventHandler val = this.m_OnActionChange;
			EventHandler val2;
			do
			{
				val2 = val;
				EventHandler val3 = (EventHandler)System.Delegate.Remove((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<EventHandler>(ref this.m_OnActionChange, val3, val2);
			}
			while (val != val2);
		}
	}

	public System.IDisposable Push(IMainAction action)
	{
		return Push(Observable.Return(action));
	}

	public System.IDisposable Push(IObservable<IMainAction?> actions)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		IObservable<IMainAction?> actions2 = actions;
		lock (_mutex)
		{
			_actionStack.Push(actions2);
		}
		EventHandler? onActionChange = this.OnActionChange;
		if (onActionChange != null)
		{
			onActionChange.Invoke((object)this, EventArgs.Empty);
		}
		return Disposable.Create((Action)delegate
		{
			bool flag = false;
			lock (_mutex)
			{
				if (_actionStack.Peek() == actions2)
				{
					PopWithLock();
					flag = true;
				}
			}
			if (flag)
			{
				EventHandler? onActionChange2 = this.OnActionChange;
				if (onActionChange2 != null)
				{
					onActionChange2.Invoke((object)this, EventArgs.Empty);
				}
			}
		});
	}

	public void Pop()
	{
		lock (_mutex)
		{
			PopWithLock();
		}
		EventHandler? onActionChange = this.OnActionChange;
		if (onActionChange != null)
		{
			onActionChange.Invoke((object)this, EventArgs.Empty);
		}
	}

	private static IObservable<IMainAction?> AllSequencesStartWithNoButton(IObservable<IMainAction?> arg)
	{
		return Observable.Return<IMainAction>(null).Concat<IMainAction>(arg);
	}

	private void PopWithLock()
	{
		_actionStack.Pop();
	}
}
