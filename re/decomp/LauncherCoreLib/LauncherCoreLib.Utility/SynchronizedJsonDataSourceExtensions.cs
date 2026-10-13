using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;
using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Utility;

public static class SynchronizedJsonDataSourceExtensions
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CReplaceAllData_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ISynchronizedJsonDataSource replaceWith;

		public ISynchronizedJsonDataSource toReplace;

		private ConfiguredTaskAwaiter<JToken> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaiter<JToken> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_00e7;
					}
					awaiter2 = replaceWith.GetValue(string.Empty).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<JToken>, _003CReplaceAllData_003Ed__4>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<JToken>);
					num = (_003C_003E1__state = -1);
				}
				JToken result = awaiter2.GetResult();
				ConfiguredTaskAwaitable val = toReplace.StoreValue(string.Empty, result).DetachContext();
				awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CReplaceAllData_003Ed__4>(ref awaiter, ref this);
					return;
				}
				goto IL_00e7;
				IL_00e7:
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
	private struct _003CToReadonlySource_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IReadOnlyJsonDataSource> _003C_003Et__builder;

		public ISynchronizedJsonDataSource jsonDataSource;

		private ConfiguredTaskAwaiter<JToken> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			IReadOnlyJsonDataSource result;
			try
			{
				ConfiguredTaskAwaiter<JToken> awaiter;
				if (num != 0)
				{
					awaiter = jsonDataSource.GetValue(string.Empty).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<JToken>, _003CToReadonlySource_003Ed__2>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<JToken>);
					num = (_003C_003E1__state = -1);
				}
				result = new ReadOnlyJsonDataSource(awaiter.GetResult());
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

	public static IObservable<IReadOnlyJsonDataSource> WhenValueAtPathIsUpdated(this ISynchronizedJsonDataSource jsonDataSource, string jsonPath)
	{
		string jsonPath2 = jsonPath;
		return jsonDataSource.WhenValuesAreUpdated.Where<JsonDataUpdate>((Func<JsonDataUpdate, bool>)((JsonDataUpdate jsonUpdate) => jsonUpdate.JsonPath == jsonPath2)).Select<JsonDataUpdate, IReadOnlyJsonDataSource>((Func<JsonDataUpdate, IReadOnlyJsonDataSource>)((JsonDataUpdate jsonUpdate) => jsonUpdate.UpdatedValue));
	}

	public static IObservable<IReadOnlyJsonDataSource> CurrentAndFutureValuesAtPath(this ISynchronizedJsonDataSource jsonDataSource, string jsonPath)
	{
		ISynchronizedJsonDataSource jsonDataSource2 = jsonDataSource;
		string jsonPath2 = jsonPath;
		return ((IObservable<IReadOnlyJsonDataSource>)(object)Observable.FromAsync<ReadOnlyJsonDataSource>((Func<System.Threading.Tasks.Task<ReadOnlyJsonDataSource>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass1_0._003C_003CCurrentAndFutureValuesAtPath_003Eb__0_003Ed))] async () => new ReadOnlyJsonDataSource(await jsonDataSource2.GetValue(jsonPath2).DetachContext())))).Concat<IReadOnlyJsonDataSource>(jsonDataSource2.WhenValueAtPathIsUpdated(jsonPath2));
	}

	[AsyncStateMachine(typeof(_003CToReadonlySource_003Ed__2))]
	public static async System.Threading.Tasks.Task<IReadOnlyJsonDataSource> ToReadonlySource(this ISynchronizedJsonDataSource jsonDataSource)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new ReadOnlyJsonDataSource(await jsonDataSource.GetValue(string.Empty).DetachContext());
	}

	public static IReadOnlyJsonDataSource ToReadonlySourceSync(this ISynchronizedJsonDataSource jsonDataSource)
	{
		return new ReadOnlyJsonDataSource(jsonDataSource.GetValueSync(string.Empty));
	}

	[AsyncStateMachine(typeof(_003CReplaceAllData_003Ed__4))]
	public static System.Threading.Tasks.Task ReplaceAllData(this ISynchronizedJsonDataSource toReplace, ISynchronizedJsonDataSource replaceWith)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CReplaceAllData_003Ed__4 _003CReplaceAllData_003Ed__ = default(_003CReplaceAllData_003Ed__4);
		_003CReplaceAllData_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CReplaceAllData_003Ed__.toReplace = toReplace;
		_003CReplaceAllData_003Ed__.replaceWith = replaceWith;
		_003CReplaceAllData_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CReplaceAllData_003Ed__._003C_003Et__builder)).Start<_003CReplaceAllData_003Ed__4>(ref _003CReplaceAllData_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CReplaceAllData_003Ed__._003C_003Et__builder)).Task;
	}
}
