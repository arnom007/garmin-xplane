using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.AppLifeCycle;

public class RunOnceProvider : IRunOnceProvider
{
	private sealed class RunOnceRecord : IRunOnce
	{
		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CMarkCompleted_003Ed__9 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<IRunOnce> _003C_003Et__builder;

			public RunOnceRecord _003C_003E4__this;

			private ConfiguredTaskAwaiter<RunOnceRecord> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				RunOnceRecord runOnceRecord = _003C_003E4__this;
				IRunOnce result;
				try
				{
					ConfiguredTaskAwaiter<RunOnceRecord> awaiter;
					if (num != 0)
					{
						awaiter = runOnceRecord._provider.UpdateRecord(runOnceRecord.OperationName).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<RunOnceRecord>, _003CMarkCompleted_003Ed__9>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<RunOnceRecord>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter.GetResult();
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

		private readonly RunOnceProvider _provider;

		[field: CompilerGenerated]
		public string OperationName
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		public DateTimeOffset? CompletedAt
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			private set;
		}

		public RunOnceRecord(RunOnceProvider provider, string operationName, DateTimeOffset? completed)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			_provider = provider ?? throw new ArgumentNullException("provider");
			OperationName = operationName ?? throw new ArgumentNullException("operationName");
			CompletedAt = completed;
		}

		[AsyncStateMachine(typeof(_003CMarkCompleted_003Ed__9))]
		public async System.Threading.Tasks.Task<IRunOnce> MarkCompleted()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await _provider.UpdateRecord(OperationName).DetachContext();
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003CGetByOperationName_003Eb__3_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

		public RunOnceProvider _003C_003E4__this;

		public string expectedRecordPath;

		private ConfiguredTaskAwaiter<string> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			RunOnceProvider runOnceProvider = _003C_003E4__this;
			string result;
			try
			{
				ConfiguredTaskAwaiter<string> awaiter;
				if (num != 0)
				{
					awaiter = runOnceProvider._lowLevel.FileSystem.ReadAllText(expectedRecordPath).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003C_003CGetByOperationName_003Eb__3_0_003Ed>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<string>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult();
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

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public RunOnceRecord updated;

		internal RunOnceRecord _003CUpdateRecord_003Eb__0(string k, RunOnceRecord previous)
		{
			if (!previous.HasCompleted())
			{
				return updated;
			}
			return previous;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetByOperationName_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IRunOnce> _003C_003Et__builder;

		public RunOnceProvider _003C_003E4__this;

		public string operationName;

		private ConfiguredTaskAwaiter<RunOnceRecord> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			RunOnceProvider CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			IRunOnce result;
			try
			{
				ConfiguredTaskAwaiter<RunOnceRecord> awaiter;
				if (num != 0)
				{
					awaiter = CS_0024_003C_003E8__locals0.GetRecord(operationName, [AsyncStateMachine(typeof(_003C_003CGetByOperationName_003Eb__3_0_003Ed))] [CompilerGenerated] async (string expectedRecordPath) => await CS_0024_003C_003E8__locals0._lowLevel.FileSystem.ReadAllText(expectedRecordPath).DetachContext()).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<RunOnceRecord>, _003CGetByOperationName_003Ed__3>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<RunOnceRecord>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult();
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetRecord_003Ed__5 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<RunOnceRecord> _003C_003Et__builder;

		public RunOnceProvider _003C_003E4__this;

		public string operationName;

		public Func<string, System.Threading.Tasks.Task<string>> getFileContents;

		private ConfiguredTaskAwaiter<string> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			RunOnceProvider runOnceProvider = _003C_003E4__this;
			RunOnceRecord result;
			try
			{
				try
				{
					RunOnceRecord orAdd = default(RunOnceRecord);
					ConfiguredTaskAwaiter<string> awaiter;
					if (num != 0)
					{
						if (runOnceProvider._cache.TryGetValue(operationName, ref orAdd))
						{
							goto IL_0152;
						}
						string text = runOnceProvider.BuildOperationFilePath(operationName);
						if (!runOnceProvider._lowLevel.FileSystem.PathExists(text))
						{
							orAdd = new RunOnceRecord(runOnceProvider, operationName, null);
							goto IL_013f;
						}
						awaiter = getFileContents.Invoke(text).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003CGetRecord_003Ed__5>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<string>);
						num = (_003C_003E1__state = -1);
					}
					DateTimeOffset minValue = default(DateTimeOffset);
					if (!DateTimeOffset.TryParse(awaiter.GetResult(), ref minValue))
					{
						Logger.Log(LogLevel.Error, "The {0} one time operation completed date/time could not be parsed.", operationName);
						minValue = DateTimeOffset.MinValue;
					}
					else
					{
						Logger.Log(LogLevel.Info, "The {0} one time operation completed at {1}.", operationName, minValue);
					}
					orAdd = new RunOnceRecord(runOnceProvider, operationName, minValue);
					goto IL_013f;
					IL_013f:
					orAdd = runOnceProvider._cache.GetOrAdd(operationName, orAdd);
					goto IL_0152;
					IL_0152:
					result = orAdd;
				}
				catch (System.Exception e)
				{
					Logger.LogFormattedException(e, "Getting the {0} operation completed info.", operationName);
					throw;
				}
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CUpdateRecord_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<RunOnceRecord> _003C_003Et__builder;

		public RunOnceProvider _003C_003E4__this;

		public string operationName;

		private _003C_003Ec__DisplayClass7_0 _003C_003E8__1;

		private DateTimeOffset _003CcompletedAtTime_003E5__2;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			RunOnceProvider runOnceProvider = _003C_003E4__this;
			RunOnceRecord updated;
			try
			{
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						_003C_003E8__1 = new _003C_003Ec__DisplayClass7_0();
						if (runOnceProvider._cache.TryGetValue(operationName, ref _003C_003E8__1.updated) && _003C_003E8__1.updated.HasCompleted())
						{
							goto IL_0195;
						}
						_003CcompletedAtTime_003E5__2 = DateTimeOffset.Now;
						string text = ((DateTimeOffset)(ref _003CcompletedAtTime_003E5__2)).ToString("o", (IFormatProvider)(object)CultureInfo.InvariantCulture);
						string path = runOnceProvider.BuildOperationFilePath(operationName);
						runOnceProvider._lowLevel.FileSystem.CreateDirectory(LauncherPathHelpers.NonRootPathParentDirectory(path));
						Logger.Log(LogLevel.Info, "Marking the {0} one time operation completed at {1}", operationName, text);
						ConfiguredTaskAwaitable val = runOnceProvider._lowLevel.FileSystem.WriteAllText(path, text).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateRecord_003Ed__7>(ref awaiter, ref this);
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
					_003C_003E8__1.updated = new RunOnceRecord(runOnceProvider, operationName, _003CcompletedAtTime_003E5__2);
					_003C_003E8__1.updated = runOnceProvider._cache.AddOrUpdate(operationName, _003C_003E8__1.updated, (Func<string, RunOnceRecord, RunOnceRecord>)((string k, RunOnceRecord previous) => (!previous.HasCompleted()) ? _003C_003E8__1.updated : previous));
					_003CcompletedAtTime_003E5__2 = default(DateTimeOffset);
					goto IL_0195;
					IL_0195:
					updated = _003C_003E8__1.updated;
				}
				catch (System.Exception e)
				{
					Logger.LogFormattedException(e, "Completing the {0} one time operation.", operationName);
					throw;
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(updated);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly ILowLevelDal _lowLevel;

	private readonly ConcurrentDictionary<string, RunOnceRecord> _cache = new ConcurrentDictionary<string, RunOnceRecord>((IEqualityComparer<string>)(object)StringComparer.OrdinalIgnoreCase);

	public RunOnceProvider(ILowLevelDal lowLevelDal)
	{
		_lowLevel = lowLevelDal;
	}

	[AsyncStateMachine(typeof(_003CGetByOperationName_003Ed__3))]
	public async System.Threading.Tasks.Task<IRunOnce> GetByOperationName(string operationName)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await GetRecord(operationName, [AsyncStateMachine(typeof(_003C_003CGetByOperationName_003Eb__3_0_003Ed))] [CompilerGenerated] async (string expectedRecordPath) => await _lowLevel.FileSystem.ReadAllText(expectedRecordPath).DetachContext()).DetachContext();
	}

	public IRunOnce GetByOperationNameSync(string operationName)
	{
		return GetRecord(operationName, [CompilerGenerated] (string expectedRecordPath) => System.Threading.Tasks.Task.FromResult<string>(_lowLevel.FileSystem.ReadAllTextSync(expectedRecordPath))).Result;
	}

	[AsyncStateMachine(typeof(_003CGetRecord_003Ed__5))]
	private async System.Threading.Tasks.Task<RunOnceRecord> GetRecord(string operationName, Func<string, System.Threading.Tasks.Task<string>> getFileContents)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			RunOnceRecord orAdd = default(RunOnceRecord);
			if (!_cache.TryGetValue(operationName, ref orAdd))
			{
				string text = BuildOperationFilePath(operationName);
				if (_lowLevel.FileSystem.PathExists(text))
				{
					DateTimeOffset minValue = default(DateTimeOffset);
					if (!DateTimeOffset.TryParse(await getFileContents.Invoke(text).DetachContext(), ref minValue))
					{
						Logger.Log(LogLevel.Error, "The {0} one time operation completed date/time could not be parsed.", operationName);
						minValue = DateTimeOffset.MinValue;
					}
					else
					{
						Logger.Log(LogLevel.Info, "The {0} one time operation completed at {1}.", operationName, minValue);
					}
					orAdd = new RunOnceRecord(this, operationName, minValue);
				}
				else
				{
					orAdd = new RunOnceRecord(this, operationName, null);
				}
				orAdd = _cache.GetOrAdd(operationName, orAdd);
			}
			return orAdd;
		}
		catch (System.Exception e)
		{
			Logger.LogFormattedException(e, "Getting the {0} operation completed info.", operationName);
			throw;
		}
	}

	private string BuildOperationFilePath(string operationName)
	{
		return Path.Combine(_lowLevel.Deployment.UserAppDataDirPath, "RunOnce", operationName + ".dat");
	}

	[AsyncStateMachine(typeof(_003CUpdateRecord_003Ed__7))]
	private async System.Threading.Tasks.Task<RunOnceRecord> UpdateRecord(string operationName)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			RunOnceRecord updated = default(RunOnceRecord);
			if (!_cache.TryGetValue(operationName, ref updated) || !updated.HasCompleted())
			{
				DateTimeOffset completedAtTime = DateTimeOffset.Now;
				string text = ((DateTimeOffset)(ref completedAtTime)).ToString("o", (IFormatProvider)(object)CultureInfo.InvariantCulture);
				string path = BuildOperationFilePath(operationName);
				_lowLevel.FileSystem.CreateDirectory(LauncherPathHelpers.NonRootPathParentDirectory(path));
				Logger.Log(LogLevel.Info, "Marking the {0} one time operation completed at {1}", operationName, text);
				await _lowLevel.FileSystem.WriteAllText(path, text).DetachContext();
				updated = new RunOnceRecord(this, operationName, completedAtTime);
				updated = _cache.AddOrUpdate(operationName, updated, (Func<string, RunOnceRecord, RunOnceRecord>)((string k, RunOnceRecord previous) => (!previous.HasCompleted()) ? updated : previous));
				completedAtTime = default(DateTimeOffset);
			}
			return updated;
		}
		catch (System.Exception e)
		{
			Logger.LogFormattedException(e, "Completing the {0} one time operation.", operationName);
			throw;
		}
	}
}
