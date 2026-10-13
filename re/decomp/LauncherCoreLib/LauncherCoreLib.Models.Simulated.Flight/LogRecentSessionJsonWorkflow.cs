using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Tasks;
using LauncherCoreLib.Models.DeployedEnv;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class LogRecentSessionJsonWorkflow : PreflightOnlyWorkflow
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CPreFlight_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISessionEnvironment> _003C_003Et__builder;

		public ISessionEnvironment sessionEnvironment;

		public LogRecentSessionJsonWorkflow _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			LogRecentSessionJsonWorkflow logRecentSessionJsonWorkflow = _003C_003E4__this;
			ISessionEnvironment result;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					string sessionJsonPath = sessionEnvironment.Inputs.SessionJsonPath;
					if (sessionJsonPath == null || !logRecentSessionJsonWorkflow._fs.PathExists(sessionJsonPath))
					{
						throw new InvalidOperationException("The session JSON logging workflow must run after the serialize workflow.");
					}
					string text = Path.Combine(logRecentSessionJsonWorkflow._deployment.LogDirectoryPath(), FormatRecentSessionJsonFileName(sessionJsonPath));
					logRecentSessionJsonWorkflow._fs.DeleteIfExists(text);
					ConfiguredTaskAwaitable val = logRecentSessionJsonWorkflow._fs.CopyAndCensorFileAndKeepTimestamps(sessionJsonPath, text, logRecentSessionJsonWorkflow._aliasProvider).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CPreFlight_003Ed__7>(ref awaiter, ref this);
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
				OldFileCleanup.KeepMostRecentFilesThatMatch(5, logRecentSessionJsonWorkflow._fs, logRecentSessionJsonWorkflow._deployment.LogDirectoryPath(), IsRecentSessionLog).LogFileCleanupRecords("session JSON file");
				result = sessionEnvironment;
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

	private const string RECENT_SESSION_LOG_PREFIX = "session_";

	private const int MAX_RECENT_JSON_SESSION_FILES = 5;

	private readonly IDeploymentPathProvider _deployment;

	private readonly IFileSystem _fs;

	private readonly IConfidentialAliasProvider _aliasProvider;

	public LogRecentSessionJsonWorkflow(IDeploymentPathProvider deployment, IFileSystem fileSystem, IConfidentialAliasProvider aliasProvider)
	{
		_deployment = deployment;
		_fs = fileSystem;
		_aliasProvider = aliasProvider;
	}

	public static bool IsRecentSessionLog(FileSystemEntry entry)
	{
		if (entry.Name.StartsWith("session_", (StringComparison)4))
		{
			return string.Equals(Path.GetExtension(entry.Name), ".json", (StringComparison)4);
		}
		return false;
	}

	[AsyncStateMachine(typeof(_003CPreFlight_003Ed__7))]
	public override async System.Threading.Tasks.Task<ISessionEnvironment> PreFlight(ISessionEnvironment sessionEnvironment, CancellationToken token)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string sessionJsonPath = sessionEnvironment.Inputs.SessionJsonPath;
		if (sessionJsonPath == null || !_fs.PathExists(sessionJsonPath))
		{
			throw new InvalidOperationException("The session JSON logging workflow must run after the serialize workflow.");
		}
		string text = Path.Combine(_deployment.LogDirectoryPath(), FormatRecentSessionJsonFileName(sessionJsonPath));
		_fs.DeleteIfExists(text);
		await _fs.CopyAndCensorFileAndKeepTimestamps(sessionJsonPath, text, _aliasProvider).DetachContext();
		OldFileCleanup.KeepMostRecentFilesThatMatch(5, _fs, _deployment.LogDirectoryPath(), IsRecentSessionLog).LogFileCleanupRecords("session JSON file");
		return sessionEnvironment;
	}

	private static string FormatRecentSessionJsonFileName(string sessionJsonpath)
	{
		return "session_" + Path.GetFileName(Path.GetDirectoryName(sessionJsonpath)) + "_" + Path.GetFileName(sessionJsonpath);
	}
}
