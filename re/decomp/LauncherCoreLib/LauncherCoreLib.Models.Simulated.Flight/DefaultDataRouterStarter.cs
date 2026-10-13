using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AdvancedProcessCreation;
using DataRouterMatrix;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Processes;
using LOLA.StringHelpers.Paths;
using LOLA.Tasks;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class DefaultDataRouterStarter : IDataRouterStarter
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003CStartRouter_003Eb__7_4_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ValueTuple<IProcessSession, string>> _003C_003Et__builder;

		public IProcessSession rp;

		public DefaultDataRouterStarter _003C_003E4__this;

		private IProcessSession _003C_003E7__wrap1;

		private ConfiguredTaskAwaiter<string?> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DefaultDataRouterStarter defaultDataRouterStarter = _003C_003E4__this;
			ValueTuple<IProcessSession, string> result2;
			try
			{
				ConfiguredTaskAwaiter<string> awaiter;
				if (num != 0)
				{
					_003C_003E7__wrap1 = rp;
					awaiter = defaultDataRouterStarter._processApi.GetExePath(rp.ProcessId, CancellationToken.None).ConfigureAwait(false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003C_003CStartRouter_003Eb__7_4_003Ed>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<string>);
					num = (_003C_003E1__state = -1);
				}
				string result = awaiter.GetResult();
				result2 = new ValueTuple<IProcessSession, string>(_003C_003E7__wrap1, result);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result2);
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
		public List<DataRouterAssemblyInfo> allLocalRoarPaths;

		public IProcessSession session;

		internal bool _003CStartRouter_003Eb__1(ValueTuple<IProcessSession, string> record)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass7_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass7_1
			{
				record = record
			};
			if (CS_0024_003C_003E8__locals0.record.Item2 != null)
			{
				return Enumerable.Any<DataRouterAssemblyInfo>((System.Collections.Generic.IEnumerable<DataRouterAssemblyInfo>)allLocalRoarPaths, (Func<DataRouterAssemblyInfo, bool>)((DataRouterAssemblyInfo r) => IsSameExePath(r.AssemblyFullPath, CS_0024_003C_003E8__locals0.record.Item2)));
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_1
	{
		public ValueTuple<IProcessSession, string> record;

		internal bool _003CStartRouter_003Eb__5(DataRouterAssemblyInfo r)
		{
			return IsSameExePath(r.AssemblyFullPath, record.Item2);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CStartRouter_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IDataRouterSession> _003C_003Et__builder;

		public DefaultDataRouterStarter _003C_003E4__this;

		public DataRouterAssemblyInfo router;

		public DataRouterAssemblyInfo client;

		private _003C_003Ec__DisplayClass7_0 _003C_003E8__1;

		private IProcessSession[] _003CroarProcesses_003E5__2;

		private ConfiguredTaskAwaiter<ValueTuple<IProcessSession, string>[]> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<IProcessSession> _003C_003Eu__2;

		private bool _003CrunningConfirmed_003E5__3;

		private ConfiguredTaskAwaiter<string> _003C_003Eu__3;

		private ConfiguredTaskAwaiter<ProcessResult> _003C_003Eu__4;

		private void MoveNext()
		{
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DefaultDataRouterStarter CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			IDataRouterSession result5;
			try
			{
				if ((uint)num > 3u)
				{
					CS_0024_003C_003E8__locals0.VerifyRouterAssembly(router);
					CS_0024_003C_003E8__locals0.VerifyRouterAssembly(client);
					if (Enumerable.Any<ProcessSnapshot>((System.Collections.Generic.IEnumerable<ProcessSnapshot>)CS_0024_003C_003E8__locals0._processApi.OsProcessApi.GetProcessSnapshotsByName("roar"), (Func<ProcessSnapshot, bool>)((ProcessSnapshot pi) => !pi.CurrentUserCanAccess)))
					{
						throw new InvalidOperationException("The data router cannot be restarted because it was started by another user. Please ask other users of this computer to close their trainers or restart the computer to remedy this situation.");
					}
					_003CroarProcesses_003E5__2 = CS_0024_003C_003E8__locals0._processApi.OsProcessApi.GetProcessesByName("roar");
				}
				try
				{
					ConfiguredTaskAwaiter<ValueTuple<IProcessSession, string>[]> awaiter2;
					ConfiguredTaskAwaiter<IProcessSession> awaiter;
					IProcessSession result;
					ValueTuple<IProcessSession, string>[] result2;
					List<IProcessSession> val;
					Enumerator<IProcessSession> enumerator;
					ProcessStartupInfoAdapter startup;
					switch (num)
					{
					default:
						_003C_003E8__1 = new _003C_003Ec__DisplayClass7_0();
						awaiter2 = System.Threading.Tasks.Task.WhenAll<ValueTuple<IProcessSession, string>>(Enumerable.Select<IProcessSession, System.Threading.Tasks.Task<ValueTuple<IProcessSession, string>>>((System.Collections.Generic.IEnumerable<IProcessSession>)_003CroarProcesses_003E5__2, (Func<IProcessSession, System.Threading.Tasks.Task<ValueTuple<IProcessSession, string>>>)([AsyncStateMachine(typeof(_003C_003CStartRouter_003Eb__7_4_003Ed))] [CompilerGenerated] async (IProcessSession rp) => new ValueTuple<IProcessSession, string>(rp, await CS_0024_003C_003E8__locals0._processApi.GetExePath(rp.ProcessId, CancellationToken.None).ConfigureAwait(false))))).ConfigureAwait(false).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ValueTuple<IProcessSession, string>[]>, _003CStartRouter_003Ed__7>(ref awaiter2, ref this);
							return;
						}
						goto IL_0124;
					case 0:
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ValueTuple<IProcessSession, string>[]>);
						num = (_003C_003E1__state = -1);
						goto IL_0124;
					case 1:
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<IProcessSession>);
						num = (_003C_003E1__state = -1);
						goto IL_0279;
					case 2:
					case 3:
						break;
						IL_0279:
						result = awaiter.GetResult();
						_003C_003E8__1.session = result;
						if (_003C_003E8__1.session == null)
						{
							throw new InvalidOperationException("Could not start the router process.");
						}
						break;
						IL_0124:
						result2 = awaiter2.GetResult();
						_003C_003E8__1.allLocalRoarPaths = Enumerable.ToList<DataRouterAssemblyInfo>(CS_0024_003C_003E8__locals0._dataRouterAssemblies.Routers.GetAll());
						val = Enumerable.ToList<IProcessSession>(Enumerable.Select<ValueTuple<IProcessSession, string>, IProcessSession>(Enumerable.Where<ValueTuple<IProcessSession, string>>((System.Collections.Generic.IEnumerable<ValueTuple<IProcessSession, string>>)result2, (Func<ValueTuple<IProcessSession, string>, bool>)delegate(ValueTuple<IProcessSession, string> record)
						{
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							//IL_0008: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass7_1 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass7_1
							{
								record = record
							};
							return CS_0024_003C_003E8__locals1.record.Item2 != null && Enumerable.Any<DataRouterAssemblyInfo>((System.Collections.Generic.IEnumerable<DataRouterAssemblyInfo>)_003C_003E8__1.allLocalRoarPaths, (Func<DataRouterAssemblyInfo, bool>)((DataRouterAssemblyInfo r) => IsSameExePath(r.AssemblyFullPath, CS_0024_003C_003E8__locals1.record.Item2)));
						}), (Func<ValueTuple<IProcessSession, string>, IProcessSession>)((ValueTuple<IProcessSession, string> record) => record.Item1)));
						enumerator = val.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								IProcessSession current = enumerator.Current;
								CS_0024_003C_003E8__locals0._processApi.OsProcessApi.KillProcessTree(current.ProcessId);
							}
						}
						finally
						{
							if (num < 0)
							{
								((System.IDisposable)enumerator).Dispose();
							}
						}
						if (Enumerable.Any<IProcessSession>(Enumerable.Except<IProcessSession>((System.Collections.Generic.IEnumerable<IProcessSession>)_003CroarProcesses_003E5__2, (System.Collections.Generic.IEnumerable<IProcessSession>)val)))
						{
							throw new InvalidOperationException("The data router is already running from a different program.");
						}
						startup = new ProcessStartupInfoAdapter(Environment.CurrentDirectory, router.AssemblyFullPath, string.Empty);
						awaiter = CS_0024_003C_003E8__locals0._processApi.Fork(startup).ConfigureAwait(false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IProcessSession>, _003CStartRouter_003Ed__7>(ref awaiter, ref this);
							return;
						}
						goto IL_0279;
					}
					try
					{
						ConfiguredTaskAwaiter<ProcessResult> awaiter3;
						if (num != 2)
						{
							if (num != 3)
							{
								_003C_003E8__1.session.CloseOnDispose = true;
								_003CrunningConfirmed_003E5__3 = false;
								goto IL_0363;
							}
							awaiter3 = _003C_003Eu__4;
							_003C_003Eu__4 = default(ConfiguredTaskAwaiter<ProcessResult>);
							num = (_003C_003E1__state = -1);
							goto IL_0464;
						}
						ConfiguredTaskAwaiter<string> awaiter4 = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter<string>);
						num = (_003C_003E1__state = -1);
						goto IL_033f;
						IL_033f:
						string result3 = awaiter4.GetResult();
						if (result3 == null || !result3.Contains("Running", (StringComparison)4))
						{
							goto IL_0363;
						}
						_003CrunningConfirmed_003E5__3 = true;
						goto IL_0378;
						IL_0464:
						ProcessResult result4 = awaiter3.GetResult();
						Logger.Log(LogLevel.Error, ProcessLogFormatting.LogExitWithSquareBracketNotation(result4));
						goto IL_0487;
						IL_0378:
						LogProcessOutput(_003C_003E8__1.session.StdOut, "STDOUT");
						LogProcessOutput(_003C_003E8__1.session.StdError, "STDERR");
						if (!_003CrunningConfirmed_003E5__3)
						{
							if (_003C_003E8__1.session.HasExited())
							{
								awaiter3 = _003C_003E8__1.session.Exited.ConfigureAwait(false).GetAwaiter();
								if (!awaiter3.IsCompleted)
								{
									num = (_003C_003E1__state = 3);
									_003C_003Eu__4 = awaiter3;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ProcessResult>, _003CStartRouter_003Ed__7>(ref awaiter3, ref this);
									return;
								}
								goto IL_0464;
							}
							Logger.Log(LogLevel.Error, "CRITICAL ROAR ERROR! It did not exit and we stopped looking for its running signal.");
							goto IL_0487;
						}
						result5 = CS_0024_003C_003E8__locals0._sessionFactory.CreateSession(_003C_003E8__1.session, client);
						goto end_IL_02a8;
						IL_0487:
						throw new InvalidOperationException("Router did not start properly.");
						IL_0363:
						if (!_003C_003E8__1.session.HasExited())
						{
							awaiter4 = ((TextReader)_003C_003E8__1.session.StdOut).ReadLineAsync().DetachContext().GetAwaiter();
							if (!awaiter4.IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter4;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003CStartRouter_003Ed__7>(ref awaiter4, ref this);
								return;
							}
							goto IL_033f;
						}
						goto IL_0378;
						end_IL_02a8:;
					}
					catch (System.Exception)
					{
						((System.IDisposable)_003C_003E8__1.session).Dispose();
						throw;
					}
				}
				finally
				{
					if (num < 0)
					{
						IProcessSession[] array = _003CroarProcesses_003E5__2;
						for (int i = 0; i < array.Length; i++)
						{
							((System.IDisposable)array[i]).Dispose();
						}
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CroarProcesses_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CroarProcesses_003E5__2 = null;
			_003C_003Et__builder.SetResult(result5);
			[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass7_0._003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed))]
			System.Threading.Tasks.Task LogProcessOutput(StreamReader reader, string prefix)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				_003C_003Ec__DisplayClass7_0._003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed _003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed = default(_003C_003Ec__DisplayClass7_0._003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed);
				_003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed._003C_003E4__this = this;
				_003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed.reader = reader;
				_003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed.prefix = prefix;
				_003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass7_0._003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed>(ref _003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed._003C_003Et__builder)).Task;
			}
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private const string ROAR_PROCESS_NAME = "roar";

	private readonly DataRouterProcessApi _processApi;

	private readonly IFileSystem _fileSystem;

	private readonly IFileVerification _verification;

	private readonly IDataRouterAssemblyTypes _dataRouterAssemblies;

	private readonly IDataRouterSessionFactory _sessionFactory;

	public DefaultDataRouterStarter(DataRouterProcessApi processApi, IFileSystem fileSystem, IFileVerification fileVerification, IDataRouterAssemblyTypes dataRouterAssemblies, IDataRouterSessionFactory sessionFactory)
	{
		_processApi = processApi;
		_fileSystem = fileSystem;
		_verification = fileVerification;
		_dataRouterAssemblies = dataRouterAssemblies;
		_sessionFactory = sessionFactory;
	}

	[AsyncStateMachine(typeof(_003CStartRouter_003Ed__7))]
	public async System.Threading.Tasks.Task<IDataRouterSession> StartRouter(DataRouterAssemblyInfo router, DataRouterAssemblyInfo client)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		VerifyRouterAssembly(router);
		VerifyRouterAssembly(client);
		if (Enumerable.Any<ProcessSnapshot>((System.Collections.Generic.IEnumerable<ProcessSnapshot>)_processApi.OsProcessApi.GetProcessSnapshotsByName("roar"), (Func<ProcessSnapshot, bool>)((ProcessSnapshot pi) => !pi.CurrentUserCanAccess)))
		{
			throw new InvalidOperationException("The data router cannot be restarted because it was started by another user. Please ask other users of this computer to close their trainers or restart the computer to remedy this situation.");
		}
		IProcessSession[] roarProcesses = _processApi.OsProcessApi.GetProcessesByName("roar");
		try
		{
			_003C_003Ec__DisplayClass7_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass7_0();
			ValueTuple<IProcessSession, string>[] obj = await System.Threading.Tasks.Task.WhenAll<ValueTuple<IProcessSession, string>>(Enumerable.Select<IProcessSession, System.Threading.Tasks.Task<ValueTuple<IProcessSession, string>>>((System.Collections.Generic.IEnumerable<IProcessSession>)roarProcesses, (Func<IProcessSession, System.Threading.Tasks.Task<ValueTuple<IProcessSession, string>>>)([AsyncStateMachine(typeof(_003C_003CStartRouter_003Eb__7_4_003Ed))] [CompilerGenerated] async (IProcessSession rp) => new ValueTuple<IProcessSession, string>(rp, await _processApi.GetExePath(rp.ProcessId, CancellationToken.None).ConfigureAwait(false))))).ConfigureAwait(false);
			CS_0024_003C_003E8__locals0.allLocalRoarPaths = Enumerable.ToList<DataRouterAssemblyInfo>(_dataRouterAssemblies.Routers.GetAll());
			List<IProcessSession> val = Enumerable.ToList<IProcessSession>(Enumerable.Select<ValueTuple<IProcessSession, string>, IProcessSession>(Enumerable.Where<ValueTuple<IProcessSession, string>>((System.Collections.Generic.IEnumerable<ValueTuple<IProcessSession, string>>)obj, (Func<ValueTuple<IProcessSession, string>, bool>)((ValueTuple<IProcessSession, string> record) => record.Item2 != null && Enumerable.Any<DataRouterAssemblyInfo>((System.Collections.Generic.IEnumerable<DataRouterAssemblyInfo>)CS_0024_003C_003E8__locals0.allLocalRoarPaths, (Func<DataRouterAssemblyInfo, bool>)((DataRouterAssemblyInfo r) => IsSameExePath(r.AssemblyFullPath, record.Item2))))), (Func<ValueTuple<IProcessSession, string>, IProcessSession>)((ValueTuple<IProcessSession, string> record) => record.Item1)));
			Enumerator<IProcessSession> enumerator = val.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					IProcessSession current = enumerator.Current;
					_processApi.OsProcessApi.KillProcessTree(current.ProcessId);
				}
			}
			finally
			{
				((System.IDisposable)enumerator).Dispose();
			}
			if (Enumerable.Any<IProcessSession>(Enumerable.Except<IProcessSession>((System.Collections.Generic.IEnumerable<IProcessSession>)roarProcesses, (System.Collections.Generic.IEnumerable<IProcessSession>)val)))
			{
				throw new InvalidOperationException("The data router is already running from a different program.");
			}
			ProcessStartupInfoAdapter startup = new ProcessStartupInfoAdapter(Environment.CurrentDirectory, router.AssemblyFullPath, string.Empty);
			CS_0024_003C_003E8__locals0.session = await _processApi.Fork(startup).ConfigureAwait(false);
			if (CS_0024_003C_003E8__locals0.session == null)
			{
				throw new InvalidOperationException("Could not start the router process.");
			}
			try
			{
				CS_0024_003C_003E8__locals0.session.CloseOnDispose = true;
				bool runningConfirmed = false;
				while (!CS_0024_003C_003E8__locals0.session.HasExited())
				{
					if ((await ((TextReader)CS_0024_003C_003E8__locals0.session.StdOut).ReadLineAsync().DetachContext())?.Contains("Running", (StringComparison)4) ?? false)
					{
						runningConfirmed = true;
						break;
					}
				}
				LogProcessOutput(CS_0024_003C_003E8__locals0.session.StdOut, "STDOUT");
				LogProcessOutput(CS_0024_003C_003E8__locals0.session.StdError, "STDERR");
				if (runningConfirmed)
				{
					return _sessionFactory.CreateSession(CS_0024_003C_003E8__locals0.session, client);
				}
				if (CS_0024_003C_003E8__locals0.session.HasExited())
				{
					Logger.Log(LogLevel.Error, ProcessLogFormatting.LogExitWithSquareBracketNotation(await CS_0024_003C_003E8__locals0.session.Exited.ConfigureAwait(false)));
				}
				else
				{
					Logger.Log(LogLevel.Error, "CRITICAL ROAR ERROR! It did not exit and we stopped looking for its running signal.");
				}
				throw new InvalidOperationException("Router did not start properly.");
			}
			catch (System.Exception)
			{
				((System.IDisposable)CS_0024_003C_003E8__locals0.session).Dispose();
				throw;
			}
			[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass7_0._003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed))]
			System.Threading.Tasks.Task LogProcessOutput(StreamReader reader, string prefix)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				_003C_003Ec__DisplayClass7_0._003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed _003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed = default(_003C_003Ec__DisplayClass7_0._003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed);
				_003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
				_003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed.reader = reader;
				_003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed.prefix = prefix;
				_003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass7_0._003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed>(ref _003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003CStartRouter_003Eg__LogProcessOutput_007C3_003Ed._003C_003Et__builder)).Task;
			}
		}
		finally
		{
			IProcessSession[] array = roarProcesses;
			for (int i = 0; i < array.Length; i++)
			{
				((System.IDisposable)array[i]).Dispose();
			}
		}
	}

	private static bool IsSameExePath(string localRoarPath, string processExePath)
	{
		return ((EqualityComparer<string>)(object)PathHelpers.AnyCaseAnySlash).Equals(localRoarPath, processExePath);
	}

	private void VerifyRouterAssembly(DataRouterAssemblyInfo assemblyInfo)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!_fileSystem.PathExists(assemblyInfo.AssemblyFullPath))
		{
			throw new FileNotFoundException("Missing data router resource. An issue may have occurred during product installation or update. Reinstallation may be required.", assemblyInfo.AssemblyFullPath);
		}
		if (!_verification.IsFileSignedByGarmin(assemblyInfo.AssemblyFullPath))
		{
			throw new InvalidOperationException("The data router resource is not signed by Garmin.");
		}
	}
}
