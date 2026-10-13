using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;
using LauncherCoreLib.Models.AppLifeCycle;
using Nito.AsyncEx;

namespace LauncherCoreLib.Models.TrainerPackages;

public class LauncherGtiProvider : ILauncherGtiProvider
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CSetupLauncherGtiRunner_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ILauncherGtiRunner> _003C_003Et__builder;

		public IStartupRegistry startup;

		public IBaseGtiRunnerProvider baseProvider;

		private ConfiguredTaskAwaiter<AppLifeCycleActionRecord> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ILauncherGtiRunner result;
			try
			{
				ConfiguredTaskAwaiter<AppLifeCycleActionRecord> awaiter;
				if (num != 0)
				{
					awaiter = startup.WaitForStartupActionToComplete<AutoInitCatalogStartup>().DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AppLifeCycleActionRecord>, _003CSetupLauncherGtiRunner_003Ed__4>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AppLifeCycleActionRecord>);
					num = (_003C_003E1__state = -1);
				}
				awaiter.GetResult();
				result = new LauncherGtiRunner(baseProvider.UnprotectedGtiRunner);
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

	[field: CompilerGenerated]
	public System.Threading.Tasks.Task<ILauncherGtiRunner> GtiExe
	{
		[CompilerGenerated]
		get;
	}

	public LauncherGtiProvider(IBaseGtiRunnerProvider baseProvider, IStartupRegistry startupRegistry)
	{
		IBaseGtiRunnerProvider baseProvider2 = baseProvider;
		IStartupRegistry startupRegistry2 = startupRegistry;
		base._002Ector();
		AsyncLazy<ILauncherGtiRunner> asyncLazy = new AsyncLazy<ILauncherGtiRunner>(() => SetupLauncherGtiRunner(baseProvider2, startupRegistry2));
		GtiExe = asyncLazy.Task;
	}

	[AsyncStateMachine(typeof(_003CSetupLauncherGtiRunner_003Ed__4))]
	private static async System.Threading.Tasks.Task<ILauncherGtiRunner> SetupLauncherGtiRunner(IBaseGtiRunnerProvider baseProvider, IStartupRegistry startup)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		await startup.WaitForStartupActionToComplete<AutoInitCatalogStartup>().DetachContext();
		return new LauncherGtiRunner(baseProvider.UnprotectedGtiRunner);
	}
}
