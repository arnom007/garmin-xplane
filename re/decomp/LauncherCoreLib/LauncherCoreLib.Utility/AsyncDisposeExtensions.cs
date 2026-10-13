using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LauncherCoreLib.Utility;

public static class AsyncDisposeExtensions
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CUseAsync_003Ed__0<TResource, TResult> : IAsyncStateMachine where TResource : System.IAsyncDisposable
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TResult> _003C_003Et__builder;

		public Func<TResource, System.Threading.Tasks.Task<TResult>> useResource;

		public TResource resource;

		private object _003C_003E7__wrap1;

		private int _003C_003E7__wrap2;

		private TResult _003C_003E7__wrap3;

		private ConfiguredTaskAwaiter<TResult> _003C_003Eu__1;

		private ConfiguredValueTaskAwaiter _003C_003Eu__2;

		private unsafe void MoveNext()
		{
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			TResult val2 = default(TResult);
			try
			{
				ConfiguredValueTaskAwaiter awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredValueTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_0120;
					}
					_003C_003E7__wrap1 = null;
					_003C_003E7__wrap2 = 0;
				}
				try
				{
					ConfiguredTaskAwaiter<TResult> awaiter2;
					if (num != 0)
					{
						awaiter2 = ((ConfiguredTaskAwaitable<_003F>)((System.Threading.Tasks.Task<_003F>)(object)((Func<TResource, System.Threading.Tasks.Task<System.Threading.Tasks.Task<TResult>>>)(object)useResource).Invoke(resource)).ConfigureAwait(false)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter<_003F>*)(&awaiter2))->IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							System.Runtime.CompilerServices.Unsafe.As<AsyncTaskMethodBuilder<TResult>, AsyncTaskMethodBuilder<_003F>>(ref _003C_003Et__builder).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TResult>, _003CUseAsync_003Ed__0<TResource, TResult>>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TResult>);
						num = (_003C_003E1__state = -1);
					}
					TResult result = ((ConfiguredTaskAwaiter<_003F>*)(&awaiter2))->GetResult();
					_003C_003E7__wrap3 = result;
					_003C_003E7__wrap2 = 1;
				}
				catch (object obj)
				{
					_003C_003E7__wrap1 = obj;
				}
				ConfiguredValueTaskAwaitable val = ((System.IAsyncDisposable)resource).DisposeAsync().ConfigureAwait(false);
				awaiter = ((ConfiguredValueTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredValueTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					System.Runtime.CompilerServices.Unsafe.As<AsyncTaskMethodBuilder<TResult>, AsyncTaskMethodBuilder<_003F>>(ref _003C_003Et__builder).AwaitUnsafeOnCompleted<ConfiguredValueTaskAwaiter, _003CUseAsync_003Ed__0<TResource, TResult>>(ref awaiter, ref this);
					return;
				}
				goto IL_0120;
				IL_0120:
				((ConfiguredValueTaskAwaiter)(ref awaiter)).GetResult();
				object obj2 = _003C_003E7__wrap1;
				if (obj2 != null)
				{
					ExceptionDispatchInfo.Capture((obj2 as System.Exception) ?? throw obj2).Throw();
				}
				int num2 = _003C_003E7__wrap2;
				if (num2 == 1)
				{
					val2 = _003C_003E7__wrap3;
				}
				else
				{
					_003C_003E7__wrap1 = null;
					_003C_003E7__wrap3 = default(TResult);
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				System.Runtime.CompilerServices.Unsafe.As<AsyncTaskMethodBuilder<TResult>, AsyncTaskMethodBuilder<_003F>>(ref _003C_003Et__builder).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			System.Runtime.CompilerServices.Unsafe.As<AsyncTaskMethodBuilder<TResult>, AsyncTaskMethodBuilder<_003F>>(ref _003C_003Et__builder).SetResult(val2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			System.Runtime.CompilerServices.Unsafe.As<AsyncTaskMethodBuilder<TResult>, AsyncTaskMethodBuilder<_003F>>(ref _003C_003Et__builder).SetStateMachine(stateMachine);
		}
	}

	[AsyncStateMachine(typeof(_003CUseAsync_003Ed__0<, >))]
	public static async System.Threading.Tasks.Task<TResult> UseAsync<TResource, TResult>(this TResource resource, Func<TResource, System.Threading.Tasks.Task<TResult>> useResource) where TResource : System.IAsyncDisposable
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		object obj = null;
		int num = 0;
		TResult result = default(TResult);
		try
		{
			result = await useResource.Invoke(resource).ConfigureAwait(false);
			num = 1;
		}
		catch (object obj2)
		{
			obj = obj2;
		}
		await ((System.IAsyncDisposable)resource).DisposeAsync().ConfigureAwait(false);
		object obj3 = obj;
		if (obj3 != null)
		{
			ExceptionDispatchInfo.Capture((obj3 as System.Exception) ?? throw obj3).Throw();
		}
		if (num == 1)
		{
			return result;
		}
		TResult result2 = default(TResult);
		return result2;
	}
}
