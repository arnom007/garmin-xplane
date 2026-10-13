using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.Logging;
using LOLA.Tasks;
using LanguageExt;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.Winfs;
using LauncherCoreLib.Properties;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.CrashReporting;

internal class CrashReportingSessionWorkflow : ISessionWorkflow
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass8_0
	{
		[StructLayout(3)]
		private struct _003C_003CPostFlight_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass8_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass8_0 _003C_003Ec__DisplayClass8_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass8_.crashScope.ContainedScope.RunCensorOnTextFiles().ConfigureAwait(false);
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CPostFlight_003Eb__0_003Ed>(ref awaiter, ref this);
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

		[StructLayout(3)]
		private struct _003C_003CPostFlight_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass8_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass8_0 _003C_003Ec__DisplayClass8_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass8_.crashScope.CopyApplicationLogToSession(_003C_003Ec__DisplayClass8_._003C_003E4__this._deploymentPaths.LogFilePath()).ConfigureAwait(false);
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CPostFlight_003Eb__1_003Ed>(ref awaiter, ref this);
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

		public CrashReportingSessionWorkflow _003C_003E4__this;

		public IInProgressCrashScope crashScope;

		[AsyncStateMachine(typeof(_003C_003CPostFlight_003Eb__0_003Ed))]
		internal System.Threading.Tasks.Task _003CPostFlight_003Eb__0()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CPostFlight_003Eb__0_003Ed _003C_003CPostFlight_003Eb__0_003Ed = default(_003C_003CPostFlight_003Eb__0_003Ed);
			_003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CPostFlight_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CPostFlight_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CPostFlight_003Eb__0_003Ed>(ref _003C_003CPostFlight_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CPostFlight_003Eb__1_003Ed))]
		internal System.Threading.Tasks.Task _003CPostFlight_003Eb__1()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CPostFlight_003Eb__1_003Ed _003C_003CPostFlight_003Eb__1_003Ed = default(_003C_003CPostFlight_003Eb__1_003Ed);
			_003C_003CPostFlight_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CPostFlight_003Eb__1_003Ed._003C_003E4__this = this;
			_003C_003CPostFlight_003Eb__1_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003CPostFlight_003Eb__1_003Ed>(ref _003C_003CPostFlight_003Eb__1_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__1_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass8_1
	{
		[StructLayout(3)]
		private struct _003C_003CPostFlight_003Eb__2_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass8_1 _003C_003E4__this;

			private ConfiguredTaskAwaiter<Unit> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass8_1 _003C_003Ec__DisplayClass8_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<Unit> awaiter;
					if (num != 0)
					{
						awaiter = _003C_003Ec__DisplayClass8_.CS_0024_003C_003E8__locals1.crashScope.ContainedScope.CopyFileToCrashZip<Unit>(_003C_003Ec__DisplayClass8_.dxDiagPath, new RelativePath("./DxDiag.xml"), () => Unit.Default, _003C_003Ec__DisplayClass8_._003CPostFlight_003Eg__LogError_007C3("it was not found. Possibly not done exporting?"), _003C_003Ec__DisplayClass8_._003CPostFlight_003Eg__LogError_007C3("access was denied. Possibly being written?")).ConfigureAwait(false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<Unit>, _003C_003CPostFlight_003Eb__2_003Ed>(ref awaiter, ref this);
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

		public string dxDiagPath;

		public _003C_003Ec__DisplayClass8_0 CS_0024_003C_003E8__locals1;

		[AsyncStateMachine(typeof(_003C_003CPostFlight_003Eb__2_003Ed))]
		internal System.Threading.Tasks.Task _003CPostFlight_003Eb__2()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CPostFlight_003Eb__2_003Ed _003C_003CPostFlight_003Eb__2_003Ed = default(_003C_003CPostFlight_003Eb__2_003Ed);
			_003C_003CPostFlight_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CPostFlight_003Eb__2_003Ed._003C_003E4__this = this;
			_003C_003CPostFlight_003Eb__2_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003CPostFlight_003Eb__2_003Ed>(ref _003C_003CPostFlight_003Eb__2_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__2_003Ed._003C_003Et__builder)).Task;
		}

		internal Func<Unit> _003CPostFlight_003Eg__LogError_007C3(string reason)
		{
			return new _003C_003Ec__DisplayClass8_2
			{
				CS_0024_003C_003E8__locals2 = this,
				reason = reason
			}._003CPostFlight_003Eb__5;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass8_2
	{
		public string reason;

		public _003C_003Ec__DisplayClass8_1 CS_0024_003C_003E8__locals2;

		internal Unit _003CPostFlight_003Eb__5()
		{
			Logger.Log(LogLevel.Warning, "Could not copy " + CS_0024_003C_003E8__locals2.dxDiagPath + " because " + reason);
			return Unit.Default;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CPostFlight_003Ed__8 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISessionEnvironment> _003C_003Et__builder;

		public CrashReportingSessionWorkflow _003C_003E4__this;

		public ISessionEnvironment sessionEnvironment;

		private _003C_003Ec__DisplayClass8_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CrashReportingSessionWorkflow crashReportingSessionWorkflow = _003C_003E4__this;
			ISessionEnvironment result;
			try
			{
				if (num != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass8_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.crashScope = sessionEnvironment.GetWorkflowState<IInProgressCrashScope>(crashReportingSessionWorkflow._attachmentKey);
				}
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num == 0)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_0182;
					}
					if (_003C_003E8__1.crashScope == null)
					{
						throw new InvalidOperationException("No crash scope was attached to the session.");
					}
					Logger.Log(LogLevel.Info, "Crash reporting session {0} has ended.", _003C_003E8__1.crashScope);
					if (_003C_003E8__1.crashScope.CrashOccurred())
					{
						_003C_003Ec__DisplayClass8_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass8_1
						{
							CS_0024_003C_003E8__locals1 = _003C_003E8__1
						};
						Logger.Log(LogLevel.Info, "Crashes were detected in {0}.", CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.crashScope);
						CS_0024_003C_003E8__locals0.dxDiagPath = Path.Combine(crashReportingSessionWorkflow._deploymentPaths.LogDirectoryPath(), "DxDiag.xml");
						ConfiguredTaskAwaitable val = TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__0_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__0_003Ed _003C_003CPostFlight_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__0_003Ed);
							_003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CPostFlight_003Eb__0_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1;
							_003C_003CPostFlight_003Eb__0_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__0_003Ed>(ref _003C_003CPostFlight_003Eb__0_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder)).Task;
						}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__1_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__1_003Ed _003C_003CPostFlight_003Eb__1_003Ed = default(_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__1_003Ed);
							_003C_003CPostFlight_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CPostFlight_003Eb__1_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1;
							_003C_003CPostFlight_003Eb__1_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__1_003Ed>(ref _003C_003CPostFlight_003Eb__1_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__1_003Ed._003C_003Et__builder)).Task;
						}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass8_1._003C_003CPostFlight_003Eb__2_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass8_1._003C_003CPostFlight_003Eb__2_003Ed _003C_003CPostFlight_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass8_1._003C_003CPostFlight_003Eb__2_003Ed);
							_003C_003CPostFlight_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CPostFlight_003Eb__2_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
							_003C_003CPostFlight_003Eb__2_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass8_1._003C_003CPostFlight_003Eb__2_003Ed>(ref _003C_003CPostFlight_003Eb__2_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__2_003Ed._003C_003Et__builder)).Task;
						}).ConfigureAwait(false);
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CPostFlight_003Ed__8>(ref awaiter, ref this);
							return;
						}
						goto IL_0182;
					}
					Logger.Log(LogLevel.Info, "No crashes were detected in {0}.", _003C_003E8__1.crashScope);
					goto end_IL_004a;
					IL_0182:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					end_IL_004a:;
				}
				finally
				{
					if (num < 0 && _003C_003E8__1.crashScope != null)
					{
						((System.IDisposable)_003C_003E8__1.crashScope).Dispose();
					}
				}
				result = sessionEnvironment.DetachWorkflowState(crashReportingSessionWorkflow._attachmentKey);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	public const string CRASH_REPORTING_SETTING_NAME = "CrashReportDirectoryPath";

	private const int SIM_SUPPORTED_MAX_PATH = 260;

	private readonly ICrashReportCollector _collector;

	private readonly IDeploymentPathProvider _deploymentPaths;

	private readonly object _attachmentKey = new object();

	public CrashReportingSessionWorkflow(ICrashReportCollector crashReportCollector, IDeploymentPathProvider deploymentPathProvider)
	{
		_collector = crashReportCollector;
		_deploymentPaths = deploymentPathProvider;
	}

	public static string GetCrashReportingPath(TrainerSessionData trainerData)
	{
		return ((IReadOnlyDictionary<string, string>)(object)trainerData.Settings)["CrashReportDirectoryPath"];
	}

	public System.Threading.Tasks.Task<ISessionEnvironment> PreFlight(ISessionEnvironment sessionEnvironment, CancellationToken token)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		if (sessionEnvironment.Data == null || sessionEnvironment.Data.TrainerData == null)
		{
			throw new InvalidOperationException("Crash reporting workflow must follow the basic trainer setup workflow.");
		}
		IInProgressCrashScope crashScope = _collector.CreateCrashScope(sessionEnvironment.Inputs);
		Logger.Log(LogLevel.Info, "Created Crash reporting session {0}", crashScope);
		try
		{
			SessionData sessionData = sessionEnvironment.Data.UpdateAllTrainerData(delegate(TrainerSessionData tsd, TrainerSessionDataBuilder builder)
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				builder.Settings = builder.Settings.SetItems(BuildCrashReportingPathSetting(crashScope, tsd).Yield<KeyValuePair<string, string>>());
			});
			System.Collections.Generic.IEnumerable<TrainerSessionData> enumerable = Enumerable.Where<TrainerSessionData>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)sessionData.TrainerData).Values, (Func<TrainerSessionData, bool>)((TrainerSessionData tsd) => ((IReadOnlyDictionary<string, string>)(object)tsd.Settings)["CrashReportDirectoryPath"].Length > 260));
			if (Enumerable.Any<TrainerSessionData>(enumerable))
			{
				Logger.Log(LogLevel.Info, "%TEMP% directory: {0}", _deploymentPaths.AppTempDirectoryPath);
				AggregateException val = new AggregateException((System.Collections.Generic.IEnumerable<System.Exception>)Enumerable.Select<TrainerSessionData, PathTooLongException>(enumerable, (Func<TrainerSessionData, PathTooLongException>)((TrainerSessionData tsd) => new PathTooLongException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "The '{0}' crash reporting path exceeds the max path length ({2}). '{1}'", (object)tsd.EndpointName, (object)((IReadOnlyDictionary<string, string>)(object)tsd.Settings)["CrashReportDirectoryPath"], (object)260)))));
				throw new PathTooLongException(Resources.ONE_OR_MORE_CRASH_REPORT_DIRECTORY_PATHS_TOO_LONG, (System.Exception)(object)val);
			}
			return System.Threading.Tasks.Task.FromResult<ISessionEnvironment>(sessionEnvironment.UpdateData(sessionData).AttachWorkflowState(_attachmentKey, crashScope));
		}
		catch (System.Exception)
		{
			((System.IDisposable)crashScope).Dispose();
			throw;
		}
	}

	[AsyncStateMachine(typeof(_003CPostFlight_003Ed__8))]
	public async System.Threading.Tasks.Task<ISessionEnvironment> PostFlight(ISessionEnvironment sessionEnvironment)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass8_0 _003C_003Ec__DisplayClass8_ = new _003C_003Ec__DisplayClass8_0();
		_003C_003Ec__DisplayClass8_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass8_.crashScope = sessionEnvironment.GetWorkflowState<IInProgressCrashScope>(_attachmentKey);
		try
		{
			if (_003C_003Ec__DisplayClass8_.crashScope == null)
			{
				throw new InvalidOperationException("No crash scope was attached to the session.");
			}
			Logger.Log(LogLevel.Info, "Crash reporting session {0} has ended.", _003C_003Ec__DisplayClass8_.crashScope);
			if (_003C_003Ec__DisplayClass8_.crashScope.CrashOccurred())
			{
				_003C_003Ec__DisplayClass8_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass8_1();
				CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass8_;
				Logger.Log(LogLevel.Info, "Crashes were detected in {0}.", CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1.crashScope);
				CS_0024_003C_003E8__locals0.dxDiagPath = Path.Combine(_deploymentPaths.LogDirectoryPath(), "DxDiag.xml");
				await TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__0_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__0_003Ed _003C_003CPostFlight_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__0_003Ed);
					_003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CPostFlight_003Eb__0_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1;
					_003C_003CPostFlight_003Eb__0_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__0_003Ed>(ref _003C_003CPostFlight_003Eb__0_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder)).Task;
				}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__1_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__1_003Ed _003C_003CPostFlight_003Eb__1_003Ed = default(_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__1_003Ed);
					_003C_003CPostFlight_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CPostFlight_003Eb__1_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals1;
					_003C_003CPostFlight_003Eb__1_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass8_0._003C_003CPostFlight_003Eb__1_003Ed>(ref _003C_003CPostFlight_003Eb__1_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__1_003Ed._003C_003Et__builder)).Task;
				}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass8_1._003C_003CPostFlight_003Eb__2_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass8_1._003C_003CPostFlight_003Eb__2_003Ed _003C_003CPostFlight_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass8_1._003C_003CPostFlight_003Eb__2_003Ed);
					_003C_003CPostFlight_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CPostFlight_003Eb__2_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
					_003C_003CPostFlight_003Eb__2_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass8_1._003C_003CPostFlight_003Eb__2_003Ed>(ref _003C_003CPostFlight_003Eb__2_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__2_003Ed._003C_003Et__builder)).Task;
				}).ConfigureAwait(false);
			}
			else
			{
				Logger.Log(LogLevel.Info, "No crashes were detected in {0}.", _003C_003Ec__DisplayClass8_.crashScope);
			}
		}
		finally
		{
			if (_003C_003Ec__DisplayClass8_.crashScope != null)
			{
				((System.IDisposable)_003C_003Ec__DisplayClass8_.crashScope).Dispose();
			}
		}
		return sessionEnvironment.DetachWorkflowState(_attachmentKey);
	}

	private static KeyValuePair<string, string> BuildCrashReportingPathSetting(IInProgressCrashScope crashScope, TrainerSessionData tsd)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		string text = crashScope.CrashReportingDirectoryFor(tsd);
		return new KeyValuePair<string, string>("CrashReportDirectoryPath", text);
	}
}
