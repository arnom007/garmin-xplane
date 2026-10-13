using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Tasks;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.CrashReporting;

public class CompressInMemoryLogArchiveBuilder : IFeedbackLogArchiveBuilder
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		[StructLayout(3)]
		private struct _003C_003CBuildUpLogArchiveInMemory_003Eb__10_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<ValueTuple<MemoryStream, FileStats>> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass7_0 _003C_003E4__this;

			public FileSystemEntry logFile;

			private ConfiguredTaskAwaiter<ValueTuple<MemoryStream, FileStats>> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass7_0 _003C_003Ec__DisplayClass7_ = _003C_003E4__this;
				ValueTuple<MemoryStream, FileStats> result;
				try
				{
					ConfiguredTaskAwaiter<ValueTuple<MemoryStream, FileStats>> awaiter;
					if (num != 0)
					{
						awaiter = _003C_003Ec__DisplayClass7_._003C_003E4__this.CensorFileToIntermediateStream(logFile).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ValueTuple<MemoryStream, FileStats>>, _003C_003CBuildUpLogArchiveInMemory_003Eb__10_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ValueTuple<MemoryStream, FileStats>>);
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

		public CompressInMemoryLogArchiveBuilder _003C_003E4__this;

		public int maxSize;

		public Func<FileSystemEntry, System.Threading.Tasks.Task<ValueTuple<MemoryStream, FileStats>>> _003C_003E9__10;

		internal FileStats _003CBuildUpLogArchiveInMemory_003Eb__3(FileSystemEntry fse)
		{
			return _003C_003E4__this._fileSystem.StatIfExists(fse.FullPath);
		}

		internal IObservable<ValueTuple<MemoryStream, FileStats>> _003CBuildUpLogArchiveInMemory_003Eb__7(System.Collections.Generic.IList<FileSystemEntry> logFileChunk)
		{
			return ((System.Collections.Generic.IEnumerable<FileSystemEntry>)logFileChunk).ToObservable().Select<FileSystemEntry, System.Threading.Tasks.Task<ValueTuple<MemoryStream, FileStats>>>((Func<FileSystemEntry, System.Threading.Tasks.Task<ValueTuple<MemoryStream, FileStats>>>)([AsyncStateMachine(typeof(_003C_003CBuildUpLogArchiveInMemory_003Eb__10_003Ed))] async (FileSystemEntry logFile) => await _003C_003E4__this.CensorFileToIntermediateStream(logFile).DetachContext())).Merge<ValueTuple<MemoryStream, FileStats>>();
		}

		[AsyncStateMachine(typeof(_003C_003CBuildUpLogArchiveInMemory_003Eb__10_003Ed))]
		internal async System.Threading.Tasks.Task<ValueTuple<MemoryStream, FileStats>> _003CBuildUpLogArchiveInMemory_003Eb__10(FileSystemEntry logFile)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await _003C_003E4__this.CensorFileToIntermediateStream(logFile).DetachContext();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_1
	{
		public MemoryStream memory;

		public _003C_003Ec__DisplayClass7_0 CS_0024_003C_003E8__locals1;

		internal bool _003CBuildUpLogArchiveInMemory_003Eb__9(FileStats _)
		{
			return ((Stream)memory).Length < CS_0024_003C_003E8__locals1.maxSize;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_2
	{
		[StructLayout(3)]
		private struct _003C_003CBuildUpLogArchiveInMemory_003Eb__8_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<FileStats> _003C_003Et__builder;

			public ValueTuple<MemoryStream, FileStats> censoredTuple;

			public _003C_003Ec__DisplayClass7_2 _003C_003E4__this;

			private MemoryStream _003C_003E7__wrap1;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass7_2 _003C_003Ec__DisplayClass7_ = _003C_003E4__this;
				FileStats item;
				try
				{
					if ((uint)num > 1u)
					{
						_003C_003E7__wrap1 = censoredTuple.Item1;
					}
					try
					{
						ConfiguredTaskAwaitable val;
						ConfiguredTaskAwaiter awaiter;
						if (num != 0)
						{
							if (num == 1)
							{
								goto IL_0095;
							}
							val = _003C_003Ec__DisplayClass7_.archiveLock.WaitAsync().DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CBuildUpLogArchiveInMemory_003Eb__8_003Ed>(ref awaiter, ref this);
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
						goto IL_0095;
						IL_0095:
						try
						{
							if (num != 1)
							{
								val = AddLogToArchive(_003C_003Ec__DisplayClass7_.CS_0024_003C_003E8__locals2.memory, censoredTuple.Item1, censoredTuple.Item2).DetachContext();
								awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
								if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
								{
									num = (_003C_003E1__state = 1);
									_003C_003Eu__1 = awaiter;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CBuildUpLogArchiveInMemory_003Eb__8_003Ed>(ref awaiter, ref this);
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
						finally
						{
							if (num < 0)
							{
								_003C_003Ec__DisplayClass7_.archiveLock.Release();
							}
						}
					}
					finally
					{
						if (num < 0 && _003C_003E7__wrap1 != null)
						{
							((System.IDisposable)_003C_003E7__wrap1).Dispose();
						}
					}
					_003C_003E7__wrap1 = null;
					item = censoredTuple.Item2;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult(item);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public SemaphoreSlim archiveLock;

		public _003C_003Ec__DisplayClass7_1 CS_0024_003C_003E8__locals2;

		[AsyncStateMachine(typeof(_003C_003CBuildUpLogArchiveInMemory_003Eb__8_003Ed))]
		internal async System.Threading.Tasks.Task<FileStats> _003CBuildUpLogArchiveInMemory_003Eb__8(ValueTuple<MemoryStream, FileStats> censoredTuple)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			MemoryStream item = censoredTuple.Item1;
			try
			{
				await archiveLock.WaitAsync().DetachContext();
				try
				{
					await AddLogToArchive(CS_0024_003C_003E8__locals2.memory, censoredTuple.Item1, censoredTuple.Item2).DetachContext();
				}
				finally
				{
					archiveLock.Release();
				}
			}
			finally
			{
				((System.IDisposable)item)?.Dispose();
			}
			return censoredTuple.Item2;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass8_0
	{
		[StructLayout(3)]
		private struct _003C_003CCensorFileToIntermediateStream_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<ValueTuple<MemoryStream, FileStats>> _003C_003Et__builder;

			public Stream logStream;

			public _003C_003Ec__DisplayClass8_0 _003C_003E4__this;

			private StreamReader _003Creader_003E5__2;

			private StreamWriter _003Cwriter_003E5__3;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Expected O, but got Unknown
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Expected O, but got Unknown
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_0162: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass8_0 _003C_003Ec__DisplayClass8_ = _003C_003E4__this;
				ValueTuple<MemoryStream, FileStats> result;
				try
				{
					if (num != 0)
					{
						_003Creader_003E5__2 = new StreamReader(logStream, Encoding.UTF8, true, 4096, true);
					}
					try
					{
						if (num != 0)
						{
							_003Cwriter_003E5__3 = new StreamWriter((Stream)(object)_003C_003Ec__DisplayClass8_.censoredStream, Encoding.UTF8, 4096, true);
						}
						try
						{
							ConfiguredTaskAwaiter awaiter;
							if (num != 0)
							{
								ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass8_._003C_003E4__this._aliasProvider.CopyAndCensor(_003Creader_003E5__2, _003Cwriter_003E5__3).DetachContext();
								awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
								if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
								{
									num = (_003C_003E1__state = 0);
									_003C_003Eu__1 = awaiter;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CCensorFileToIntermediateStream_003Eb__0_003Ed>(ref awaiter, ref this);
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
							((Stream)_003C_003Ec__DisplayClass8_.censoredStream).Seek(0L, (SeekOrigin)0);
							result = new ValueTuple<MemoryStream, FileStats>(_003C_003Ec__DisplayClass8_.censoredStream, _003C_003Ec__DisplayClass8_.stat);
						}
						finally
						{
							if (num < 0 && _003Cwriter_003E5__3 != null)
							{
								((System.IDisposable)_003Cwriter_003E5__3).Dispose();
							}
						}
					}
					finally
					{
						if (num < 0 && _003Creader_003E5__2 != null)
						{
							((System.IDisposable)_003Creader_003E5__2).Dispose();
						}
					}
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003Creader_003E5__2 = null;
					_003Cwriter_003E5__3 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003Creader_003E5__2 = null;
				_003Cwriter_003E5__3 = null;
				_003C_003Et__builder.SetResult(result);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public MemoryStream censoredStream;

		public CompressInMemoryLogArchiveBuilder _003C_003E4__this;

		public FileStats stat;

		[AsyncStateMachine(typeof(_003C_003CCensorFileToIntermediateStream_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<ValueTuple<MemoryStream, FileStats>> _003CCensorFileToIntermediateStream_003Eb__0(Stream logStream)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			StreamReader reader = new StreamReader(logStream, Encoding.UTF8, true, 4096, true);
			try
			{
				StreamWriter writer = new StreamWriter((Stream)(object)censoredStream, Encoding.UTF8, 4096, true);
				try
				{
					await _003C_003E4__this._aliasProvider.CopyAndCensor(reader, writer).DetachContext();
					((Stream)censoredStream).Seek(0L, (SeekOrigin)0);
					return new ValueTuple<MemoryStream, FileStats>(censoredStream, stat);
				}
				finally
				{
					((System.IDisposable)writer)?.Dispose();
				}
			}
			finally
			{
				((System.IDisposable)reader)?.Dispose();
			}
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CAddLogToArchive_003Ed__6 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public MemoryStream memory;

		public FileStats fileStats;

		public MemoryStream censoredLog;

		private ZipArchive _003Carchive_003E5__2;

		private ZipArchiveEntry _003Centry_003E5__3;

		private Stream _003CentryStream_003E5__4;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				if (num != 0)
				{
					_003Carchive_003E5__2 = new ZipArchive((Stream)(object)memory, (ZipArchiveMode)2, true);
				}
				try
				{
					if (num != 0)
					{
						_003Centry_003E5__3 = _003Carchive_003E5__2.CreateEntry(fileStats.Entry.Name, (CompressionLevel)0);
						_003CentryStream_003E5__4 = _003Centry_003E5__3.Open();
					}
					try
					{
						ConfiguredTaskAwaiter awaiter;
						if (num != 0)
						{
							ConfiguredTaskAwaitable val = ((Stream)censoredLog).CopyToAsync(_003CentryStream_003E5__4).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CAddLogToArchive_003Ed__6>(ref awaiter, ref this);
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
					finally
					{
						if (num < 0 && _003CentryStream_003E5__4 != null)
						{
							((System.IDisposable)_003CentryStream_003E5__4).Dispose();
						}
					}
					_003CentryStream_003E5__4 = null;
					_003Centry_003E5__3.LastWriteTime = fileStats.ModifiedAt;
					_003Centry_003E5__3 = null;
				}
				finally
				{
					if (num < 0 && _003Carchive_003E5__2 != null)
					{
						((System.IDisposable)_003Carchive_003E5__2).Dispose();
					}
				}
				_003Carchive_003E5__2 = null;
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
	private struct _003CBuildUpLogArchiveInMemory_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<byte[]> _003C_003Et__builder;

		public CompressInMemoryLogArchiveBuilder _003C_003E4__this;

		public int maxSize;

		private _003C_003Ec__DisplayClass7_2 _003C_003E8__1;

		private _003C_003Ec__DisplayClass7_1 _003C_003E8__2;

		private Stopwatch _003Cstopwatch_003E5__2;

		private System.Collections.Generic.IList<FileStats> _003CfilesInArchive_003E5__3;

		private ConfiguredTaskAwaiter<System.Collections.Generic.IList<FileStats>> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Expected O, but got Unknown
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Expected O, but got Unknown
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CompressInMemoryLogArchiveBuilder compressInMemoryLogArchiveBuilder = _003C_003E4__this;
			byte[] result2;
			try
			{
				if ((uint)num > 1u)
				{
					_003C_003Ec__DisplayClass7_0 cS_0024_003C_003E8__locals = new _003C_003Ec__DisplayClass7_0
					{
						_003C_003E4__this = _003C_003E4__this,
						maxSize = maxSize
					};
					_003C_003E8__2 = new _003C_003Ec__DisplayClass7_1();
					_003C_003E8__2.CS_0024_003C_003E8__locals1 = cS_0024_003C_003E8__locals;
					_003C_003E8__2.memory = new MemoryStream();
				}
				try
				{
					_ = 1;
					try
					{
						System.Collections.Generic.IEnumerable<FileSystemEntry> enumerable3 = default(System.Collections.Generic.IEnumerable<FileSystemEntry>);
						System.Collections.Generic.IEnumerable<FileSystemEntry> enumerable4 = default(System.Collections.Generic.IEnumerable<FileSystemEntry>);
						if ((uint)num > 1u)
						{
							string text = Path.Combine(compressInMemoryLogArchiveBuilder._deployment.LogDirectoryPath(), "GtiOfflineInstaller");
							System.Collections.Generic.IEnumerable<FileSystemEntry> enumerable2;
							if (!compressInMemoryLogArchiveBuilder._fileSystem.PathExists(text))
							{
								System.Collections.Generic.IEnumerable<FileSystemEntry> enumerable = Enumerable.Empty<FileSystemEntry>();
								enumerable2 = enumerable;
							}
							else
							{
								enumerable2 = Enumerable.Where<FileSystemEntry>(compressInMemoryLogArchiveBuilder._fileSystem.FilesIn(text), (Func<FileSystemEntry, bool>)((FileSystemEntry offlineLogFile) => offlineLogFile.Name.EndsWith(".clef", (StringComparison)4)));
							}
							enumerable3 = Enumerable.Concat<FileSystemEntry>(enumerable2, Enumerable.Where<FileSystemEntry>(compressInMemoryLogArchiveBuilder._fileSystem.FilesIn(compressInMemoryLogArchiveBuilder._deployment.LogDirectoryPath()), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => SystemInfoLoggingStartupAction.IsSystemInfoLoggingFile(fse) || LogRecentSessionJsonWorkflow.IsRecentSessionLog(fse))));
							enumerable4 = Enumerable.Select<FileStats, FileSystemEntry>((System.Collections.Generic.IEnumerable<FileStats>)Enumerable.OrderByDescending<FileStats, DateTimeOffset>(Enumerable.Where<FileStats>(Enumerable.Select<FileSystemEntry, FileStats>(Enumerable.Where<FileSystemEntry>(compressInMemoryLogArchiveBuilder._fileSystem.FilesIn(compressInMemoryLogArchiveBuilder._deployment.LogDirectoryPath()), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => Path.GetExtension(fse.Name) == ".log")), (Func<FileSystemEntry, FileStats>)((FileSystemEntry fse) => _003C_003E8__2.CS_0024_003C_003E8__locals1._003C_003E4__this._fileSystem.StatIfExists(fse.FullPath))), (Func<FileStats, bool>)((FileStats stat) => stat.Entry.EntryType != FileSystemEntryType.None)), (Func<FileStats, DateTimeOffset>)((FileStats stat) => stat.ModifiedAt)), (Func<FileStats, FileSystemEntry>)((FileStats stat) => stat.Entry));
							_003C_003E8__1 = new _003C_003Ec__DisplayClass7_2();
							_003C_003E8__1.CS_0024_003C_003E8__locals2 = _003C_003E8__2;
							_003C_003E8__1.archiveLock = new SemaphoreSlim(1);
						}
						try
						{
							ConfiguredTaskAwaiter awaiter;
							ConfiguredTaskAwaiter<System.Collections.Generic.IList<FileStats>> awaiter2;
							if (num != 0)
							{
								if (num == 1)
								{
									awaiter = _003C_003Eu__2;
									_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
									num = (_003C_003E1__state = -1);
									goto IL_036d;
								}
								_003Cstopwatch_003E5__2 = new Stopwatch();
								_003Cstopwatch_003E5__2.Start();
								awaiter2 = Enumerable.Concat<FileSystemEntry>(enumerable3, enumerable4).ToObservable().Buffer<FileSystemEntry>(Environment.ProcessorCount)
									.Select<System.Collections.Generic.IList<FileSystemEntry>, IObservable<ValueTuple<MemoryStream, FileStats>>>((Func<System.Collections.Generic.IList<FileSystemEntry>, IObservable<ValueTuple<MemoryStream, FileStats>>>)((System.Collections.Generic.IList<FileSystemEntry> logFileChunk) => ((System.Collections.Generic.IEnumerable<FileSystemEntry>)logFileChunk).ToObservable().Select<FileSystemEntry, System.Threading.Tasks.Task<ValueTuple<MemoryStream, FileStats>>>((Func<FileSystemEntry, System.Threading.Tasks.Task<ValueTuple<MemoryStream, FileStats>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass7_0._003C_003CBuildUpLogArchiveInMemory_003Eb__10_003Ed))] async (FileSystemEntry logFile) => await _003C_003E8__1.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this.CensorFileToIntermediateStream(logFile).DetachContext())).Merge<ValueTuple<MemoryStream, FileStats>>()))
									.Concat<ValueTuple<MemoryStream, FileStats>>()
									.Select<ValueTuple<MemoryStream, FileStats>, System.Threading.Tasks.Task<FileStats>>((Func<ValueTuple<MemoryStream, FileStats>, System.Threading.Tasks.Task<FileStats>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass7_2._003C_003CBuildUpLogArchiveInMemory_003Eb__8_003Ed))] async (ValueTuple<MemoryStream, FileStats> censoredTuple) =>
									{
										//IL_0002: Unknown result type (might be due to invalid IL or missing references)
										//IL_0007: Unknown result type (might be due to invalid IL or missing references)
										//IL_0016: Unknown result type (might be due to invalid IL or missing references)
										//IL_0017: Unknown result type (might be due to invalid IL or missing references)
										MemoryStream item = censoredTuple.Item1;
										try
										{
											await _003C_003E8__1.archiveLock.WaitAsync().DetachContext();
											try
											{
												await AddLogToArchive(_003C_003E8__1.CS_0024_003C_003E8__locals2.memory, censoredTuple.Item1, censoredTuple.Item2).DetachContext();
											}
											finally
											{
												_003C_003E8__1.archiveLock.Release();
											}
										}
										finally
										{
											((System.IDisposable)item)?.Dispose();
										}
										return censoredTuple.Item2;
									}))
									.Concat<FileStats>()
									.TakeWhile<FileStats>((Func<FileStats, bool>)((FileStats _) => ((Stream)_003C_003E8__1.CS_0024_003C_003E8__locals2.memory).Length < _003C_003E8__1.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.maxSize))
									.ToList<FileStats>()
									.ToTask<System.Collections.Generic.IList<FileStats>>()
									.DetachContext()
									.GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = (_003C_003E1__state = 0);
									_003C_003Eu__1 = awaiter2;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IList<FileStats>>, _003CBuildUpLogArchiveInMemory_003Ed__7>(ref awaiter2, ref this);
									return;
								}
							}
							else
							{
								awaiter2 = _003C_003Eu__1;
								_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IList<FileStats>>);
								num = (_003C_003E1__state = -1);
							}
							System.Collections.Generic.IList<FileStats> result = awaiter2.GetResult();
							_003CfilesInArchive_003E5__3 = result;
							ConfiguredTaskAwaitable val = _003C_003E8__1.archiveLock.WaitAsync().DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CBuildUpLogArchiveInMemory_003Ed__7>(ref awaiter, ref this);
								return;
							}
							goto IL_036d;
							IL_036d:
							((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
							try
							{
								while (((Stream)_003C_003E8__1.CS_0024_003C_003E8__locals2.memory).Length > _003C_003E8__1.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.maxSize && ((System.Collections.Generic.ICollection<FileStats>)_003CfilesInArchive_003E5__3).Count > 0)
								{
									int num2 = ((System.Collections.Generic.ICollection<FileStats>)_003CfilesInArchive_003E5__3).Count - 1;
									RemoveEntry(_003C_003E8__1.CS_0024_003C_003E8__locals2.memory, _003CfilesInArchive_003E5__3[num2]);
									_003CfilesInArchive_003E5__3.RemoveAt(num2);
								}
							}
							finally
							{
								if (num < 0)
								{
									_003C_003E8__1.archiveLock.Release();
								}
							}
							_003Cstopwatch_003E5__2.Stop();
							Logger.Log(LogLevel.Debug, $"Log sanitation/compression took {_003Cstopwatch_003E5__2.Elapsed}");
							((Stream)_003C_003E8__1.CS_0024_003C_003E8__locals2.memory).Seek(0L, (SeekOrigin)0);
							result2 = _003C_003E8__1.CS_0024_003C_003E8__locals2.memory.ToArray();
						}
						finally
						{
							if (num < 0 && _003C_003E8__1.archiveLock != null)
							{
								((System.IDisposable)_003C_003E8__1.archiveLock).Dispose();
							}
						}
					}
					catch (System.Exception e)
					{
						Logger.LogFormattedException(e, "Compressing logs to attach to feedback.");
						throw;
					}
				}
				finally
				{
					if (num < 0 && _003C_003E8__2.memory != null)
					{
						((System.IDisposable)_003C_003E8__2.memory).Dispose();
					}
				}
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCensorFileToIntermediateStream_003Ed__8 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ValueTuple<MemoryStream, FileStats>> _003C_003Et__builder;

		public CompressInMemoryLogArchiveBuilder _003C_003E4__this;

		public FileSystemEntry logFile;

		private _003C_003Ec__DisplayClass8_0 _003C_003E8__1;

		private object _003C_003E7__wrap1;

		private ConfiguredTaskAwaiter<ValueTuple<MemoryStream, FileStats>> _003C_003Eu__1;

		private ConfiguredValueTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CompressInMemoryLogArchiveBuilder compressInMemoryLogArchiveBuilder = _003C_003E4__this;
			ValueTuple<MemoryStream, FileStats> result = default(ValueTuple<MemoryStream, FileStats>);
			try
			{
				ConfiguredValueTaskAwaiter awaiter;
				int num2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredValueTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_018e;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass8_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.censoredStream = new MemoryStream();
					num2 = 0;
				}
				try
				{
					ConfiguredTaskAwaiter<ValueTuple<MemoryStream, FileStats>> awaiter2;
					if (num != 0)
					{
						_003C_003E8__1.stat = compressInMemoryLogArchiveBuilder._fileSystem.StatIfExists(logFile.FullPath);
						awaiter2 = compressInMemoryLogArchiveBuilder._fileSystem.Open(logFile.FullPath, (FileMode)3, (FileAccess)3, (FileShare)3, (FileOptions)1207959552).UseAsync<Stream, ValueTuple<MemoryStream, FileStats>>((Func<Stream, System.Threading.Tasks.Task<ValueTuple<MemoryStream, FileStats>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass8_0._003C_003CCensorFileToIntermediateStream_003Eb__0_003Ed))] async (Stream logStream) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							StreamReader reader = new StreamReader(logStream, Encoding.UTF8, true, 4096, true);
							try
							{
								StreamWriter writer = new StreamWriter((Stream)(object)_003C_003E8__1.censoredStream, Encoding.UTF8, 4096, true);
								try
								{
									await _003C_003E8__1._003C_003E4__this._aliasProvider.CopyAndCensor(reader, writer).DetachContext();
									((Stream)_003C_003E8__1.censoredStream).Seek(0L, (SeekOrigin)0);
									return new ValueTuple<MemoryStream, FileStats>(_003C_003E8__1.censoredStream, _003C_003E8__1.stat);
								}
								finally
								{
									((System.IDisposable)writer)?.Dispose();
								}
							}
							finally
							{
								((System.IDisposable)reader)?.Dispose();
							}
						})).ConfigureAwait(false)
							.GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ValueTuple<MemoryStream, FileStats>>, _003CCensorFileToIntermediateStream_003Ed__8>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ValueTuple<MemoryStream, FileStats>>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter2.GetResult();
				}
				catch (object obj)
				{
					_003C_003E7__wrap1 = obj;
					num2 = 1;
					goto IL_0118;
				}
				goto end_IL_000e;
				IL_0118:
				if (num2 == 1)
				{
					ConfiguredValueTaskAwaitable val = ((Stream)_003C_003E8__1.censoredStream).DisposeAsync().ConfigureAwait(false);
					awaiter = ((ConfiguredValueTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredValueTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredValueTaskAwaiter, _003CCensorFileToIntermediateStream_003Ed__8>(ref awaiter, ref this);
						return;
					}
					goto IL_018e;
				}
				goto IL_01b4;
				IL_018e:
				((ConfiguredValueTaskAwaiter)(ref awaiter)).GetResult();
				ExceptionDispatchInfo.Capture((_003C_003E7__wrap1 as System.Exception) ?? throw _003C_003E7__wrap1).Throw();
				goto IL_01b4;
				IL_01b4:
				_003C_003E7__wrap1 = null;
				_003C_003E8__1 = null;
				end_IL_000e:;
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCreateLogArchive_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<byte[]> _003C_003Et__builder;

		public CompressInMemoryLogArchiveBuilder _003C_003E4__this;

		public int maxSize;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private ConfiguredTaskAwaiter<byte[]> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CompressInMemoryLogArchiveBuilder compressInMemoryLogArchiveBuilder = _003C_003E4__this;
			byte[] result;
			try
			{
				ConfiguredTaskAwaiter<byte[]> awaiter;
				ConfiguredTaskAwaiter awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<byte[]>);
						num = (_003C_003E1__state = -1);
						goto IL_00ff;
					}
					if (!compressInMemoryLogArchiveBuilder._fileSystem.PathExists(compressInMemoryLogArchiveBuilder._deployment.LogDirectoryPath()))
					{
						goto IL_013f;
					}
					ConfiguredTaskAwaitable val = Logger.Flush().DetachContext();
					awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCreateLogArchive_003Ed__4>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
				}
				((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
				awaiter = compressInMemoryLogArchiveBuilder.BuildUpLogArchiveInMemory(maxSize).DetachContext().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<byte[]>, _003CCreateLogArchive_003Ed__4>(ref awaiter, ref this);
					return;
				}
				goto IL_00ff;
				IL_013f:
				result = System.Array.Empty<byte>();
				goto end_IL_000e;
				IL_00ff:
				byte[] result2 = awaiter.GetResult();
				if (result2 == null || result2.Length == 0)
				{
					Logger.Log(LogLevel.Error, "No log data could be created.");
					goto IL_013f;
				}
				if (result2.Length >= maxSize)
				{
					string text = "Log data was too large to attach.";
					Logger.Log(LogLevel.Error, text);
					throw new InvalidOperationException(text);
				}
				result = result2;
				end_IL_000e:;
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

	private readonly IFileSystem _fileSystem;

	private readonly IDeploymentPathProvider _deployment;

	private readonly IConfidentialAliasProvider _aliasProvider;

	public CompressInMemoryLogArchiveBuilder(IFileSystem fs, IDeploymentPathProvider deployment, IConfidentialAliasProvider aliases)
	{
		_fileSystem = fs;
		_deployment = deployment;
		_aliasProvider = aliases;
	}

	[AsyncStateMachine(typeof(_003CCreateLogArchive_003Ed__4))]
	public async System.Threading.Tasks.Task<byte[]> CreateLogArchive(int maxSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (_fileSystem.PathExists(_deployment.LogDirectoryPath()))
		{
			await Logger.Flush().DetachContext();
			byte[] array = await BuildUpLogArchiveInMemory(maxSize).DetachContext();
			if (array != null && array.Length != 0)
			{
				if (array.Length < maxSize)
				{
					return array;
				}
				string text = "Log data was too large to attach.";
				Logger.Log(LogLevel.Error, text);
				throw new InvalidOperationException(text);
			}
			Logger.Log(LogLevel.Error, "No log data could be created.");
		}
		return System.Array.Empty<byte>();
	}

	private static void RemoveEntry(MemoryStream memory, FileStats logFile)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		ZipArchive val = new ZipArchive((Stream)(object)memory, (ZipArchiveMode)2, true);
		try
		{
			ZipArchiveEntry entry = val.GetEntry(logFile.Entry.Name);
			if (entry != null)
			{
				entry.Delete();
			}
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
	}

	[AsyncStateMachine(typeof(_003CAddLogToArchive_003Ed__6))]
	private static System.Threading.Tasks.Task AddLogToArchive(MemoryStream memory, MemoryStream censoredLog, FileStats fileStats)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CAddLogToArchive_003Ed__6 _003CAddLogToArchive_003Ed__ = default(_003CAddLogToArchive_003Ed__6);
		_003CAddLogToArchive_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CAddLogToArchive_003Ed__.memory = memory;
		_003CAddLogToArchive_003Ed__.censoredLog = censoredLog;
		_003CAddLogToArchive_003Ed__.fileStats = fileStats;
		_003CAddLogToArchive_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CAddLogToArchive_003Ed__._003C_003Et__builder)).Start<_003CAddLogToArchive_003Ed__6>(ref _003CAddLogToArchive_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CAddLogToArchive_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CBuildUpLogArchiveInMemory_003Ed__7))]
	private async System.Threading.Tasks.Task<byte[]> BuildUpLogArchiveInMemory(int maxSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		MemoryStream memory = new MemoryStream();
		try
		{
			_ = 1;
			try
			{
				string text = Path.Combine(_deployment.LogDirectoryPath(), "GtiOfflineInstaller");
				System.Collections.Generic.IEnumerable<FileSystemEntry> enumerable2;
				if (!_fileSystem.PathExists(text))
				{
					System.Collections.Generic.IEnumerable<FileSystemEntry> enumerable = Enumerable.Empty<FileSystemEntry>();
					enumerable2 = enumerable;
				}
				else
				{
					enumerable2 = Enumerable.Where<FileSystemEntry>(_fileSystem.FilesIn(text), (Func<FileSystemEntry, bool>)((FileSystemEntry offlineLogFile) => offlineLogFile.Name.EndsWith(".clef", (StringComparison)4)));
				}
				System.Collections.Generic.IEnumerable<FileSystemEntry> enumerable3 = Enumerable.Concat<FileSystemEntry>(enumerable2, Enumerable.Where<FileSystemEntry>(_fileSystem.FilesIn(_deployment.LogDirectoryPath()), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => SystemInfoLoggingStartupAction.IsSystemInfoLoggingFile(fse) || LogRecentSessionJsonWorkflow.IsRecentSessionLog(fse))));
				System.Collections.Generic.IEnumerable<FileSystemEntry> enumerable4 = Enumerable.Select<FileStats, FileSystemEntry>((System.Collections.Generic.IEnumerable<FileStats>)Enumerable.OrderByDescending<FileStats, DateTimeOffset>(Enumerable.Where<FileStats>(Enumerable.Select<FileSystemEntry, FileStats>(Enumerable.Where<FileSystemEntry>(_fileSystem.FilesIn(_deployment.LogDirectoryPath()), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => Path.GetExtension(fse.Name) == ".log")), (Func<FileSystemEntry, FileStats>)((FileSystemEntry fse) => _fileSystem.StatIfExists(fse.FullPath))), (Func<FileStats, bool>)((FileStats stat) => stat.Entry.EntryType != FileSystemEntryType.None)), (Func<FileStats, DateTimeOffset>)((FileStats stat) => stat.ModifiedAt)), (Func<FileStats, FileSystemEntry>)((FileStats stat) => stat.Entry));
				SemaphoreSlim archiveLock = new SemaphoreSlim(1);
				try
				{
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					System.Collections.Generic.IList<FileStats> filesInArchive = await Enumerable.Concat<FileSystemEntry>(enumerable3, enumerable4).ToObservable().Buffer<FileSystemEntry>(Environment.ProcessorCount)
						.Select<System.Collections.Generic.IList<FileSystemEntry>, IObservable<ValueTuple<MemoryStream, FileStats>>>((Func<System.Collections.Generic.IList<FileSystemEntry>, IObservable<ValueTuple<MemoryStream, FileStats>>>)((System.Collections.Generic.IList<FileSystemEntry> logFileChunk) => ((System.Collections.Generic.IEnumerable<FileSystemEntry>)logFileChunk).ToObservable().Select<FileSystemEntry, System.Threading.Tasks.Task<ValueTuple<MemoryStream, FileStats>>>((Func<FileSystemEntry, System.Threading.Tasks.Task<ValueTuple<MemoryStream, FileStats>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass7_0._003C_003CBuildUpLogArchiveInMemory_003Eb__10_003Ed))] async (FileSystemEntry logFile) => await CensorFileToIntermediateStream(logFile).DetachContext())).Merge<ValueTuple<MemoryStream, FileStats>>()))
						.Concat<ValueTuple<MemoryStream, FileStats>>()
						.Select<ValueTuple<MemoryStream, FileStats>, System.Threading.Tasks.Task<FileStats>>((Func<ValueTuple<MemoryStream, FileStats>, System.Threading.Tasks.Task<FileStats>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass7_2._003C_003CBuildUpLogArchiveInMemory_003Eb__8_003Ed))] async (ValueTuple<MemoryStream, FileStats> censoredTuple) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							//IL_0016: Unknown result type (might be due to invalid IL or missing references)
							//IL_0017: Unknown result type (might be due to invalid IL or missing references)
							MemoryStream item = censoredTuple.Item1;
							try
							{
								await archiveLock.WaitAsync().DetachContext();
								try
								{
									await AddLogToArchive(memory, censoredTuple.Item1, censoredTuple.Item2).DetachContext();
								}
								finally
								{
									archiveLock.Release();
								}
							}
							finally
							{
								((System.IDisposable)item)?.Dispose();
							}
							return censoredTuple.Item2;
						}))
						.Concat<FileStats>()
						.TakeWhile<FileStats>((Func<FileStats, bool>)((FileStats _) => ((Stream)memory).Length < maxSize))
						.ToList<FileStats>()
						.ToTask<System.Collections.Generic.IList<FileStats>>()
						.DetachContext();
					await archiveLock.WaitAsync().DetachContext();
					try
					{
						while (((Stream)memory).Length > maxSize && ((System.Collections.Generic.ICollection<FileStats>)filesInArchive).Count > 0)
						{
							int num = ((System.Collections.Generic.ICollection<FileStats>)filesInArchive).Count - 1;
							RemoveEntry(memory, filesInArchive[num]);
							filesInArchive.RemoveAt(num);
						}
					}
					finally
					{
						archiveLock.Release();
					}
					stopwatch.Stop();
					Logger.Log(LogLevel.Debug, $"Log sanitation/compression took {stopwatch.Elapsed}");
					((Stream)memory).Seek(0L, (SeekOrigin)0);
					return memory.ToArray();
				}
				finally
				{
					if (archiveLock != null)
					{
						((System.IDisposable)archiveLock).Dispose();
					}
				}
			}
			catch (System.Exception e)
			{
				Logger.LogFormattedException(e, "Compressing logs to attach to feedback.");
				throw;
			}
		}
		finally
		{
			if (memory != null)
			{
				((System.IDisposable)memory).Dispose();
			}
		}
	}

	[AsyncStateMachine(typeof(_003CCensorFileToIntermediateStream_003Ed__8))]
	private async System.Threading.Tasks.Task<ValueTuple<MemoryStream, FileStats>> CensorFileToIntermediateStream(FileSystemEntry logFile)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		MemoryStream censoredStream = new MemoryStream();
		ValueTuple<MemoryStream, FileStats> result = default(ValueTuple<MemoryStream, FileStats>);
		try
		{
			FileStats stat = _fileSystem.StatIfExists(logFile.FullPath);
			result = await _fileSystem.Open(logFile.FullPath, (FileMode)3, (FileAccess)3, (FileShare)3, (FileOptions)1207959552).UseAsync<Stream, ValueTuple<MemoryStream, FileStats>>((Func<Stream, System.Threading.Tasks.Task<ValueTuple<MemoryStream, FileStats>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass8_0._003C_003CCensorFileToIntermediateStream_003Eb__0_003Ed))] async (Stream logStream) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				StreamReader reader = new StreamReader(logStream, Encoding.UTF8, true, 4096, true);
				try
				{
					StreamWriter writer = new StreamWriter((Stream)(object)censoredStream, Encoding.UTF8, 4096, true);
					try
					{
						await _aliasProvider.CopyAndCensor(reader, writer).DetachContext();
						((Stream)censoredStream).Seek(0L, (SeekOrigin)0);
						return new ValueTuple<MemoryStream, FileStats>(censoredStream, stat);
					}
					finally
					{
						((System.IDisposable)writer)?.Dispose();
					}
				}
				finally
				{
					((System.IDisposable)reader)?.Dispose();
				}
			})).ConfigureAwait(false);
			return result;
		}
		catch (object obj)
		{
			await ((Stream)censoredStream).DisposeAsync().ConfigureAwait(false);
			ExceptionDispatchInfo.Capture((obj as System.Exception) ?? throw obj).Throw();
		}
		return result;
	}
}
