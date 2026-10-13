using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LOLA.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Utility;

public class TypedSyncedJsonDataSourceWrapper<TJsonModel> where TJsonModel : class
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetValue_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TJsonModel> _003C_003Et__builder;

		public TypedSyncedJsonDataSourceWrapper<TJsonModel> _003C_003E4__this;

		private ConfiguredTaskAwaiter<JToken> _003C_003Eu__1;

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
			TypedSyncedJsonDataSourceWrapper<TJsonModel> typedSyncedJsonDataSourceWrapper = _003C_003E4__this;
			TJsonModel result;
			try
			{
				ConfiguredTaskAwaiter<JToken> awaiter;
				if (num != 0)
				{
					awaiter = typedSyncedJsonDataSourceWrapper.DataSource.GetValue(typedSyncedJsonDataSourceWrapper.JsonPath).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<JToken>, _003CGetValue_003Ed__7>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<JToken>);
					num = (_003C_003E1__state = -1);
				}
				result = TypedSyncedJsonDataSourceWrapper<TJsonModel>.DeserializeToModelOrThrow(awaiter.GetResult());
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
	private struct _003CStoreValue_003Ed__9 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public TJsonModel value;

		public TypedSyncedJsonDataSourceWrapper<TJsonModel> _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			TypedSyncedJsonDataSourceWrapper<TJsonModel> typedSyncedJsonDataSourceWrapper = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					JObject jObject;
					try
					{
						jObject = JObject.FromObject(value, SerializationUtil.JSON_SERIALIZER.Value);
					}
					catch (JsonException ex)
					{
						throw new SerializationException("Could not serialize the data.", (System.Exception)ex);
					}
					ConfiguredTaskAwaitable val = typedSyncedJsonDataSourceWrapper.DataSource.StoreValue(typedSyncedJsonDataSourceWrapper.JsonPath, jObject).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CStoreValue_003Ed__9>(ref awaiter, ref this);
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

	public ISynchronizedJsonDataSource DataSource
	{
		[CompilerGenerated]
		get;
	}

	public string JsonPath
	{
		[CompilerGenerated]
		get;
	}

	public TypedSyncedJsonDataSourceWrapper(ISynchronizedJsonDataSource dataSource, string jsonPath)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		DataSource = dataSource ?? throw new ArgumentNullException("dataSource");
		JsonPath = jsonPath;
	}

	[AsyncStateMachine(typeof(TypedSyncedJsonDataSourceWrapper<>._003CGetValue_003Ed__7))]
	public virtual async System.Threading.Tasks.Task<TJsonModel?> GetValue()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return DeserializeToModelOrThrow(await DataSource.GetValue(JsonPath).DetachContext());
	}

	public virtual TJsonModel? GetValueSync()
	{
		return DeserializeToModelOrThrow(DataSource.GetValueSync(JsonPath));
	}

	[AsyncStateMachine(typeof(TypedSyncedJsonDataSourceWrapper<>._003CStoreValue_003Ed__9))]
	public virtual System.Threading.Tasks.Task StoreValue(TJsonModel value)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CStoreValue_003Ed__9 _003CStoreValue_003Ed__ = default(_003CStoreValue_003Ed__9);
		_003CStoreValue_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CStoreValue_003Ed__._003C_003E4__this = this;
		_003CStoreValue_003Ed__.value = value;
		_003CStoreValue_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CStoreValue_003Ed__._003C_003Et__builder)).Start<_003CStoreValue_003Ed__9>(ref _003CStoreValue_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CStoreValue_003Ed__._003C_003Et__builder)).Task;
	}

	public virtual void StoreValueSync(TJsonModel value)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		JObject value2;
		try
		{
			value2 = JObject.FromObject(value);
		}
		catch (JsonException ex)
		{
			throw new SerializationException("Could not serialize the data.", (System.Exception)ex);
		}
		DataSource.StoreValueSync(JsonPath, value2);
	}

	public IReadOnlyJsonDataSource ToReadonlySource()
	{
		return new ReadOnlyJsonDataSource(DataSource.GetValueSync(string.Empty));
	}

	private static TJsonModel? DeserializeToModelOrThrow(JToken? jobject)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (jobject == null)
		{
			return null;
		}
		try
		{
			return jobject.ToObject<TJsonModel>();
		}
		catch (JsonException ex)
		{
			throw new SerializationException("Could not deserialize the data.", (System.Exception)ex);
		}
	}
}
