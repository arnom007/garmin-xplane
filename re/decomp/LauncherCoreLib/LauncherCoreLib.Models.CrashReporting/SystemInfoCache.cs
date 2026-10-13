using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Tasks;
using Nito.AsyncEx;

namespace LauncherCoreLib.Models.CrashReporting;

internal class SystemInfoCache : ISystemInfoCache, System.IDisposable
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetLazyInfo_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<SystemInfo> _003C_003Et__builder;

		public SystemInfoCache _003C_003E4__this;

		private TaskAwaiter<SystemInfo> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SystemInfoCache systemInfoCache = _003C_003E4__this;
			SystemInfo result;
			try
			{
				TaskAwaiter<SystemInfo> awaiter;
				if (num != 0)
				{
					awaiter = systemInfoCache._lazySystemInfo.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<SystemInfo>, _003CGetLazyInfo_003Ed__7>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<SystemInfo>);
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
	private struct _003CGetSystemInfo_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<SystemInfo> _003C_003Et__builder;

		public SystemInfoCache _003C_003E4__this;

		public CancellationToken cancel;

		private ICancellation _003Ccombined_003E5__2;

		private System.Threading.Tasks.Task<SystemInfo> _003ClazyResult_003E5__3;

		private ConfiguredTaskAwaiter<System.Threading.Tasks.Task> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<SystemInfo> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SystemInfoCache systemInfoCache = _003C_003E4__this;
			SystemInfo result;
			try
			{
				if ((uint)num > 1u)
				{
					_003Ccombined_003E5__2 = CancellationFactory.CreateLinked(systemInfoCache._cancelOnDispose.Token, cancel);
				}
				try
				{
					ConfiguredTaskAwaiter<SystemInfo> awaiter;
					ConfiguredTaskAwaiter<System.Threading.Tasks.Task> awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<SystemInfo>);
							num = (_003C_003E1__state = -1);
							goto IL_0140;
						}
						_003ClazyResult_003E5__3 = systemInfoCache.GetLazyInfo();
						awaiter2 = System.Threading.Tasks.Task.WhenAny((System.Threading.Tasks.Task)_003ClazyResult_003E5__3, _003Ccombined_003E5__2.Token.UntilCancelled()).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Threading.Tasks.Task>, _003CGetSystemInfo_003Ed__4>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Threading.Tasks.Task>);
						num = (_003C_003E1__state = -1);
					}
					awaiter2.GetResult();
					if (((System.Threading.Tasks.Task)_003ClazyResult_003E5__3).IsCompleted)
					{
						awaiter = _003ClazyResult_003E5__3.DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SystemInfo>, _003CGetSystemInfo_003Ed__4>(ref awaiter, ref this);
							return;
						}
						goto IL_0140;
					}
					result = null;
					goto end_IL_0043;
					IL_0140:
					result = awaiter.GetResult();
					end_IL_0043:;
				}
				finally
				{
					if (num < 0 && _003Ccombined_003E5__2 != null)
					{
						((System.IDisposable)_003Ccombined_003E5__2).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Ccombined_003E5__2 = null;
				_003ClazyResult_003E5__3 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Ccombined_003E5__2 = null;
			_003ClazyResult_003E5__3 = null;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly AsyncLazy<SystemInfo> _lazySystemInfo;

	private readonly ICancellation _cancelOnDispose = CancellationFactory.Create();

	private bool _disposed;

	public SystemInfoCache()
	{
		_lazySystemInfo = new AsyncLazy<SystemInfo>(() => SystemInfo.Query());
	}

	[AsyncStateMachine(typeof(_003CGetSystemInfo_003Ed__4))]
	public async System.Threading.Tasks.Task<SystemInfo?> GetSystemInfo(CancellationToken cancel = default(CancellationToken))
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		using ICancellation combined = CancellationFactory.CreateLinked(_cancelOnDispose.Token, cancel);
		System.Threading.Tasks.Task<SystemInfo> lazyResult = GetLazyInfo();
		await System.Threading.Tasks.Task.WhenAny((System.Threading.Tasks.Task)lazyResult, combined.Token.UntilCancelled()).DetachContext();
		if (((System.Threading.Tasks.Task)lazyResult).IsCompleted)
		{
			return await lazyResult.DetachContext();
		}
		return null;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize((object)this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			_disposed = true;
			if (disposing)
			{
				((System.IDisposable)_cancelOnDispose).Dispose();
			}
		}
	}

	[AsyncStateMachine(typeof(_003CGetLazyInfo_003Ed__7))]
	private async System.Threading.Tasks.Task<SystemInfo> GetLazyInfo()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await _lazySystemInfo;
	}
}
