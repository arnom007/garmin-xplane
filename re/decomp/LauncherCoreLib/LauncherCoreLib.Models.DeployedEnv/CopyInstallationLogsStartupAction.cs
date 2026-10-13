using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
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

namespace LauncherCoreLib.Models.DeployedEnv;

public class CopyInstallationLogsStartupAction : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003COnStartup_003Eb__5_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ValueTuple<string, string, bool>> _003C_003Et__builder;

		public string logFilePath;

		public CopyInstallationLogsStartupAction _003C_003E4__this;

		private string _003CdestinationPath_003E5__2;

		private bool _003CshouldBeCopied_003E5__3;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CopyInstallationLogsStartupAction copyInstallationLogsStartupAction = _003C_003E4__this;
			ValueTuple<string, string, bool> result;
			try
			{
				if (num != 0)
				{
					string fileName = Path.GetFileName(logFilePath);
					_003CdestinationPath_003E5__2 = Path.Combine(copyInstallationLogsStartupAction._deployment.LogDirectoryPath(), fileName);
					_003CshouldBeCopied_003E5__3 = !copyInstallationLogsStartupAction._fs.PathExists(_003CdestinationPath_003E5__2);
					if (!_003CshouldBeCopied_003E5__3)
					{
						goto IL_0162;
					}
					copyInstallationLogsStartupAction.Log(LogLevel.Info, "Copying '{0}' -> '{1}'", logFilePath, _003CdestinationPath_003E5__2);
				}
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = copyInstallationLogsStartupAction._fs.CopyAndCensorFileAndKeepTimestamps(logFilePath, _003CdestinationPath_003E5__2, copyInstallationLogsStartupAction._aliasProvider).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003COnStartup_003Eb__5_0_003Ed>(ref awaiter, ref this);
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
				catch (System.Exception e)
				{
					copyInstallationLogsStartupAction.Log(e, "Copying '{0}' -> '{1}'", logFilePath, _003CdestinationPath_003E5__2);
					copyInstallationLogsStartupAction._fs.DeleteIfExists(_003CdestinationPath_003E5__2);
					throw;
				}
				copyInstallationLogsStartupAction.Log(LogLevel.Info, "Copied '{0}' -> '{1}'", logFilePath, _003CdestinationPath_003E5__2);
				goto IL_0162;
				IL_0162:
				result = new ValueTuple<string, string, bool>(logFilePath, _003CdestinationPath_003E5__2, _003CshouldBeCopied_003E5__3);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CdestinationPath_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CdestinationPath_003E5__2 = null;
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
	private struct _003COnStartup_003Ed__5 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public CopyInstallationLogsStartupAction _003C_003E4__this;

		public CancellationToken shutdown;

		private ConfiguredTaskAwaiter<System.Collections.Generic.IList<ValueTuple<string, string, bool>>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CopyInstallationLogsStartupAction CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			try
			{
				try
				{
					ConfiguredTaskAwaiter<System.Collections.Generic.IList<ValueTuple<string, string, bool>>> awaiter;
					if (num != 0)
					{
						awaiter = CS_0024_003C_003E8__locals0._logFiles.InstallationLogFilePaths.TakeUntil<string>(shutdown).Select<string, System.Threading.Tasks.Task<ValueTuple<string, string, bool>>>((Func<string, System.Threading.Tasks.Task<ValueTuple<string, string, bool>>>)([AsyncStateMachine(typeof(_003C_003COnStartup_003Eb__5_0_003Ed))] [CompilerGenerated] async (string logFilePath) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							string fileName = Path.GetFileName(logFilePath);
							string destinationPath = Path.Combine(CS_0024_003C_003E8__locals0._deployment.LogDirectoryPath(), fileName);
							bool shouldBeCopied = !CS_0024_003C_003E8__locals0._fs.PathExists(destinationPath);
							if (shouldBeCopied)
							{
								CS_0024_003C_003E8__locals0.Log(LogLevel.Info, "Copying '{0}' -> '{1}'", logFilePath, destinationPath);
								try
								{
									await CS_0024_003C_003E8__locals0._fs.CopyAndCensorFileAndKeepTimestamps(logFilePath, destinationPath, CS_0024_003C_003E8__locals0._aliasProvider).DetachContext();
								}
								catch (System.Exception e)
								{
									CS_0024_003C_003E8__locals0.Log(e, "Copying '{0}' -> '{1}'", logFilePath, destinationPath);
									CS_0024_003C_003E8__locals0._fs.DeleteIfExists(destinationPath);
									throw;
								}
								CS_0024_003C_003E8__locals0.Log(LogLevel.Info, "Copied '{0}' -> '{1}'", logFilePath, destinationPath);
							}
							return new ValueTuple<string, string, bool>(logFilePath, destinationPath, shouldBeCopied);
						})).Merge<ValueTuple<string, string, bool>>()
							.ToList<ValueTuple<string, string, bool>>()
							.Do<System.Collections.Generic.IList<ValueTuple<string, string, bool>>>((Action<System.Collections.Generic.IList<ValueTuple<string, string, bool>>>)([CompilerGenerated] (System.Collections.Generic.IList<ValueTuple<string, string, bool>> allCopiedFilePaths) =>
							{
								if (((System.Collections.Generic.ICollection<ValueTuple<string, string, bool>>)allCopiedFilePaths).Count == 0)
								{
									CS_0024_003C_003E8__locals0.Log(LogLevel.Warning, "No installation logs were found to copy.");
								}
								else if (!Enumerable.Any<ValueTuple<string, string, bool>>((System.Collections.Generic.IEnumerable<ValueTuple<string, string, bool>>)allCopiedFilePaths, (Func<ValueTuple<string, string, bool>, bool>)((ValueTuple<string, string, bool> copyResult) => copyResult.Item3)))
								{
									CS_0024_003C_003E8__locals0.Log(LogLevel.Info, "No new installation logs were copied.");
								}
							}))
							.LastOrDefaultAsync<System.Collections.Generic.IList<ValueTuple<string, string, bool>>>()
							.SubscribeOn<System.Collections.Generic.IList<ValueTuple<string, string, bool>>>((IScheduler)Scheduler.Default)
							.ToTask<System.Collections.Generic.IList<ValueTuple<string, string, bool>>>()
							.DetachContext()
							.GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IList<ValueTuple<string, string, bool>>>, _003COnStartup_003Ed__5>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IList<ValueTuple<string, string, bool>>>);
						num = (_003C_003E1__state = -1);
					}
					awaiter.GetResult();
				}
				catch (System.Exception ex) when (ex is IOException || ex is FileNotFoundException)
				{
					CS_0024_003C_003E8__locals0.Log(ex, "Could not copy Installation Logs.");
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

	private readonly IInstallationLogProvider _logFiles;

	private readonly IDeploymentPathProvider _deployment;

	private readonly IFileSystem _fs;

	private readonly IConfidentialAliasProvider _aliasProvider;

	public CopyInstallationLogsStartupAction(IInstallationLogProvider logFiles, IDeploymentPathProvider deployment, IFileSystem fs, IConfidentialAliasProvider aliasProvider)
	{
		base.ActionName = "Copy Installation Logs";
		_logFiles = logFiles;
		_deployment = deployment;
		_fs = fs;
		_aliasProvider = aliasProvider;
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
}
