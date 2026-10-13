using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Processes;
using LOLA.Tasks;
using LauncherCoreLib.Models.AppLifeCycle;

namespace LauncherCoreLib.Models.DeployedEnv;

public class SystemInfoLoggingStartupAction : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass11_0
	{
		public SystemInfoLoggingStartupAction _003C_003E4__this;

		public string dxDiagPath;

		internal IProcessSession _003CCheckAndExportDxDiag_003Eb__0(string exportPath)
		{
			return _003C_003E4__this._processes.Fork(_003C_003E4__this._deployment.LogDirectoryPath(), dxDiagPath, "/dontskip", "/whql:off", "/t", exportPath);
		}

		internal IProcessSession _003CCheckAndExportDxDiag_003Eb__1(string exportPath)
		{
			return _003C_003E4__this._processes.Fork(_003C_003E4__this._deployment.LogDirectoryPath(), dxDiagPath, "/dontskip", "/whql:off", "/x", exportPath);
		}

		internal IProcessSession _003CCheckAndExportDxDiag_003Eb__2(string exportPath)
		{
			return _003C_003E4__this._processes.Fork(_003C_003E4__this._deployment.LogDirectoryPath(), dxDiagPath, "/dontskip", "/whql:off", "/t", exportPath);
		}

		internal IProcessSession _003CCheckAndExportDxDiag_003Eb__3(string exportPath)
		{
			return _003C_003E4__this._processes.Fork(_003C_003E4__this._deployment.LogDirectoryPath(), dxDiagPath, "/dontskip", "/whql:off", "/x", exportPath);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass12_0
	{
		public Func<string, IProcessSession> exportProcessFactory;

		public SystemInfoLoggingStartupAction _003C_003E4__this;

		public string exportName;

		public string filePath;

		public string tmp;

		internal IProcessSession _003CCheckFileAndExport_003Eb__0()
		{
			return exportProcessFactory.Invoke(tmp);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass12_1
	{
		public IProcessSession exportProcess;

		public _003C_003Ec__DisplayClass12_0 CS_0024_003C_003E8__locals1;

		internal void _003CCheckFileAndExport_003Eb__1()
		{
			CS_0024_003C_003E8__locals1._003C_003E4__this.Log(LogLevel.Info, "Canceling the " + CS_0024_003C_003E8__locals1.exportName + " export.");
			exportProcess.ForceClose();
			CS_0024_003C_003E8__locals1._003C_003E4__this._fs.DeleteIfExists(CS_0024_003C_003E8__locals1.filePath);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCheckAndExportDxDiag_003Ed__11 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SystemInfoLoggingStartupAction _003C_003E4__this;

		public CancellationToken shutdown;

		private _003C_003Ec__DisplayClass11_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SystemInfoLoggingStartupAction systemInfoLoggingStartupAction = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaitable val;
				ConfiguredTaskAwaiter awaiter;
				switch (num)
				{
				default:
					_003C_003E8__1 = new _003C_003Ec__DisplayClass11_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.dxDiagPath = Path.Combine(Environment.GetFolderPath((SpecialFolder)37), "dxdiag.exe");
					if (Environment.Is64BitOperatingSystem)
					{
						if (!Environment.Is64BitProcess)
						{
							_003C_003E8__1.dxDiagPath = Path.Combine(Environment.GetFolderPath((SpecialFolder)36), "Sysnative", "dxdiag.exe");
						}
						val = systemInfoLoggingStartupAction.CheckFileAndExport("DxDiag_64(txt)", Path.Combine(systemInfoLoggingStartupAction._deployment.LogDirectoryPath(), "DxDiag_64.txt"), (string exportPath) => _003C_003E8__1._003C_003E4__this._processes.Fork(_003C_003E8__1._003C_003E4__this._deployment.LogDirectoryPath(), _003C_003E8__1.dxDiagPath, "/dontskip", "/whql:off", "/t", exportPath), shutdown).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCheckAndExportDxDiag_003Ed__11>(ref awaiter, ref this);
							return;
						}
						goto IL_011b;
					}
					goto IL_01d2;
				case 0:
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_011b;
				case 1:
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_01af;
				case 2:
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_025f;
				case 3:
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						break;
					}
					IL_011b:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					val = systemInfoLoggingStartupAction.CheckFileAndExport("DxDiag_64(xml)", Path.Combine(systemInfoLoggingStartupAction._deployment.LogDirectoryPath(), "DxDiag_64.xml"), (string exportPath) => _003C_003E8__1._003C_003E4__this._processes.Fork(_003C_003E8__1._003C_003E4__this._deployment.LogDirectoryPath(), _003C_003E8__1.dxDiagPath, "/dontskip", "/whql:off", "/x", exportPath), shutdown).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCheckAndExportDxDiag_003Ed__11>(ref awaiter, ref this);
						return;
					}
					goto IL_01af;
					IL_01af:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					_003C_003E8__1.dxDiagPath = Path.Combine(Environment.GetFolderPath((SpecialFolder)41), "dxdiag.exe");
					goto IL_01d2;
					IL_025f:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					val = systemInfoLoggingStartupAction.CheckFileAndExport("DxDiag(xml)", Path.Combine(systemInfoLoggingStartupAction._deployment.LogDirectoryPath(), "DxDiag.xml"), (string exportPath) => _003C_003E8__1._003C_003E4__this._processes.Fork(_003C_003E8__1._003C_003E4__this._deployment.LogDirectoryPath(), _003C_003E8__1.dxDiagPath, "/dontskip", "/whql:off", "/x", exportPath), shutdown).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 3);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCheckAndExportDxDiag_003Ed__11>(ref awaiter, ref this);
						return;
					}
					break;
					IL_01d2:
					val = systemInfoLoggingStartupAction.CheckFileAndExport("DxDiag(txt)", Path.Combine(systemInfoLoggingStartupAction._deployment.LogDirectoryPath(), "DxDiag.txt"), (string exportPath) => _003C_003E8__1._003C_003E4__this._processes.Fork(_003C_003E8__1._003C_003E4__this._deployment.LogDirectoryPath(), _003C_003E8__1.dxDiagPath, "/dontskip", "/whql:off", "/t", exportPath), shutdown).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCheckAndExportDxDiag_003Ed__11>(ref awaiter, ref this);
						return;
					}
					goto IL_025f;
				}
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
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
	private struct _003CCheckFileAndExport_003Ed__12 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public Func<string, IProcessSession> exportProcessFactory;

		public SystemInfoLoggingStartupAction _003C_003E4__this;

		public string exportName;

		public string filePath;

		public CancellationToken shutdown;

		private _003C_003Ec__DisplayClass12_1 _003C_003E8__1;

		private _003C_003Ec__DisplayClass12_0 _003C_003E8__2;

		private CancellationTokenRegistration _003C_003E7__wrap1;

		private ConfiguredTaskAwaiter<ProcessResult> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Expected O, but got Unknown
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SystemInfoLoggingStartupAction systemInfoLoggingStartupAction = _003C_003E4__this;
			try
			{
				if ((uint)num <= 1u)
				{
					goto IL_00ec;
				}
				_003C_003E8__2 = new _003C_003Ec__DisplayClass12_0();
				_003C_003E8__2.exportProcessFactory = exportProcessFactory;
				_003C_003E8__2._003C_003E4__this = _003C_003E4__this;
				_003C_003E8__2.exportName = exportName;
				_003C_003E8__2.filePath = filePath;
				if (!systemInfoLoggingStartupAction._fs.PathExists(_003C_003E8__2.filePath))
				{
					systemInfoLoggingStartupAction.Log(LogLevel.Info, "Exporting " + _003C_003E8__2.exportName + " data to '{0}'", _003C_003E8__2.filePath);
					_003C_003E8__2.tmp = _003C_003E8__2.filePath + ".tmp";
					systemInfoLoggingStartupAction._fs.DeleteIfExists(_003C_003E8__2.tmp);
					goto IL_00ec;
				}
				systemInfoLoggingStartupAction.Log(LogLevel.Info, "The '{0}' file already exists. Skipping export.", _003C_003E8__2.filePath);
				goto end_IL_000e;
				IL_00ec:
				try
				{
					if ((uint)num > 1u)
					{
						_003C_003E8__1 = new _003C_003Ec__DisplayClass12_1();
						_003C_003E8__1.CS_0024_003C_003E8__locals1 = _003C_003E8__2;
						_003C_003E8__1.exportProcess = systemInfoLoggingStartupAction.CreateProcessSafe(_003C_003E8__1.CS_0024_003C_003E8__locals1.exportName, () => _003C_003E8__1.CS_0024_003C_003E8__locals1.exportProcessFactory.Invoke(_003C_003E8__1.CS_0024_003C_003E8__locals1.tmp));
					}
					try
					{
						if (num == 0)
						{
							goto IL_01a9;
						}
						ConfiguredTaskAwaiter awaiter;
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_02d7;
						}
						if (_003C_003E8__1.exportProcess != null)
						{
							_003C_003E7__wrap1 = ((CancellationToken)(ref shutdown)).Register((Action)delegate
							{
								_003C_003E8__1.CS_0024_003C_003E8__locals1._003C_003E4__this.Log(LogLevel.Info, "Canceling the " + _003C_003E8__1.CS_0024_003C_003E8__locals1.exportName + " export.");
								_003C_003E8__1.exportProcess.ForceClose();
								_003C_003E8__1.CS_0024_003C_003E8__locals1._003C_003E4__this._fs.DeleteIfExists(_003C_003E8__1.CS_0024_003C_003E8__locals1.filePath);
							}, false);
							goto IL_01a9;
						}
						systemInfoLoggingStartupAction.Log(LogLevel.Error, "The {0} export factory returned a null process.", _003C_003E8__1.CS_0024_003C_003E8__locals1.exportName);
						goto end_IL_00ec;
						IL_02d7:
						((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
						systemInfoLoggingStartupAction.Log(LogLevel.Info, _003C_003E8__1.CS_0024_003C_003E8__locals1.exportName + " export completed.");
						goto end_IL_0144;
						IL_01a9:
						ProcessResult result;
						try
						{
							ConfiguredTaskAwaiter<ProcessResult> awaiter2;
							if (num != 0)
							{
								awaiter2 = _003C_003E8__1.exportProcess.Exited.DetachContext().GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = (_003C_003E1__state = 0);
									_003C_003Eu__1 = awaiter2;
									((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ProcessResult>, _003CCheckFileAndExport_003Ed__12>(ref awaiter2, ref this);
									return;
								}
							}
							else
							{
								awaiter2 = _003C_003Eu__1;
								_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ProcessResult>);
								num = (_003C_003E1__state = -1);
							}
							result = awaiter2.GetResult();
						}
						finally
						{
							if (num < 0)
							{
								((System.IDisposable)(CancellationTokenRegistration)(ref _003C_003E7__wrap1)).Dispose();
							}
						}
						_003C_003E7__wrap1 = default(CancellationTokenRegistration);
						if (result.ExitCode == 0)
						{
							ConfiguredTaskAwaitable val = systemInfoLoggingStartupAction._fs.CopyAndCensorFileAndKeepTimestamps(_003C_003E8__1.CS_0024_003C_003E8__locals1.tmp, _003C_003E8__1.CS_0024_003C_003E8__locals1.filePath, systemInfoLoggingStartupAction._aliasProvider).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCheckFileAndExport_003Ed__12>(ref awaiter, ref this);
								return;
							}
							goto IL_02d7;
						}
						if (!((CancellationToken)(ref shutdown)).IsCancellationRequested)
						{
							systemInfoLoggingStartupAction.Log(LogLevel.Warning, "{0} export returned an unexpected return code: {1}", _003C_003E8__1.CS_0024_003C_003E8__locals1.exportName, ProcessLogFormatting.FormatHexExitCode(result.ExitCode));
						}
						else
						{
							systemInfoLoggingStartupAction.Log(LogLevel.Info, _003C_003E8__1.CS_0024_003C_003E8__locals1.exportName + " export canceled.");
						}
						end_IL_0144:;
					}
					finally
					{
						if (num < 0 && _003C_003E8__1.exportProcess != null)
						{
							((System.IDisposable)_003C_003E8__1.exportProcess).Dispose();
						}
					}
					_003C_003E8__1 = null;
					end_IL_00ec:;
				}
				finally
				{
					if (num < 0)
					{
						systemInfoLoggingStartupAction._fs.DeleteIfExists(_003C_003E8__2.tmp);
					}
				}
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__2 = null;
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
	private struct _003COnStartup_003Ed__10 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SystemInfoLoggingStartupAction _003C_003E4__this;

		public CancellationToken shutdown;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SystemInfoLoggingStartupAction systemInfoLoggingStartupAction = _003C_003E4__this;
			try
			{
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = systemInfoLoggingStartupAction.CheckAndExportDxDiag(shutdown).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003COnStartup_003Ed__10>(ref awaiter, ref this);
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
					systemInfoLoggingStartupAction.Log(e, "Failed to gather DX Diag info. Not worth crashing the app over.");
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

	private const string DXDIAG_TXT_FILE_NAME = "DxDiag.txt";

	private const string DXDIAG_XML_FILE_NAME = "DxDiag.xml";

	private const string DXDIAG_64_TXT_FILE_NAME = "DxDiag_64.txt";

	private const string DXDIAG_64_XML_FILE_NAME = "DxDiag_64.xml";

	private readonly IDeploymentPathProvider _deployment;

	private readonly IFileSystem _fs;

	private readonly IOsProcessOperations _processes;

	private readonly IConfidentialAliasProvider _aliasProvider;

	public SystemInfoLoggingStartupAction(IDeploymentPathProvider deployment, IFileSystem fileSystem, IOsProcessOperations processes, IConfidentialAliasProvider aliasProvider)
	{
		_deployment = deployment;
		_fs = fileSystem;
		_processes = processes;
		_aliasProvider = aliasProvider;
		base.ActionName = "System Information Logging";
	}

	public static bool IsSystemInfoLoggingFile(FileSystemEntry entry)
	{
		if (!string.Equals(entry.Name, "DxDiag.txt", (StringComparison)5) && !string.Equals(entry.Name, "DxDiag.xml", (StringComparison)5) && !string.Equals(entry.Name, "DxDiag_64.xml", (StringComparison)5))
		{
			return string.Equals(entry.Name, "DxDiag_64.xml", (StringComparison)5);
		}
		return true;
	}

	[AsyncStateMachine(typeof(_003COnStartup_003Ed__10))]
	public System.Threading.Tasks.Task OnStartup(CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003COnStartup_003Ed__10 _003COnStartup_003Ed__ = default(_003COnStartup_003Ed__10);
		_003COnStartup_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnStartup_003Ed__._003C_003E4__this = this;
		_003COnStartup_003Ed__.shutdown = shutdown;
		_003COnStartup_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Start<_003COnStartup_003Ed__10>(ref _003COnStartup_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CCheckAndExportDxDiag_003Ed__11))]
	private System.Threading.Tasks.Task CheckAndExportDxDiag(CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003CCheckAndExportDxDiag_003Ed__11 _003CCheckAndExportDxDiag_003Ed__ = default(_003CCheckAndExportDxDiag_003Ed__11);
		_003CCheckAndExportDxDiag_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CCheckAndExportDxDiag_003Ed__._003C_003E4__this = this;
		_003CCheckAndExportDxDiag_003Ed__.shutdown = shutdown;
		_003CCheckAndExportDxDiag_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CCheckAndExportDxDiag_003Ed__._003C_003Et__builder)).Start<_003CCheckAndExportDxDiag_003Ed__11>(ref _003CCheckAndExportDxDiag_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CCheckAndExportDxDiag_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CCheckFileAndExport_003Ed__12))]
	private System.Threading.Tasks.Task CheckFileAndExport(string exportName, string filePath, Func<string, IProcessSession> exportProcessFactory, CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		_003CCheckFileAndExport_003Ed__12 _003CCheckFileAndExport_003Ed__ = default(_003CCheckFileAndExport_003Ed__12);
		_003CCheckFileAndExport_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CCheckFileAndExport_003Ed__._003C_003E4__this = this;
		_003CCheckFileAndExport_003Ed__.exportName = exportName;
		_003CCheckFileAndExport_003Ed__.filePath = filePath;
		_003CCheckFileAndExport_003Ed__.exportProcessFactory = exportProcessFactory;
		_003CCheckFileAndExport_003Ed__.shutdown = shutdown;
		_003CCheckFileAndExport_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CCheckFileAndExport_003Ed__._003C_003Et__builder)).Start<_003CCheckFileAndExport_003Ed__12>(ref _003CCheckFileAndExport_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CCheckFileAndExport_003Ed__._003C_003Et__builder)).Task;
	}

	private IProcessSession CreateProcessSafe(string exportName, Func<IProcessSession> exportProcessFactory)
	{
		try
		{
			return exportProcessFactory.Invoke();
		}
		catch (System.Exception e)
		{
			Log(e, "Exporting {0} failed.", exportName);
			throw;
		}
	}
}
