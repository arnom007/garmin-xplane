using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;

namespace LauncherCoreLib.Models.AppLifeCycle.Migrations;

public class BuiltInMigrations : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnStartup_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public BuiltInMigrations _003C_003E4__this;

		private ConfiguredTaskAwaiter<MigrationData<MigrationStatusUpdate>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			BuiltInMigrations builtInMigrations = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<MigrationData<MigrationStatusUpdate>> awaiter;
				if (num != 0)
				{
					awaiter = builtInMigrations._migrationRunner.ScheduleMigrations(builtInMigrations._globalMigrationWrapper, builtInMigrations._hangarMigration).StatusUpdates.LastOrDefaultAsync<MigrationData<MigrationStatusUpdate>>().ToTask<MigrationData<MigrationStatusUpdate>>().ConfigureAwait(false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<MigrationData<MigrationStatusUpdate>>, _003COnStartup_003Ed__4>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<MigrationData<MigrationStatusUpdate>>);
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

	private readonly IRunMigrations _migrationRunner;

	private readonly GlobalMigrationWrapper _globalMigrationWrapper;

	private readonly HangarMigration _hangarMigration;

	public BuiltInMigrations(IRunMigrations migrations, GlobalMigrationWrapper globals, HangarMigration hangar)
	{
		_migrationRunner = migrations;
		_globalMigrationWrapper = globals;
		_hangarMigration = hangar;
		base.ActionName = "Built in Migrations";
	}

	[AsyncStateMachine(typeof(_003COnStartup_003Ed__4))]
	public System.Threading.Tasks.Task OnStartup(CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003COnStartup_003Ed__4 _003COnStartup_003Ed__ = default(_003COnStartup_003Ed__4);
		_003COnStartup_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnStartup_003Ed__._003C_003E4__this = this;
		_003COnStartup_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Start<_003COnStartup_003Ed__4>(ref _003COnStartup_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Task;
	}
}
