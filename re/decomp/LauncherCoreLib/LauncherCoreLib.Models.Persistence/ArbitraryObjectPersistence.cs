using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Tasks;
using LauncherCoreLib.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Models.Persistence;

public class ArbitraryObjectPersistence : IPersistArbitraryObjects
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCommit_003Ed__2<TPersistenceModel> : IAsyncStateMachine where TPersistenceModel : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public string path;

		public ArbitraryObjectPersistence _003C_003E4__this;

		public TPersistenceModel persistenceModel;

		private string _003CdirectoryPath_003E5__2;

		private bool _003CdirectoryExisted_003E5__3;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ArbitraryObjectPersistence arbitraryObjectPersistence = _003C_003E4__this;
			try
			{
				if (num != 0)
				{
					_003CdirectoryPath_003E5__2 = LauncherPathHelpers.NonRootPathParentDirectory(path);
					_003CdirectoryExisted_003E5__3 = arbitraryObjectPersistence._fs.PathExists(_003CdirectoryPath_003E5__2);
					if (!_003CdirectoryExisted_003E5__3)
					{
						arbitraryObjectPersistence._fs.CreateDirectory(_003CdirectoryPath_003E5__2);
					}
				}
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = SerializationUtil.SaveToJsonAsync(persistenceModel, path, arbitraryObjectPersistence._fs).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCommit_003Ed__2<TPersistenceModel>>(ref awaiter, ref this);
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
				catch (JsonException ex)
				{
					if (!_003CdirectoryExisted_003E5__3 && !Enumerable.Any<FileSystemEntry>(arbitraryObjectPersistence._fs.EntriesIn(_003CdirectoryPath_003E5__2)))
					{
						arbitraryObjectPersistence._fs.DeleteIfExists(_003CdirectoryPath_003E5__2);
					}
					throw new SerializationException(((System.Exception)ex).Message, (System.Exception)ex);
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CdirectoryPath_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CdirectoryPath_003E5__2 = null;
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
	private struct _003CLoad_003Ed__3<TPersistenceModel> : IAsyncStateMachine where TPersistenceModel : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TPersistenceModel> _003C_003Et__builder;

		public string path;

		public ArbitraryObjectPersistence _003C_003E4__this;

		private ConfiguredTaskAwaiter<TPersistenceModel> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ArbitraryObjectPersistence arbitraryObjectPersistence = _003C_003E4__this;
			TPersistenceModel result;
			try
			{
				try
				{
					ConfiguredTaskAwaiter<TPersistenceModel> awaiter;
					if (num != 0)
					{
						awaiter = SerializationUtil.LoadFromJsonAsync<TPersistenceModel>(path, arbitraryObjectPersistence._fs).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TPersistenceModel>, _003CLoad_003Ed__3<TPersistenceModel>>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TPersistenceModel>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter.GetResult();
				}
				catch (JsonException ex)
				{
					throw new SerializationException(((System.Exception)ex).Message, (System.Exception)ex);
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
	private struct _003CLoad_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<JToken> _003C_003Et__builder;

		public string path;

		public ArbitraryObjectPersistence _003C_003E4__this;

		private ConfiguredTaskAwaiter<JToken> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ArbitraryObjectPersistence arbitraryObjectPersistence = _003C_003E4__this;
			JToken result;
			try
			{
				try
				{
					ConfiguredTaskAwaiter<JToken> awaiter;
					if (num != 0)
					{
						awaiter = SerializationUtil.LoadFromJsonAsync(path, arbitraryObjectPersistence._fs).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<JToken>, _003CLoad_003Ed__4>(ref awaiter, ref this);
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
				catch (JsonException ex)
				{
					throw new SerializationException(((System.Exception)ex).Message, (System.Exception)ex);
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

	private readonly IFileSystem _fs;

	public ArbitraryObjectPersistence(IFileSystem fs)
	{
		_fs = fs;
	}

	[AsyncStateMachine(typeof(_003CCommit_003Ed__2<>))]
	public System.Threading.Tasks.Task Commit<TPersistenceModel>(TPersistenceModel persistenceModel, string path)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CCommit_003Ed__2<TPersistenceModel> _003CCommit_003Ed__ = default(_003CCommit_003Ed__2<TPersistenceModel>);
		_003CCommit_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CCommit_003Ed__._003C_003E4__this = this;
		_003CCommit_003Ed__.persistenceModel = persistenceModel;
		_003CCommit_003Ed__.path = path;
		_003CCommit_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CCommit_003Ed__._003C_003Et__builder)).Start<_003CCommit_003Ed__2<TPersistenceModel>>(ref _003CCommit_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CCommit_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CLoad_003Ed__3<>))]
	public async System.Threading.Tasks.Task<TPersistenceModel> Load<TPersistenceModel>(string path)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			return await SerializationUtil.LoadFromJsonAsync<TPersistenceModel>(path, _fs).DetachContext();
		}
		catch (JsonException ex)
		{
			throw new SerializationException(((System.Exception)ex).Message, (System.Exception)ex);
		}
	}

	[AsyncStateMachine(typeof(_003CLoad_003Ed__4))]
	public async System.Threading.Tasks.Task<JToken> Load(string path)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			return await SerializationUtil.LoadFromJsonAsync(path, _fs).DetachContext();
		}
		catch (JsonException ex)
		{
			throw new SerializationException(((System.Exception)ex).Message, (System.Exception)ex);
		}
	}

	public TPersistenceModel LoadSync<TPersistenceModel>(string path)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			return SerializationUtil.LoadFromJson<TPersistenceModel>(path, _fs);
		}
		catch (JsonException ex)
		{
			throw new SerializationException(((System.Exception)ex).Message, (System.Exception)ex);
		}
	}
}
