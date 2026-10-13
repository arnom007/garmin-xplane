using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Compression;
using LOLA.FileSystem;
using LOLA.Tasks;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherCoreLib.Models.CrashReporting;

internal class CrashReportCollector : ICrashReportCollector
{
	private sealed class InprogressScope : IInProgressCrashScope, System.IDisposable
	{
		private readonly CrashReportCollector _collector;

		[field: CompilerGenerated]
		public SessionCrashScope ContainedScope
		{
			[CompilerGenerated]
			get;
		}

		public InprogressScope(CrashReportCollector collector, SessionCrashScope scope)
		{
			_collector = collector;
			ContainedScope = scope;
		}

		public void Dispose()
		{
			_collector.ReleaseScope(ContainedScope);
		}

		public string ToString()
		{
			return ContainedScope.SessionName;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CFormatAndSendCrash_003Ed__18 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SimInstanceCrashInfo simCrash;

		public CrashReportCollector _003C_003E4__this;

		private ConfiguredTaskAwaiter<CrashRptInfo> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CrashReportCollector crashReportCollector = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaiter<CrashRptInfo> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_00f1;
					}
					awaiter2 = CrashRptXmlParser.ParseXmlFileAsync(simCrash.CrashRptPath).ConfigureAwait(false).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<CrashRptInfo>, _003CFormatAndSendCrash_003Ed__18>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<CrashRptInfo>);
					num = (_003C_003E1__state = -1);
				}
				CrashRptInfo result = awaiter2.GetResult();
				ConfiguredTaskAwaitable val = crashReportCollector._errorMediator.PostCrashReport(simCrash, result).DetachContext();
				awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CFormatAndSendCrash_003Ed__18>(ref awaiter, ref this);
					return;
				}
				goto IL_00f1;
				IL_00f1:
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CPackAndSendSessionReports_003Ed__17 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SessionCrashScope session_scope;

		public CrashReportCollector _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CrashReportCollector crashReportCollector = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaitable val;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_0106;
					}
					val = session_scope.RunCensorOnTextFiles().ConfigureAwait(false);
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CPackAndSendSessionReports_003Ed__17>(ref awaiter, ref this);
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
				session_scope.SaveToZipArchive();
				val = System.Threading.Tasks.Task.WhenAll((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task>)Enumerable.ToList<System.Threading.Tasks.Task>(Enumerable.Select<SimInstanceCrashInfo, System.Threading.Tasks.Task>(session_scope.GetIndividualSimCrashes(), (Func<SimInstanceCrashInfo, System.Threading.Tasks.Task>)crashReportCollector.FormatAndSendCrash))).DetachContext();
				awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__1 = awaiter;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CPackAndSendSessionReports_003Ed__17>(ref awaiter, ref this);
					return;
				}
				goto IL_0106;
				IL_0106:
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CSendReports_003Ed__11 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public CrashReportCollector _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CrashReportCollector crashReportCollector = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = System.Threading.Tasks.Task.WhenAll((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task>)Enumerable.ToList<System.Threading.Tasks.Task>(Enumerable.Select<SessionCrashScope, System.Threading.Tasks.Task>((System.Collections.Generic.IEnumerable<SessionCrashScope>)crashReportCollector._discoveredScopes, (Func<SessionCrashScope, System.Threading.Tasks.Task>)crashReportCollector.PackAndSendSessionReports))).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CSendReports_003Ed__11>(ref awaiter, ref this);
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

	private readonly IDeploymentPathProvider _deployment;

	private readonly IErrorReportMediator _errorMediator;

	private readonly IModifiableGlobalSettingsTable _globalSettings;

	private readonly IConfidentialAliasProvider _confidentialAliasProvider;

	private readonly IFileSystem _fs;

	private readonly IZipCompression _zipCompression;

	private readonly List<SessionCrashScope> _discoveredScopes = new List<SessionCrashScope>();

	private readonly List<SessionCrashScope> _inProgressScopes = new List<SessionCrashScope>();

	private readonly object _inprogressMutex = new object();

	[CompilerGenerated]
	private EventHandler? m_ReportFound;

	public event EventHandler ReportFound
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			EventHandler val = this.m_ReportFound;
			EventHandler val2;
			do
			{
				val2 = val;
				EventHandler val3 = (EventHandler)System.Delegate.Combine((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<EventHandler>(ref this.m_ReportFound, val3, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			EventHandler val = this.m_ReportFound;
			EventHandler val2;
			do
			{
				val2 = val;
				EventHandler val3 = (EventHandler)System.Delegate.Remove((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<EventHandler>(ref this.m_ReportFound, val3, val2);
			}
			while (val != val2);
		}
	}

	public CrashReportCollector(IDeploymentPathProvider deployment, IModifiableGlobalSettingsTable global_settings, IErrorReportMediator errorReportMediator, IFileSystem fileSystem, IZipCompression zipCompression, IConfidentialAliasProvider confidentialAliasProvider)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		_deployment = deployment ?? throw new ArgumentNullException("deployment");
		_globalSettings = global_settings ?? throw new ArgumentNullException("global_settings");
		_errorMediator = errorReportMediator ?? throw new ArgumentNullException("errorReportMediator");
		_fs = fileSystem ?? throw new ArgumentNullException("fileSystem");
		_zipCompression = zipCompression ?? throw new ArgumentNullException("zipCompression");
		_confidentialAliasProvider = confidentialAliasProvider;
		LoadCrashReportUserPref();
	}

	public System.Threading.Tasks.Task CheckForOutstandingReports()
	{
		List<SessionCrashScope> val = Enumerable.ToList<SessionCrashScope>(Enumerable.Except<SessionCrashScope>((System.Collections.Generic.IEnumerable<SessionCrashScope>)SessionCrashScope.SessionDirectories(_fs, _zipCompression, _deployment, _confidentialAliasProvider), (System.Collections.Generic.IEnumerable<SessionCrashScope>)_inProgressScopes));
		_discoveredScopes.AddRange((System.Collections.Generic.IEnumerable<SessionCrashScope>)val);
		if (val.Count > 0)
		{
			RaiseDetected();
		}
		return System.Threading.Tasks.Task.CompletedTask;
	}

	[AsyncStateMachine(typeof(_003CSendReports_003Ed__11))]
	public System.Threading.Tasks.Task SendReports()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CSendReports_003Ed__11 _003CSendReports_003Ed__ = default(_003CSendReports_003Ed__11);
		_003CSendReports_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CSendReports_003Ed__._003C_003E4__this = this;
		_003CSendReports_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CSendReports_003Ed__._003C_003Et__builder)).Start<_003CSendReports_003Ed__11>(ref _003CSendReports_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CSendReports_003Ed__._003C_003Et__builder)).Task;
	}

	public System.Threading.Tasks.Task ClearReports()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		List<SessionCrashScope> obj = Enumerable.ToList<SessionCrashScope>((System.Collections.Generic.IEnumerable<SessionCrashScope>)_discoveredScopes);
		_discoveredScopes.Clear();
		Enumerator<SessionCrashScope> enumerator = obj.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.ClearScope();
			}
		}
		finally
		{
			((System.IDisposable)enumerator).Dispose();
		}
		return System.Threading.Tasks.Task.CompletedTask;
	}

	public void ReleaseScope(SessionCrashScope crashScope)
	{
		bool flag = false;
		lock (_inprogressMutex)
		{
			flag = _inProgressScopes.Remove(crashScope);
		}
		if (flag)
		{
			if (crashScope.ContainsCrashes())
			{
				_discoveredScopes.Add(crashScope);
				RaiseDetected();
			}
			else
			{
				crashScope.ClearScope();
			}
		}
	}

	public IInProgressCrashScope CreateCrashScope(ISessionInfo sessionInfo)
	{
		SessionCrashScope sessionCrashScope = new SessionCrashScope(_fs, _zipCompression, _confidentialAliasProvider, sessionInfo.SessionPathRoot);
		lock (_inprogressMutex)
		{
			_inProgressScopes.Add(sessionCrashScope);
		}
		return new InprogressScope(this, sessionCrashScope);
	}

	protected void RaiseDetected()
	{
		EventHandler? reportFound = this.ReportFound;
		if (reportFound != null)
		{
			reportFound.Invoke((object)this, EventArgs.Empty);
		}
	}

	private void LoadCrashReportUserPref()
	{
		_globalSettings.AddCrashReportingSetting();
	}

	[AsyncStateMachine(typeof(_003CPackAndSendSessionReports_003Ed__17))]
	private System.Threading.Tasks.Task PackAndSendSessionReports(SessionCrashScope session_scope)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CPackAndSendSessionReports_003Ed__17 _003CPackAndSendSessionReports_003Ed__ = default(_003CPackAndSendSessionReports_003Ed__17);
		_003CPackAndSendSessionReports_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CPackAndSendSessionReports_003Ed__._003C_003E4__this = this;
		_003CPackAndSendSessionReports_003Ed__.session_scope = session_scope;
		_003CPackAndSendSessionReports_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CPackAndSendSessionReports_003Ed__._003C_003Et__builder)).Start<_003CPackAndSendSessionReports_003Ed__17>(ref _003CPackAndSendSessionReports_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CPackAndSendSessionReports_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CFormatAndSendCrash_003Ed__18))]
	private System.Threading.Tasks.Task FormatAndSendCrash(SimInstanceCrashInfo simCrash)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CFormatAndSendCrash_003Ed__18 _003CFormatAndSendCrash_003Ed__ = default(_003CFormatAndSendCrash_003Ed__18);
		_003CFormatAndSendCrash_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CFormatAndSendCrash_003Ed__._003C_003E4__this = this;
		_003CFormatAndSendCrash_003Ed__.simCrash = simCrash;
		_003CFormatAndSendCrash_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CFormatAndSendCrash_003Ed__._003C_003Et__builder)).Start<_003CFormatAndSendCrash_003Ed__18>(ref _003CFormatAndSendCrash_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CFormatAndSendCrash_003Ed__._003C_003Et__builder)).Task;
	}
}
