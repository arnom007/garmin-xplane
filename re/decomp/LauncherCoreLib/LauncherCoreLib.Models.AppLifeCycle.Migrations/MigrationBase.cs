using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle.Migrations;

public abstract class MigrationBase : ILauncherMigration
{
	public class MigrationHarness
	{
		private readonly IObserver<MigrationStatusUpdate> _observer;

		private MigrationStatusUpdate _currentStatus;

		public MigrationStatusUpdate CurrentStatus
		{
			get
			{
				return _currentStatus;
			}
			set
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				_currentStatus = value ?? throw new ArgumentNullException("value");
			}
		}

		[field: CompilerGenerated]
		public CancellationToken CancelToken
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			private set;
		}

		public MigrationHarness(IObserver<MigrationStatusUpdate> observer, string initialMessage, CancellationToken token)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			_observer = observer;
			CancelToken = token;
			_currentStatus = new MigrationStatusUpdate(initialMessage);
		}

		public void ReportCurrentStatus()
		{
			_observer.OnNext(CurrentStatus);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003CMigrationSteps_003Eb__6_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public IObserver<MigrationStatusUpdate> o;

		public MigrationBase _003C_003E4__this;

		public CancellationToken t;

		private ConfiguredTaskAwaiter<System.Exception> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			MigrationBase migrationBase = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.Exception> awaiter;
				if (num != 0)
				{
					MigrationHarness harness = new MigrationHarness(o, migrationBase.MigrationName, t);
					awaiter = migrationBase.DoMigration(harness).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Exception>, _003C_003CMigrationSteps_003Eb__6_0_003Ed>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Exception>);
					num = (_003C_003E1__state = -1);
				}
				System.Exception result = awaiter.GetResult();
				if (result != null)
				{
					o.OnError(result);
				}
				else
				{
					o.OnCompleted();
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

	[field: CompilerGenerated]
	public string MigrationName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		protected set;
	}

	protected MigrationBase(string migrationName)
	{
		MigrationName = migrationName ?? string.Empty;
	}

	public abstract System.Threading.Tasks.Task<bool> IsMigrationRequired();

	public IObservable<MigrationStatusUpdate> MigrationSteps()
	{
		return Observable.Create<MigrationStatusUpdate>((Func<IObserver<MigrationStatusUpdate>, CancellationToken, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003CMigrationSteps_003Eb__6_0_003Ed))] [CompilerGenerated] (IObserver<MigrationStatusUpdate> o, CancellationToken t) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CMigrationSteps_003Eb__6_0_003Ed _003C_003CMigrationSteps_003Eb__6_0_003Ed = default(_003C_003CMigrationSteps_003Eb__6_0_003Ed);
			_003C_003CMigrationSteps_003Eb__6_0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CMigrationSteps_003Eb__6_0_003Ed._003C_003E4__this = this;
			_003C_003CMigrationSteps_003Eb__6_0_003Ed.o = o;
			_003C_003CMigrationSteps_003Eb__6_0_003Ed.t = t;
			_003C_003CMigrationSteps_003Eb__6_0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CMigrationSteps_003Eb__6_0_003Ed._003C_003Et__builder)).Start<_003C_003CMigrationSteps_003Eb__6_0_003Ed>(ref _003C_003CMigrationSteps_003Eb__6_0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CMigrationSteps_003Eb__6_0_003Ed._003C_003Et__builder)).Task;
		}));
	}

	protected abstract System.Threading.Tasks.Task<System.Exception?> DoMigration(MigrationHarness harness);
}
