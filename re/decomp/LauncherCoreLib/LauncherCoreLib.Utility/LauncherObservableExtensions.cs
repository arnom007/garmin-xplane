using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LauncherCoreLib.Utility;

public static class LauncherObservableExtensions
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass1_0<T> where T : notnull
	{
		[StructLayout(3)]
		private struct _003C_003CSchedule_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<T> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass1_0<T> _003C_003E4__this;

			private ConfiguredTaskAwaiter<T> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass1_0<T> _003C_003Ec__DisplayClass1_ = _003C_003E4__this;
				T result;
				try
				{
					ConfiguredTaskAwaiter<T> awaiter;
					if (num != 0)
					{
						awaiter = ((Func<System.Threading.Tasks.Task<System.Threading.Tasks.Task<T>>>)(object)_003C_003Ec__DisplayClass1_.asyncAction).Invoke().ConfigureAwait(false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<T>, _003C_003CSchedule_003Eb__0_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<T>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter.GetResult();
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

		public Func<System.Threading.Tasks.Task<T>> asyncAction;

		[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass1_0<>._003C_003CSchedule_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<T> _003CSchedule_003Eb__0()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await ((Func<System.Threading.Tasks.Task<System.Threading.Tasks.Task<T>>>)(object)asyncAction).Invoke().ConfigureAwait(false);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass2_0
	{
		[StructLayout(3)]
		private struct _003C_003CSchedule_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass2_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass2_0 _003C_003Ec__DisplayClass2_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass2_.asyncAction.Invoke().ConfigureAwait(false);
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CSchedule_003Eb__0_003Ed>(ref awaiter, ref this);
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

		public Func<System.Threading.Tasks.Task> asyncAction;

		[AsyncStateMachine(typeof(_003C_003CSchedule_003Eb__0_003Ed))]
		internal System.Threading.Tasks.Task _003CSchedule_003Eb__0()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CSchedule_003Eb__0_003Ed _003C_003CSchedule_003Eb__0_003Ed = default(_003C_003CSchedule_003Eb__0_003Ed);
			_003C_003CSchedule_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CSchedule_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CSchedule_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CSchedule_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CSchedule_003Eb__0_003Ed>(ref _003C_003CSchedule_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CSchedule_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CSchedule_003Ed__1<T> : IAsyncStateMachine where T : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<T> _003C_003Et__builder;

		public Func<System.Threading.Tasks.Task<T>> asyncAction;

		public IScheduler scheduler;

		private ConfiguredTaskAwaiter<T> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			T result;
			try
			{
				ConfiguredTaskAwaiter<T> awaiter;
				if (num != 0)
				{
					awaiter = Observable.FromAsync<T>((Func<System.Threading.Tasks.Task<T>>)(object)new Func<System.Threading.Tasks.Task<System.Threading.Tasks.Task<T>>>(new _003C_003Ec__DisplayClass1_0<T>
					{
						asyncAction = asyncAction
					}._003CSchedule_003Eb__0), scheduler).SubscribeOn<T>(scheduler).ToTask<T>()
						.ConfigureAwait(false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<T>, _003CSchedule_003Ed__1<T>>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<T>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult();
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
	private struct _003CSchedule_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public Func<System.Threading.Tasks.Task> asyncAction;

		public IScheduler scheduler;

		private ConfiguredTaskAwaiter<Unit> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter<Unit> awaiter;
				if (num != 0)
				{
					awaiter = Observable.FromAsync((Func<System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass2_0
					{
						asyncAction = asyncAction
					}._003CSchedule_003Eb__0, scheduler).SubscribeOn<Unit>(scheduler).ToTask<Unit>()
						.ConfigureAwait(false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<Unit>, _003CSchedule_003Ed__2>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<Unit>);
					num = (_003C_003E1__state = -1);
				}
				awaiter.GetResult();
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

	public static IObservable<T> WhereEqual<T>(this IObservable<T> stream, T operand, IEqualityComparer<T>? comparer = null)
	{
		IEqualityComparer<T> comparer2 = comparer;
		T operand2 = operand;
		IEqualityComparer<T> val = comparer2;
		comparer2 = (IEqualityComparer<T>)(((object)val) ?? ((object)EqualityComparer<T>.Default));
		return stream.Where<T>((Func<T, bool>)((T e) => comparer2.Equals(e, operand2)));
	}

	[AsyncStateMachine(typeof(_003CSchedule_003Ed__1<>))]
	public static async System.Threading.Tasks.Task<T> Schedule<T>(this IScheduler scheduler, Func<System.Threading.Tasks.Task<T>> asyncAction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Func<System.Threading.Tasks.Task<T>> asyncAction2 = asyncAction;
		return await Observable.FromAsync<T>((Func<System.Threading.Tasks.Task<T>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass1_0<>._003C_003CSchedule_003Eb__0_003Ed))] async () => await asyncAction2.Invoke().ConfigureAwait(false)), scheduler).SubscribeOn<T>(scheduler).ToTask<T>()
			.ConfigureAwait(false);
	}

	[AsyncStateMachine(typeof(_003CSchedule_003Ed__2))]
	public static System.Threading.Tasks.Task Schedule(this IScheduler scheduler, Func<System.Threading.Tasks.Task> asyncAction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CSchedule_003Ed__2 _003CSchedule_003Ed__ = default(_003CSchedule_003Ed__2);
		_003CSchedule_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CSchedule_003Ed__.scheduler = scheduler;
		_003CSchedule_003Ed__.asyncAction = asyncAction;
		_003CSchedule_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CSchedule_003Ed__._003C_003Et__builder)).Start<_003CSchedule_003Ed__2>(ref _003CSchedule_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CSchedule_003Ed__._003C_003Et__builder)).Task;
	}

	public static IObservable<T> Do<T>(this IObservable<T> stream, Action<T, int> actionWithIndex)
	{
		IObservable<T> stream2 = stream;
		Action<T, int> actionWithIndex2 = actionWithIndex;
		return Observable.Defer<T>((Func<IObservable<T>>)delegate
		{
			int counter = 0;
			return Observable.Do<T>(stream2, (Action<T>)delegate(T t)
			{
				actionWithIndex2.Invoke(t, counter++);
			});
		});
	}
}
