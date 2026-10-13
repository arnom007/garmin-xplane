using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Reactive;
using LauncherCoreLib.Models.AppLifeCycle;

namespace LauncherCoreLib.Models.DeployedEnv;

public class DeploymentLogging : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnStartup_003Ed__5 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public DeploymentLogging _003C_003E4__this;

		public CancellationToken shutdown;

		private object _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DeploymentLogging CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			try
			{
				AsyncSubject<string> asyncSubject;
				if (num != 0)
				{
					bool? flag = CS_0024_003C_003E8__locals0.IsAdministrator();
					if (flag.HasValue)
					{
						if (flag.GetValueOrDefault())
						{
							CS_0024_003C_003E8__locals0.Log(LogLevel.Warning, "Running as admin.");
						}
						else
						{
							CS_0024_003C_003E8__locals0.Log(LogLevel.Info, "Running as standard user.");
						}
					}
					else
					{
						CS_0024_003C_003E8__locals0.Log(LogLevel.Error, "Could not determine user type.");
					}
					if (CS_0024_003C_003E8__locals0.IsDeploymentRegistryBased())
					{
						System.Collections.Generic.IEnumerator<InstallationVariable> enumerator = InstallationVariableProviderExtensions.ValidVariables().GetEnumerator();
						try
						{
							while (((System.Collections.IEnumerator)enumerator).MoveNext())
							{
								InstallationVariable current = enumerator.Current;
								if (!CS_0024_003C_003E8__locals0._installationVariables.VariableWasDeployed(current))
								{
									string format = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "The {0} installation variable was not deployed. Expected to find it in [{1}]", (object)current, (object)string.Join(", ", Enumerable.Select<string, string>(CS_0024_003C_003E8__locals0._installationVariables.GetExpectedPaths(current), (Func<string, string>)((string s) => "'" + s + "'"))));
									CS_0024_003C_003E8__locals0.Log(LogLevel.Warning, format);
								}
							}
						}
						finally
						{
							if (num < 0)
							{
								((System.IDisposable)enumerator)?.Dispose();
							}
						}
					}
					Dictionary<string, string> obj = new Dictionary<string, string>();
					obj.Add("Bin", CS_0024_003C_003E8__locals0._deployment.RootBinPath);
					obj.Add("Data", CS_0024_003C_003E8__locals0._deployment.RootDataPath);
					obj.Add("User", CS_0024_003C_003E8__locals0._deployment.UserAppDataDirPath);
					obj.Add("TEMP", CS_0024_003C_003E8__locals0._deployment.AppTempDirectoryPath);
					obj.Add("Process", CS_0024_003C_003E8__locals0._deployment.PerProcessTempDirectory());
					Dictionary<string, string> val = obj;
					CS_0024_003C_003E8__locals0.Log(LogLevel.Info, "Deployment paths:\n\t{0}", string.Join("\n\t", Enumerable.Select<KeyValuePair<string, string>, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)val, (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kv) => kv.Key + ":\t" + kv.Value))));
					asyncSubject = CS_0024_003C_003E8__locals0._realityXpDetector.RealityXpInstallationSigns().TakeUntil<string, Unit>(shutdown.ToObservable()).SubscribeOn<string>((IScheduler)Scheduler.Default)
						.Do<string>((Action<string>)([CompilerGenerated] (string detection) =>
						{
							CS_0024_003C_003E8__locals0.Log(LogLevel.Warning, "[Reality XP Detection] " + detection);
						}))
						.CatchAndComplete<string>([CompilerGenerated] (System.Exception e) =>
						{
							CS_0024_003C_003E8__locals0.Log(e, "[Reality XP Detection]");
						})
						.LastOrDefaultAsync<string>()
						.GetAwaiter<string>();
					if (!asyncSubject.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = asyncSubject;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitOnCompleted<AsyncSubject<string>, _003COnStartup_003Ed__5>(ref asyncSubject, ref this);
						return;
					}
				}
				else
				{
					asyncSubject = (AsyncSubject<string>)_003C_003Eu__1;
					_003C_003Eu__1 = null;
					num = (_003C_003E1__state = -1);
				}
				asyncSubject.GetResult();
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

	private const string REALITY_XP_DETECTION_MARKER = "[Reality XP Detection]";

	private readonly IInstallationVariableProvider _installationVariables;

	private readonly IDeploymentPathProvider _deployment;

	private readonly IRealityXpInstallationDetector _realityXpDetector;

	public DeploymentLogging(IInstallationVariableProvider installationVariables, IDeploymentPathProvider deployment, IRealityXpInstallationDetector realityXPDetector)
	{
		base.ActionName = "Deployment Logging";
		_installationVariables = installationVariables;
		_deployment = deployment;
		_realityXpDetector = realityXPDetector;
	}

	[AsyncStateMachine(typeof(_003COnStartup_003Ed__5))]
	public System.Threading.Tasks.Task OnStartup(CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003COnStartup_003Ed__5 _003COnStartup_003Ed__ = default(_003COnStartup_003Ed__5);
		_003COnStartup_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnStartup_003Ed__._003C_003E4__this = this;
		_003COnStartup_003Ed__.shutdown = shutdown;
		_003COnStartup_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Start<_003COnStartup_003Ed__5>(ref _003COnStartup_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Task;
	}

	private bool? IsAdministrator()
	{
		//IL_0029: Expected O, but got Unknown
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			WindowsIdentity current = WindowsIdentity.GetCurrent();
			try
			{
				return new WindowsPrincipal(current).IsInRole((WindowsBuiltInRole)544);
			}
			finally
			{
				((System.IDisposable)current)?.Dispose();
			}
		}
		catch (SecurityException val)
		{
			SecurityException e = val;
			Log((System.Exception)(object)e, "Checking User account type");
			return null;
		}
	}

	private bool IsDeploymentRegistryBased()
	{
		IDeploymentPathProvider deploymentPathProvider = _deployment;
		if (deploymentPathProvider is PathProviderSelector pathProviderSelector)
		{
			deploymentPathProvider = pathProviderSelector.ActivePathProvider;
		}
		return deploymentPathProvider is DeploymentPathProvider;
	}
}
