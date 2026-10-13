using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.DeployedEnv;

public static class FileCensorHelpers
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCopyAndCensor_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public StreamReader reader;

		public IConfidentialAliasProvider censors;

		public StreamWriter writer;

		private ConfiguredTaskAwaiter<string> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter<string> awaiter2;
				ConfiguredTaskAwaiter awaiter;
				string result;
				ConfiguredTaskAwaitable val;
				switch (num)
				{
				default:
					awaiter2 = ((TextReader)reader).ReadLineAsync().DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003CCopyAndCensor_003Ed__2>(ref awaiter2, ref this);
						return;
					}
					goto IL_007b;
				case 0:
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<string>);
					num = (_003C_003E1__state = -1);
					goto IL_007b;
				case 1:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_00f9;
				case 2:
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						break;
					}
					IL_00f9:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					goto default;
					IL_007b:
					result = awaiter2.GetResult();
					if (result != null)
					{
						string text = censors.CensorString(result);
						val = ((TextWriter)writer).WriteLineAsync(text).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCopyAndCensor_003Ed__2>(ref awaiter, ref this);
							return;
						}
						goto IL_00f9;
					}
					val = ((TextWriter)writer).FlushAsync().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCopyAndCensor_003Ed__2>(ref awaiter, ref this);
						return;
					}
					break;
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
	[CompilerGenerated]
	private struct _003CCopyAndCensorFileAndKeepTimestamps_003Ed__0 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public IFileSystem fs;

		public string sourcePath;

		public string destinationPath;

		public IConfidentialAliasProvider censors;

		public Encoding sourceEncoding;

		public Encoding targetEncoding;

		private FileStats _003CsourceStat_003E5__2;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					_003CsourceStat_003E5__2 = fs.StatIfExists(sourcePath);
					ConfiguredTaskAwaitable val = fs.CopyAndCensorFiles(sourcePath, destinationPath, censors, sourceEncoding, targetEncoding).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCopyAndCensorFileAndKeepTimestamps_003Ed__0>(ref awaiter, ref this);
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
				fs.SetFileTimes(destinationPath, _003CsourceStat_003E5__2.Times);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CsourceStat_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CsourceStat_003E5__2 = null;
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
	private struct _003CCopyAndCensorFiles_003Ed__1 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public IFileSystem fs;

		public string sourcePath;

		public string destinationPath;

		public Encoding sourceEncoding;

		public Encoding targetEncoding;

		public IConfidentialAliasProvider censors;

		private Stream _003Cread_003E5__2;

		private Stream _003Cwrite_003E5__3;

		private StreamReader _003Creader_003E5__4;

		private StreamWriter _003Cwriter_003E5__5;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Expected O, but got Unknown
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				if (num != 0)
				{
					_003Cread_003E5__2 = fs.Open(sourcePath, (FileMode)3, (FileAccess)1, (FileShare)3, (FileOptions)1207959552);
				}
				try
				{
					if (num != 0)
					{
						_003Cwrite_003E5__3 = fs.Open(destinationPath, (FileMode)1, (FileAccess)2, (FileShare)3, (FileOptions)1207959552);
					}
					try
					{
						if (num != 0)
						{
							_003Creader_003E5__4 = ((sourceEncoding != null) ? new StreamReader(_003Cread_003E5__2, sourceEncoding, false, 4096) : new StreamReader(_003Cread_003E5__2, targetEncoding ?? FileSystemDefaults.FileEncoding, true, 4096));
						}
						try
						{
							if (num != 0)
							{
								_003Cwriter_003E5__5 = new StreamWriter(_003Cwrite_003E5__3, targetEncoding ?? FileSystemDefaults.FileEncoding, 4096);
							}
							try
							{
								ConfiguredTaskAwaiter awaiter;
								if (num != 0)
								{
									ConfiguredTaskAwaitable val = censors.CopyAndCensor(_003Creader_003E5__4, _003Cwriter_003E5__5).DetachContext();
									awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
									if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
									{
										num = (_003C_003E1__state = 0);
										_003C_003Eu__1 = awaiter;
										((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCopyAndCensorFiles_003Ed__1>(ref awaiter, ref this);
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
								if (num < 0 && _003Cwriter_003E5__5 != null)
								{
									((System.IDisposable)_003Cwriter_003E5__5).Dispose();
								}
							}
						}
						finally
						{
							if (num < 0 && _003Creader_003E5__4 != null)
							{
								((System.IDisposable)_003Creader_003E5__4).Dispose();
							}
						}
					}
					finally
					{
						if (num < 0 && _003Cwrite_003E5__3 != null)
						{
							((System.IDisposable)_003Cwrite_003E5__3).Dispose();
						}
					}
				}
				finally
				{
					if (num < 0 && _003Cread_003E5__2 != null)
					{
						((System.IDisposable)_003Cread_003E5__2).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Cread_003E5__2 = null;
				_003Cwrite_003E5__3 = null;
				_003Creader_003E5__4 = null;
				_003Cwriter_003E5__5 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Cread_003E5__2 = null;
			_003Cwrite_003E5__3 = null;
			_003Creader_003E5__4 = null;
			_003Cwriter_003E5__5 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	[AsyncStateMachine(typeof(_003CCopyAndCensorFileAndKeepTimestamps_003Ed__0))]
	public static System.Threading.Tasks.Task CopyAndCensorFileAndKeepTimestamps(this IFileSystem fs, string sourcePath, string destinationPath, IConfidentialAliasProvider censors, Encoding? sourceEncoding = null, Encoding? targetEncoding = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CCopyAndCensorFileAndKeepTimestamps_003Ed__0 _003CCopyAndCensorFileAndKeepTimestamps_003Ed__ = default(_003CCopyAndCensorFileAndKeepTimestamps_003Ed__0);
		_003CCopyAndCensorFileAndKeepTimestamps_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CCopyAndCensorFileAndKeepTimestamps_003Ed__.fs = fs;
		_003CCopyAndCensorFileAndKeepTimestamps_003Ed__.sourcePath = sourcePath;
		_003CCopyAndCensorFileAndKeepTimestamps_003Ed__.destinationPath = destinationPath;
		_003CCopyAndCensorFileAndKeepTimestamps_003Ed__.censors = censors;
		_003CCopyAndCensorFileAndKeepTimestamps_003Ed__.sourceEncoding = sourceEncoding;
		_003CCopyAndCensorFileAndKeepTimestamps_003Ed__.targetEncoding = targetEncoding;
		_003CCopyAndCensorFileAndKeepTimestamps_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CCopyAndCensorFileAndKeepTimestamps_003Ed__._003C_003Et__builder)).Start<_003CCopyAndCensorFileAndKeepTimestamps_003Ed__0>(ref _003CCopyAndCensorFileAndKeepTimestamps_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CCopyAndCensorFileAndKeepTimestamps_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CCopyAndCensorFiles_003Ed__1))]
	public static System.Threading.Tasks.Task CopyAndCensorFiles(this IFileSystem fs, string sourcePath, string destinationPath, IConfidentialAliasProvider censors, Encoding? sourceEncoding = null, Encoding? targetEncoding = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CCopyAndCensorFiles_003Ed__1 _003CCopyAndCensorFiles_003Ed__ = default(_003CCopyAndCensorFiles_003Ed__1);
		_003CCopyAndCensorFiles_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CCopyAndCensorFiles_003Ed__.fs = fs;
		_003CCopyAndCensorFiles_003Ed__.sourcePath = sourcePath;
		_003CCopyAndCensorFiles_003Ed__.destinationPath = destinationPath;
		_003CCopyAndCensorFiles_003Ed__.censors = censors;
		_003CCopyAndCensorFiles_003Ed__.sourceEncoding = sourceEncoding;
		_003CCopyAndCensorFiles_003Ed__.targetEncoding = targetEncoding;
		_003CCopyAndCensorFiles_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CCopyAndCensorFiles_003Ed__._003C_003Et__builder)).Start<_003CCopyAndCensorFiles_003Ed__1>(ref _003CCopyAndCensorFiles_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CCopyAndCensorFiles_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CCopyAndCensor_003Ed__2))]
	public static System.Threading.Tasks.Task CopyAndCensor(this IConfidentialAliasProvider censors, StreamReader reader, StreamWriter writer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CCopyAndCensor_003Ed__2 _003CCopyAndCensor_003Ed__ = default(_003CCopyAndCensor_003Ed__2);
		_003CCopyAndCensor_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CCopyAndCensor_003Ed__.censors = censors;
		_003CCopyAndCensor_003Ed__.reader = reader;
		_003CCopyAndCensor_003Ed__.writer = writer;
		_003CCopyAndCensor_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CCopyAndCensor_003Ed__._003C_003Et__builder)).Start<_003CCopyAndCensor_003Ed__2>(ref _003CCopyAndCensor_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CCopyAndCensor_003Ed__._003C_003Et__builder)).Task;
	}

	public static string CensorString(this IConfidentialAliasProvider aliasProvider, string str)
	{
		System.Collections.Generic.IEnumerator<ConfidentialAlias> enumerator = aliasProvider.Aliases.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				ConfidentialAlias current = enumerator.Current;
				str = current.SecretRegex.Replace(str, current.Alias);
			}
			return str;
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}
}
