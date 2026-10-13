using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Tasks;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherCoreLib.Models.CrashReporting;

internal static class FullLogArchiveFactory
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass1_0
	{
		public IFileSystem fs;

		internal DateTimeOffset _003CBuildUpLogArchiveInMemory_003Eb__2(FileSystemEntry fse)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return fs.Stat(fse.FullPath).ModifiedAt;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CAddFileToArchive_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public MemoryStream memory;

		public FileSystemEntry logFile;

		public IFileSystem fs;

		private ZipArchive _003Carchive_003E5__2;

		private ZipArchiveEntry _003Centry_003E5__3;

		private Stream _003ClogStream_003E5__4;

		private Stream _003CentryStream_003E5__5;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
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
						_003Centry_003E5__3 = _003Carchive_003E5__2.CreateEntry(logFile.Name, (CompressionLevel)0);
						_003ClogStream_003E5__4 = fs.Open(logFile.FullPath, (FileMode)3, (FileAccess)3, (FileShare)3, (FileOptions)1207959552);
					}
					try
					{
						if (num != 0)
						{
							_003CentryStream_003E5__5 = _003Centry_003E5__3.Open();
						}
						try
						{
							ConfiguredTaskAwaiter awaiter;
							if (num != 0)
							{
								ConfiguredTaskAwaitable val = _003ClogStream_003E5__4.CopyToAsync(_003CentryStream_003E5__5).DetachContext();
								awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
								if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
								{
									num = (_003C_003E1__state = 0);
									_003C_003Eu__1 = awaiter;
									((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CAddFileToArchive_003Ed__3>(ref awaiter, ref this);
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
							if (num < 0 && _003CentryStream_003E5__5 != null)
							{
								((System.IDisposable)_003CentryStream_003E5__5).Dispose();
							}
						}
						_003CentryStream_003E5__5 = null;
					}
					finally
					{
						if (num < 0 && _003ClogStream_003E5__4 != null)
						{
							((System.IDisposable)_003ClogStream_003E5__4).Dispose();
						}
					}
					_003ClogStream_003E5__4 = null;
					FileStats fileStats = fs.Stat(logFile.FullPath);
					if (fileStats != null)
					{
						_003Centry_003E5__3.LastWriteTime = fileStats.ModifiedAt;
					}
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
	private struct _003CBuildUpLogArchiveInMemory_003Ed__1 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<MemoryStream> _003C_003Et__builder;

		public IFileSystem fs;

		public IDeploymentPathProvider deployment;

		private _003C_003Ec__DisplayClass1_0 _003C_003E8__1;

		public int maxSize;

		private MemoryStream _003Cmemory_003E5__2;

		private object _003C_003E7__wrap2;

		private System.Collections.Generic.IEnumerator<FileSystemEntry> _003C_003E7__wrap3;

		private FileSystemEntry _003ClogFile_003E5__5;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private ConfiguredValueTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			MemoryStream result = default(MemoryStream);
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
						goto IL_027b;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass1_0();
					_003C_003E8__1.fs = fs;
					_003Cmemory_003E5__2 = new MemoryStream();
					num2 = 0;
				}
				try
				{
					if (num != 0)
					{
						System.Collections.Generic.IEnumerable<FileSystemEntry> enumerable = Enumerable.Where<FileSystemEntry>(_003C_003E8__1.fs.FilesIn(deployment.LogDirectoryPath()), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => SystemInfoLoggingStartupAction.IsSystemInfoLoggingFile(fse) || LogRecentSessionJsonWorkflow.IsRecentSessionLog(fse)));
						IOrderedEnumerable<FileSystemEntry> val = Enumerable.OrderByDescending<FileSystemEntry, DateTimeOffset>(Enumerable.Where<FileSystemEntry>(_003C_003E8__1.fs.FilesIn(deployment.LogDirectoryPath()), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => Path.GetExtension(fse.Name) == ".log")), (Func<FileSystemEntry, DateTimeOffset>)((FileSystemEntry fse) => _003C_003E8__1.fs.Stat(fse.FullPath).ModifiedAt));
						_003C_003E7__wrap3 = Enumerable.Concat<FileSystemEntry>(enumerable, (System.Collections.Generic.IEnumerable<FileSystemEntry>)val).GetEnumerator();
					}
					try
					{
						if (num != 0)
						{
							goto IL_01b0;
						}
						ConfiguredTaskAwaiter awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_017b;
						IL_017b:
						((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
						if (((Stream)_003Cmemory_003E5__2).Length <= maxSize)
						{
							_003ClogFile_003E5__5 = null;
							goto IL_01b0;
						}
						RemoveFileFromArchive(_003Cmemory_003E5__2, _003ClogFile_003E5__5);
						goto end_IL_00ec;
						IL_01b0:
						if (((System.Collections.IEnumerator)_003C_003E7__wrap3).MoveNext())
						{
							_003ClogFile_003E5__5 = _003C_003E7__wrap3.Current;
							ConfiguredTaskAwaitable val2 = AddFileToArchive(_003C_003E8__1.fs, _003Cmemory_003E5__2, _003ClogFile_003E5__5).DetachContext();
							awaiter2 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CBuildUpLogArchiveInMemory_003Ed__1>(ref awaiter2, ref this);
								return;
							}
							goto IL_017b;
						}
						end_IL_00ec:;
					}
					finally
					{
						if (num < 0 && _003C_003E7__wrap3 != null)
						{
							((System.IDisposable)_003C_003E7__wrap3).Dispose();
						}
					}
					_003C_003E7__wrap3 = null;
					((Stream)_003Cmemory_003E5__2).Seek(0L, (SeekOrigin)0);
					result = _003Cmemory_003E5__2;
				}
				catch (System.Exception ex)
				{
					_003C_003E7__wrap2 = ex;
					num2 = 1;
					goto IL_020a;
				}
				goto end_IL_0007;
				IL_020a:
				if (num2 == 1)
				{
					ConfiguredValueTaskAwaitable val3 = ((Stream)_003Cmemory_003E5__2).DisposeAsync().ConfigureAwait(false);
					awaiter = ((ConfiguredValueTaskAwaitable)(ref val3)).GetAwaiter();
					if (!((ConfiguredValueTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredValueTaskAwaiter, _003CBuildUpLogArchiveInMemory_003Ed__1>(ref awaiter, ref this);
						return;
					}
					goto IL_027b;
				}
				goto IL_02a1;
				IL_027b:
				((ConfiguredValueTaskAwaiter)(ref awaiter)).GetResult();
				ExceptionDispatchInfo.Capture((_003C_003E7__wrap2 as System.Exception) ?? throw _003C_003E7__wrap2).Throw();
				goto IL_02a1;
				IL_02a1:
				_003C_003E7__wrap2 = null;
				_003C_003E8__1 = null;
				_003Cmemory_003E5__2 = null;
				end_IL_0007:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003Cmemory_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003Cmemory_003E5__2 = null;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	public const int DEFAULT_MAX_ATTACHMENT_SIZE = 33554432;

	[AsyncStateMachine(typeof(_003CBuildUpLogArchiveInMemory_003Ed__1))]
	public static async System.Threading.Tasks.Task<MemoryStream> BuildUpLogArchiveInMemory(IFileSystem fs, IDeploymentPathProvider deployment, int maxSize = 33554432)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		IFileSystem fs2 = fs;
		MemoryStream memory = new MemoryStream();
		MemoryStream result = default(MemoryStream);
		try
		{
			System.Collections.Generic.IEnumerable<FileSystemEntry> enumerable = Enumerable.Where<FileSystemEntry>(fs2.FilesIn(deployment.LogDirectoryPath()), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => SystemInfoLoggingStartupAction.IsSystemInfoLoggingFile(fse) || LogRecentSessionJsonWorkflow.IsRecentSessionLog(fse)));
			IOrderedEnumerable<FileSystemEntry> val = Enumerable.OrderByDescending<FileSystemEntry, DateTimeOffset>(Enumerable.Where<FileSystemEntry>(fs2.FilesIn(deployment.LogDirectoryPath()), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => Path.GetExtension(fse.Name) == ".log")), (Func<FileSystemEntry, DateTimeOffset>)((FileSystemEntry fse) => fs2.Stat(fse.FullPath).ModifiedAt));
			System.Collections.Generic.IEnumerator<FileSystemEntry> enumerator = Enumerable.Concat<FileSystemEntry>(enumerable, (System.Collections.Generic.IEnumerable<FileSystemEntry>)val).GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					FileSystemEntry logFile = enumerator.Current;
					await AddFileToArchive(fs2, memory, logFile).DetachContext();
					if (((Stream)memory).Length > maxSize)
					{
						RemoveFileFromArchive(memory, logFile);
						break;
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
			((Stream)memory).Seek(0L, (SeekOrigin)0);
			result = memory;
			return result;
		}
		catch (System.Exception ex)
		{
			await ((Stream)memory).DisposeAsync().ConfigureAwait(false);
			ExceptionDispatchInfo.Capture((ex as System.Exception) ?? throw ex).Throw();
		}
		return result;
	}

	private static void RemoveFileFromArchive(MemoryStream memory, FileSystemEntry logFile)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		ZipArchive val = new ZipArchive((Stream)(object)memory, (ZipArchiveMode)2, true);
		try
		{
			ZipArchiveEntry entry = val.GetEntry(logFile.Name);
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

	[AsyncStateMachine(typeof(_003CAddFileToArchive_003Ed__3))]
	private static System.Threading.Tasks.Task AddFileToArchive(IFileSystem fs, MemoryStream memory, FileSystemEntry logFile)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CAddFileToArchive_003Ed__3 _003CAddFileToArchive_003Ed__ = default(_003CAddFileToArchive_003Ed__3);
		_003CAddFileToArchive_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CAddFileToArchive_003Ed__.fs = fs;
		_003CAddFileToArchive_003Ed__.memory = memory;
		_003CAddFileToArchive_003Ed__.logFile = logFile;
		_003CAddFileToArchive_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CAddFileToArchive_003Ed__._003C_003Et__builder)).Start<_003CAddFileToArchive_003Ed__3>(ref _003CAddFileToArchive_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CAddFileToArchive_003Ed__._003C_003Et__builder)).Task;
	}
}
