using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Compression;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.StringHelpers.Paths;
using LOLA.Tasks;
using LanguageExt;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Winfs;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.CrashReporting;

public sealed class SessionCrashScope : IEquatable<SessionCrashScope>
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCopyFileToCrashZip_003Ed__31<T> : IAsyncStateMachine where T : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<T> _003C_003Et__builder;

		public RelativePath destinationPath;

		public SessionCrashScope _003C_003E4__this;

		public string sourcePath;

		public Func<T> success;

		public Func<T> notFound;

		public Func<T> accessDenied;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SessionCrashScope sessionCrashScope = _003C_003E4__this;
			T result;
			try
			{
				if (num == 0)
				{
					goto IL_005c;
				}
				if (PathHelpers.PathEscapesIntoParent(destinationPath.RelPath))
				{
					throw new ArgumentException("Must not use parent directory ('../') pathing.");
				}
				string path = Path.Combine(sessionCrashScope.BaseReportingPath, destinationPath.RelPath);
				if (sessionCrashScope._fs.PathExists(sourcePath))
				{
					goto IL_005c;
				}
				result = notFound.Invoke();
				goto end_IL_000e;
				IL_005c:
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						sessionCrashScope._fs.DeleteIfExists(path);
						ConfiguredTaskAwaitable val = sessionCrashScope._fs.CopyAndCensorFileAndKeepTimestamps(sourcePath, path, sessionCrashScope._confidentialAliasProvider).ConfigureAwait(false);
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCopyFileToCrashZip_003Ed__31<T>>(ref awaiter, ref this);
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
					result = success.Invoke();
				}
				catch (System.Exception ex) when (ex is FileNotFoundException || ex.IsAccessDenied())
				{
					if (ex is FileNotFoundException)
					{
						result = notFound.Invoke();
					}
					else
					{
						Logger.LogFormattedException(ex, "Copying DxDiag To Crash scope.");
						result = accessDenied.Invoke();
					}
				}
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCreateCopyOfLogForReporting_003Ed__30 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SessionCrashScope _003C_003E4__this;

		public string logPath;

		private string _003CsessionLogPath_003E5__2;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private Stream _003CsourceStream_003E5__3;

		private Stream _003CdestinationStream_003E5__4;

		private void MoveNext()
		{
			//IL_01da: Expected O, but got Unknown
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SessionCrashScope sessionCrashScope = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaitable val;
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_00aa;
					}
					_003CsessionLogPath_003E5__2 = sessionCrashScope.SessionLogPath;
					Logger.Log(LogLevel.Info, "Copying log from {0} to {1}. This will likely be the last entry shown in the crash report.", logPath, _003CsessionLogPath_003E5__2);
					val = Logger.Flush().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCreateCopyOfLogForReporting_003Ed__30>(ref awaiter, ref this);
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
				goto IL_00aa;
				IL_00aa:
				try
				{
					if (num == 1)
					{
						goto IL_00fa;
					}
					if (sessionCrashScope._fs.PathExists(logPath) && !sessionCrashScope._fs.PathExists(_003CsessionLogPath_003E5__2))
					{
						_003CsourceStream_003E5__3 = sessionCrashScope._fs.Open(logPath, (FileMode)3, (FileAccess)1, (FileShare)3, (FileOptions)1207959552);
						goto IL_00fa;
					}
					goto end_IL_00aa;
					IL_00fa:
					try
					{
						if (num != 1)
						{
							_003CdestinationStream_003E5__4 = sessionCrashScope._fs.Open(_003CsessionLogPath_003E5__2, (FileMode)1, (FileAccess)2, (FileShare)0, (FileOptions)1207959552);
						}
						try
						{
							if (num != 1)
							{
								val = _003CsourceStream_003E5__3.CopyToAsync(_003CdestinationStream_003E5__4).DetachContext();
								awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
								if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
								{
									num = (_003C_003E1__state = 1);
									_003C_003Eu__1 = awaiter;
									((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCreateCopyOfLogForReporting_003Ed__30>(ref awaiter, ref this);
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
							if (num < 0 && _003CdestinationStream_003E5__4 != null)
							{
								((System.IDisposable)_003CdestinationStream_003E5__4).Dispose();
							}
						}
					}
					finally
					{
						if (num < 0 && _003CsourceStream_003E5__3 != null)
						{
							((System.IDisposable)_003CsourceStream_003E5__3).Dispose();
						}
					}
					_003CsourceStream_003E5__3 = null;
					_003CdestinationStream_003E5__4 = null;
					end_IL_00aa:;
				}
				catch (IOException val2)
				{
					Logger.Log((System.Exception)val2);
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CsessionLogPath_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CsessionLogPath_003E5__2 = null;
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
	private struct _003CRunCensorOnRelativePath_003Ed__29 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SessionCrashScope _003C_003E4__this;

		public RelativePath sessionRelativePath;

		private string _003CabsolutePath_003E5__2;

		private string _003CtmpPath_003E5__3;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SessionCrashScope CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			try
			{
				if (num == 0)
				{
					goto IL_007e;
				}
				_003CabsolutePath_003E5__2 = Path.Combine(CS_0024_003C_003E8__locals0.BaseReportingPath, sessionRelativePath.RelPath);
				if (PathHelpers.PathEscapesIntoParent(_003CabsolutePath_003E5__2))
				{
					throw new ArgumentException("Relative path cannot escape into a parent directory.");
				}
				if (CS_0024_003C_003E8__locals0._fs.PathExists(_003CabsolutePath_003E5__2))
				{
					_003CtmpPath_003E5__3 = Enumerable.First<string>(_003CRunCensorOnRelativePath_003Eg__GenerateTemporaryFileName_007C29_0(_003CabsolutePath_003E5__2), (Func<string, bool>)([CompilerGenerated] (string p) => !CS_0024_003C_003E8__locals0._fs.PathExists(p)));
					goto IL_007e;
				}
				goto end_IL_000e;
				IL_007e:
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = CS_0024_003C_003E8__locals0._fs.CopyAndCensorFileAndKeepTimestamps(_003CabsolutePath_003E5__2, _003CtmpPath_003E5__3, CS_0024_003C_003E8__locals0._confidentialAliasProvider).ConfigureAwait(false);
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CRunCensorOnRelativePath_003Ed__29>(ref awaiter, ref this);
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
					CS_0024_003C_003E8__locals0._fs.MoveAndOverwrite(_003CtmpPath_003E5__3, _003CabsolutePath_003E5__2);
				}
				catch
				{
					CS_0024_003C_003E8__locals0._fs.DeleteAnythingNotLocked(_003CtmpPath_003E5__3);
					throw;
				}
				_003CtmpPath_003E5__3 = null;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CabsolutePath_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CabsolutePath_003E5__2 = null;
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
	private struct _003CRunCensorOnTextFiles_003Ed__28 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SessionCrashScope _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SessionCrashScope CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_00d9;
				}
				if (CS_0024_003C_003E8__locals0._fs.PathExists(CS_0024_003C_003E8__locals0.BaseReportingPath))
				{
					System.Collections.Generic.IReadOnlyCollection<FileSystemEntry> readOnlyCollection;
					try
					{
						readOnlyCollection = (System.Collections.Generic.IReadOnlyCollection<FileSystemEntry>)Enumerable.ToList<FileSystemEntry>(Enumerable.Where<FileSystemEntry>(CS_0024_003C_003E8__locals0._fs.RecursiveFilesIn(CS_0024_003C_003E8__locals0.BaseReportingPath), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => Path.GetExtension(fse.Name).Apply<string, bool>((string ext) => ext == ".json" || ext == ".xml" || ext == ".txt"))));
					}
					catch (DirectoryNotFoundException)
					{
						goto end_IL_000e;
					}
					ConfiguredTaskAwaitable val2 = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<FileSystemEntry, System.Threading.Tasks.Task>((System.Collections.Generic.IEnumerable<FileSystemEntry>)readOnlyCollection, (Func<FileSystemEntry, System.Threading.Tasks.Task>)([CompilerGenerated] (FileSystemEntry fse) => CS_0024_003C_003E8__locals0.RunCensorOnRelativePath(new RelativePath(fse.FullPath))))).ConfigureAwait(false);
					awaiter = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CRunCensorOnTextFiles_003Ed__28>(ref awaiter, ref this);
						return;
					}
					goto IL_00d9;
				}
				goto end_IL_000e;
				IL_00d9:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				end_IL_000e:;
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

	private readonly IFileSystem _fs;

	private readonly IZipCompression _zipCompression;

	private readonly IConfidentialAliasProvider _confidentialAliasProvider;

	[field: CompilerGenerated]
	public DateTimeOffset SessionStartTime
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public int SessionCount
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string BaseReportingPath
	{
		[CompilerGenerated]
		get;
	}

	public string SessionName => Path.GetFileName(BaseReportingPath);

	public string SessionLogPath => Path.Combine(BaseReportingPath, "launcher.log");

	public string SessionZipPath => Path.Combine(Path.GetDirectoryName(BaseReportingPath) ?? throw new InvalidOperationException("BaseReportingPath was a drive root!"), SessionName + ".zip");

	public SessionCrashScope(IFileSystem fileSystem, IZipCompression zipCompression, IConfidentialAliasProvider confidentialAliasProvider, string baseDir, DateTimeOffset createdUtc, int session_count)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		_fs = fileSystem ?? throw new ArgumentNullException("fileSystem");
		_zipCompression = zipCompression ?? throw new ArgumentNullException("zipCompression");
		_confidentialAliasProvider = confidentialAliasProvider;
		SessionStartTime = createdUtc;
		SessionCount = session_count;
		BaseReportingPath = BuildFolderPath(baseDir, SessionStartTime, SessionCount);
		CreateDirectoryIfItDoesntExist();
	}

	public SessionCrashScope(IFileSystem fileSystem, IZipCompression zipCompression, IConfidentialAliasProvider confidentialAliasProvider, string folderPath)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		_fs = fileSystem ?? throw new ArgumentNullException("fileSystem");
		_zipCompression = zipCompression ?? throw new ArgumentNullException("zipCompression");
		_confidentialAliasProvider = confidentialAliasProvider;
		BaseReportingPath = folderPath;
		string fileName = Path.GetFileName(folderPath);
		string[] array = fileName.Split(new string[1] { "_" }, (StringSplitOptions)1);
		if (!SessionPartsAreValidFormat(array))
		{
			throw new FormatException("session folder: '" + fileName + "' is not a valid format.");
		}
		SessionCount = int.Parse(array[0], (NumberStyles)7, (IFormatProvider)(object)CultureInfo.InvariantCulture);
		SessionStartTime = DateTimeOffset.op_Implicit(new System.DateTime(long.Parse(array[1], (NumberStyles)7, (IFormatProvider)(object)CultureInfo.InvariantCulture), (DateTimeKind)2));
		CreateDirectoryIfItDoesntExist();
	}

	public static System.Collections.Generic.IList<SessionCrashScope> SessionDirectories(IFileSystem fileSystem, IZipCompression zipCompression, IDeploymentPathProvider deployment, IConfidentialAliasProvider confidentialAliasProvider)
	{
		IFileSystem fileSystem2 = fileSystem;
		IZipCompression zipCompression2 = zipCompression;
		IConfidentialAliasProvider confidentialAliasProvider2 = confidentialAliasProvider;
		string text = deployment.SessionBaseDirectoryPath();
		if (!fileSystem2.PathExists(text))
		{
			return (System.Collections.Generic.IList<SessionCrashScope>)new List<SessionCrashScope>();
		}
		return (System.Collections.Generic.IList<SessionCrashScope>)Enumerable.ToList<SessionCrashScope>(Enumerable.Where<SessionCrashScope>(Enumerable.Select<FileSystemEntry, SessionCrashScope>(Enumerable.Where<FileSystemEntry>(fileSystem2.DirectoriesIn(text), (Func<FileSystemEntry, bool>)FolderNameIsFormattedCorrectly), (Func<FileSystemEntry, SessionCrashScope>)((FileSystemEntry sessionDir) => new SessionCrashScope(fileSystem2, zipCompression2, confidentialAliasProvider2, sessionDir.FullPath))), (Func<SessionCrashScope, bool>)((SessionCrashScope s) => s.ContainsCrashes())));
	}

	public static string BuildFolderPath(string baseDir, DateTimeOffset now, int sess_count)
	{
		string text = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0}_{1}", (object)sess_count, (object)((DateTimeOffset)(ref now)).Ticks);
		return Path.Combine(baseDir, text);
	}

	public bool ContainsCrashes()
	{
		if (_fs.PathExists(BaseReportingPath) && Enumerable.Any<string>(GetXmlCrashMetaPaths()))
		{
			return true;
		}
		return false;
	}

	public System.Collections.Generic.IEnumerable<SimInstanceCrashInfo> GetIndividualSimCrashes()
	{
		return Enumerable.Select<string, SimInstanceCrashInfo>(GetXmlCrashMetaPaths(), (Func<string, SimInstanceCrashInfo>)([CompilerGenerated] (string f) => new SimInstanceCrashInfo(SessionLogPath, f, SessionZipPath, this)));
	}

	public override string ToString()
	{
		return SessionName;
	}

	public bool Equals(SessionCrashScope? other)
	{
		if (this != other)
		{
			if (other != null)
			{
				return BaseReportingPath == other.BaseReportingPath;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is SessionCrashScope other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return BaseReportingPath.GetHashCode((StringComparison)4);
	}

	[AsyncStateMachine(typeof(_003CRunCensorOnTextFiles_003Ed__28))]
	public System.Threading.Tasks.Task RunCensorOnTextFiles()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CRunCensorOnTextFiles_003Ed__28 _003CRunCensorOnTextFiles_003Ed__ = default(_003CRunCensorOnTextFiles_003Ed__28);
		_003CRunCensorOnTextFiles_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CRunCensorOnTextFiles_003Ed__._003C_003E4__this = this;
		_003CRunCensorOnTextFiles_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CRunCensorOnTextFiles_003Ed__._003C_003Et__builder)).Start<_003CRunCensorOnTextFiles_003Ed__28>(ref _003CRunCensorOnTextFiles_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CRunCensorOnTextFiles_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CRunCensorOnRelativePath_003Ed__29))]
	public System.Threading.Tasks.Task RunCensorOnRelativePath(RelativePath sessionRelativePath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CRunCensorOnRelativePath_003Ed__29 _003CRunCensorOnRelativePath_003Ed__ = default(_003CRunCensorOnRelativePath_003Ed__29);
		_003CRunCensorOnRelativePath_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CRunCensorOnRelativePath_003Ed__._003C_003E4__this = this;
		_003CRunCensorOnRelativePath_003Ed__.sessionRelativePath = sessionRelativePath;
		_003CRunCensorOnRelativePath_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CRunCensorOnRelativePath_003Ed__._003C_003Et__builder)).Start<_003CRunCensorOnRelativePath_003Ed__29>(ref _003CRunCensorOnRelativePath_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CRunCensorOnRelativePath_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CCreateCopyOfLogForReporting_003Ed__30))]
	internal System.Threading.Tasks.Task CreateCopyOfLogForReporting(string logPath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CCreateCopyOfLogForReporting_003Ed__30 _003CCreateCopyOfLogForReporting_003Ed__ = default(_003CCreateCopyOfLogForReporting_003Ed__30);
		_003CCreateCopyOfLogForReporting_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CCreateCopyOfLogForReporting_003Ed__._003C_003E4__this = this;
		_003CCreateCopyOfLogForReporting_003Ed__.logPath = logPath;
		_003CCreateCopyOfLogForReporting_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CCreateCopyOfLogForReporting_003Ed__._003C_003Et__builder)).Start<_003CCreateCopyOfLogForReporting_003Ed__30>(ref _003CCreateCopyOfLogForReporting_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CCreateCopyOfLogForReporting_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CCopyFileToCrashZip_003Ed__31<>))]
	internal async System.Threading.Tasks.Task<T> CopyFileToCrashZip<T>(string sourcePath, RelativePath destinationPath, Func<T> success, Func<T> notFound, Func<T> accessDenied)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (PathHelpers.PathEscapesIntoParent(destinationPath.RelPath))
		{
			throw new ArgumentException("Must not use parent directory ('../') pathing.");
		}
		string text = Path.Combine(BaseReportingPath, destinationPath.RelPath);
		if (_fs.PathExists(sourcePath))
		{
			try
			{
				_fs.DeleteIfExists(text);
				await _fs.CopyAndCensorFileAndKeepTimestamps(sourcePath, text, _confidentialAliasProvider).ConfigureAwait(false);
				return success.Invoke();
			}
			catch (System.Exception ex) when (ex is FileNotFoundException || ex.IsAccessDenied())
			{
				if (ex is FileNotFoundException)
				{
					return notFound.Invoke();
				}
				Logger.LogFormattedException(ex, "Copying DxDiag To Crash scope.");
				return accessDenied.Invoke();
			}
		}
		return notFound.Invoke();
	}

	internal string SaveToZipArchive()
	{
		//IL_0049: Expected O, but got Unknown
		string sessionZipPath = SessionZipPath;
		if (!_fs.PathExists(sessionZipPath))
		{
			if (!_fs.PathExists(BaseReportingPath))
			{
				throw new NativeCrashReportPackingException("The session directory did not exist. Another process must have deleted it.");
			}
			try
			{
				_zipCompression.ZipDirectory(BaseReportingPath, sessionZipPath);
			}
			catch (IOException val)
			{
				IOException innerException = val;
				throw new NativeCrashReportPackingException("The session archive could not be created.", (System.Exception)(object)innerException);
			}
		}
		return sessionZipPath;
	}

	internal void ClearScope()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		Logger.Log(LogLevel.Info, "Removing {0} and {1}", BaseReportingPath, SessionZipPath);
		if (_fs.PathExists(SessionZipPath))
		{
			CatchFileException((Action)([CompilerGenerated] () =>
			{
				_fs.DeleteAnythingNotLocked(SessionZipPath);
			}));
		}
		if (_fs.PathExists(BaseReportingPath))
		{
			CatchFileException((Action)([CompilerGenerated] () =>
			{
				_fs.DeleteAnythingNotLocked(BaseReportingPath);
			}));
		}
	}

	private static bool FolderNameIsFormattedCorrectly(FileSystemEntry folderEntry)
	{
		return SessionPartsAreValidFormat(Path.GetFileName(folderEntry.FullPath).Split(new string[1] { "_" }, (StringSplitOptions)1));
	}

	private static bool SessionPartsAreValidFormat(string[] parts)
	{
		int num = default(int);
		long num2 = default(long);
		if (parts.Length == 2 && int.TryParse(parts[0], (NumberStyles)7, (IFormatProvider)(object)CultureInfo.InvariantCulture, ref num) && num >= 0 && long.TryParse(parts[1], (NumberStyles)7, (IFormatProvider)(object)CultureInfo.InvariantCulture, ref num2))
		{
			return num2 >= 0;
		}
		return false;
	}

	private void CreateDirectoryIfItDoesntExist()
	{
		if (!_fs.PathExists(BaseReportingPath))
		{
			_fs.CreateDirectory(BaseReportingPath);
		}
	}

	private void CatchFileException(Action ioOperation)
	{
		try
		{
			ioOperation.Invoke();
		}
		catch (System.Exception ex)
		{
			Logger.Log(ex, string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "While attempting to clear session scope {0}", (object)BaseReportingPath));
			if (!(ex is IOException) && !(ex is UnauthorizedAccessException))
			{
				throw;
			}
		}
	}

	private System.Collections.Generic.IEnumerable<string> GetXmlCrashMetaPaths()
	{
		return Enumerable.ToArray<string>(Enumerable.Select<FileSystemEntry, string>(Enumerable.Where<FileSystemEntry>(_fs.RecursiveEntriesIn(BaseReportingPath), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => fse.EntryType == FileSystemEntryType.File && string.Equals(fse.Name.Trim(), "crashrpt.xml", (StringComparison)4))), (Func<FileSystemEntry, string>)((FileSystemEntry fse) => fse.FullPath)));
	}

	[CompilerGenerated]
	internal static System.Collections.Generic.IEnumerable<string> _003CRunCensorOnRelativePath_003Eg__GenerateTemporaryFileName_007C29_0(string targetFilePath)
	{
		return LauncherPathHelpers.GenerateNumberedFileNamesWithSuffix(targetFilePath, ".tmp");
	}
}
