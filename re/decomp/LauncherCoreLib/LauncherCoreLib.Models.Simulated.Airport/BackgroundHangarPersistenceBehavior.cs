using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Models.AppLifeCycle;
using LauncherCoreLib.Models.DAL;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class BackgroundHangarPersistenceBehavior : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		[StructLayout(3)]
		private struct _003C_003COnStartup_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass3_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				//IL_0123: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0110: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass3_0 _003C_003Ec__DisplayClass3_ = _003C_003E4__this;
				try
				{
					if (num != 0)
					{
						_003C_003Ec__DisplayClass3_._003C_003E4__this._squawks.Remove(_003C_003Ec__DisplayClass3_.squawk);
						_003C_003Ec__DisplayClass3_._003C_003E4__this.Log(LogLevel.Debug, "Starting background flush.");
					}
					try
					{
						ConfiguredTaskAwaiter awaiter;
						if (num != 0)
						{
							ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass3_._003C_003E4__this._unitOfWork.FlushEverything().DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003COnStartup_003Eb__1_003Ed>(ref awaiter, ref this);
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
						_003C_003Ec__DisplayClass3_._003C_003E4__this.Log(LogLevel.Debug, "Finished background flush.");
					}
					catch (System.Exception ex)
					{
						Logger.LogFormattedException(ex, "Background Flush");
						IOException val2 = (IOException)(object)((ex is IOException) ? ex : null);
						if (val2 == null)
						{
							goto IL_0114;
						}
						ValueTuple<string, bool> val3;
						if (val2.IsDiskFull())
						{
							val3 = new ValueTuple<string, bool>("Aircraft data could not be saved because the disk is full. Please free some space to save your work.", false);
						}
						else
						{
							if (!val2.IsAccessDenied())
							{
								goto IL_0114;
							}
							val3 = new ValueTuple<string, bool>("Aircraft data could not be saved because another program interfered. If possible add an exception to your anti-virus software for the Garmin trainers. If this persists then please send feedback via the About page.", false);
						}
						goto IL_0121;
						IL_0114:
						val3 = new ValueTuple<string, bool>(string.Empty, true);
						goto IL_0121;
						IL_0121:
						ValueTuple<string, bool> val4 = val3;
						string item = val4.Item1;
						bool item2 = val4.Item2;
						if (item.Length > 0)
						{
							_003C_003Ec__DisplayClass3_.squawk = Squawk.CreateError(item);
							_003C_003Ec__DisplayClass3_._003C_003E4__this._squawks.Add(_003C_003Ec__DisplayClass3_.squawk);
						}
						if (item2)
						{
							throw;
						}
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

		public BackgroundHangarPersistenceBehavior _003C_003E4__this;

		public Squawk squawk;

		public Func<System.Threading.Tasks.Task> _003C_003E9__1;

		internal IObservable<Unit> _003COnStartup_003Eb__0(TimingEvent _)
		{
			return Observable.FromAsync((Func<System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003COnStartup_003Eb__1_003Ed))] () =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				_003C_003COnStartup_003Eb__1_003Ed _003C_003COnStartup_003Eb__1_003Ed = default(_003C_003COnStartup_003Eb__1_003Ed);
				_003C_003COnStartup_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003COnStartup_003Eb__1_003Ed._003C_003E4__this = this;
				_003C_003COnStartup_003Eb__1_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003COnStartup_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003COnStartup_003Eb__1_003Ed>(ref _003C_003COnStartup_003Eb__1_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003COnStartup_003Eb__1_003Ed._003C_003Et__builder)).Task;
			}));
		}

		[AsyncStateMachine(typeof(_003C_003COnStartup_003Eb__1_003Ed))]
		internal System.Threading.Tasks.Task _003COnStartup_003Eb__1()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003COnStartup_003Eb__1_003Ed _003C_003COnStartup_003Eb__1_003Ed = default(_003C_003COnStartup_003Eb__1_003Ed);
			_003C_003COnStartup_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003COnStartup_003Eb__1_003Ed._003C_003E4__this = this;
			_003C_003COnStartup_003Eb__1_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003COnStartup_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003COnStartup_003Eb__1_003Ed>(ref _003C_003COnStartup_003Eb__1_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003COnStartup_003Eb__1_003Ed._003C_003Et__builder)).Task;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnStartup_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public BackgroundHangarPersistenceBehavior _003C_003E4__this;

		public CancellationToken shutdown;

		private ConfiguredTaskAwaiter<Unit> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			BackgroundHangarPersistenceBehavior backgroundHangarPersistenceBehavior = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<Unit> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass3_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass3_0
					{
						_003C_003E4__this = _003C_003E4__this,
						squawk = Squawk.Empty
					};
					awaiter = Observable.Interval(TimeSpan.FromSeconds(30.0)).ToTimingEventStream<long>().Merge<TimingEvent>(backgroundHangarPersistenceBehavior._unitOfWork.Aircraft.WhenSelectedAircraftChanges.ToTimingEventStream<AircraftIdentifier>())
						.TakeUntil<TimingEvent>(shutdown)
						.Select<TimingEvent, IObservable<Unit>>((Func<TimingEvent, IObservable<Unit>>)((TimingEvent _) => Observable.FromAsync((Func<System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_0._003C_003COnStartup_003Eb__1_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass3_0._003C_003COnStartup_003Eb__1_003Ed _003C_003COnStartup_003Eb__1_003Ed = default(_003C_003Ec__DisplayClass3_0._003C_003COnStartup_003Eb__1_003Ed);
							_003C_003COnStartup_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003COnStartup_003Eb__1_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
							_003C_003COnStartup_003Eb__1_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003COnStartup_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass3_0._003C_003COnStartup_003Eb__1_003Ed>(ref _003C_003COnStartup_003Eb__1_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003COnStartup_003Eb__1_003Ed._003C_003Et__builder)).Task;
						}))))
						.Concat<Unit>()
						.LastOrDefaultAsync<Unit>()
						.ToTask<Unit>()
						.DetachContext()
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<Unit>, _003COnStartup_003Ed__3>(ref awaiter, ref this);
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

	private readonly IUnitOfWork _unitOfWork;

	private readonly ISquawkBoard _squawks;

	public BackgroundHangarPersistenceBehavior(IUnitOfWork unitOfWork, ISquawkBoard squawks)
	{
		_unitOfWork = unitOfWork;
		_squawks = squawks;
	}

	[AsyncStateMachine(typeof(_003COnStartup_003Ed__3))]
	public System.Threading.Tasks.Task OnStartup(CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003COnStartup_003Ed__3 _003COnStartup_003Ed__ = default(_003COnStartup_003Ed__3);
		_003COnStartup_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnStartup_003Ed__._003C_003E4__this = this;
		_003COnStartup_003Ed__.shutdown = shutdown;
		_003COnStartup_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Start<_003COnStartup_003Ed__3>(ref _003COnStartup_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Task;
	}
}
