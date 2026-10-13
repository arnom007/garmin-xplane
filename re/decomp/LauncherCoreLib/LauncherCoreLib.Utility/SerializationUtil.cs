using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.StringHelpers.Paths;
using LOLA.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Utility;

public static class SerializationUtil
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CLoadFromJsonAsync_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<JToken> _003C_003Et__builder;

		public IFileSystem fs;

		public string srcFile;

		private Stream _003CasyncFileStream_003E5__2;

		private StreamReader _003Csw_003E5__3;

		private JsonTextReader _003Cjw_003E5__4;

		private ConfiguredTaskAwaiter<JToken> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			JToken result;
			try
			{
				if (num != 0)
				{
					_003CasyncFileStream_003E5__2 = fs.Open(srcFile, (FileMode)3, (FileAccess)1, (FileShare)3, (FileOptions)1073741824);
				}
				try
				{
					if (num != 0)
					{
						_003Csw_003E5__3 = new StreamReader(_003CasyncFileStream_003E5__2);
					}
					try
					{
						if (num != 0)
						{
							_003Cjw_003E5__4 = new JsonTextReader((TextReader)(object)_003Csw_003E5__3);
						}
						try
						{
							ConfiguredTaskAwaiter<JToken> awaiter;
							if (num != 0)
							{
								awaiter = JToken.LoadAsync(_003Cjw_003E5__4).DetachContext().GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_003C_003E1__state = 0);
									_003C_003Eu__1 = awaiter;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<JToken>, _003CLoadFromJsonAsync_003Ed__7>(ref awaiter, ref this);
									return;
								}
							}
							else
							{
								awaiter = _003C_003Eu__1;
								_003C_003Eu__1 = default(ConfiguredTaskAwaiter<JToken>);
								num = (_003C_003E1__state = -1);
							}
							result = awaiter.GetResult();
						}
						finally
						{
							if (num < 0 && _003Cjw_003E5__4 != null)
							{
								((System.IDisposable)_003Cjw_003E5__4).Dispose();
							}
						}
					}
					finally
					{
						if (num < 0 && _003Csw_003E5__3 != null)
						{
							((System.IDisposable)_003Csw_003E5__3).Dispose();
						}
					}
				}
				finally
				{
					if (num < 0 && _003CasyncFileStream_003E5__2 != null)
					{
						((System.IDisposable)_003CasyncFileStream_003E5__2).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CasyncFileStream_003E5__2 = null;
				_003Csw_003E5__3 = null;
				_003Cjw_003E5__4 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CasyncFileStream_003E5__2 = null;
			_003Csw_003E5__3 = null;
			_003Cjw_003E5__4 = null;
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
	private struct _003CLoadFromJsonAsync_003Ed__9<TJsonModel> : IAsyncStateMachine where TJsonModel : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TJsonModel> _003C_003Et__builder;

		public string srcFile;

		public IFileSystem fs;

		private ConfiguredTaskAwaiter<JToken> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			TJsonModel result;
			try
			{
				ConfiguredTaskAwaiter<JToken> awaiter;
				if (num != 0)
				{
					awaiter = LoadFromJsonAsync(srcFile, fs).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<JToken>, _003CLoadFromJsonAsync_003Ed__9<TJsonModel>>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<JToken>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToObject<TJsonModel>(JSON_SERIALIZER.Value);
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
	private struct _003CSaveToJsonAsync_003Ed__11<TJsonModel> : IAsyncStateMachine where TJsonModel : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public string path;

		public IFileSystem fs;

		public TJsonModel jsonModel;

		private string _003CtmpFile_003E5__2;

		private Stream _003CfileStream_003E5__3;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				if (num != 0)
				{
					_003CtmpFile_003E5__2 = path + ".temp";
				}
				try
				{
					if (num != 0)
					{
						_003CfileStream_003E5__3 = fs.Open(_003CtmpFile_003E5__2, (FileMode)2, (FileAccess)3, (FileShare)3, (FileOptions)1073741824);
					}
					try
					{
						ConfiguredTaskAwaiter awaiter;
						if (num != 0)
						{
							ConfiguredTaskAwaitable val = SaveToJsonAsync(jsonModel, _003CfileStream_003E5__3).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CSaveToJsonAsync_003Ed__11<TJsonModel>>(ref awaiter, ref this);
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
						if (num < 0 && _003CfileStream_003E5__3 != null)
						{
							((System.IDisposable)_003CfileStream_003E5__3).Dispose();
						}
					}
					_003CfileStream_003E5__3 = null;
					OverWritePrevious(fs, path, _003CtmpFile_003E5__2);
				}
				finally
				{
					if (num < 0)
					{
						fs.DeleteIfExists(_003CtmpFile_003E5__2);
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CtmpFile_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CtmpFile_003E5__2 = null;
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
	private struct _003CSaveToJsonAsync_003Ed__12<TJsonModel> : IAsyncStateMachine where TJsonModel : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public Stream writeStream;

		public TJsonModel jsonModel;

		private StreamWriter _003Csw_003E5__2;

		private JsonTextWriter _003Cjw_003E5__3;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				if ((uint)num > 1u)
				{
					_003Csw_003E5__2 = new StreamWriter(writeStream);
				}
				try
				{
					if ((uint)num > 1u)
					{
						_003Cjw_003E5__3 = new JsonTextWriter((TextWriter)(object)_003Csw_003E5__2)
						{
							Formatting = Formatting.Indented
						};
					}
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
								goto IL_0136;
							}
							val = JToken.FromObject(jsonModel, JSON_SERIALIZER.Value).WriteToAsync(_003Cjw_003E5__3).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CSaveToJsonAsync_003Ed__12<TJsonModel>>(ref awaiter, ref this);
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
						val = _003Cjw_003E5__3.FlushAsync().ConfigureAwait(false);
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CSaveToJsonAsync_003Ed__12<TJsonModel>>(ref awaiter, ref this);
							return;
						}
						goto IL_0136;
						IL_0136:
						((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					}
					finally
					{
						if (num < 0 && _003Cjw_003E5__3 != null)
						{
							((System.IDisposable)_003Cjw_003E5__3).Dispose();
						}
					}
				}
				finally
				{
					if (num < 0 && _003Csw_003E5__2 != null)
					{
						((System.IDisposable)_003Csw_003E5__2).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Csw_003E5__2 = null;
				_003Cjw_003E5__3 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Csw_003E5__2 = null;
			_003Cjw_003E5__3 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	public static readonly Lazy<JsonSerializer> JSON_SERIALIZER = new Lazy<JsonSerializer>((Func<JsonSerializer>)CreateSerializer);

	public static void SaveToJson(string dest_file, object data)
	{
		SaveToJson(dest_file, data, LolaFileSystem.Instance);
	}

	public static void SaveToJson(string dest_file, object? data, IFileSystem fileSystem)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		Stream val = fileSystem.Open(dest_file, (FileMode)2, (FileAccess)2, (FileShare)3, (FileOptions)0);
		try
		{
			StreamWriter val2 = new StreamWriter(val);
			try
			{
				using JsonTextWriter writer = new JsonTextWriter((TextWriter)(object)val2)
				{
					Formatting = Formatting.Indented
				};
				((data == null) ? JValue.CreateNull() : JToken.FromObject(data, JSON_SERIALIZER.Value)).WriteTo(writer);
			}
			finally
			{
				((System.IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
	}

	public static void SaveToJson<TJsonModel>(TJsonModel jsonModel, Stream writeStream)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		StreamWriter val = new StreamWriter(writeStream, Encoding.UTF8, 4096, true);
		try
		{
			using JsonTextWriter writer = new JsonTextWriter((TextWriter)(object)val)
			{
				Formatting = Formatting.Indented
			};
			JToken.FromObject(jsonModel, JSON_SERIALIZER.Value).WriteTo(writer);
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
	}

	public static T LoadFromJson<T>(string src_file)
	{
		return LoadFromJson<T>(src_file, LolaFileSystem.Instance);
	}

	public static T LoadFromJson<T>(string src_file, IFileSystem fileSystem)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		Stream val = fileSystem.Open(src_file, (FileMode)3, (FileAccess)1, (FileShare)3, (FileOptions)134217728);
		try
		{
			StreamReader val2 = new StreamReader(val);
			try
			{
				return JsonConvert.DeserializeObject<T>(((TextReader)val2).ReadToEnd());
			}
			finally
			{
				((System.IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
	}

	public static System.Threading.Tasks.Task<JToken> LoadFromJsonAsync(string srcFile)
	{
		return LoadFromJsonAsync(srcFile, LolaFileSystem.Instance);
	}

	[AsyncStateMachine(typeof(_003CLoadFromJsonAsync_003Ed__7))]
	public static async System.Threading.Tasks.Task<JToken> LoadFromJsonAsync(string srcFile, IFileSystem fs)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Stream asyncFileStream = fs.Open(srcFile, (FileMode)3, (FileAccess)1, (FileShare)3, (FileOptions)1073741824);
		try
		{
			StreamReader sw = new StreamReader(asyncFileStream);
			try
			{
				using JsonTextReader jw = new JsonTextReader((TextReader)(object)sw);
				return await JToken.LoadAsync(jw).DetachContext();
			}
			finally
			{
				((System.IDisposable)sw)?.Dispose();
			}
		}
		finally
		{
			((System.IDisposable)asyncFileStream)?.Dispose();
		}
	}

	public static System.Threading.Tasks.Task<TJsonModel> LoadFromJsonAsync<TJsonModel>(string srcFile)
	{
		return LoadFromJsonAsync<TJsonModel>(srcFile, LolaFileSystem.Instance);
	}

	[AsyncStateMachine(typeof(_003CLoadFromJsonAsync_003Ed__9<>))]
	public static async System.Threading.Tasks.Task<TJsonModel> LoadFromJsonAsync<TJsonModel>(string srcFile, IFileSystem fs)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return (await LoadFromJsonAsync(srcFile, fs).DetachContext()).ToObject<TJsonModel>(JSON_SERIALIZER.Value);
	}

	public static System.Threading.Tasks.Task SaveToJsonAsync<TJsonModel>(TJsonModel jsonModel, string path)
	{
		return SaveToJsonAsync(jsonModel, path, LolaFileSystem.Instance);
	}

	[AsyncStateMachine(typeof(_003CSaveToJsonAsync_003Ed__11<>))]
	public static System.Threading.Tasks.Task SaveToJsonAsync<TJsonModel>(TJsonModel jsonModel, string path, IFileSystem fs)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CSaveToJsonAsync_003Ed__11<TJsonModel> _003CSaveToJsonAsync_003Ed__ = default(_003CSaveToJsonAsync_003Ed__11<TJsonModel>);
		_003CSaveToJsonAsync_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CSaveToJsonAsync_003Ed__.jsonModel = jsonModel;
		_003CSaveToJsonAsync_003Ed__.path = path;
		_003CSaveToJsonAsync_003Ed__.fs = fs;
		_003CSaveToJsonAsync_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CSaveToJsonAsync_003Ed__._003C_003Et__builder)).Start<_003CSaveToJsonAsync_003Ed__11<TJsonModel>>(ref _003CSaveToJsonAsync_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CSaveToJsonAsync_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CSaveToJsonAsync_003Ed__12<>))]
	public static System.Threading.Tasks.Task SaveToJsonAsync<TJsonModel>(TJsonModel jsonModel, Stream writeStream)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CSaveToJsonAsync_003Ed__12<TJsonModel> _003CSaveToJsonAsync_003Ed__ = default(_003CSaveToJsonAsync_003Ed__12<TJsonModel>);
		_003CSaveToJsonAsync_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CSaveToJsonAsync_003Ed__.jsonModel = jsonModel;
		_003CSaveToJsonAsync_003Ed__.writeStream = writeStream;
		_003CSaveToJsonAsync_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CSaveToJsonAsync_003Ed__._003C_003Et__builder)).Start<_003CSaveToJsonAsync_003Ed__12<TJsonModel>>(ref _003CSaveToJsonAsync_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CSaveToJsonAsync_003Ed__._003C_003Et__builder)).Task;
	}

	public static string SaveToJsonString<TModel>(TModel model)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		MemoryStream val = new MemoryStream();
		try
		{
			SaveToJson(model, (Stream)(object)val);
			((Stream)val).Seek(0L, (SeekOrigin)0);
			StreamReader val2 = new StreamReader((Stream)(object)val);
			try
			{
				return ((TextReader)val2).ReadToEnd();
			}
			finally
			{
				((System.IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
	}

	public static T LoadFromJsonString<T>(string jsonString)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		MemoryStream val = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
		try
		{
			StreamReader val2 = new StreamReader((Stream)(object)val);
			try
			{
				using JsonTextReader reader = new JsonTextReader((TextReader)(object)val2);
				return JToken.Load(reader).ToObject<T>(JSON_SERIALIZER.Value);
			}
			finally
			{
				((System.IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
	}

	private static void OverWritePrevious(IFileSystem fs, string path, string tmpFile)
	{
		//IL_000b: Expected O, but got Unknown
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		try
		{
			fs.MoveAndOverwrite(tmpFile, path);
		}
		catch (FileNotFoundException val)
		{
			FileNotFoundException val2 = val;
			InvalidOperationException val3 = new InvalidOperationException("Race condition detected. A temporary file was deleted out from under us.", (System.Exception)(object)val2);
			Logger.LogFormattedException((System.Exception)(object)val2, "replacing a persisted json file: {0}", PathHelpers.QuotePathIfNeeded(path));
			throw val3;
		}
	}

	private static JsonSerializer CreateSerializer()
	{
		JsonSerializer jsonSerializer = new JsonSerializer();
		jsonSerializer.TypeNameHandling = TypeNameHandling.None;
		jsonSerializer.Formatting = Formatting.Indented;
		jsonSerializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
		jsonSerializer.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
		jsonSerializer.DateParseHandling = DateParseHandling.DateTimeOffset;
		((Collection<JsonConverter>)(object)jsonSerializer.Converters).Add((JsonConverter)new VersionConverter());
		((Collection<JsonConverter>)(object)jsonSerializer.Converters).Add((JsonConverter)new StringEnumConverter());
		return jsonSerializer;
	}
}
